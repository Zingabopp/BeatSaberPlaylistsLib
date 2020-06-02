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
                return _hash;
            }
            set
            {
                if (_hash == value)
                    return;
                if (!string.IsNullOrEmpty(value))
                {
                    _hash = value;
                    AddIdentifierFlag(Identifier.Hash);
                }
                else
                {
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
                    return CustomLevelIdPrefix + Hash;
                return null;
            }
            set
            {
                if (_levelId == value)
                    return;
                if (!string.IsNullOrEmpty(value))
                {
                    _levelId = value;
                    AddIdentifierFlag(Identifier.LevelId);
                }
                else
                {
                    RemoveIdentifierFlag(Identifier.LevelId);
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
                if (!string.IsNullOrEmpty(value))
                {
                    _key = value;
                    AddIdentifierFlag(Identifier.Key);
                }
                else
                {
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
        public string? Name { get; set; }
        ///<inheritdoc/>
        public string? LevelAuthorName { get; set; }
        ///<inheritdoc/>
        public Identifier Identifiers { get; protected set; }
        ///<inheritdoc/>
        public abstract bool Equals(IPlaylistSong other);
    }
}
