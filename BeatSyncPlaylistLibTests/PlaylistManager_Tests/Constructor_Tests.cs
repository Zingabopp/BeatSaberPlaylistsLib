using BeatSaberPlaylistsLib;
using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLibTests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
