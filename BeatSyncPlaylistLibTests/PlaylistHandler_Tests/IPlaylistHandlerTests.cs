using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BeatSaberPlaylistsLib;
using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLib.Types;
using BeatSaberPlaylistsLibTests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeatSaberPlaylistsLibTests.PlaylistHandler_Tests
{
    public static class IPlaylistHandlerTests
    {
        public static readonly string ReadOnlyData = Path.Combine(TestTools.DataFolder, "IPlaylistHandler_Tests");
        public static readonly string OutputDirectory = Path.Combine(TestTools.OutputFolder, "IPlaylistHandler_Tests");
        public static T[] CreateSongArray<T>(string songNamePrefix, string levelAuthorPrefix, int size, Identifier useIdentifiers = Identifier.Hash) where T : IPlaylistSong, new()
        {
            T[] songs = new T[size];
            for (int i = 0; i < size; i++)
            {
                
                string? levelId = useIdentifiers.HasFlag(Identifier.LevelId) ? $"custom_level_" + $"ABC{i}".PadLeft(40, '0') : null;
                string? hash = useIdentifiers.HasFlag(Identifier.LevelId) ? $"ABC{i}".PadLeft(40, '0') : null;
                string? key = useIdentifiers.HasFlag(Identifier.LevelId) ? $"{i}".PadLeft(4, '0') : null;
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
        static IPlaylistHandlerTests()
        {
            Directory.CreateDirectory(ReadOnlyData);
            Directory.CreateDirectory(OutputDirectory);
        }
    }
}
