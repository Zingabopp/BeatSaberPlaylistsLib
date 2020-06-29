#if BeatSaber
extern alias BeatSaber;
#endif
using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Base class for a PlaylistSong.
    /// </summary>
    public abstract class PlaylistSong : IPlaylistSong
    {
#if BeatSaber
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
                if (LevelId.StartsWith("custom_level_"))
                {
                    _previewBeatmapLevel = SongCore.Loader.GetLevelById(LevelId);
                }
                else
                {
                    _previewBeatmapLevel = SongCore.Loader.GetOfficialLevelById(LevelId).PreviewBeatmapLevel;
                }
                return _previewBeatmapLevel;
            }
            internal set => _previewBeatmapLevel = value;
        }

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
        BeatSaber.EnvironmentInfoSO? BeatSaber.IPreviewBeatmapLevel.environmentInfo => throw new NotImplementedException();

        [IgnoreDataMember]
        BeatSaber.EnvironmentInfoSO? BeatSaber.IPreviewBeatmapLevel.allDirectionsEnvironmentInfo
            => PreviewBeatmapLevel?.allDirectionsEnvironmentInfo;

        [IgnoreDataMember]
        BeatSaber.PreviewDifficultyBeatmapSet[]? BeatSaber.IPreviewBeatmapLevel.previewDifficultyBeatmapSets 
            => PreviewBeatmapLevel?.previewDifficultyBeatmapSets;


        Task<BeatSaber.UnityEngine.AudioClip>? BeatSaber.IPreviewBeatmapLevel.GetPreviewAudioClipAsync(CancellationToken cancellationToken)
         => PreviewBeatmapLevel?.GetPreviewAudioClipAsync(cancellationToken);

        Task<BeatSaber.UnityEngine.Texture2D>? BeatSaber.IPreviewBeatmapLevel.GetCoverImageTexture2DAsync(CancellationToken cancellationToken) 
            => PreviewBeatmapLevel?.GetCoverImageTexture2DAsync(cancellationToken);
        
        public void RefreshFromSongCore()
        {
            if(LevelId != null && LevelId.Length > 0)
            {
                if (LevelId.StartsWith(CustomLevelIdPrefix))
                    PreviewBeatmapLevel = SongCore.Loader.GetLevelById(LevelId);
                else
                    PreviewBeatmapLevel = SongCore.Loader.GetOfficialLevelById(LevelId).PreviewBeatmapLevel;
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#endif
        /// <summary>
        /// LevelId prefix for custom songs in Beat Saber.
        /// </summary>
        public static readonly string CustomLevelIdPrefix = "custom_level_";
        private string? _hash;
        private string? _levelId;
        private string? _key;

        ///<inheritdoc/>
        public DateTime? DateAdded { get; set; }
        ///<inheritdoc/>
        public string? Hash
        {
            get
            {
                if (_hash != null && _hash.Length > 0)
                    return _hash;
                else if (_levelId != null && _levelId.StartsWith(PlaylistSong.CustomLevelIdPrefix))
                {
                    _hash = _levelId.Substring(PlaylistSong.CustomLevelIdPrefix.Length);
                    AddIdentifierFlag(Identifier.Hash);
                }
                return _hash;
            }
            set
            {
                if (_hash == value)
                    return;
                if (value != null && value.Length > 0)
                {
                    _hash = value.ToUpper();
                    if (_levelId == null || !_levelId.EndsWith(_hash))
                        _levelId = PlaylistSong.CustomLevelIdPrefix + _hash;
                    AddIdentifierFlag(Identifier.Hash);
                    AddIdentifierFlag(Identifier.LevelId);
                }
                else
                {
                    _hash = null;
                    if (!(_levelId != null && _levelId.StartsWith(PlaylistSong.CustomLevelIdPrefix)))
                        RemoveIdentifierFlag(Identifier.Hash);
                }
            }
        }
        ///<inheritdoc/>
        public string? LevelId
        {
            get
            {
                if (_levelId != null && _levelId.Length > 0)
                    return _levelId;
                else if (_hash != null && _hash.Length > 0)
                {
                    _levelId = PlaylistSong.CustomLevelIdPrefix + Hash;
                    AddIdentifierFlag(Identifier.LevelId);
                }
                return _levelId;
            }
            set
            {
                if (_levelId == value)
                    return;
                if (value != null && value.Length > 0)
                {
                    AddIdentifierFlag(Identifier.LevelId);
                    if (value.StartsWith(PlaylistSong.CustomLevelIdPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        string hash = value.Substring(PlaylistSong.CustomLevelIdPrefix.Length);
                        _levelId = PlaylistSong.CustomLevelIdPrefix + hash.ToUpper();
                        if (_hash == null || _hash != hash)
                            Hash = hash;
                        AddIdentifierFlag(Identifier.Hash);
                    }
                    else
                        _levelId = value;
                }
                else
                {
                    _levelId = null;
                    if (string.IsNullOrEmpty(_hash))
                        RemoveIdentifierFlag(Identifier.Hash);
                }
            }
        }
        ///<inheritdoc/>
        public string? Key
        {
            get
            {
                return _key;
            }
            set
            {
                if (_key == value)
                    return;
                if (value != null && value.Length > 0)
                {
                    _key = value.ToUpper();
                    AddIdentifierFlag(Identifier.Key);
                }
                else
                {
                    _key = null;
                    RemoveIdentifierFlag(Identifier.Key);
                }
            }
        }

        /// <summary>
        /// Adds the given flag to the <see cref="Identifiers"/> property.
        /// </summary>
        /// <param name="identifier"></param>
        protected void AddIdentifierFlag(Identifier identifier)
        {
            Identifiers |= identifier;
        }
        /// <summary>
        /// Removes the given flag from the <see cref="Identifiers"/> property.
        /// </summary>
        /// <param name="identifier"></param>
        protected void RemoveIdentifierFlag(Identifier identifier)
        {
            Identifiers &= ~identifier;
        }
        ///<inheritdoc/>
        public virtual string? Name { get; set; }
        ///<inheritdoc/>
        public virtual string? LevelAuthorName { get; set; }
        ///<inheritdoc/>
        public Identifier Identifiers { get; protected set; }
        ///<inheritdoc/>
        public abstract bool Equals(IPlaylistSong other);
    }
}
