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
    public class TryGetPlaylist
    {
        public static readonly string ReadOnlyData = Path.Combine(TestTools.DataFolder, "NestedDirectories", "NestedPlaylists");
        public static readonly string OutputPath = Path.Combine(TestTools.OutputFolder, "PlaylistManager_Tests", "TryGetPlaylist");

        [TestMethod]
        public void GetAllFromParent()
        {
            string playlistsDir = ReadOnlyData;
            int expectedPlaylists = 2;
            PlaylistManager manager = new PlaylistManager(playlistsDir, new LegacyPlaylistHandler());
            IPlaylist[] playlists = manager.GetAllPlaylists();
            Assert.AreEqual(expectedPlaylists, playlists.Length);
        }


        [TestMethod]
        public void GetAllIncludingChildren()
        {
            string playlistsDir = ReadOnlyData;
            int expectedPlaylists = 14;
            PlaylistManager manager = new PlaylistManager(playlistsDir, new LegacyPlaylistHandler());
            IPlaylist[] playlists = manager.GetAllPlaylists(true);
            Assert.AreEqual(expectedPlaylists, playlists.Length);
        }

        [TestMethod]
        public void GetIncludingChildren()
        {
#pragma warning disable CS8604 // Possible null reference argument.
            string playlistsDir = ReadOnlyData;
            PlaylistManager manager = new PlaylistManager(playlistsDir, new LegacyPlaylistHandler());
            manager.GetAllPlaylists(true);
            IPlaylist? playlist = manager.GetPlaylist("L3_0_0_1");
            Assert.IsNotNull(playlist);
            PlaylistManager? childManager = manager.GetManagerForPlaylist(playlist);
            Assert.IsNotNull(childManager);
            Assert.AreNotEqual(manager, childManager);
        }
#pragma warning restore CS8604 // Possible null reference argument.
    }
}
