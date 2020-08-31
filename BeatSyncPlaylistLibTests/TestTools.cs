using BeatSaberPlaylistsLib;
using BeatSaberPlaylistsLib.Blist;
using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLib.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BeatSaberPlaylistsLibTests
{
    public static class TestTools
    {
        public const string DataFolder = "ReadOnlyData";
        public const string OutputFolder = "Output";


        public static PlaylistManager GetPlaylistManager(string playlistDir, IPlaylistHandler defaultHandler, params IPlaylistHandler[] playlistHandlers)
        {
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));
            PlaylistManager manager = new PlaylistManager(playlistDir, defaultHandler, playlistHandlers);
            Assert.IsTrue(Directory.Exists(playlistDir));
            return manager;
        }
        public static PlaylistManager GetPlaylistManager(string playlistDir) => GetPlaylistManager(playlistDir, new LegacyPlaylistHandler(), new BlistPlaylistHandler());

        public static IPlaylistSong CreatePlaylistSongWithFactory(Func<string?, string?, string?, string?, string?, IPlaylistSong> factory, string? hash, string? levelId, string? songName, string? key, string? levelAuthorName)
        {
            return factory(hash, levelId, songName, key, levelAuthorName);
        }
        public static IPlaylistSong CreatePlaylistSong<T>(string? hash, string? levelId,
            string? songName, string? key, string? levelAuthorName, bool assignHashFirst = false)
            where T : IPlaylistSong, new()
        {
            IPlaylistSong playlistSong;
            if (assignHashFirst)
            {
                playlistSong = new T()
                {
                    Hash = hash,
                    LevelId = levelId,
                    Key = key,
                    Name = songName,
                    LevelAuthorName = levelAuthorName
                };
            }
            else
            {
                playlistSong = new T()
                {
                    LevelId = levelId,
                    Hash = hash,
                    Key = key,
                    Name = songName,
                    LevelAuthorName = levelAuthorName
                };
            }
            return playlistSong;
        }

        public static void Cleanup(string playlistDir)
        {
            try
            {
                if (Directory.Exists(playlistDir))
                    Directory.Delete(playlistDir, true);
            }
            catch (Exception) { }
        }

        public static IPlaylist CreatePlaylist<T, TSong>(string fileName, string title, string author, int numSongs,
            string? description = null, string? suggestedExtension = null)
            where T : IPlaylistHandler, new()
            where TSong : IPlaylistSong, new()
        {
            T handler = new T();
            IPlaylist playlist = handler.CreatePlaylist(fileName, title, author, description, suggestedExtension);

            TSong[] songs = CreateSongArray<TSong>(fileName + "_", author + "_", numSongs, Identifier.Hash | Identifier.Key, fileName);
            for(int i = 0; i < songs.Length ; i++)
            {
                playlist.Add(songs[i]);
            }
            playlist.RaisePlaylistChanged();
            return playlist;
        }

        public static T[] CreateSongArray<T>(string songNamePrefix, string levelAuthorPrefix, int size, Identifier useIdentifiers = Identifier.Hash, string hashPrefix = "")
            where T : IPlaylistSong, new()
        {
            T[] songs = new T[size];
            for (int i = 0; i < size; i++)
            {

                string? levelId = useIdentifiers.HasFlag(Identifier.LevelId) ? $"custom_level_" + $"{hashPrefix}ABC{i}_".PadRight(40, '0') : null;
                string? hash = useIdentifiers.HasFlag(Identifier.Hash) ? $"{hashPrefix}ABC{i}_".PadRight(40, '0') : null;
                string? key = useIdentifiers.HasFlag(Identifier.Key) ? $"{i}".PadLeft(4, '0') : null;
                songs[i] = new T()
                {
                    LevelId = levelId,
                    Hash = hash,
                    Key = key,
                    Name = !string.IsNullOrEmpty(songNamePrefix) ? $"{songNamePrefix}{i}" : null,
                    LevelAuthorName = !string.IsNullOrEmpty(levelAuthorPrefix) ? $"{levelAuthorPrefix}{i}" : null
                };
            }
            return songs;
        }
    }
}
