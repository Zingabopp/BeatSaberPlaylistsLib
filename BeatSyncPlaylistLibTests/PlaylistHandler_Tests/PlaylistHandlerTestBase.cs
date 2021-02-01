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
            Dictionary<string, object> customData = playlist.CustomData ?? new Dictionary<string, object>();
            if (customData["extensionStringList"] is JArray)
            {
                Assert.Fail("Failed to convert string list.");
            }
            if (customData["extensionIntList"] is JArray)
            {
                Assert.Fail("Failed to convert int list.");
            }
            Assert.AreEqual(123, Convert.ToInt32(customData["customInt"]));
            Assert.AreEqual("test string", customData["extensionString"]);
            Assert.AreEqual(123.456d, customData["extensionFloat"]);
            using var fs2 = File.Create(outputFile);
            handler.Serialize(playlist, fs2);
        }
    }
}
