﻿using BeatSaberPlaylistsLib;
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
        public void WritePlaylist_Image()
        {
            string playlistDir = Path.Combine(OutputPath, "WritePlaylist_Image");
            string fileName = "testPlaylist";
            string playlistTitle = "WritePlaylist_NoImage";
            string playlistAuthor = "BlisterTests";
            string playlistDescription = "A test playlist";
            string playlistExtension = "blist";
            string coverFile = Path.Combine(ReadOnlyData, "testCover.jpg");
            string playlistFile = Path.Combine(playlistDir, fileName + "." + playlistExtension);
            Assert.IsTrue(File.Exists(coverFile), $"Cover not found: '{coverFile}'");
            IPlaylistHandler handler = new BlisterPlaylistHandler();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));
            Directory.CreateDirectory(playlistDir);
            var songs = CreateSongArray<LegacyPlaylistSong>("Blister_", "BlisterAuthor_", 1000, Identifier.LevelId | Identifier.Hash | Identifier.Key);
            BlisterPlaylist playlist = new BlisterPlaylist(fileName, playlistTitle, playlistAuthor)
            {
                Description = playlistDescription,
                SuggestedExtension = playlistExtension
            };
            foreach (var song in songs)
            {
                playlist.Add(song);
            }
            playlist.SetCover(File.OpenRead(coverFile));
            playlist.Cover = Path.GetFileName(coverFile);
            playlist.RaisePlaylistChanged();
            Assert.AreEqual(songs.Length, playlist.Count);

            Directory.CreateDirectory(playlistDir);
            using FileStream fileStream = File.Create(playlistFile);
            handler.Serialize(playlist, fileStream);

            Assert.IsTrue(File.Exists(playlistFile));
            Console.WriteLine(Path.GetFullPath(playlistFile));
            using FileStream newFileStream = File.OpenRead(playlistFile);
            BlisterPlaylist? readPlaylist = handler.Deserialize(newFileStream) as BlisterPlaylist 
                ?? throw new AssertFailedException("readPlaylist is null.");
            Assert.AreEqual(playlist.Count, readPlaylist.Count);
            Assert.IsTrue(readPlaylist.HasCover);
            var newSongList = readPlaylist.Take(10).ToArray();
            readPlaylist.Clear();
            Assert.AreEqual(0, readPlaylist.Count);
            foreach (var item in newSongList)
            {
                readPlaylist.Add(item);
            }
            readPlaylist.SetCover(new byte[] { 11, 12, 13, 14, 15, 16, 17 });
            readPlaylist.RaisePlaylistChanged();
            using FileStream finalStream = File.Open(playlistFile, FileMode.Create, FileAccess.ReadWrite);
            handler.Serialize(readPlaylist, finalStream);
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
            var songs = CreateSongArray<LegacyPlaylistSong>("Blister_", "BlisterAuthor_", 1000, Identifier.LevelId | Identifier.Hash | Identifier.Key);
            BlisterPlaylist playlist = new BlisterPlaylist(fileName, playlistTitle, playlistAuthor )
            {
                Description = playlistDescription,
                SuggestedExtension = playlistExtension
            };
            foreach (var song in songs)
            {
                playlist.Add(song);
            }
            playlist.SetCover(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            playlist.RaisePlaylistChanged();
            Assert.AreEqual(songs.Length, playlist.Count);

            Directory.CreateDirectory(playlistDir);
            using FileStream fileStream = File.Create(playlistFile);
            handler.Serialize(playlist, fileStream);

            Assert.IsTrue(File.Exists(playlistFile));
            Console.WriteLine(Path.GetFullPath(playlistFile));
            using FileStream newFileStream = File.OpenRead(playlistFile);
            BlisterPlaylist readPlaylist = handler.Deserialize(newFileStream) as BlisterPlaylist
                ?? throw new AssertFailedException("readPlaylist is null.");
            Assert.AreEqual(playlist.Count, readPlaylist.Count);
            Assert.IsTrue(readPlaylist.HasCover);
            BlisterPlaylistSong[] newSongList = readPlaylist.Take(10).Select(s => (BlisterPlaylistSong)s).ToArray();
            readPlaylist.Clear();
            Assert.AreEqual(0, readPlaylist.Count);
            foreach (var item in newSongList)
            {
                item.AddDifficulty(new Difficulty() { Characteristic = "Standard", Name = "Hard" });
                readPlaylist.Add(item);
            }
            readPlaylist.SetCover(new byte[] { 11, 12, 13, 14, 15, 16, 17 });
            readPlaylist.RaisePlaylistChanged();
            using FileStream finalStream = File.Open(playlistFile, FileMode.Create, FileAccess.ReadWrite);
            handler.Serialize(readPlaylist, finalStream);
        }


        [TestMethod]
        public void UpdatePlaylist_WithImage()
        {
            string fileName = "MDBB.blist";
            string originalFile = Path.Combine(ReadOnlyData, fileName);
            string playlistDir = Path.Combine(OutputPath, "UpdatePlaylist");
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));
            Directory.CreateDirectory(playlistDir);
            File.Copy(originalFile, Path.Combine(playlistDir, fileName));
            Assert.IsTrue(File.Exists(Path.Combine(playlistDir, fileName)));
            string playlistFile = "MDBB";
            BlisterPlaylistHandler handler = new BlisterPlaylistHandler();
            PlaylistManager manager = new PlaylistManager(playlistDir, handler);
            BlisterPlaylist playlist = manager.GetPlaylist(playlistFile) as BlisterPlaylist ?? throw new AssertFailedException("Playlist is null");
            Assert.IsNotNull(playlist);
            Assert.AreEqual(7, playlist.Count);
            Assert.IsTrue(playlist.HasCover);
            var song = playlist.First();
            playlist.Clear();
            Assert.IsTrue(playlist.Count == 0);
            playlist.Add(song);
            playlist.RaisePlaylistChanged();
            Assert.AreEqual(1, playlist.Count);
            manager.StoreAllPlaylists();

            manager = new PlaylistManager(playlistDir, handler);
            playlist = manager.GetPlaylist(playlistFile) as BlisterPlaylist ?? throw new AssertFailedException("Playlist is null");
            Assert.IsNotNull(playlist);
            Assert.AreEqual(1, playlist.Count);
            Assert.IsTrue(playlist.HasCover);
        }
    }
}
