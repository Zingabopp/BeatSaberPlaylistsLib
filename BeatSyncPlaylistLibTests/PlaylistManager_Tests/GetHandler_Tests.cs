using BeatSaberPlaylistsLib;
using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLib.Types;
using BeatSaberPlaylistsLibTests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BeatSaberPlaylistsLibTests.PlaylistManager_Tests
{
    [TestClass]
    public class GetHandler_Tests
    {
        public static readonly string OutputPath = Path.Combine(TestTools.OutputFolder, "PlaylistManager_Tests", "GetHandler_Tests");

        [TestMethod]
        public void MultipleExtensions_NotDefault()
        {
            string playlistDir = Path.Combine(OutputPath, "MultipleExtensions_NotDefault");
            IPlaylistHandler expectedHandler = new LegacyPlaylistHandler();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir, new MockPlaylistHandler());
            Assert.IsNull(manager.GetHandlerForExtension("bplist"));

            Assert.IsTrue(manager.RegisterHandler(expectedHandler));
            Assert.IsTrue(Directory.Exists(playlistDir));
            Assert.AreEqual(typeof(MockPlaylistHandler), manager.DefaultHandler?.GetType());
            Assert.AreEqual(expectedHandler, manager.GetHandlerForExtension("bplist"));
            Assert.AreEqual(expectedHandler, manager.GetHandlerForExtension("JsOn"));

            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }
    }
}
