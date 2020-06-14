#if BeatSaber
extern alias BeatSaber;
#endif
using System;
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
        private BeatSaber.IPreviewBeatmapLevel? _previewBeatmapLevel;
        public BeatSaber.IPreviewBeatmapLevel? PreviewBeatmapLevel 
        {
            get
            {
                if (_previewBeatmapLevel != null)
                    return _previewBeatmapLevel;
                if (LevelId == null || LevelId.Length == 0)
                    return null;
                if (LevelId.StartsWith("custom_level_") &&
                    SongCore.Loader.GetLevelById(LevelId) is BeatSaber.CustomBeatmapLevel customLevel)
                {
                    _previewBeatmapLevel = customLevel;
                }
                else if(SongCore.Loader.GetOfficialLevelById(LevelId).PreviewBeatmapLevel is BeatSaber.IPreviewBeatmapLevel officialLevel)
                {
                    _previewBeatmapLevel = officialLevel;
                }
                return _previewBeatmapLevel;
            }
            internal set => _previewBeatmapLevel = value;
        }

        public string? levelID
            => PreviewBeatmapLevel?.levelID ?? this.LevelId;

        public string songName
            => PreviewBeatmapLevel?.songName;

        public string songSubName
            => PreviewBeatmapLevel?.songSubName;

        public string songAuthorName
            => PreviewBeatmapLevel?.songAuthorName;

        public string levelAuthorName
            => PreviewBeatmapLevel?.levelAuthorName;

        public float beatsPerMinute
            => PreviewBeatmapLevel?.beatsPerMinute ?? 0;

        public float songTimeOffset
            => PreviewBeatmapLevel?.songTimeOffset ?? 0;

        public float shuffle
            => PreviewBeatmapLevel?.shuffle ?? 0;

        public float shufflePeriod
            => PreviewBeatmapLevel?.shufflePeriod ?? 0;

        public float previewStartTime
            => PreviewBeatmapLevel?.previewStartTime ?? 0;

        public float previewDuration
            => PreviewBeatmapLevel?.previewDuration ?? 0;

        public float songDuration
            => PreviewBeatmapLevel?.songDuration ?? 0;

        public BeatSaber.EnvironmentInfoSO? environmentInfo => throw new NotImplementedException();

        public BeatSaber.EnvironmentInfoSO? allDirectionsEnvironmentInfo
            => PreviewBeatmapLevel?.allDirectionsEnvironmentInfo;

        public BeatSaber.PreviewDifficultyBeatmapSet[]? previewDifficultyBeatmapSets 
            => PreviewBeatmapLevel?.previewDifficultyBeatmapSets;


        public Task<BeatSaber.UnityEngine.AudioClip>? GetPreviewAudioClipAsync(CancellationToken cancellationToken)
         => PreviewBeatmapLevel?.GetPreviewAudioClipAsync(cancellationToken);

        public Task<BeatSaber.UnityEngine.Texture2D>? GetCoverImageTexture2DAsync(CancellationToken cancellationToken) 
            => PreviewBeatmapLevel?.GetCoverImageTexture2DAsync(cancellationToken);
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
