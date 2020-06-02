using System;
using System.IO;

namespace BeatSaberPlaylistsLib
{
    /// <summary>
    /// Wrapper for exceptions that cause errors during playlist Serialization/Deserialization.
    /// </summary>
    public class PlaylistSerializationException : IOException
    {
        /// <summary>
        /// Creates a new <see cref="PlaylistSerializationException"/> with no Message or InnerException.
        /// </summary>
        public PlaylistSerializationException()
        {
        }
        /// <summary>
        /// Creates a new <see cref="PlaylistSerializationException"/> with the Message set to <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public PlaylistSerializationException(string message) : base(message)
        {
        }
        /// <summary>
        /// Creates a new <see cref="PlaylistSerializationException"/> with the Message set to <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The <see cref="Exception"/> that caused this <see cref="PlaylistSerializationException"/>.</param>
        public PlaylistSerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
