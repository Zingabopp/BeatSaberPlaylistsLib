using BeatSaberPlaylistsLib;
using BeatSaberPlaylistsLib.Blist;
using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLibTests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatSaberPlaylistsLibTests.PlaylistManager_Tests
{
    [TestClass]
    public class Constructor_Tests
    {
        public static readonly string OutputPath = Path.Combine(TestTools.OutputFolder, "PlaylistManager_Tests", "Constructor_Tests");

        [TestMethod]
        public void PathOnlyConstructor_DirectoryCreated()
        {
            string playlistDir = Path.Combine(OutputPath, "PathOnlyConstructor");
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir);
            manager.RegisterHandler(new LegacyPlaylistHandler());
            Assert.IsTrue(Directory.Exists(playlistDir));
            Assert.AreEqual(typeof(LegacyPlaylistHandler), manager.DefaultHandler?.GetType());
            Assert.IsNotNull(manager.GetHandlerForExtension("bplist"));
            Assert.IsNotNull(manager.GetHandlerForExtension("JsOn"));

            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }

        [TestMethod]
        public void HandlerConstructor()
        {
            string playlistDir = Path.Combine(OutputPath, "HandlerConstructor");
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir, new MockPlaylistHandler());

            Assert.IsTrue(Directory.Exists(playlistDir));
            Assert.AreEqual(typeof(MockPlaylistHandler), manager.DefaultHandler?.GetType());
            Assert.IsNotNull(manager.GetHandlerForExtension("mOck"));

            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }

        public void GeneratedNested()
        {
            string playlistsDir = Path.Combine(OutputPath, "NestedPlaylists");
            string[] levelTwo = new string[]
            {
                Path.Combine(playlistsDir, "L2_1"),
                Path.Combine(playlistsDir, "L2_2"),
                Path.Combine(playlistsDir, "L2_3"),
            };
            string[] levelThree = new string[]
            {

                Path.Combine(levelTwo[0], "L3_1"),
                Path.Combine(levelTwo[0], "L3_2"),
                Path.Combine(levelTwo[1], "L3_3"),
            };
            PlaylistManager manager = TestTools.GetPlaylistManager(playlistsDir);
            foreach (var dir in levelTwo)
            {
                TestTools.GetPlaylistManager(dir);
            }
            foreach (var dir in levelThree)
            {
                TestTools.GetPlaylistManager(dir);
            }
            manager = new PlaylistManager(playlistsDir, new LegacyPlaylistHandler(), new BlistPlaylistHandler());
            manager.RegisterPlaylist(TestTools.CreatePlaylist<LegacyPlaylistHandler, LegacyPlaylistSong>("L1_1", "Top Level 1", "TopLevel", 5));
            manager.RegisterPlaylist(TestTools.CreatePlaylist<LegacyPlaylistHandler, LegacyPlaylistSong>("L1_2", "Top Level 2", "TopLevel", 5));
            var childManagers = manager.GetChildManagers().ToArray();
            for(int i = 0; i < childManagers.Length; i++)
            {
                var child = childManagers[i];
                child.RegisterPlaylist(TestTools.CreatePlaylist<LegacyPlaylistHandler, LegacyPlaylistSong>($"L2_{i}_1", $"Level 2_{i}", "Level 2", 5));
                child.RegisterPlaylist(TestTools.CreatePlaylist<LegacyPlaylistHandler, LegacyPlaylistSong>($"L2_{i}_2", $"Level 2_{i}", "Level 2", 5));
                var nestedChildren = child.GetChildManagers().ToArray();
                for (int j = 0; j < nestedChildren.Length; j++)
                {
                    var nestedChild = nestedChildren[j];
                    nestedChild.RegisterPlaylist(TestTools.CreatePlaylist<LegacyPlaylistHandler, LegacyPlaylistSong>($"L3_{i}_{j}_1", $"Level 3_{i}_{j}", "Level 3", 5));
                    nestedChild.RegisterPlaylist(TestTools.CreatePlaylist<LegacyPlaylistHandler, LegacyPlaylistSong>($"L3_{i}_{j}_2", $"Level 3_{i}_{j}", "Level 3", 5));
                }
            }
            manager.StoreAllPlaylists();
        }
    }
}
