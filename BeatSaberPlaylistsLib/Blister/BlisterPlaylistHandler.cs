using BeatSaberPlaylistsLib.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatSaberPlaylistsLib.Blister
{
    /// <summary>
    /// <see cref="IPlaylistHandler"/> for Blister playlists (.blist).
    /// </summary>
    public class BlisterPlaylistHandler : IPlaylistHandler<BlisterPlaylist>
    {
        ///<inheritdoc/>
        public string DefaultExtension => "blist";

        ///<inheritdoc/>
        public Type HandledType => typeof(BlisterPlaylist);

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
        public IPlaylist Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public string[] GetSupportedExtensions() => SupportedExtensions.ToArray();

        ///<inheritdoc/>
        public void Populate(Stream stream, BlisterPlaylist target)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public void Populate(Stream stream, IPlaylist target)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public void Serialize(BlisterPlaylist playlist, Stream stream)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public void Serialize(IPlaylist playlist, Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
