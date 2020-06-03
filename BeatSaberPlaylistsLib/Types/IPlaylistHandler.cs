using System;
using System.IO;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Interface for a class that can serialize/deserialize an <see cref="IPlaylist"/>.
    /// </summary>
    public interface IPlaylistHandler
    {
        /// <summary>
        /// The default extension used by this <see cref="IPlaylistHandler"/>.
        /// </summary>
        string DefaultExtension { get; }
        /// <summary>
        /// Returns a new array containing file extensions supported by this handler (does not include the '.' prefix).
        /// </summary>
        /// <returns>A new array containing file extensions supported by this handler</returns>
        string[] GetSupportedExtensions();
        /// <summary>
        /// Return true if the <see cref="IPlaylistHandler"/> supports the given <paramref name="extension"/>.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns>True if <paramref name="extension"/> is supported, false otherwise.</returns>
        /// <remarks>Comparison is case-insensitive. Also returns false if <paramref name="extension"/> is a null or empty string.</remarks>
        bool SupportsExtension(string extension);
        /// <summary>
        /// The <see cref="IPlaylist"/> type used by this <see cref="IPlaylistHandler"/>.
        /// </summary>
        Type HandledType { get; }
        /// <summary>
        /// Attempts to deserialize and return an <see cref="IPlaylist"/> from a stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to deserialize</param>
        /// <returns>An <see cref="IPlaylist"/> of type <see cref="HandledType"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
        /// <exception cref="PlaylistSerializationException">Thrown if an error occurs while deserializing.</exception>
        IPlaylist Deserialize(Stream stream);
        /// <summary>
        /// Serializes an <see cref="IPlaylist"/> to a stream. 
        /// </summary>
        /// <param name="playlist">The <see cref="IPlaylist"/> to serialize</param>
        /// <param name="stream">The target <see cref="Stream"/> to serialize to</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="playlist"/> is not of type <see cref="HandledType"/>.</exception>
        /// <exception cref="PlaylistSerializationException">Thrown if an error occurs while deserializing.</exception>
        void Serialize(IPlaylist playlist, Stream stream);

        /// <summary>
        /// Populates the target <see cref="IPlaylist"/> with data from <paramref name="stream"/>.
        /// <paramref name="target"/> must be of type <see cref="HandledType"/>.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="target"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> or <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="target"/> is not of type <see cref="HandledType"/>.</exception>
        /// <exception cref="PlaylistSerializationException">Thrown if an error occurs while deserializing.</exception>
        void Populate(Stream stream, IPlaylist target);
    }

    /// <summary>
    /// Interface for a class that can serialize/deserialize an <see cref="IPlaylist{T}"/>.
    /// </summary>
    public interface IPlaylistHandler<T>
        : IPlaylistHandler where T : IPlaylist
    {
        /// <summary>
        /// Serializes an <see cref="IPlaylist{T}"/> to a stream.
        /// </summary>
        /// <param name="playlist">The <see cref="IPlaylist{T}"/> to serialize</param>
        /// <param name="stream">The target <see cref="Stream"/> to serialize to</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
        /// <exception cref="PlaylistSerializationException">Thrown if an error occurs while deserializing.</exception>
        void Serialize(T playlist, Stream stream);
        /// <summary>
        /// Populates the target <see cref="IPlaylist{T}"/> with data from <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="target"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> or <paramref name="target"/> is null.</exception>
        /// <exception cref="PlaylistSerializationException">Thrown if an error occurs while deserializing.</exception>
        void Populate(Stream stream, T target);
    }
}
