using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLib.Types;
using BeatSaberPlaylistsLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BeatSaberPlaylistsLibTests.Mock;

namespace BeatSaberPlaylistsLibTests.PlaylistLibExtension_Tests
{
    [TestClass]
    public class FileReadWrite_Tests
    {
        private static readonly string PlaylistSource = Path.Combine(TestTools.DataFolder, "GetPlaylistTests");
        private static readonly string OutputPath = Path.Combine(TestTools.OutputFolder, "FileReadWrite_Tests");

        [TestMethod]
        public void BackupExists()
        {
            string directory = Path.Combine(OutputPath, "BackupExists");
            string backupFile = "5LegacySongs.bPlist" + ".bak";
            string badFile = "InvalidJson.bPlist";
            string backupSource = Path.Combine(PlaylistSource, Path.GetFileNameWithoutExtension(backupFile));
            string badFileSource = Path.Combine(PlaylistSource, badFile);
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
            Directory.CreateDirectory(directory);
            backupFile = Path.Combine(directory, backupFile);
            badFile = Path.Combine(directory, "5LegacySongs.bPlist");
            File.Copy(backupSource, backupFile);
            File.Copy(badFileSource, badFile);
            Assert.IsTrue(File.Exists(backupFile));
            Assert.IsTrue(File.Exists(badFile));
            IPlaylistHandler handler = new DerivedLegacyPlaylistHandler();
            IPlaylist? playlist = handler.Deserialize(badFile);
            Assert.IsNotNull(playlist);
            Assert.AreEqual(typeof(DerivedLegacyPlaylist), playlist.GetType());
        }
    }
}
