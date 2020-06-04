using BeatSaberPlaylistsLib.Types;
using BeatSaberPlaylistsLibTests.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberPlaylistsLibTests.IPlaylistSong_Tests.Mock.PlaylistSong
{
    [TestClass]
    public class Construction
    {
        public static readonly Func<string?, string?, string?, string?, string?, IPlaylistSong> PlaylistSongFactory =
            (hash, levelId, songName, key, levelAuthorName) => new MockPlaylistSong(hash, levelId, songName, key, levelAuthorName);
        public static readonly IPlaylistSongTestRunners<MockPlaylistSong> TestRunner 
            = new IPlaylistSongTestRunners<MockPlaylistSong>(PlaylistSongFactory);
        #region Using Constructor
        [TestMethod]
        public void HashOnly_ctor()
        {
            TestRunner.HashOnly_ctor();
        }

        [TestMethod]
        public void LevelIdOnly_ctor()
        {
            TestRunner.LevelIdOnly_ctor();
        }

        [TestMethod]
        public void KeyOnly_ctor()
        {
            TestRunner.KeyOnly_ctor();
        }

        [TestMethod]
        public void HashAndLevelId_NotMatched_ctor()
        {
            TestRunner.HashAndLevelId_NotMatched_ctor();
        }

        [TestMethod]
        public void NoIdentifiers_ctor()
        {
            TestRunner.NoIdentifiers_ctor();
        }

#endregion
        #region Default Constructor with Property Assignments
        [TestMethod]
        public void HashOnly()
        {
            TestRunner.HashOnly();
        }

        [TestMethod]
        public void HashOnly_HashFirst()
        {
            TestRunner.HashOnly_HashFirst();
        }

        [TestMethod]
        public void LevelIdOnly()
        {
            TestRunner.LevelIdOnly();
        }

        [TestMethod]
        public void LevelIdOnly_NullHashFirst()
        {
            TestRunner.LevelIdOnly_NullHashFirst();
        }

        [TestMethod]
        public void KeyOnly()
        {
            TestRunner.KeyOnly();
        }

        [TestMethod]
        public void HashAndLevelId_NotMatched()
        {
            TestRunner.HashAndLevelId_NotMatched();
        }

        [TestMethod]
        public void HashAndLevelId_NotMatched_HashFirst()
        {
            TestRunner.HashAndLevelId_NotMatched_HashFirst();
        }

        [TestMethod]
        public void NoIdentifiers()
        {
            TestRunner.NoIdentifiers();
        }
        #endregion
    }
}
