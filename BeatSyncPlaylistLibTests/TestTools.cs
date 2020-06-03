using BeatSaberPlaylistsLib;
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

            return manager;
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
