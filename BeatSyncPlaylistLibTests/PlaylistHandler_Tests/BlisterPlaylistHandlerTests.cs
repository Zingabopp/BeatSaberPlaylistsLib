using BeatSaberPlaylistsLib;
using BeatSaberPlaylistsLib.Blister;
using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLib.Types;
using BeatSaberPlaylistsLibTests.IPlaylistSong_Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static BeatSaberPlaylistsLibTests.PlaylistHandler_Tests.IPlaylistHandlerTests;

namespace BeatSaberPlaylistsLibTests.PlaylistHandler_Tests
{
    [TestClass]
    public class BlisterPlaylistHandlerTests
    {
        public static readonly string ReadOnlyData = Path.Combine(TestTools.DataFolder, "BlisterPlaylists");
        public static readonly string OutputPath = Path.Combine(TestTools.OutputFolder, "BlisterPlaylistHandler_Tests");
        [TestMethod]
        public void StorePlaylist()
        {
            string playlistDir = Path.Combine(OutputPath, "StorePlaylist");
            IPlaylistHandler handler = new LegacyPlaylistHandler();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir, handler);
            var songs = CreateSongArray<LegacyPlaylistSong>("Legacy_", "LegacyAuthor_", 5, Identifier.LevelId | Identifier.Hash | Identifier.Key);
            IPlaylist playlist = manager.CreatePlaylist("5LegacySongs", "Five Legacy Songs", "TestAuthor", string.Empty, "Test Description");
            foreach (var song in songs)
                playlist.Add(song);
            playlist.RaisePlaylistChanged();
            manager.RegisterPlaylist(playlist);
            manager.StoreAllPlaylists();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }

        [TestMethod]
        public void ReadPlaylist_WithImage()
        {
            string sourcePlaylist = Path.Combine(ReadOnlyData, "MDBB.blist");
            IPlaylistHandler handler = new BlisterPlaylistHandler();
            Assert.IsTrue(File.Exists(sourcePlaylist), $"File doesn't exist: '{sourcePlaylist}'");
            using Stream playlistStream = File.OpenRead(sourcePlaylist);
            IPlaylist playlist = handler.Deserialize(playlistStream);

        }
        [TestMethod]
        public void ReadPlaylist_NoImage()
        {
            string sourcePlaylist = Path.Combine(ReadOnlyData, "AnniversarySongPack.blist");
            IPlaylistHandler handler = new BlisterPlaylistHandler();
            Assert.IsTrue(File.Exists(sourcePlaylist), $"File doesn't exist: '{sourcePlaylist}'");
            using Stream playlistStream = File.OpenRead(sourcePlaylist);
            IPlaylist playlist = handler.Deserialize(playlistStream);

        }

        [TestMethod]
        public void WritePlaylist_NoImage()
        {
            string playlistDir = Path.Combine(OutputPath, "StorePlaylist");
            string fileName = "testPlaylist";
            string playlistTitle = "WritePlaylist_NoImage";
            string playlistAuthor = "BlisterTests";
            string playlistDescription = "A test playlist";
            string playlistExtension = "blist";
            string playlistFile = Path.Combine(playlistDir, fileName + "." + playlistExtension);
            IPlaylistHandler handler = new BlisterPlaylistHandler();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));
            Directory.CreateDirectory(playlistDir);
            var songs = CreateSongArray<LegacyPlaylistSong>("Blister_", "BlisterAuthor_", 5, Identifier.LevelId | Identifier.Hash | Identifier.Key);
            BlisterPlaylist playlist = new BlisterPlaylist(fileName, playlistTitle, playlistAuthor )
            {
                Description = playlistDescription,
                SuggestedExtension = playlistExtension
            };
            foreach (var song in songs)
            {
                playlist.Add(song);
            }
            playlist.RaisePlaylistChanged();
            Assert.AreEqual(songs.Length, playlist.Count);

            using FileStream fileStream = File.Create(playlistFile);
            handler.Serialize(playlist, fileStream);

            Assert.IsTrue(File.Exists(playlistFile));
            Console.WriteLine(Path.GetFullPath(playlistFile));
        }
    }
}
