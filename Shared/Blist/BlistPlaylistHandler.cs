using BeatSaberPlaylistsLib.Types;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace BeatSaberPlaylistsLib.Blist
{
    /// <summary>
    /// <see cref="IPlaylistHandler"/> for Blist playlists (.blist).
    /// </summary>
    public class BlistPlaylistHandler : IPlaylistHandler<BlistPlaylist>
    {
        private static readonly JsonSerializer jsonSerializer = new JsonSerializer()
        {
            Formatting = Formatting.Indented,
            DateParseHandling = DateParseHandling.DateTime
        };
        ///<inheritdoc/>
        public string DefaultExtension => "blist";

        ///<inheritdoc/>
        public Type HandledType => typeof(BlistPlaylist);

        /// <summary>
        /// Array of the supported extensions (no leading '.').
        /// </summary>
        protected string[] SupportedExtensions = new string[] { "blist" };


        ///<inheritdoc/>
        public bool SupportsExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return false;
            extension = extension.TrimStart('.');
            string[] extensions = SupportedExtensions;
            for (int i = 0; i < extensions.Length; i++)
            {
                if (extensions[i].Equals(extension, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        ///<inheritdoc/>
        public string[] GetSupportedExtensions() => SupportedExtensions.ToArray();

        ///<inheritdoc/>
        public void Populate(Stream stream, BlistPlaylist target)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), $"{nameof(stream)} cannot be null.");
            try
            {
                using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
                ZipArchiveEntry entry = zipArchive.GetEntry("playlist.json")
                    ?? throw new PlaylistSerializationException("Container is missing 'playlist.json'");
                using StreamReader sr = new StreamReader(entry.Open());
                jsonSerializer.Populate(sr, target);
                string? coverPath = target.Cover;
                if (coverPath != null && coverPath.Length > 0)
                {
                    ZipArchiveEntry? imageEntry = zipArchive.GetEntry(coverPath);
                    if (imageEntry != null)
                        target.SetCover(imageEntry.Open());
                }
                else
                {
                    target.RaiseCoverImageChangedForDefaultCover();
                }

            }
            catch (PlaylistSerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PlaylistSerializationException(ex.Message, ex);
            }
        }

        ///<inheritdoc/>
        public void Populate(Stream stream, IPlaylist target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), $"{nameof(target)} cannot be null.");
            BlistPlaylist blistPlaylist = (target as BlistPlaylist)
                ?? throw new ArgumentException($"{target.GetType().Name} is not a supported Type for {nameof(BlistPlaylistHandler)}");
            Populate(stream, blistPlaylist);
        }

        ///<inheritdoc/>
        public void Serialize(BlistPlaylist playlist, Stream stream)
        {
            if (playlist == null)
                throw new ArgumentNullException(nameof(playlist), $"{nameof(playlist)} cannot be null.");
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), $"{nameof(stream)} cannot be null.");
            try
            {
                using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Update);
                ZipArchiveEntry? playlistEntry = zipArchive.GetEntry("playlist.json");
                if (playlistEntry != null)
                    playlistEntry.Delete();
                playlistEntry = zipArchive.CreateEntry("playlist.json");
                if (playlist.HasCover)
                {
                    if (string.IsNullOrEmpty(playlist.Cover))
                        playlist.Cover = "cover";
                    ZipArchiveEntry? coverEntry = zipArchive.GetEntry(playlist.Cover);
                    if (coverEntry != null)
                        coverEntry.Delete();
                    coverEntry = zipArchive.CreateEntry(playlist.Cover);
                    using (Stream coverEntryStream = coverEntry.Open())
                    {
                        using Stream coverStream = playlist.GetCoverStream();
                        coverStream.CopyTo(coverEntryStream);
                        coverEntryStream.Flush();
                    }

                }
                using StreamWriter sw = new StreamWriter(playlistEntry.Open());
                jsonSerializer.Serialize(sw, playlist, typeof(BlistPlaylist));
                sw.Flush();
            }
            catch (Exception ex)
            {
                throw new PlaylistSerializationException(ex.Message, ex);
            }
        }

        ///<inheritdoc/>
        public void Serialize(IPlaylist playlist, Stream stream)
        {
            BlistPlaylist blistPlaylist = (playlist as BlistPlaylist)
                ?? throw new ArgumentException($"{playlist.GetType().Name} is not a supported Type for {nameof(BlistPlaylistHandler)}");
            Serialize(blistPlaylist, stream);
        }


        ///<inheritdoc/>
        public BlistPlaylist Deserialize<T>(Stream stream) where T : BlistPlaylist
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), $"{nameof(stream)} cannot be null.");
            try
            {
                using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
                ZipArchiveEntry entry = zipArchive.GetEntry("playlist.json")
                    ?? throw new PlaylistSerializationException("Container is missing 'playlist.json'");
                using StreamReader sr = new StreamReader(entry.Open());
                if (!(jsonSerializer.Deserialize(sr, typeof(T)) is BlistPlaylist playlist))
                    throw new PlaylistSerializationException("Deserialized playlist was null.");
                string? coverPath = playlist.Cover;
                if (coverPath != null && coverPath.Length > 0)
                {
                    ZipArchiveEntry? imageEntry = zipArchive.GetEntry(coverPath);
                    if (imageEntry != null)
                        playlist.SetCover(imageEntry.Open());
                }
                return playlist;

            }
            catch (PlaylistSerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PlaylistSerializationException(ex.Message, ex);
            }
        }

        ///<inheritdoc/>
        public virtual BlistPlaylist Deserialize(Stream stream)
        {
            return Deserialize<BlistPlaylist>(stream);
        }

        /// <summary>
        /// Creates a new <see cref="BlistPlaylist"/> using the given parameters.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="description"></param>
        /// <param name="suggestedExtension"></param>
        /// <returns></returns>
        public BlistPlaylist CreatePlaylist(string fileName, string title, string? author, string? description, string? suggestedExtension)
        {
            BlistPlaylist playlist = new BlistPlaylist(fileName, title, author)
            {
                Description = description,
                SuggestedExtension = suggestedExtension
            };
            return playlist;
        }

        ///<inheritdoc/>
        IPlaylist IPlaylistHandler.CreatePlaylist(string fileName, string title, string? author, string? description, string? suggestedExtension)
        => CreatePlaylist(fileName, title, author, description, suggestedExtension);

        ///<inheritdoc/>
        IPlaylist IPlaylistHandler.Deserialize(Stream stream)
        {
            return Deserialize(stream);
        }
    }
}
