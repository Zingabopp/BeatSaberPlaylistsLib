using BeatSaberPlaylistsLib;
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


        public static PlaylistManager GetPlaylistManager(string playlistDir, IPlaylistHandler playlistHandler)
        {
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir, playlistHandler);
            Assert.IsTrue(Directory.Exists(playlistDir));
            return manager;
        }

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
    }
}
