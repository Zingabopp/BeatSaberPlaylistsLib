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

            manager.RegisterHandler(expectedHandler);
            Assert.IsTrue(Directory.Exists(playlistDir));
            Assert.AreEqual(typeof(MockPlaylistHandler), manager.DefaultHandler.GetType());
            Assert.AreEqual(expectedHandler, manager.GetHandlerForExtension("bplist"));
            Assert.AreEqual(expectedHandler, manager.GetHandlerForExtension("JsOn"));

            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }

        [TestMethod]
        public void RegisterHandlerForExtension_NewHandler()
        {
            string playlistDir = Path.Combine(OutputPath, "RegisterHandlerForExtension_NewHandler");
            IPlaylistHandler expectedHandler = new LegacyPlaylistHandler();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir, new MockPlaylistHandler());
            Assert.IsNull(manager.GetHandlerForExtension("bplist"));
            Assert.IsNull(manager.GetHandlerForExtension("json"));

            manager.RegisterHandlerForExtension("json", expectedHandler);
            Assert.IsTrue(Directory.Exists(playlistDir));
            Assert.AreEqual(typeof(MockPlaylistHandler), manager.DefaultHandler.GetType());
            Assert.AreEqual(expectedHandler, manager.GetHandlerForExtension("json"));
            Assert.IsNull(manager.GetHandlerForExtension("bplist"));

            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }

        [TestMethod]
        public void RegisterHandlerForExtension_ExistingHandler()
        {
            string playlistDir = Path.Combine(OutputPath, "RegisterHandlerForExtension_ExistingHandler");
            IPlaylistHandler expectedHandler = new LegacyPlaylistHandler();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir, expectedHandler);

            manager.RegisterHandlerForExtension("json", expectedHandler);
            Assert.IsTrue(Directory.Exists(playlistDir));
            Assert.AreEqual(typeof(LegacyPlaylistHandler), manager.DefaultHandler.GetType());
            Assert.AreEqual(expectedHandler, manager.GetHandlerForExtension("json"));
            Assert.AreEqual(expectedHandler, manager.GetHandlerForExtension("bplist"));

            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }
        [TestMethod]
        public void RegisterHandlerForExtension_NullExtension()
        {
            string playlistDir = Path.Combine(OutputPath, "RegisterHandlerForExtension_NullExtension");
            IPlaylistHandler expectedHandler = new LegacyPlaylistHandler();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir, expectedHandler);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsException<ArgumentNullException>(() => manager.RegisterHandlerForExtension(null, expectedHandler));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsException<ArgumentNullException>(() => manager.RegisterHandlerForExtension("", expectedHandler));

            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }

        [TestMethod]
        public void RegisterHandlerForExtension_UnsupportedExtension()
        {
            string playlistDir = Path.Combine(OutputPath, "RegisterHandlerForExtension_UnsupportedExtension");
            IPlaylistHandler handler = new LegacyPlaylistHandler();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir, handler);

            Assert.ThrowsException<ArgumentException>(() => manager.RegisterHandlerForExtension("mock", handler));

            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }

        [TestMethod]
        public void RegisterHandlerForExtension_NullHandler()
        {
            string playlistDir = Path.Combine(OutputPath, "RegisterHandlerForExtension_NullHandler");
            IPlaylistHandler handler = new LegacyPlaylistHandler();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir, handler);

            Assert.ThrowsException<ArgumentNullException>(() => manager.RegisterHandlerForExtension("mock", null));

            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }
    }
}
