﻿using BeatSaberPlaylistsLib.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeatSaberPlaylistsLibTests.Mock
{
    public class MockPlaylist : Playlist<MockPlaylistSong>
    {
        public override string Title { get; set; } = string.Empty;
        public override string? Author { get; set; }
        public override string? Description { get; set; }
        public override string Filename { get; set; } = string.Empty;

        public override Stream GetCoverStream()
        {
            throw new NotImplementedException();
        }

        public override int RemoveAll(Func<MockPlaylistSong, bool> match)
        {
            int removedSongs = 0;
            if (match != null)
                removedSongs = ((List<MockPlaylistSong>)Songs).RemoveAll(s => match(s));
            return removedSongs;
        }

        public override bool HasCover => false;


        public override void SetCover(byte[] coverImage)
        {
            throw new NotImplementedException();
        }

        public override void SetCover(string? coverImageStr)
        {
            throw new NotImplementedException();
        }

        public override void SetCover(Stream stream)
        {
            throw new NotImplementedException();
        }

        protected override MockPlaylistSong CreateWith(ISong song)
        {
            return new MockPlaylistSong()
            {
                DateAdded = DateTime.Now,
                LevelId = song.LevelId,
                Hash = song.Hash,
                Key = song.Key,
                LevelAuthorName = song.LevelAuthorName,
                Name = song.Name
            };
        }

        ///<inheritdoc/>
        protected override MockPlaylistSong CreateWithHash(string songHash, string? songName, string? songKey, string? mapper)
        {
            return new MockPlaylistSong()
            {
                Hash = songHash,
                Name = songName,
                Key = songKey,
                LevelAuthorName = mapper
            };
        }

        ///<inheritdoc/>
        protected override MockPlaylistSong CreateWithLevelId(string levelId, string? songName, string? songKey, string? mapper)
        {
            return new MockPlaylistSong()
            {
                LevelId = levelId,
                Name = songName,
                Key = songKey,
                LevelAuthorName = mapper
            };
        }
    }
}
