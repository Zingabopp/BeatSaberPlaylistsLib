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
            string? hash = "sDFKLjSDLFKJ";
            string? levelId = null;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            string expectedHash = hash.ToUpper();
            string expectedLevelId = PlaylistSong.CustomLevelIdPrefix + expectedHash;
            IPlaylistSong songA = CreatePlaylistSongWithFactory(PlaylistSongFactory, hash, levelId, songName, key, levelAuthorName);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash), "Song should have Hash Identifier.");
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should have LevelId Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key), "Song should not have Key Identifier.");
            Assert.IsFalse(songA.Identifiers == Identifier.None, "Song should not have None Identifier.");
            Assert.AreEqual(expectedHash, songA.Hash, "Song did not capitalize Hash.");
            Assert.IsTrue(expectedLevelId.Equals(songA.LevelId, StringComparison.OrdinalIgnoreCase), "Song did not assign the correct LevelId.");
            Assert.AreEqual(expectedLevelId, songA.LevelId, "Song did not capitalize the hash part of the LevelId.");
        }

        
        public void LevelIdOnly_ctor()
        {
            string songHash = "LSKDFJLKJSDf";
            string? hash = null;
            string? levelId = PlaylistSong.CustomLevelIdPrefix + songHash;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            string expectedHash = songHash.ToUpper();
            string expectedLevelId = PlaylistSong.CustomLevelIdPrefix + expectedHash;
            IPlaylistSong songA = CreatePlaylistSongWithFactory(PlaylistSongFactory, hash, levelId, songName, key, levelAuthorName);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash), "Song should have Hash Identifier.");
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should have LevelId Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key), "Song should not have Key Identifier.");
            Assert.IsFalse(songA.Identifiers == Identifier.None, "Song should not have None Identifier.");
            Assert.AreEqual(expectedLevelId, songA.LevelId, "Song did not capitalize the hash part of the LevelId.");
            Assert.IsTrue(expectedHash.Equals(songA.Hash, StringComparison.OrdinalIgnoreCase), "Song did not assign the correct LevelId.");
            Assert.AreEqual(expectedHash, songA.Hash, "Song did not capitalize Hash.");
        }

        
        public void KeyOnly_ctor()
        {
            string? hash = null;
            string? levelId = null;
            string? key = "abcD";
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            string expectedKey = key.ToUpper();
            IPlaylistSong songA = CreatePlaylistSongWithFactory(PlaylistSongFactory, hash, levelId, songName, key, levelAuthorName);
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Hash), "Song should not have Hash Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should not have LevelId Identifier.");
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Key), "Song should have Key Identifier.");
            Assert.IsFalse(songA.Identifiers == Identifier.None, "Song should not have None Identifier.");
            Assert.IsTrue(expectedKey.Equals(songA.Key, StringComparison.OrdinalIgnoreCase), "Song did not assign the Key correctly.");
            Assert.AreEqual(expectedKey, songA.Key, "Song did not capitalize Key.");
        }

        
        public void HashAndLevelId_NotMatched_ctor()
        {
            string? hash = "SDFKLJSDLFKj";
            string? levelId = PlaylistSong.CustomLevelIdPrefix + hash + "d";
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            string expectedHash = hash.ToUpper() + "D";
            string expectedLevelId = PlaylistSong.CustomLevelIdPrefix + expectedHash + "D";
            Assert.ThrowsException<ArgumentException>(() => CreatePlaylistSongWithFactory(PlaylistSongFactory, hash, levelId, songName, key, levelAuthorName));
        }

        
        public void NoIdentifiers_ctor()
        {
            string? hash = null;
            string? levelId = null;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            IPlaylistSong songA = CreatePlaylistSongWithFactory(PlaylistSongFactory, hash, levelId, songName, key, levelAuthorName);
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Hash), "Song should not have Hash Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should not have LevelId Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key), "Song should not have Key Identifier.");
            Assert.IsTrue(songA.Identifiers == Identifier.None, "Song should have None Identifier.");
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
            bool assignHashFirst = false;
            string expectedHash = hash.ToUpper();
            string expectedLevelId = PlaylistSong.CustomLevelIdPrefix + expectedHash;
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName, assignHashFirst);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash), "Song should have Hash Identifier.");
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should have LevelId Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key), "Song should not have Key Identifier.");
            Assert.IsFalse(songA.Identifiers == Identifier.None, "Song should not have None Identifier.");
            Assert.AreEqual(expectedHash, songA.Hash, "Song did not capitalize Hash.");
            Assert.IsTrue(expectedLevelId.Equals(songA.LevelId, StringComparison.OrdinalIgnoreCase), "Song did not assign the correct LevelId.");
            Assert.AreEqual(expectedLevelId, songA.LevelId, "Song did not capitalize the hash part of the LevelId.");
        }

        
        public void HashOnly_HashFirst()
        {
            string? hash = "SDFKLjSDLFKJ";
            string? levelId = null;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            bool assignHashFirst = true;
            string expectedHash = hash.ToUpper();
            string expectedLevelId = PlaylistSong.CustomLevelIdPrefix + expectedHash;
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName, assignHashFirst);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash), "Song should have Hash Identifier.");
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should have LevelId Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key), "Song should not have Key Identifier.");
            Assert.IsFalse(songA.Identifiers == Identifier.None, "Song should not have None Identifier.");
            Assert.AreEqual(expectedHash, songA.Hash, "Song did not capitalize Hash.");
            Assert.IsTrue(expectedLevelId.Equals(songA.LevelId, StringComparison.OrdinalIgnoreCase), "Song did not assign the correct LevelId.");
            Assert.AreEqual(expectedLevelId, songA.LevelId, "Song did not capitalize the hash part of the LevelId.");
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
            string expectedHash = songHash.ToUpper();
            string expectedLevelId = PlaylistSong.CustomLevelIdPrefix + expectedHash;
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName, assignHashFirst);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash), "Song should have Hash Identifier.");
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should have LevelId Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key), "Song should not have Key Identifier.");
            Assert.IsFalse(songA.Identifiers == Identifier.None, "Song should not have None Identifier.");
            Assert.AreEqual(expectedLevelId, songA.LevelId, "Song did not capitalize the hash part of the LevelId.");
            Assert.IsTrue(expectedHash.Equals(songA.Hash, StringComparison.OrdinalIgnoreCase), "Song did not assign the correct LevelId.");
            Assert.AreEqual(expectedHash, songA.Hash, "Song did not capitalize Hash.");
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
            string expectedHash = songHash.ToUpper();
            string expectedLevelId = PlaylistSong.CustomLevelIdPrefix + expectedHash;
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName, assignHashFirst);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash), "Song should have Hash Identifier.");
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should have LevelId Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key), "Song should not have Key Identifier.");
            Assert.IsFalse(songA.Identifiers == Identifier.None, "Song should not have None Identifier.");
            Assert.AreEqual(expectedLevelId, songA.LevelId, "Song did not capitalize the hash part of the LevelId.");
            Assert.IsTrue(expectedHash.Equals(songA.Hash, StringComparison.OrdinalIgnoreCase), "Song did not assign the correct LevelId.");
            Assert.AreEqual(expectedHash, songA.Hash, "Song did not capitalize Hash.");
        }


        public void LevelIdOnly_EmptyHash()
        {
            string songHash = "LSKDFJLKJSDf";
            string? hash = "";
            string? levelId = PlaylistSong.CustomLevelIdPrefix + songHash;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            bool assignHashFirst = false;
            string expectedHash = songHash.ToUpper();
            string expectedLevelId = PlaylistSong.CustomLevelIdPrefix + expectedHash;
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName, assignHashFirst);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash), "Song should have Hash Identifier.");
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should have LevelId Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key), "Song should not have Key Identifier.");
            Assert.IsFalse(songA.Identifiers == Identifier.None, "Song should not have None Identifier.");
            Assert.AreEqual(expectedLevelId, songA.LevelId, "Song did not capitalize the hash part of the LevelId.");
            Assert.IsTrue(expectedHash.Equals(songA.Hash, StringComparison.OrdinalIgnoreCase), "Song did not assign the correct LevelId.");
            Assert.AreEqual(expectedHash, songA.Hash, "Song did not capitalize Hash.");
        }

        public void LevelIdOnly_EmptyHashFirst()
        {
            string songHash = "LSKDFJLKJSDf";
            string? hash = "";
            string? levelId = PlaylistSong.CustomLevelIdPrefix + songHash;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            bool assignHashFirst = true;
            string expectedHash = songHash.ToUpper();
            string expectedLevelId = PlaylistSong.CustomLevelIdPrefix + expectedHash;
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName, assignHashFirst);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash), "Song should have Hash Identifier.");
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should have LevelId Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key), "Song should not have Key Identifier.");
            Assert.IsFalse(songA.Identifiers == Identifier.None, "Song should not have None Identifier.");
            Assert.AreEqual(expectedLevelId, songA.LevelId, "Song did not capitalize the hash part of the LevelId.");
            Assert.IsTrue(expectedHash.Equals(songA.Hash, StringComparison.OrdinalIgnoreCase), "Song did not assign the correct LevelId.");
            Assert.AreEqual(expectedHash, songA.Hash, "Song did not capitalize Hash.");
        }

        public void KeyOnly()
        {
            string? hash = null;
            string? levelId = null;
            string? key = "abcD";
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName);
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Hash), "Song should not have Hash Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should not have LevelId Identifier.");
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Key), "Song should have Key Identifier.");
            Assert.IsFalse(songA.Identifiers == Identifier.None, "Song should not have None Identifier.");
            Assert.AreEqual(key.ToUpper(), songA.Key, "Song did not capitalize Key.");
        }

        
        public void HashAndLevelId_NotMatched()
        {
            string? hash = "sDFKLJSDLFKJ";
            string? levelId = PlaylistSong.CustomLevelIdPrefix + hash + "D";
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            bool assignHashFirst = false;
            string expectedHash = hash.ToUpper();
            string expectedLevelId = PlaylistSong.CustomLevelIdPrefix + hash.ToUpper();
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName, assignHashFirst);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash), "Song should have Hash Identifier.");
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should have LevelId Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key), "Song should not have Key Identifier.");
            Assert.IsFalse(songA.Identifiers == Identifier.None, "Song should not have None Identifier.");
            Assert.AreEqual(expectedHash, songA.Hash, "Song did not keep the original Hash.");
            Assert.AreEqual(expectedLevelId, songA.LevelId, "Song did not change LevelId to match Hash.");
        }

        
        public void HashAndLevelId_NotMatched_HashFirst()
        {
            string? hash = "SDFKLJSDLFKj";
            string? levelId = PlaylistSong.CustomLevelIdPrefix + hash + "D";
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            bool assignHashFirst = true;
            string expectedHash = hash.ToUpper() + "D";
            string expectedLevelId = PlaylistSong.CustomLevelIdPrefix + hash.ToUpper() + "D";
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName, assignHashFirst);
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.Hash), "Song should have Hash Identifier.");
            Assert.IsTrue(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should have LevelId Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key), "Song should not have Key Identifier.");
            Assert.IsFalse(songA.Identifiers == Identifier.None, "Song should not have None Identifier.");
            Assert.AreEqual(expectedHash, songA.Hash, "Song did not change hash to match LevelId.");
            Assert.AreEqual(expectedLevelId, songA.LevelId, "Song did not keep the original LevelId.");
        }

        
        public void NoIdentifiers()
        {
            string? hash = null;
            string? levelId = null;
            string? key = null;
            string? songName = "Test";
            string? levelAuthorName = "TestMapper";
            IPlaylistSong songA = CreatePlaylistSong<LegacyPlaylistSong>(hash, levelId, songName, key, levelAuthorName);
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Hash), "Song should not have Hash Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.LevelId), "Song should not have LevelId Identifier.");
            Assert.IsFalse(songA.Identifiers.HasFlag(Identifier.Key), "Song should not have Key Identifier.");
            Assert.IsTrue(songA.Identifiers == Identifier.None, "Song should have None Identifier.");
        }
        #endregion
    }
}
