using BeatSaberPlaylistsLib.Types;
using Newtonsoft.Json;
using System;

namespace BeatSaberPlaylistsLib.Legacy
{
    /// <summary>
    /// An <see cref="IPlaylistSong"/> that can be serialized in a <see cref="LegacyPlaylist"/>.
    /// </summary>
    public class LegacyPlaylistSong : IPlaylistSong, IEquatable<IPlaylistSong>
    {
        /// <summary>
        /// Creates an empty <see cref="LegacyPlaylistSong"/>.
        /// </summary>
        public LegacyPlaylistSong()
        { }

        /// <summary>
        /// Creates a new <see cref="LegacyPlaylistSong"/> from the given <paramref name="song"/>.
        /// </summary>
        /// <param name="song"></param>
        public LegacyPlaylistSong(IPlaylistSong song)
            : this()
        {
            this.Populate(song);
        }
        /// <summary>
        /// Creates a new <see cref="LegacyPlaylistSong"/> with the given parameters.
        /// At least one identifier must be provided (<paramref name="hash"/>, <paramref name="levelId"/>, <paramref name="songKey"/>).
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="levelId"></param>
        /// <param name="songName"></param>
        /// <param name="songKey"></param>
        /// <param name="mapper"></param>
        public LegacyPlaylistSong(string? hash = null, string? levelId = null, string? songName = null, string? songKey = null, string? mapper = null)
            : this()
        {
            if(hash != null && hash.Length > 0 && levelId != null && levelId.Length > 0)
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

        /// <summary>
        /// Creates a new <see cref="LegacyPlaylistSong"/> from the given <see cref="ISong"/>.
        /// </summary>
        /// <param name="song"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="song"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="song"/> has no valid identifiers.</exception>
        public LegacyPlaylistSong(ISong song)
            : this()
        {
            if (song == null)
                throw new ArgumentNullException(nameof(song), "song cannot be null.");
            LevelId = song.LevelId;
            Hash = song.Hash;
            Name = song.Name;
            Key = song.Key;
            if (Identifiers == Identifier.None)
                throw new ArgumentException("song has no identifiers.", nameof(song));
            LevelAuthorName = song.LevelAuthorName;
            DateAdded = Utilities.CurrentTime;
        }


        ///<inheritdoc/>
        [JsonProperty("key", Order = -10)]
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

        ///<inheritdoc/>
        [JsonProperty("hash", Order = -7)]
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
        [JsonProperty("levelid", Order = -6)]
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
        [JsonProperty("songName", Order = -9)]
        public string? Name { get; set; }

        ///<inheritdoc/>
        [JsonProperty("levelAuthorName", Order = -8)]
        public string? LevelAuthorName { get; set; }

        ///<inheritdoc/>
        [JsonProperty("dateAdded", Order = 10)]
        public DateTime? DateAdded { get; set; }

        ///<inheritdoc/>
        [JsonIgnore]
        public Identifier Identifiers { get; protected set; }


        [JsonIgnore]
        private string? _hash;
        [JsonIgnore]
        private string? _levelId;
        [JsonIgnore]
        private string? _key;

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
        public override string ToString()
        {
            string? keyPart = string.IsNullOrEmpty(Key) ? string.Empty : $"({Key}) ";
            return $"{keyPart}{Name} by {LevelAuthorName}";
        }

        ///<inheritdoc/>
        public bool Equals(IPlaylistSong other)
        {
            if (other == null)
                return false;
            return Hash == other?.Hash;
        }
    }
}
