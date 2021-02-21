using BeatSaberPlaylistsLib;
using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLib.Types;
using BeatSaberPlaylistsLibTests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static BeatSaberPlaylistsLibTests.PlaylistHandler_Tests.IPlaylistHandlerTests;
using static BeatSaberPlaylistsLibTests.TestTools;

namespace BeatSaberPlaylistsLibTests.PlaylistHandler_Tests
{
    [TestClass]
    public class LegacyPlaylistHandlerTests : PlaylistHandlerTestBase
    {
        public static readonly string kReadOnlyData = TestTools.DataFolder;
        public static readonly string kOutputPath = Path.Combine(TestTools.OutputFolder, "LegacyPlaylistHandler_Tests");
        public override string ReadOnlyData => kReadOnlyData;
        public override string OutputPath => kOutputPath;
        [TestMethod]
        public void StorePlaylist()
        {
            string playlistDir = Path.Combine(OutputPath, "StorePlaylist");
            IPlaylistHandler handler = new LegacyPlaylistHandler();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir, handler);
            var songs = CreateSongArray<LegacyPlaylistSong>("Legacy_", "LegacyAuthor_", 1000, Identifier.LevelId | Identifier.Hash | Identifier.Key);
            IPlaylist playlist = manager.CreatePlaylist("5LegacySongs", "Five Legacy Songs", "TestAuthor", string.Empty, "Test Description");
            foreach (var song in songs)
                playlist.Add(song);
            playlist.RaisePlaylistChanged();
            manager.RegisterPlaylist(playlist);
            Assert.IsTrue(manager.IsPlaylistChanged(playlist));
            manager.StoreAllPlaylists();
            Assert.IsFalse(manager.IsPlaylistChanged(playlist));
            playlist.Add(songs[0]);
            playlist.RaisePlaylistChanged();
            Assert.IsTrue(manager.IsPlaylistChanged(playlist));
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }

        [TestMethod]
        public void DeserializeDerivedPlaylist()
        {
            string playlistDir = Path.Combine(OutputPath, "DeserializeDerivedPlaylist");
            IPlaylistHandler handler = new LegacyPlaylistHandler();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir, handler);
            var songs = CreateSongArray<LegacyPlaylistSong>("Legacy_", "LegacyAuthor_", 1000, Identifier.LevelId | Identifier.Hash | Identifier.Key);
            IPlaylist? playlist = manager.CreatePlaylist("5LegacySongs", "Five Legacy Songs", "TestAuthor", string.Empty, "Test Description");
            foreach (var song in songs)
                playlist.Add(song);
            playlist.RaisePlaylistChanged();
            manager.RegisterPlaylist(playlist);
            manager.StoreAllPlaylists();
            manager = new PlaylistManager(playlistDir, handler);
            playlist = manager.GetHandler<LegacyPlaylistHandler>()?.Deserialize<DerivedLegacyPlaylist>(File.OpenRead(Path.Combine(playlistDir, "5LegacySongs.bplist")));
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }

        [TestMethod]
        public void DeserializeExtraData()
        {
            string playlistFile = Path.Combine(ReadOnlyData, "LegacyPlaylists", "ExtraData.bplist");
            TestDeserializedExtraData(new LegacyPlaylistHandler(), playlistFile);
        }
    }
}
