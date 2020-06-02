using BeatSaberPlaylistsLib.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BeatSaberPlaylistsLib.Legacy
{
    /// <summary>
    /// <see cref="IPlaylistHandler"/> for legacy playlists (.bplist/.json).
    /// </summary>
    public class LegacyPlaylistHandler : IPlaylistHandler<LegacyPlaylist>
    {
        private static readonly JsonSerializer jsonSerializer = new JsonSerializer() { Formatting = Formatting.Indented };

        ///<inheritdoc/>
        public string[] GetSupportedExtensions()
        {
            return new string[] { "bplist", "json" };
        }

        ///<inheritdoc/>
        public string DefaultExtension => "bplist";

        ///<inheritdoc/>
        public Type HandledType { get; } = typeof(LegacyPlaylist);

        ///<inheritdoc/>
        public void Populate(Stream stream, LegacyPlaylist target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), $"{nameof(target)} cannot be null for {nameof(Populate)}.");
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), $"{nameof(stream)} cannot be null for {nameof(Populate)}.");
            try
            {
                using StreamReader sr = new StreamReader(stream);
                jsonSerializer.Populate(sr, target);
            }
            catch (Exception ex)
            {
                throw new PlaylistSerializationException(ex.Message, ex);
            }
        }
        ///<inheritdoc/>
#pragma warning disable CA1822 // Mark members as static
        public LegacyPlaylist Deserialize(Stream stream)
#pragma warning restore CA1822 // Mark members as static
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), $"{nameof(stream)} cannot be null.");
            try
            {
                using StreamReader sr = new StreamReader(stream);
                if (!(jsonSerializer.Deserialize(sr, typeof(LegacyPlaylist)) is LegacyPlaylist playlist))
                    throw new PlaylistSerializationException("Deserialized playlist was null.");
                return playlist;
            }
            catch (Exception ex)
            {
                throw new PlaylistSerializationException(ex.Message, ex);
            }
        }

        ///<inheritdoc/>
        public void Serialize(LegacyPlaylist playlist, Stream stream)
        {
            if (playlist == null)
                throw new ArgumentNullException(nameof(playlist), $"{nameof(playlist)} cannot be null.");
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), $"{nameof(stream)} cannot be null.");
            try
            {
                using StreamWriter sw = new StreamWriter(stream);
                jsonSerializer.Serialize(sw, playlist, typeof(LegacyPlaylist));
            }
            catch (Exception ex)
            {
                throw new PlaylistSerializationException(ex.Message, ex);
            }
        }

        ///<inheritdoc/>
        void IPlaylistHandler.Serialize(IPlaylist playlist, Stream stream)
        {
            LegacyPlaylist legacyPlaylist = (playlist as LegacyPlaylist)
                ?? throw new ArgumentException($"{playlist.GetType().Name} is not a supported Type for {nameof(LegacyPlaylistHandler)}");
            Serialize(legacyPlaylist, stream);
        }

        ///<inheritdoc/>
        void IPlaylistHandler.Populate(Stream stream, IPlaylist target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), $"{nameof(target)} cannot be null.");
            LegacyPlaylist legacyPlaylist = (target as LegacyPlaylist)
                ?? throw new ArgumentException($"{target.GetType().Name} is not a supported Type for {nameof(LegacyPlaylistHandler)}");
            Populate(stream, legacyPlaylist);
        }

        ///<inheritdoc/>
        IPlaylist IPlaylistHandler.Deserialize(Stream stream)
        {
            return Deserialize(stream);
        }
    }
}
