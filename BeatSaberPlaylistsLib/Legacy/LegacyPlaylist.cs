using BeatSaberPlaylistsLib.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace BeatSaberPlaylistsLib.Legacy
{
    /// <summary>
    /// An <see cref="IPlaylist"/> that can be serialized by a <see cref="LegacyPlaylistHandler"/>.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class LegacyPlaylist : Playlist<LegacyPlaylistSong>
    {

        ///<inheritdoc/>
        [DataMember]
        [JsonProperty("songs", Order = 5)]
        protected List<LegacyPlaylistSong> _serializedSongs 
        {
            get => Songs;
            set => Songs = value ?? new List<LegacyPlaylistSong>();
        }
        private Lazy<string>? ImageLoader;
        /// <summary>
        /// Creates an empty <see cref="LegacyPlaylist"/>.
        /// </summary>
        protected LegacyPlaylist()
        { }

        /// <summary>
        /// Creates a new <see cref="LegacyPlaylist"/> from the given parameters.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        protected LegacyPlaylist(string fileName, string title, string? author)
        {
            Filename = fileName;
            Title = title;
            Author = author;
        }

        /// <summary>
        /// Creates a new <see cref="LegacyPlaylist"/> from the given parameters.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="imageLoader"></param>
        public LegacyPlaylist(string fileName, string title, string? author, Lazy<string> imageLoader)
            : this(fileName, title, author)
        {
            ImageLoader = imageLoader;
        }

        /// <summary>
        /// Creates a new <see cref="LegacyPlaylist"/> from the given parameters.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="coverImage"></param>
        public LegacyPlaylist(string fileName, string title, string? author, string? coverImage)
            : this(fileName, title, author)
        {
            SetCover(coverImage);
        }

        ///<inheritdoc/>
        protected override LegacyPlaylistSong CreateFrom(ISong song)
        {
            if (song is LegacyPlaylistSong legacySong)
                return legacySong;
            return new LegacyPlaylistSong(song);
        }

        ///<inheritdoc/>
        [DataMember]
        [JsonProperty("playlistTitle", Order = -10)]
        public override string Title { get; set; } = string.Empty;
        ///<inheritdoc/>
        [DataMember]
        [JsonProperty("playlistAuthor", Order = -5)]
        public override string? Author { get; set; }
        ///<inheritdoc/>
        [DataMember]
        [JsonProperty("playlistDescription", Order = 0, NullValueHandling = NullValueHandling.Ignore)]
        public override string? Description { get; set; }

        private string? _coverString;
        /// <summary>
        /// A base64 string conversion of the cover image.
        /// </summary>
        [DataMember]
        [JsonProperty("image", Order = 10)]
        protected string CoverString
        {
            get
            {
                if (_coverString == null)
                    _coverString = ImageLoader?.Value ?? string.Empty;
                return _coverString;
            }
            set
            {
                _coverString = value;
            }
        }

        ///<inheritdoc/>
        public override bool IsReadOnly => false;

        ///<inheritdoc/>
        public override Stream GetCoverStream()
        {
            if (string.IsNullOrEmpty(CoverString))
                return new MemoryStream(Array.Empty<byte>());
            return new MemoryStream(Utilities.Base64ToByteArray(CoverString));
        }

        ///<inheritdoc/>
        public override void SetCover(byte[]? coverImage)
        {
            CoverString = Utilities.ByteArrayToBase64(coverImage);
        }

        ///<inheritdoc/>
        public override void SetCover(string? coverImageStr)
        {
            if (coverImageStr == null)
                return;
            ImageLoader = new Lazy<string>(() => coverImageStr);
        }

        ///<inheritdoc/>
        public override void SetCover(Stream stream)
        {
            if (stream == null || !stream.CanRead)
                CoverString = "";
            else if (stream is MemoryStream cast)
            {
                CoverString = Utilities.ByteArrayToBase64(cast.ToArray());
            }
            else
            {
                using MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
            }
        }
    }
}
