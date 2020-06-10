using System;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Base class for a PlaylistSong.
    /// </summary>
    public abstract class PlaylistSong : IPlaylistSong
    {
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
