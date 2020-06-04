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
    public class RegisterHandlerForExtensionTests
    {
        public static readonly string OutputPath = Path.Combine(TestTools.OutputFolder, "PlaylistManager_Tests", "RegisterHandlerForExtensionTests");

        [TestMethod]
        public void NewHandler()
        {
            string playlistDir = Path.Combine(OutputPath, "NewHandler");
            IPlaylistHandler expectedHandler = new LegacyPlaylistHandler();

            PlaylistManager manager = TestTools.GetPlaylistManager(playlistDir, new MockPlaylistHandler());
            Assert.IsNull(manager.GetHandlerForExtension("bplist"));
            Assert.IsNull(manager.GetHandlerForExtension("json"));

            manager.RegisterHandlerForExtension("json", expectedHandler);
            
            Assert.AreEqual(typeof(MockPlaylistHandler), manager.DefaultHandler.GetType());
            Assert.AreEqual(expectedHandler, manager.GetHandlerForExtension("json"));
            Assert.IsNull(manager.GetHandlerForExtension("bplist"));

            TestTools.Cleanup(playlistDir);
        }

        [TestMethod]
        public void ExistingHandlerType()
        {
            string playlistDir = Path.Combine(OutputPath, "ExistingHandlerType");
            IPlaylistHandler existingHandler = new LegacyPlaylistHandler();
            IPlaylistHandler expectedHandler = new LegacyPlaylistHandler();
            string extension = "json";

            PlaylistManager manager = TestTools.GetPlaylistManager(playlistDir, existingHandler);

            manager.RegisterHandlerForExtension(extension, expectedHandler);
            IPlaylistHandler? actualHandler = manager.GetHandlerForExtension(extension);
            IPlaylistHandler? bplistHandler = manager.GetHandlerForExtension("bplist");

            Assert.IsTrue(Directory.Exists(playlistDir));
            Assert.AreEqual(expectedHandler, actualHandler);
            Assert.IsNotNull(bplistHandler);
            Assert.AreNotEqual(actualHandler, bplistHandler);

            TestTools.Cleanup(playlistDir);
        }
        [TestMethod]
        public void NullExtension()
        {
            string playlistDir = Path.Combine(OutputPath, "NullExtension");
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
        public void UnsupportedExtension()
        {
            string playlistDir = Path.Combine(OutputPath, "UnsupportedExtension");
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
        public void NullHandler()
        {
            string playlistDir = Path.Combine(OutputPath, "NullHandler");
            IPlaylistHandler handler = new LegacyPlaylistHandler();
            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
            Assert.IsFalse(Directory.Exists(playlistDir));

            PlaylistManager manager = new PlaylistManager(playlistDir, handler);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsException<ArgumentNullException>(() => manager.RegisterHandlerForExtension("mock", null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            if (Directory.Exists(playlistDir))
                Directory.Delete(playlistDir, true);
        }
    }
}
