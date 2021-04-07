using BeatSaberPlaylistsLib;
using BeatSaberPlaylistsLib.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatSaberPlaylistsLibTests.Mock
{
    public class MockPlaylistSong : PlaylistSong
    {
        public MockPlaylistSong()
            : base()
        { }

        public MockPlaylistSong(string? hash = null, string? levelId = null, string? songName = null, string? songKey = null, string? mapper = null)
            : base()
        {
            if (hash != null && hash.Length > 0 && levelId != null && levelId.Length > 0)
            {
                if (levelId.StartsWith(PlaylistSong.CustomLevelIdPrefix, StringComparison.OrdinalIgnoreCase)
                    && !levelId.EndsWith(hash))
                    throw new ArgumentException(nameof(levelId), "CustomLevel levelId and hash do not match.");
            }
            LevelId = levelId;
            if (hash != null) // Don't assign null, LevelId could've set Hash
                Hash = hash;
            Name = songName;
            Key = songKey;
            LevelAuthorName = mapper;
            DateAdded = Utilities.CurrentTime;
        }
        public override bool Equals(IPlaylistSong? other)
        {
            if (other == null)
                return false;
            return Hash == other?.Hash;
        }
    }
}
