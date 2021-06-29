#if BeatSaber
extern alias BeatSaber;
using System;
using System.Collections.Generic;
using System.Text;
using BeatSaberPlaylistsLib.Types;

namespace BeatSaberPlaylistsLib.Services
{
    public interface ISongMatcher
    {
        bool MatchSong(ISong song);
    }

    public class SongCoreMatcher : ISongMatcher
    {
        public bool MatchSong(ISong song)
        {

            var _previewBeatmapLevel = SongCore.Loader.GetLevelById(song.LevelId);
            song.SetPreviewBeatmap(_previewBeatmapLevel);
            return _previewBeatmapLevel != null;
        }
    }
}
#endif