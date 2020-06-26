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
        
        static IPlaylistHandlerTests()
        {
            Directory.CreateDirectory(ReadOnlyData);
            Directory.CreateDirectory(OutputDirectory);
        }
    }
}
