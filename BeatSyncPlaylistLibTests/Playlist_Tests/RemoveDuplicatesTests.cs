using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLib.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberPlaylistsLibTests.Playlist_Tests
{
    [TestClass]
    public class RemoveDuplicatesTests
    {
        public static IPlaylistSong[] DefaultSongs = TestTools.CreateSongArray<LegacyPlaylistSong>("Test_", "Test-", 30, Identifier.Hash);

        [TestMethod]
        public void HasDuplicates()
        {
            IPlaylist playlist = new LegacyPlaylist("DuplicateTest", "Has Duplicates", "Test");
            bool changedEventRaised = false;
            playlist.PlaylistChanged += (s, e) =>
            {
                changedEventRaised = true;
            };
            foreach (var song in DefaultSongs)
            {
                playlist.Add(song);
            }
            int uniqueSongs = DefaultSongs.Length; 
            foreach (var song in DefaultSongs)
            {
                playlist.Add(new LegacyPlaylistSong(song));
            }
            changedEventRaised = false;
            Assert.AreEqual(uniqueSongs * 2, playlist.Count);
            playlist.RemoveDuplicates();
            Assert.IsTrue(changedEventRaised);
            Assert.AreEqual(uniqueSongs, playlist.Count);
        }
    }
}
