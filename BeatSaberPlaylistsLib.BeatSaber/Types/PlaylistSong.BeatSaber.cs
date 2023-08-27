#if BeatSaber
extern alias BeatSaber;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeatSaberPlaylistsLib.Types
{
    public abstract partial class PlaylistSong : IPlaylistSong
    {
    [NonSerialized]
        private BeatSaber.IPreviewBeatmapLevel? _previewBeatmapLevel;
        ///<inheritdoc/>
        [IgnoreDataMember]
        public BeatSaber.IPreviewBeatmapLevel? PreviewBeatmapLevel
        {
            get
            {
                if (_previewBeatmapLevel != null)
                    return _previewBeatmapLevel;
                if (LevelId == null || LevelId.Length == 0)
                    return null;
                _previewBeatmapLevel = SongCore.Loader.GetLevelById(LevelId);
                return _previewBeatmapLevel;
            }
            internal set => _previewBeatmapLevel = value;
        }

        void ISong.SetPreviewBeatmap(BeatSaber.IPreviewBeatmapLevel beatmap)
        {
            PreviewBeatmapLevel = beatmap;
        }
#region IPreviewBeatmapLevel passthrough
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        [IgnoreDataMember]
        string? BeatSaber.IPreviewBeatmapLevel.levelID
            => PreviewBeatmapLevel?.levelID ?? this.LevelId;

        [IgnoreDataMember]
        string? BeatSaber.IPreviewBeatmapLevel.songName
            => PreviewBeatmapLevel?.songName ?? Name;

        [IgnoreDataMember]
        string? BeatSaber.IPreviewBeatmapLevel.songSubName
            => PreviewBeatmapLevel?.songSubName;

        [IgnoreDataMember]
        string? BeatSaber.IPreviewBeatmapLevel.songAuthorName
            => PreviewBeatmapLevel?.songAuthorName;

        [IgnoreDataMember]
        string? BeatSaber.IPreviewBeatmapLevel.levelAuthorName
            => PreviewBeatmapLevel?.levelAuthorName ?? LevelAuthorName;

        [IgnoreDataMember]
        float BeatSaber.IPreviewBeatmapLevel.beatsPerMinute
            => PreviewBeatmapLevel?.beatsPerMinute ?? 0;

        [IgnoreDataMember]
        float BeatSaber.IPreviewBeatmapLevel.songTimeOffset
            => PreviewBeatmapLevel?.songTimeOffset ?? 0;

        [IgnoreDataMember]
        float BeatSaber.IPreviewBeatmapLevel.shuffle
            => PreviewBeatmapLevel?.shuffle ?? 0;

        [IgnoreDataMember]
        float BeatSaber.IPreviewBeatmapLevel.shufflePeriod
            => PreviewBeatmapLevel?.shufflePeriod ?? 0;

        [IgnoreDataMember]
        float BeatSaber.IPreviewBeatmapLevel.previewStartTime
            => PreviewBeatmapLevel?.previewStartTime ?? 0;

        [IgnoreDataMember]
        float BeatSaber.IPreviewBeatmapLevel.previewDuration
            => PreviewBeatmapLevel?.previewDuration ?? 0;

        [IgnoreDataMember]
        float BeatSaber.IPreviewBeatmapLevel.songDuration
            => PreviewBeatmapLevel?.songDuration ?? 0;

        [IgnoreDataMember]
        BeatSaber.EnvironmentInfoSO? BeatSaber.IPreviewBeatmapLevel.environmentInfo => PreviewBeatmapLevel?.environmentInfo;

        [IgnoreDataMember]
        BeatSaber.EnvironmentInfoSO? BeatSaber.IPreviewBeatmapLevel.allDirectionsEnvironmentInfo
            => PreviewBeatmapLevel?.allDirectionsEnvironmentInfo;

        [IgnoreDataMember]
        BeatSaber.EnvironmentInfoSO[]? BeatSaber.IPreviewBeatmapLevel.environmentInfos
            => PreviewBeatmapLevel?.environmentInfos;

        [IgnoreDataMember]
        IReadOnlyList<BeatSaber.PreviewDifficultyBeatmapSet>? BeatSaber.IPreviewBeatmapLevel.previewDifficultyBeatmapSets
            => PreviewBeatmapLevel?.previewDifficultyBeatmapSets;

        Task<BeatSaber.UnityEngine.Sprite>? BeatSaber.IPreviewBeatmapLevel.GetCoverImageAsync(CancellationToken cancellationToken)
            => PreviewBeatmapLevel?.GetCoverImageAsync(cancellationToken);
#endregion
        public void RefreshFromSongCore()
        {
            if (LevelId != null && LevelId.Length > 0)
            {
                PreviewBeatmapLevel = SongCore.Loader.GetLevelById(LevelId);
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}

#endif