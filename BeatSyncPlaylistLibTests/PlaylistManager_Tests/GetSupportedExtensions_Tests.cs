using BeatSaberPlaylistsLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeatSaberPlaylistsLibTests.PlaylistManager_Tests
{
    [TestClass]
    public class GetSupportedExtensions_Tests
    {
        private static readonly string OutputPath = Path.Combine(TestTools.OutputFolder, "GetSupportedExtensions");
        [TestMethod]
        public void Constructor()
        {
            string playlistsPath = Path.Combine(OutputPath, "Constructor");
            PlaylistManager manager = new PlaylistManager(playlistsPath);
            Assert.IsTrue(manager.GetSupportedExtensions().Length > 0);
        }
    }
}
