using BeatSaberPlaylistsLib.Blist;
using BeatSaberPlaylistsLib.Legacy;
using BeatSaberPlaylistsLib.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace BeatSaberPlaylistsLib
{
    /// <summary>
    /// Class that manages <see cref="IPlaylistHandler"/>s and <see cref="IPlaylist"/>s.
    /// </summary>
    public class PlaylistManager
    {
        /// <summary>
        /// The parent <see cref="PlaylistManager"/>, if any. 
        /// </summary>
        public PlaylistManager? Parent { get; protected set; }

        /// <summary>
        /// Returns the available child <see cref="PlaylistManager"/>s.
        /// </summary>
        public IEnumerable<PlaylistManager> GetChildManagers => ChildManagers.AsEnumerable();
        /// <summary>
        /// Internal list of ChildManagers.
        /// </summary>
        protected List<PlaylistManager> ChildManagers;
        /// <summary>
        /// Returns true if this <see cref="PlaylistManager"/> has children.
        /// </summary>
        public bool HasChildren => ChildManagers.Count > 0;
        /// <summary>
        /// Lazy loader for <see cref="DefaultManager"/>. 
        /// </summary>
        protected static readonly Lazy<PlaylistManager> _defaultManagerLoader = new Lazy<PlaylistManager>(() =>
        {
            PlaylistManager playlistManager = new PlaylistManager("Playlists", new LegacyPlaylistHandler(), new BlistPlaylistHandler());
            return playlistManager;
        }
        , System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        /// <summary>
        /// Reference to the default <see cref="PlaylistManager"/> which uses the 'Playlists' directory in the current working directory.
        /// Only access this if you want to use a <see cref="PlaylistManager"/> with the directory set to 'CurrentWorkingDirectory\Playlists'.
        /// </summary>
        public static PlaylistManager DefaultManager => _defaultManagerLoader.Value;
        /// <summary>
        /// Dictionary of <see cref="IPlaylist"/> <see cref="Type"/>s and their associated <see cref="IPlaylistHandler"/>.
        /// </summary>
        protected readonly Dictionary<Type, IPlaylistHandler> PlaylistHandlers = new Dictionary<Type, IPlaylistHandler>();
        /// <summary>
        /// Dictionary of file extensions (uppercase) and their associated <see cref="IPlaylistHandler"/>.
        /// </summary>
        protected readonly Dictionary<string, IPlaylistHandler> PlaylistExtensionHandlers = new Dictionary<string, IPlaylistHandler>();
        /// <summary>
        /// Lock object used when modifying <see cref="ChangedPlaylists"/>.
        /// </summary>
        protected object _changedLock = new object();
        /// <summary>
        /// List of <see cref="IPlaylist"/>s that are marked as changed.
        /// </summary>
        protected readonly HashSet<IPlaylist> ChangedPlaylists = new HashSet<IPlaylist>();
        /// <summary>
        /// Key is the file name in uppercase.
        /// </summary>
        protected ConcurrentDictionary<string, IPlaylist> LoadedPlaylists = new ConcurrentDictionary<string, IPlaylist>();
        /// <summary>
        /// Path to the directory the <see cref="PlaylistManager"/> loads and stores playlists.
        /// </summary>
        public string PlaylistPath { get; protected set; }

        private IPlaylistHandler? _defaultHandler;
        /// <summary>
        /// The default <see cref="IPlaylistHandler"/> for this <see cref="PlaylistManager"/>.
        /// If no default handler is set, it looks to the parent's default handler.
        /// If none of the parents have a default handler, use the first in the list of registered handlers.
        /// </summary>
        public IPlaylistHandler? DefaultHandler
        {
            get
            {
                if (_defaultHandler == null)
                {
                    _defaultHandler = Parent?.DefaultHandler;
                }
                if (_defaultHandler == null)
                    _defaultHandler = PlaylistHandlers.Values.FirstOrDefault();
                return _defaultHandler;
            }
            protected set => _defaultHandler = value;
        }

        /// <summary>
        /// Creates a new <see cref="PlaylistManager"/> to manage playlists in <paramref name="playlistDirectory"/>.
        /// Also creates the directory given in <paramref name="playlistDirectory"/>.
        /// </summary>
        /// <param name="playlistDirectory"></param>
        /// <exception cref="IOException">Thrown if directory creation fails.</exception>
        public PlaylistManager(string playlistDirectory)
        {
            if (string.IsNullOrEmpty(playlistDirectory))
                throw new ArgumentNullException(nameof(playlistDirectory), $"PlaylistManager cannot have a null {nameof(playlistDirectory)}");
            PlaylistPath = Path.GetFullPath(playlistDirectory);
            Directory.CreateDirectory(PlaylistPath);
            string[] subDirectories = Directory.GetDirectories(PlaylistPath);
            ChildManagers = new List<PlaylistManager>(subDirectories.Length);
            for (int i = 0; i < subDirectories.Length; i++)
            {
                ChildManagers.Add(new PlaylistManager(subDirectories[i], this));
            }
        }
        /// <summary>
        /// Internal constructor for a <see cref="PlaylistManager"/> with a parent.
        /// </summary>
        /// <param name="playlistDirectory"></param>
        /// <param name="parent"></param>
        protected PlaylistManager(string playlistDirectory, PlaylistManager parent)
            : this(playlistDirectory)
        {
            Parent = parent;
        }
        /// <summary>
        /// Creates a new <see cref="PlaylistManager"/> to manage playlists in <paramref name="playlistDirectory"/>
        /// and sets the default <see cref="IPlaylistHandler"/> to <paramref name="defaultHandler"/>.
        /// Also creates the directory given in <paramref name="playlistDirectory"/>.
        /// </summary>
        /// <param name="playlistDirectory"></param>
        /// <param name="defaultHandler"></param>
        /// <param name="otherHandlers"></param>
        /// <exception cref="IOException">Thrown if directory creation fails.</exception>
        public PlaylistManager(string playlistDirectory, IPlaylistHandler defaultHandler, params IPlaylistHandler[] otherHandlers)
            : this(playlistDirectory)
        {
            DefaultHandler = defaultHandler;
            RegisterHandler(defaultHandler);
            if (otherHandlers != null)
            {
                for (int i = 0; i < otherHandlers.Length; i++)
                    RegisterHandler(otherHandlers[i]);
            }
        }

        /// <summary>
        /// Internal method to retrieve supported extensions from this and parent <see cref="PlaylistManager"/>s.
        /// </summary>
        /// <param name="supportedExtensions">List to add supported extensions to.</param>
        protected void GetSupportedExtensions(List<string> supportedExtensions)
        {
            PlaylistManager? manager = this;
            do
            {
                if (manager.PlaylistExtensionHandlers.Count > 0)
                {
                    foreach (var extension in manager.PlaylistExtensionHandlers.Keys.ToArray())
                    {
                        supportedExtensions.Add(extension);
                    }
                }
                manager = manager.Parent;
            } while (manager != null);
        }

        /// <summary>
        /// Returns an array of all the extensions (UPPERCASE) that have a registered <see cref="IPlaylistHandler"/> (without the leading '.')
        /// </summary>
        /// <returns></returns>
        public string[] GetSupportedExtensions()
        {
            if (Parent == null)
                return PlaylistExtensionHandlers.Keys.ToArray();
            List<string> supportedExtensions = new List<string>();
            GetSupportedExtensions(supportedExtensions);
            return supportedExtensions.ToArray();
        }

        /// <summary>
        /// Returns true if a registered <see cref="IPlaylistHandler"/> supports <paramref name="extension"/>.
        /// <paramref name="extension"/> is case-insensitive and leading '.' are ignored.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public bool SupportsExtension(string extension)
        {
            PlaylistManager? manager = this;
            do
            {
                if (manager.PlaylistExtensionHandlers.ContainsKey(extension.TrimStart('.').ToUpper()))
                    return true;
                manager = manager.Parent;
            } while (manager != null);
            return false;
        }

        /// <summary>
        /// Attempts to register the given <see cref="IPlaylistHandler"/> with the <see cref="PlaylistManager"/>.
        /// Returns false if a handler with the same <see cref="IPlaylistHandler.HandledType"/> already exists,
        /// or if all extensions supported by <paramref name="playlistHandler"/> are already handled.
        /// </summary>
        /// <param name="playlistHandler"></param>
        /// <returns></returns>
        public bool RegisterHandler(IPlaylistHandler playlistHandler)
        {
            bool successful = false;
            Type handlerType = playlistHandler.HandledType;
            if (!PlaylistHandlers.ContainsKey(handlerType))
            {
                PlaylistHandlers.Add(handlerType, playlistHandler);
                if (DefaultHandler == null)
                    DefaultHandler = playlistHandler;
                foreach (string ext in playlistHandler.GetSupportedExtensions().Select(e => e.ToUpper()))
                {
                    if (!PlaylistExtensionHandlers.ContainsKey(ext))
                    {
                        PlaylistExtensionHandlers.Add(ext, playlistHandler);
                        successful = true;
                    }
                }
            }
            return successful;
        }

        /// <summary>
        /// Registers an <see cref="IPlaylistHandler"/> for a specific extension. 
        /// This will not register the handler for other extensions it may support.
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="playlistHandler"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="extension"/> or <paramref name="playlistHandler"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="playlistHandler"/> does not support the given <paramref name="extension"/>.</exception>
        public void RegisterHandlerForExtension(string extension, IPlaylistHandler playlistHandler)
        {
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentNullException(nameof(extension), "extension cannot be null or empty.");
            if (playlistHandler == null)
                throw new ArgumentNullException(nameof(playlistHandler), "playlistHandler cannot be null or empty.");
            extension = extension.TrimStart('.').ToUpper();
            if (!playlistHandler.SupportsExtension(extension))
                throw new ArgumentException(nameof(extension), $"{playlistHandler.GetType().Name} does not support the '{extension}' extension.");
            if (!PlaylistExtensionHandlers.ContainsKey(extension))
                PlaylistExtensionHandlers.Add(extension, playlistHandler);
            else
                PlaylistExtensionHandlers[extension] = playlistHandler;
        }

        /// <summary>
        /// Returns the first registered <see cref="IPlaylistHandler"/> of type <typeparamref name="T"/> 
        /// or null if there's no registered matching <see cref="IPlaylistHandler"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetHandler<T>() where T : class, IPlaylistHandler, new()
        {
            IPlaylistHandler? playlistHandler;
            PlaylistManager? manager = this;
            do
            {
                playlistHandler = manager.PlaylistHandlers.Values.FirstOrDefault(h => typeof(T).IsAssignableFrom(h.GetType()));
                manager = manager.Parent;
            } while (playlistHandler == null && manager != null);
            return playlistHandler as T;
        }

        /// <summary>
        /// Attempts to get an <see cref="IPlaylistHandler"/> for the provided <paramref name="playlistType"/>.
        /// Also searches parent <see cref="PlaylistManager"/>s, if available.
        /// </summary>
        /// <param name="playlistType"></param>
        /// <returns></returns>
        public IPlaylistHandler? GetHandlerForPlaylistType(Type playlistType)
        {
            IPlaylistHandler? playlistHandler;
            PlaylistManager? manager = this;
            do
            {
                manager.PlaylistHandlers.TryGetValue(playlistType, out playlistHandler);
                manager = manager.Parent;
            } while (playlistHandler == null && manager != null);
            return playlistHandler;
        }

        /// <summary>
        /// Gets an <see cref="IPlaylistHandler"/> registered for the given <paramref name="extension"/>.
        /// Returns null if no registered handler supports the <paramref name="extension"/>.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public IPlaylistHandler? GetHandlerForExtension(string extension)
        {
            IPlaylistHandler? playlistHandler;
            PlaylistManager? manager = this;
            do
            {
                extension = extension.TrimStart('.').ToUpper();
                manager.PlaylistExtensionHandlers.TryGetValue(extension, out playlistHandler);
                manager = manager.Parent;
            } while (playlistHandler == null && manager != null);

            return playlistHandler;
        }

        /// <summary>
        /// Creates a new <see cref="IPlaylist"/> using the given parameters.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="imageLoader"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public IPlaylist CreatePlaylist(string fileName, string title, string author, Lazy<string> imageLoader, string? description = null)
        {
            IPlaylist playlist = new LegacyPlaylist(fileName, title, author, imageLoader) { Description = description };
            return playlist;
        }

        /// <summary>
        /// Creates a new <see cref="IPlaylist"/> using the given parameters.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="coverImage"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public IPlaylist CreatePlaylist(string fileName, string title, string author, string coverImage, string? description = null)
        {
            IPlaylist playlist = new LegacyPlaylist(fileName, title, author, coverImage) { Description = description };
            return playlist;
        }

        /// <summary>
        /// Attempts to remove the song with the matching hash from all loaded playlists.
        /// </summary>
        /// <param name="hash"></param>
        public void RemoveSongFromAll(string hash)
        {
            hash = hash.ToUpper();
            IPlaylist[] loadedPlaylists = LoadedPlaylists.Values.ToArray();
            foreach (IPlaylist? playlist in loadedPlaylists)
            {
                if (playlist == null)
                    continue;
                playlist.TryRemoveByHash(hash);
            }
        }

        /// <summary>
        /// Attempts to remove the song from all loaded playlists.
        /// </summary>
        /// <param name="song"></param>
        public void RemoveSongFromAll(IPlaylistSong song)
        {
            string? hash = song.Hash;
            if (hash == null)
                return;
            RemoveSongFromAll(hash);
        }

        /// <summary>
        /// Writes all <see cref="IPlaylist"/>s that have been marked as changed to file.
        /// </summary>
        /// <exception cref="AggregateException"></exception>
        public void StoreAllPlaylists()
        {
            IPlaylist[]? changedPlaylists;
            List<Exception> exceptions = new List<Exception>();
            List<string> erroredPlaylists = new List<string>();
            lock (_changedLock)
            {
                changedPlaylists = ChangedPlaylists.ToArray();
                ChangedPlaylists.Clear();
            }
            foreach (IPlaylist? playlist in changedPlaylists)
            {
                if (playlist == null)
                    continue;
                try
                {
                    StorePlaylist(playlist, true);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    if (playlist != null)
                    {
                        if (playlist.Filename != null && playlist.Filename.Length > 0)
                            erroredPlaylists.Add(playlist.Filename);
                        else if (playlist.Title != null && playlist.Title.Length > 0)
                            erroredPlaylists.Add(playlist.Title);
                        else
                            erroredPlaylists.Add("<Unknown>");
                    }
                }
            }
            if (exceptions.Count > 0)
                throw new AggregateException($"{exceptions.Count} exceptions thrown by playlists '{string.Join(", ", erroredPlaylists)}' storing all playlists.", exceptions);
        }

        private void OnPlaylistChanged(object sender, EventArgs e)
        {
            if (sender is IPlaylist playlist)
            {
                MarkPlaylistChanged(playlist);
            }
        }

        /// <summary>
        /// Saves the playlist to file.
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="removeFromChanged"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="playlist"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="playlist"/> is not supported by any registered handlers
        /// or <paramref name="playlist"/> does not have a filename.</exception>
        /// <exception cref="PlaylistSerializationException">Thrown on serialization errors.</exception>
        public void StorePlaylist(IPlaylist playlist, bool removeFromChanged = true)
        {
            if (playlist == null)
                throw new ArgumentNullException(nameof(playlist), "playlist cannot be null.");
            IPlaylistHandler? playlistHandler = playlist.SuggestedExtension != null ? GetHandlerForExtension(playlist.SuggestedExtension) : null;
            if (playlistHandler == null)
                playlistHandler = GetHandlerForPlaylistType(playlist.GetType());
            if (playlistHandler == null)
                throw new ArgumentException(nameof(playlist), $"No registered handlers support playlist type {playlist.GetType().Name}");
            StorePlaylist(playlist, playlistHandler, removeFromChanged);
        }

        /// <summary>
        /// Saves the playlist to file using the provided <see cref="IPlaylistHandler"/>.
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="playlistHandler"></param>
        /// <param name="removeFromChanged"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="playlist"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="playlistHandler"/> does not support the playlist type 
        /// or <paramref name="playlist"/> does not have a filename.</exception>
        /// <exception cref="PlaylistSerializationException">Thrown on serialization errors.</exception>
        public void StorePlaylist(IPlaylist playlist, IPlaylistHandler playlistHandler, bool removeFromChanged = true)
        {
            if (playlistHandler == null)
            {
                StorePlaylist(playlist, removeFromChanged);
                return;
            }
            if (playlist == null)
                throw new ArgumentNullException(nameof(playlist));
            if (!playlist.GetType().IsAssignableFrom(playlistHandler.HandledType))
                throw new ArgumentException($"{playlistHandler.GetType().Name} does not support playlist type {playlist.GetType().Name}", nameof(playlist));
            string extension = playlistHandler.DefaultExtension;
            if (playlist.SuggestedExtension != null && playlistHandler.GetSupportedExtensions().Contains(playlist.SuggestedExtension))
                extension = playlist.SuggestedExtension;
            string fileName = playlist.Filename;
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException(nameof(playlist), "Playlist's filename is null or empty.");
            playlistHandler.SerializeToFile(playlist, Path.Combine(PlaylistPath, fileName + "." + extension));
            RegisterPlaylist(playlist, false);
            if (removeFromChanged)
                RemoveFromChanged(playlist);
        }
        /// <summary>
        /// Attempts to create an <see cref="IPlaylist"/> from a file with the given <paramref name="fileName"/>.
        /// Returns null if there is no registered <see cref="IPlaylistHandler"/> for the given type.
        /// All other failure cases throw an Exception.
        /// </summary>
        /// <remarks>If there are multiple playlists with the same filename and no handler is specified, the first playlist with an extension registered to a handler will be read.</remarks>
        /// <param name="fileName"></param>
        /// <param name="playlistHandler"><see cref="IPlaylistHandler"/> to use, if null a registered handler will be used if it exists.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Thrown on invalid filename, extension not supported by <paramref name="playlistHandler"/>, or if <paramref name="playlistHandler"/> is null and no registered handlers support the extension.</exception>
        /// <exception cref="InvalidOperationException">Thrown if no registered handlers support <paramref name="fileName"/>.</exception>
        /// <exception cref="PlaylistSerializationException"></exception>
        protected IPlaylist? LoadPlaylistFromFile(string fileName, IPlaylistHandler? playlistHandler = null)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName), "fileName cannot be null or empty.");
            IPlaylist? playlist = null;
            string[]? files = Directory.GetFiles(PlaylistPath) ?? Array.Empty<string>();
            string? fileExtension = Path.GetExtension(fileName);
            if (fileExtension != null && SupportsExtension(fileExtension))
                fileName = Path.GetFileNameWithoutExtension(fileName);
            string file = files.FirstOrDefault(f => fileName.Equals(Path.GetFileNameWithoutExtension(f), StringComparison.OrdinalIgnoreCase)
                                                    && SupportsExtension(Path.GetExtension(f)));
            if (file != null)
            {
                fileExtension = Path.GetExtension(file).TrimStart('.');
                if (string.IsNullOrEmpty(fileExtension))
                    throw new ArgumentException($"No valid file extension for provided filename '{file}'");
                if (playlistHandler == null)
                {
                    PlaylistManager? manager = this;
                    playlistHandler = manager.GetHandlerForExtension(fileExtension);
                    if (playlistHandler == null)
                        throw new InvalidOperationException($"playlist extension '{fileExtension}' not supported by any registered handlers.");
                }
                else if (!playlistHandler.SupportsExtension(fileExtension))
                    throw new ArgumentException(nameof(playlistHandler), $"playlist extension '{fileExtension}' not supported by the given playlistHandler.");

                playlist = playlistHandler.Deserialize(file);
                playlist.SuggestedExtension = fileExtension;
                if (playlist != null)
                {
                    playlist.Filename = Path.GetFileNameWithoutExtension(file);
                    RegisterPlaylist(playlist, false);
                }
            }
            return playlist;
        }

        /// <summary>
        /// Registers an <see cref="IPlaylist"/> with the <see cref="PlaylistManager"/>. 
        /// </summary>
        /// <param name="playlist">Playlist to register.</param>
        /// <param name="asChanged">Immediately mark the playlist as changed.</param>
        /// <returns>True if <paramref name="playlist"/> was successful registered, 
        /// false if a playlist with the same filename is already registered.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="playlist"/> does not have a file name.</exception>
        public bool RegisterPlaylist(IPlaylist playlist, bool asChanged = true)
        {
            if (!string.IsNullOrEmpty(playlist.Filename))
            {
                if (LoadedPlaylists.TryAdd(playlist.Filename.ToUpper(), playlist))
                {
                    playlist.PlaylistChanged -= OnPlaylistChanged;
                    playlist.PlaylistChanged += OnPlaylistChanged;
                    if (asChanged)
                        MarkPlaylistChanged(playlist);
                    return true;
                }
                else
                    return false;
            }
            throw new InvalidOperationException("Playlist Filename cannot be null or empty.");
        }

        /// <summary>
        /// Attempts to get a loaded <see cref="IPlaylist"/> with the given filename.
        /// </summary>
        /// <param name="fileName">Filename without extension or directories</param>
        /// <param name="playlist">The retrieved playlist, null if there was no matching playlist.</param>
        /// <returns>True if a playlist was retrieved, false otherwise.</returns>
        public bool TryGetPlaylist(string fileName, out IPlaylist? playlist)
        {
            return LoadedPlaylists.TryGetValue(fileName.ToUpper(), out playlist);
        }

        /// <summary>
        /// Mark <paramref name="playlist"/> as changed in this <see cref="PlaylistManager"/>.
        /// </summary>
        /// <param name="playlist"></param>
        public void MarkPlaylistChanged(IPlaylist playlist)
        {
            lock (_changedLock)
            {
                ChangedPlaylists.Add(playlist);
            }
        }

        private void RemoveFromChanged(IPlaylist playlist)
        {
            lock (_changedLock)
            {
                ChangedPlaylists.Remove(playlist);
            }
        }

        /// <summary>
        /// Returns true if the given <see cref="IPlaylist"/> is marked as changed by this <see cref="PlaylistManager"/>.
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns>True if the playlist is marked as changed, false otherwise.</returns>
        public bool IsPlaylistChanged(IPlaylist playlist)
        {
            if (playlist == null)
                return false;
            return ChangedPlaylists.Contains(playlist);
        }

        /// <summary>
        /// Retrieves the specified playlist. If the playlist doesn't exist, returns null.
        /// </summary>
        /// <remarks>If there are multiple playlists with the same filename and no handler is specified, the first matching playlist with an extension registered to a handler will be read.</remarks>
        /// <param name="playlistFileName">Playlist filename with extension.</param>
        /// <param name="handler">Optional <see cref="IPlaylistHandler"/> to use if deserialization is necessary. If null, use the first registered handler.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="playlistFileName"/> doesn't have an extension or if a <paramref name="handler"/> is given that doesn't support the file extension.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there isn't a registered <see cref="IPlaylistHandler"/> that supports the file extension.</exception>
        /// <exception cref="PlaylistSerializationException">Wraps any exceptions thrown while deserializing.</exception>
        public IPlaylist? GetPlaylist(string playlistFileName, IPlaylistHandler? handler = null)
        {
            if (string.IsNullOrEmpty(playlistFileName))
                return null;
            playlistFileName = playlistFileName.Replace(Path.GetFullPath(PlaylistPath), "").TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            // Check if this playlist exists in LoadedPlaylists
            TryGetPlaylist(playlistFileName, out IPlaylist? playlist);

            // Try to load from file
            if (playlist == null)
            {
                playlist = LoadPlaylistFromFile(playlistFileName, handler);
            }
            return playlist;
        }

        /// <summary>
        /// Returns all <see cref="IPlaylist"/>s that can be loaded by this manager.
        /// </summary>
        /// <returns></returns>
        public IPlaylist[] GetAllPlaylists()
        {
            string[] playlistNames
                = Directory.EnumerateFiles(PlaylistPath, "*.*").Select(p => Path.GetFileNameWithoutExtension(p)).ToArray();
            for (int i = 0; i < playlistNames.Length; i++)
            {
                try
                {
                    _ = GetPlaylist(playlistNames[i]);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return LoadedPlaylists.Values.ToArray();
        }

        /// <summary>
        /// Attempts to get or load a playlist with the given filename. 
        /// If the playlist doesn't exist, it will be created by <paramref name="playlistFactory"/>.
        /// </summary>
        /// <param name="playlistFileName"></param>
        /// <param name="playlistFactory"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="playlistFileName"/> doesn't have an extension.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there isn't a registered <see cref="IPlaylistHandler"/> that supports the file extension.</exception>
        /// <exception cref="PlaylistSerializationException">Wraps any exceptions thrown while deserializing.</exception>
        public IPlaylist GetOrAdd(string playlistFileName, Func<IPlaylist> playlistFactory)
        {
            if (string.IsNullOrEmpty(playlistFileName))
                throw new ArgumentNullException(nameof(playlistFileName), "playlistFileName cannot be null or empty.");
            IPlaylist? playlist = null;
            try
            {
                playlist = GetPlaylist(playlistFileName);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (InvalidOperationException) { } // No supported handlers for existing playlist.
            catch (ArgumentException) { } // No existing playlist.
#pragma warning restore CA1031 // Do not catch general exception types

            if (playlist == null)
            {
                playlist = playlistFactory() ?? throw new ArgumentException("playlistFactory returned a null IPlaylist.", nameof(playlistFactory));
                _ = GetHandlerForPlaylistType(playlist.GetType()) 
                    ?? throw new InvalidOperationException($"PlaylistManager does not have an IPlaylistHandler that supports the playlist returned by playlistFactory.");
                playlist.Filename = playlistFileName;
                RegisterPlaylist(playlist);
            }

            return playlist;
        }
    }
}
