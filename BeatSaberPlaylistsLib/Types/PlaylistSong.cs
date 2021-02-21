using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Base class for a PlaylistSong.
    /// </summary>
    public abstract partial class PlaylistSong : IPlaylistSong
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

        /// <summary>
        /// Unique identifier for playlist song, used for distinguishing between duplicates.
        /// </summary>
        public Guid playlistSongID { get; } = Guid.NewGuid();

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
                    if (value.Length < 12) // TODO: Hacky way to allow Key to be used as a URL
                    {
                        _key = value.ToUpper();
                        AddIdentifierFlag(Identifier.Key);
                    }
                    else
                        _key = value;
                }
                else
                {
                    _key = null;
                    RemoveIdentifierFlag(Identifier.Key);
                }
            }
        }

        /// <summary>
        /// The optional recommended difficulties for the beatmap
        /// </summary>
        public List<Difficulty>? Difficulties { get; set; }

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


        /// <summary>
        /// Adds a <see cref="Difficulty"/> to the <see cref="PlaylistSong"/>.
        /// </summary>
        /// <param name="diff"></param>
        public void AddDifficulty(Difficulty diff)
        {

            if (Difficulties == null) Difficulties = new List<Difficulty>();
            if (Difficulties.Contains(diff))
                return;
            Difficulties.Add(diff);
        }
        /// <summary>
        /// Adds a <see cref="Difficulty"/> with the given parameters to the <see cref="PlaylistSong"/>.
        /// </summary>
        /// <param name="characteristic"></param>
        /// <param name="difficultyName"></param>
        public void AddDifficulty(string characteristic, string difficultyName)
        {
            Difficulty diff = new Difficulty() { Characteristic = characteristic, Name = difficultyName };
            AddDifficulty(diff);
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
