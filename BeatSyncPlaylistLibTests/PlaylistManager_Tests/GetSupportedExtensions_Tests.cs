using BeatSaberPlaylistsLib;
using BeatSaberPlaylistsLib.Blister;
using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLib.Types;
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
    public class GetSupportedExtensions_Tests
    {
        private static readonly string OutputPath = Path.Combine(TestTools.OutputFolder, "GetSupportedExtensions");
        [TestMethod]
        public void Constructor()
        {
            string playlistsPath = Path.Combine(OutputPath, "Constructor");
            IPlaylistHandler[] handlers = { new LegacyPlaylistHandler(), new MockPlaylistHandler(), new BlisterPlaylistHandler() };
            PlaylistManager manager = new PlaylistManager(playlistsPath, handlers[0], handlers);
            string[] supportedExtensions = manager.GetSupportedExtensions();
            foreach (var handler in handlers)
            {
                foreach (var extension in handler.GetSupportedExtensions())
                {
                    Assert.IsTrue(supportedExtensions.Any(e => e.Equals(extension, StringComparison.OrdinalIgnoreCase)), $"manager should've supported '{extension}' from {handler.GetType().Name}");
                }
            }

        }

        [TestMethod]
        public void FromParent()
        {
            string playlistsPath = Path.Combine(OutputPath, "FromParent");
            Directory.CreateDirectory(playlistsPath);
            string subDirectory = Path.Combine(playlistsPath, "ChildFolder");
            Directory.CreateDirectory(subDirectory);
            string parentPlaylist = "5LegacySongs.bPlist";
            string parentPlaylistPath = Path.Combine(playlistsPath, parentPlaylist);
            string childPlaylist = "MDBB.blist";
            string childPlaylistPath = Path.Combine(subDirectory, childPlaylist);
            if (!File.Exists(parentPlaylistPath))
                File.Copy(Path.Combine(TestTools.DataFolder, "GetPlaylistTests", parentPlaylist), parentPlaylistPath);
            if (!File.Exists(childPlaylistPath))
                File.Copy(Path.Combine(TestTools.DataFolder, "BlisterPlaylists", childPlaylist), childPlaylistPath);
            PlaylistManager manager = new PlaylistManager(playlistsPath, new LegacyPlaylistHandler(), new BlisterPlaylistHandler());
            Assert.IsTrue(manager.HasChildren);
            PlaylistManager childManager = manager.GetChildManagers.First();
            childManager.SupportsExtension("bplist");
            IPlaylistHandler? handler = childManager.GetHandlerForExtension("bplist");
            Assert.IsNotNull(handler);
            handler = childManager.GetHandler<LegacyPlaylistHandler>();
            Assert.IsNotNull(handler);
            string[] supportedExtensions = childManager.GetSupportedExtensions();
            Assert.AreEqual(3, supportedExtensions.Length);
        }
    }
}
