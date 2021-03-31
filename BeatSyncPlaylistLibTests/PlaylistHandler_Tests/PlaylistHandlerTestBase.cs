using BeatSaberPlaylistsLib.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeatSaberPlaylistsLibTests.PlaylistHandler_Tests
{
    public abstract class PlaylistHandlerTestBase
    {
        public abstract string ReadOnlyData { get; }
        public abstract string OutputPath { get; }
        protected void TestDeserializedExtraData(IPlaylistHandler handler, string playlistFile)
        {
            string outputFile = Path.Combine(OutputPath, "SavedExtraData.bplist");
            Directory.CreateDirectory(OutputPath);
            if (File.Exists(outputFile))
                File.Delete(outputFile);
            Assert.IsTrue(File.Exists(playlistFile), $"Could not find test file at '{playlistFile}'");

            using var fs = File.OpenRead(playlistFile);
            IPlaylist playlist = handler.Deserialize(fs);
            Assert.IsTrue(playlist.TryGetCustomData("extensionStringList", out object extStrList));
            if (extStrList is JArray)
            {
                Assert.Fail("Failed to convert string list.");
            }
            Assert.IsTrue(playlist.TryGetCustomData("extensionIntList", out object extIntList));
            if (extIntList is JArray)
            {
                Assert.Fail("Failed to convert int list.");
            }

            Assert.IsTrue(playlist.TryGetCustomData("customInt", out object customInt));
            Assert.AreEqual(123, Convert.ToInt32(customInt));
            Assert.IsTrue(playlist.TryGetCustomData("extensionString", out object extensionString));
            Assert.AreEqual("test string", extensionString);
            Assert.IsTrue(playlist.TryGetCustomData("extensionFloat", out object extensionFloat));
            Assert.AreEqual(123.456d, extensionFloat);
            using var fs2 = File.Create(outputFile);
            handler.Serialize(playlist, fs2);
        }
    }
}
