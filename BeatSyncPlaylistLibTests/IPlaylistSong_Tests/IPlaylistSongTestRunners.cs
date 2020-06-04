using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLib.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static BeatSaberPlaylistsLibTests.TestTools;

namespace BeatSaberPlaylistsLibTests.IPlaylistSong_Tests
{
    public class IPlaylistSongTestRunners<T> where T: IPlaylistSong, new()
    {
        public readonly Func<string?, string?, string?, string?, string?, IPlaylistSong> PlaylistSongFactory =
            (hash, levelId, songName, key, levelAuthorName) => new LegacyPlaylistSong(hash, levelId, songName, key, levelAuthorName);

        public IPlaylistSongTestRunners(Func<string?, string?, string?, string?, string?, IPlaylistSong> playlistSongFactory)
        {
            PlaylistSongFactory = playlistSongFactory;
        }

        #region Using Constructor
        public void HashOnly_ctor()
        {
            string? hash = "SDFKLjSDLFKJ";
            string? levelId = null;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            IPlaylistSong songA = CreatePlaylistSongWithFactory(PlaylistSongFactory, hash, levelId, songName, key, levelAuthorName);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.AreEqual(hash.ToUpper(), songA.Hash);
            Assert.AreEqual(PlaylistSong.CustomLevelIdPrefix + hash.ToUpper(), songA.LevelId);
        }

        
        public void LevelIdOnly_ctor()
        {
            string songHash = "LSKDFJLKJSDf";
            string? hash = null;
            string? levelId = PlaylistSong.CustomLevelIdPrefix + songHash;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            IPlaylistSong songA = CreatePlaylistSongWithFactory(PlaylistSongFactory, hash, levelId, songName, key, levelAuthorName);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.AreEqual(PlaylistSong.CustomLevelIdPrefix + songHash.ToUpper(), songA.LevelId);
            Assert.AreEqual(songHash.ToUpper(), songA.Hash);
        }

        
        public void KeyOnly_ctor()
        {
            string? hash = null;
            string? levelId = null;
            string? key = "abcD";
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            IPlaylistSong songA = CreatePlaylistSongWithFactory(PlaylistSongFactory, hash, levelId, songName, key, levelAuthorName);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.AreEqual(key.ToUpper(), songA.Key);
        }

        
        public void HashAndLevelId_NotMatched_ctor()
        {
            string? hash = "SDFKLJSDLFKJ";
            string? levelId = PlaylistSong.CustomLevelIdPrefix + hash + "D";
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            IPlaylistSong songA = CreatePlaylistSongWithFactory(PlaylistSongFactory, hash, levelId, songName, key, levelAuthorName);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.AreEqual(hash, songA.Hash);
            Assert.AreEqual(levelId, songA.LevelId);
        }

        
        public void NoIdentifiers_ctor()
        {
            string? hash = null;
            string? levelId = null;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            IPlaylistSong songA = CreatePlaylistSongWithFactory(PlaylistSongFactory, hash, levelId, songName, key, levelAuthorName);
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.IsTrue(songA.Identifiers == Identifier.None);
        }

        #endregion
        #region Default Constructor with Property Assignments
        
        public void HashOnly()
        {
            string? hash = "SDFKLjSDLFKJ";
            string? levelId = null;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.AreEqual(hash.ToUpper(), songA.Hash);
            Assert.AreEqual(PlaylistSong.CustomLevelIdPrefix + hash.ToUpper(), songA.LevelId);
        }

        
        public void HashOnly_HashFirst()
        {
            string? hash = "SDFKLjSDLFKJ";
            string? levelId = null;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            bool assignHashFirst = true;
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName, assignHashFirst);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.AreEqual(hash.ToUpper(), songA.Hash);
            Assert.AreEqual(PlaylistSong.CustomLevelIdPrefix + hash.ToUpper(), songA.LevelId);
        }

        
        public void LevelIdOnly()
        {
            string songHash = "LSKDFJLKJSDf";
            string? hash = null;
            string? levelId = PlaylistSong.CustomLevelIdPrefix + songHash;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            bool assignHashFirst = false;
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName, assignHashFirst);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.IsFalse(songA.Identifiers == Identifier.None);
            Assert.AreEqual(PlaylistSong.CustomLevelIdPrefix + songHash.ToUpper(), songA.LevelId);
            Assert.AreEqual(songHash.ToUpper(), songA.Hash);
        }

        
        public void LevelIdOnly_NullHashFirst()
        {
            string songHash = "LSKDFJLKJSDf";
            string? hash = null;
            string? levelId = PlaylistSong.CustomLevelIdPrefix + songHash;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            bool assignHashFirst = true;
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName, assignHashFirst);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.IsFalse(songA.Identifiers == Identifier.None);
            Assert.AreEqual(PlaylistSong.CustomLevelIdPrefix + songHash.ToUpper(), songA.LevelId);
            Assert.AreEqual(songHash.ToUpper(), songA.Hash);
        }

        
        public void KeyOnly()
        {
            string? hash = null;
            string? levelId = null;
            string? key = "abcD";
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.IsFalse(songA.Identifiers == Identifier.None);
            Assert.AreEqual(key.ToUpper(), songA.Key);
        }

        
        public void HashAndLevelId_NotMatched()
        {
            string? hash = "SDFKLJSDLFKJ";
            string? levelId = PlaylistSong.CustomLevelIdPrefix + hash + "D";
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.IsFalse(songA.Identifiers == Identifier.None);
            Assert.AreEqual(hash, songA.Hash);
            Assert.AreEqual(levelId, songA.LevelId);
        }

        
        public void HashAndLevelId_NotMatched_HashFirst()
        {
            string? hash = "SDFKLJSDLFKJ";
            string? levelId = PlaylistSong.CustomLevelIdPrefix + hash + "D";
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            bool assignHashFirst = true;
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName, assignHashFirst);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.IsFalse(songA.Identifiers == Identifier.None);
            Assert.AreEqual(hash, songA.Hash);
            Assert.AreEqual(levelId, songA.LevelId);
        }

        
        public void NoIdentifiers()
        {
            string? hash = null;
            string? levelId = null;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName);
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Hash));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.LevelId));
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key));
            Assert.IsTrue(songA.Identifiers == Identifier.None);
        }
        #endregion
    }
}
