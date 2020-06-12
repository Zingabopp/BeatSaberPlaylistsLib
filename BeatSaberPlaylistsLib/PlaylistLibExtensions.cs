using BeatSaberPlaylistsLib.Types;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace BeatSaberPlaylistsLib
{
    /// <summary>
    /// Helpful extensions.
    /// </summary>
    public static class PlaylistLibExtensions
    {
        /// <summary>
        /// Gets a <see cref="Lazy{T}"/> loader for an image at the given <paramref name="resourcePath"/>.
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <returns></returns>
        public static Lazy<string> GetImageLoader(string resourcePath)
        {
            return new Lazy<string>(() => Utilities.ImageToBase64(resourcePath));
        }

        /// <summary>
        /// Clones an <see cref="ISong"/> to type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="song"></param>
        /// <returns></returns>
        public static T ToSong<T>(this ISong song) where T : ISong, new()
        {
            return new T()
            {
                Hash = song.Hash,
                Key = song.Key,
                LevelId = song.LevelId,
                LevelAuthorName = song.LevelAuthorName
            };
        }

        /// <summary>
        /// Serializes an <see cref="IPlaylist"/> to a file. 
        /// </summary>
        /// <param name="handler"><see cref="IPlaylistHandler"/> to use.</param>
        /// <param name="playlist">The <see cref="IPlaylist"/> to serialize</param>
        /// <param name="path">The path to the target file to serialize to</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="playlist"/> is not of type <see cref="IPlaylistHandler.HandledType"/>.</exception>
        /// <exception cref="PlaylistSerializationException">Thrown if an error occurs while serializing.</exception>
        public static void SerializeToFile(this IPlaylistHandler handler, IPlaylist playlist, string path)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler), $"{nameof(handler)} cannot be null.");
            if (playlist == null)
                throw new ArgumentNullException(nameof(playlist), $"{nameof(playlist)} cannot be null.");
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "path cannot be null or empty.");
            try
            {
                string backupPath = path + ".bak";
                if (File.Exists(path)) File.Move(path, backupPath);
                using FileStream stream = File.Open(path, FileMode.Create, FileAccess.ReadWrite);
                handler.Serialize(playlist, stream);
                if (File.Exists(backupPath))
                    File.Delete(backupPath);
            }
            catch (Exception ex)
            {
                throw new PlaylistSerializationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Attempts to deserialize and return an <see cref="IPlaylist"/> from a file.
        /// </summary>
        /// <param name="handler"><see cref="IPlaylistHandler"/> to use.</param>
        /// <param name="path">Path to the file.</param>
        /// <returns>An <see cref="IPlaylist"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown if a file at <paramref name="path"/> does not exist.</exception>
        /// <exception cref="PlaylistSerializationException">Thrown if an error occurs while deserializing.</exception>
        public static IPlaylist Deserialize(this IPlaylistHandler handler, string path)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler), $"{nameof(handler)} cannot be null.");
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "path cannot be null or empty.");
            path = Path.GetFullPath(path);
            if (!File.Exists(path))
                throw new ArgumentException($"File at '{path}' does not exist or is inaccessible.");
            try
            {
                using FileStream stream = Utilities.OpenFileRead(path);
                return handler.Deserialize(stream);
            }
            catch (Exception ex)
            {
                throw new PlaylistSerializationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Populates the target <see cref="IPlaylist"/> with data from a file.
        /// </summary>
        /// <param name="handler"><see cref="IPlaylistHandler"/> to use.</param>
        /// <param name="path">Path to the file.</param>
        /// <param name="target">Target <see cref="IPlaylist"/>.</param>
        /// <returns>An <see cref="IPlaylist"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown if a file at <paramref name="path"/> does not exist 
        /// or <paramref name="target"/>'s type doesn't match <see cref="IPlaylistHandler.HandledType"/>.</exception>
        /// <exception cref="PlaylistSerializationException">Thrown if an error occurs while deserializing.</exception>
        public static void Populate(this IPlaylistHandler handler, string path, IPlaylist target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), $"{nameof(target)} cannot be null.");
            if (handler == null)
                throw new ArgumentNullException(nameof(handler), $"{nameof(handler)} cannot be null.");
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "path cannot be null or empty.");
            if (!handler.HandledType.IsAssignableFrom(target.GetType()))
                throw new ArgumentException($"target's type, '{target.GetType().Name}' cannot be handled by {handler.GetType().Name}.", nameof(target));
            path = Path.GetFullPath(path);
            if (!File.Exists(path))
                throw new ArgumentException($"File at '{path}' does not exist or is inaccessible.");
            try
            {
                using FileStream stream = Utilities.OpenFileRead(path);
                handler.Populate(stream, target);
            }
            catch (Exception ex)
            {
                throw new PlaylistSerializationException(ex.Message, ex);
            }
        }
    }
}
