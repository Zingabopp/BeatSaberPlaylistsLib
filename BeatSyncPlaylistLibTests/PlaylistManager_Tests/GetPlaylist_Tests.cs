using BeatSaberPlaylistsLib;
using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLib.Types;
using BeatSaberPlaylistsLibTests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeatSaberPlaylistsLibTests.PlaylistManager_Tests
{
    [TestClass]
    public class GetPlaylist_Tests
    {
        public static readonly string ReadOnlyData = Path.Combine(TestTools.DataFolder, "GetPlaylistTests");
        public static readonly string OutputPath = Path.Combine(TestTools.OutputFolder, "PlaylistManager_Tests", "GetPlaylist_Tests");

        [TestMethod]
        public void NullFilename()
        {
            string playlistDir = Path.Combine(OutputPath, "NullFilename");
            IPlaylistHandler handler = new LegacyPlaylistHandler();
            PlaylistManager manager = TestTools.GetPlaylistManager(playlistDir, handler);
            string playlistFileName = null;

            IPlaylist? playlist = manager.GetPlaylist(playlistFileName);

            Assert.IsNull(playlist);

            TestTools.Cleanup(playlistDir);
        }

        [TestMethod]
        public void EmptyFilename()
        {
            string playlistDir = Path.Combine(OutputPath, "");
            IPlaylistHandler handler = new LegacyPlaylistHandler();
            PlaylistManager manager = TestTools.GetPlaylistManager(playlistDir, handler);
            string playlistFileName = "";

            IPlaylist? playlist = manager.GetPlaylist(playlistFileName);

            Assert.IsNull(playlist);

            TestTools.Cleanup(playlistDir);
        }

        [TestMethod]
        public void FileNoExtension()
        {
            string playlistDir = Path.Combine(OutputPath, "NoExtension");
            IPlaylistHandler handler = new LegacyPlaylistHandler();
            PlaylistManager manager = TestTools.GetPlaylistManager(playlistDir, handler);
            File.Copy(Path.Combine(ReadOnlyData, "NoExtension"), Path.Combine(playlistDir, "NoExtension"));
            string playlistFileName = "NoExtension";

            Assert.ThrowsException<ArgumentException>(() => manager.GetPlaylist(playlistFileName));

            TestTools.Cleanup(playlistDir);
        }

        [TestMethod]
        public void ExtensionHasCapital()
        {
            string playlistDir = Path.Combine(OutputPath, "ExtensionHasCapital");
            IPlaylistHandler handler = new LegacyPlaylistHandler();
            PlaylistManager manager = TestTools.GetPlaylistManager(playlistDir, handler);
            string playlistFileName = "5LegacySongs";
            File.Copy(Path.Combine(ReadOnlyData, "5LegacySongs.bPlist"), Path.Combine(playlistDir, "5LegacySongs.bPlist"));
            IPlaylist? playlist = null;
            playlist = manager.GetPlaylist(playlistFileName);
            Assert.IsNotNull(playlist);
            Assert.AreEqual(5, playlist.Count);

            TestTools.Cleanup(playlistDir);
        }

        [TestMethod]
        public void PassedUnsupportedHandler()
        {
            string playlistDir = Path.Combine(OutputPath, "PassedUnsupportedHandler");
            IPlaylistHandler defaultHandler = new LegacyPlaylistHandler();
            IPlaylistHandler providedHandler = new MockPlaylistHandler();
            PlaylistManager manager = TestTools.GetPlaylistManager(playlistDir, defaultHandler);
            string playlistFileName = "5LegacySongs";
            File.Copy(Path.Combine(ReadOnlyData, "5LegacySongs.bPlist"), Path.Combine(playlistDir, "5LegacySongs.bPlist"));

            Assert.ThrowsException<ArgumentException>(() => manager.GetPlaylist(playlistFileName, providedHandler));

            TestTools.Cleanup(playlistDir);
        }

        [TestMethod]
        public void NoMatchingHandler()
        {
            string playlistDir = Path.Combine(OutputPath, "NoMatchingHandler");
            IPlaylistHandler defaultHandler = new MockPlaylistHandler();
            PlaylistManager manager = TestTools.GetPlaylistManager(playlistDir, defaultHandler);
            string playlistFileName = "5LegacySongs";
            File.Copy(Path.Combine(ReadOnlyData, "5LegacySongs.bPlist"), Path.Combine(playlistDir, "5LegacySongs.bPlist"));

            Assert.ThrowsException<InvalidOperationException>(() => manager.GetPlaylist(playlistFileName));

            TestTools.Cleanup(playlistDir);
        }
    }
}
