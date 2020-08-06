#if BeatSaber
extern alias BeatSaber;
using BeatSaber::UnityEngine;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Base class for a Playlist.
    /// </summary>
    public abstract class Playlist
#if BeatSaber
         : IDeferredSpriteLoad
#endif
    {
#if BeatSaber
        public event EventHandler? SpriteLoaded;
        protected static readonly Queue<Action> SpriteQueue = new Queue<Action>();
        protected Sprite? _sprite;
        protected bool SpriteLoadQueued;
        private static readonly object _loaderLock = new object();
        private static bool CoroutineRunning = false;
        protected static void QueueLoadSprite(Playlist playlist)
        {
            SpriteQueue.Enqueue(() =>
            {
                if (!playlist.HasCover)
                {
                    return;
                }
                playlist._sprite = Utilities.GetSpriteFromStream(playlist.GetCoverStream());
                playlist.SpriteLoaded?.Invoke(playlist, null);
            });

            if (!CoroutineRunning)
                BeatSaber.SharedCoroutineStarter.instance.StartCoroutine(SpriteLoadCoroutine());
        }
        private static WaitForEndOfFrame LoadWait = new WaitForEndOfFrame();
        protected static IEnumerator<WaitForEndOfFrame> SpriteLoadCoroutine()
        {
            lock (_loaderLock)
            {
                if (CoroutineRunning)
                    yield break;
                CoroutineRunning = true;
            }
            while (SpriteQueue.Count > 0)
            {
                yield return LoadWait;
                var loader = SpriteQueue.Dequeue();
                loader?.Invoke();
            }
            CoroutineRunning = false;
            if (SpriteQueue.Count > 0) // Just in case
                BeatSaber.SharedCoroutineStarter.instance.StartCoroutine(SpriteLoadCoroutine());
        }

        /// <summary>
        /// 
        /// </summary>
        public Sprite? Sprite
        {
            get
            {
                if (_sprite != null)
                    return _sprite;
                if (!HasCover)
                {
                    // TODO: Default cover image?
                    return null;
                }
                if (!SpriteLoadQueued)
                {
                    SpriteLoadQueued = true;
                    QueueLoadSprite(this);
                }
                return _sprite;
            }
        }

#endif

        /// <inheritdoc/>
        public event EventHandler? PlaylistChanged;

        /// <inheritdoc/>
        public abstract string Title { get; set; }
        /// <inheritdoc/>
        public abstract string? Author { get; set; }
        /// <inheritdoc/>
        public abstract string? Description { get; set; }
        /// <inheritdoc/>
        public virtual string Filename { get; set; } = "";
        /// <inheritdoc/>
        public string? SuggestedExtension { get; set; }
        /// <inheritdoc/>
        public bool AllowDuplicates { get; set; }

        /// <inheritdoc/>
        public virtual bool IsReadOnly => false;

        /// <inheritdoc/>
        public abstract bool HasCover { get; }

        /// <inheritdoc/>
        public abstract Stream GetCoverStream();

        /// <inheritdoc/>
        public abstract void SetCover(byte[] coverImage);

        /// <inheritdoc/>
        public abstract void SetCover(string? coverImageStr);

        /// <inheritdoc/>
        public abstract void SetCover(Stream stream);

        /// <inheritdoc/>
        public void RaisePlaylistChanged()
        {
            EventHandler? handler = PlaylistChanged;
            handler?.Invoke(this, null);
        }
    }


    /// <summary>
    /// Base class for a Playlist.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Playlist<T> : Playlist, IPlaylist<T>
#if BeatSaber
        , BeatSaber.IPlaylist, BeatSaber.IBeatmapLevelCollection
#endif
        where T : class, IPlaylistSong, new()
    {
#if BeatSaber

        /// <summary>
        /// Name of the collection, uses <see cref="Title"/>.
        /// </summary>
        string BeatSaber.IAnnotatedBeatmapLevelCollection.collectionName => Title;

        /// <summary>
        /// Cover image sprite.
        /// </summary>
        Sprite? BeatSaber.IAnnotatedBeatmapLevelCollection.coverImage => Sprite;

        /// <summary>
        /// Returns itself.
        /// </summary>
        BeatSaber.IBeatmapLevelCollection BeatSaber.IAnnotatedBeatmapLevelCollection.beatmapLevelCollection => this;
        /// <summary>
        /// Returns a new array of the songs in this playlist.
        /// </summary>
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        BeatSaber.IPreviewBeatmapLevel[] BeatSaber.IBeatmapLevelCollection.beatmapLevels
            => Songs
            .Where(s => s.PreviewBeatmapLevel != null)
            .Select(s => s.PreviewBeatmapLevel)
            .ToArray();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
#endif

        /// <summary>
        /// Internal collection of songs in the playlist.
        /// </summary>
        protected List<T> Songs { get; set; } = new List<T>();

        /// <inheritdoc/>
        public IPlaylistSong this[int index]
        {
            get => Songs[index];
            set
            {
                Songs[index] = CreateFrom(value);
            }
        }


        /// <inheritdoc/>
        public int Count => Songs.Count;

        /// <summary>
        /// Creates a new <see cref="IPlaylistSong"/> of type <typeparamref name="T"/> from the given <paramref name="song"/>.
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        protected abstract T CreateFrom(ISong song);

        /// <inheritdoc/>
        public virtual IPlaylistSong? Add(ISong song)
        {
            if (AllowDuplicates || !Songs.Any(s => s.Hash == song.Hash || (song.Key != null && s.Key == song.Key)))
            {
                T playlistSong = CreateFrom(song);
                Songs.Add(playlistSong);
                return playlistSong;
            }
            return null;
        }

        /// <inheritdoc/>
        public virtual IPlaylistSong? Add(string songHash, string? songName, string? songKey, string? mapper) =>
            Add(new T()
            {
                Hash = songHash,
                Name = songName,
                Key = songKey,
                LevelAuthorName = mapper
            });

        /// <inheritdoc/>
        public virtual IPlaylistSong? Add(IPlaylistSong item) => Add((ISong)item);

        /// <inheritdoc/>
        public virtual void Clear()
        {
            Songs.Clear();
        }

        /// <inheritdoc/>
        public virtual void Sort()
        {
            Songs = Songs.OrderByDescending(s => s.DateAdded).ToList();
        }

        /// <inheritdoc/>
        public virtual bool Contains(IPlaylistSong item)
        {
            return Songs.Any(s => s.Equals(item));
        }

        /// <inheritdoc/>
        public void CopyTo(IPlaylistSong[] array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (T song in Songs)
            {
                array[index] = song;
                index++;
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that iterates through the playlist's songs.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Songs.GetEnumerator();
        }

        /// <inheritdoc/>
        public int IndexOf(IPlaylistSong item)
        {
            if (item is T song)
                return Songs.IndexOf(song);
            else
                return -1;
        }

        /// <inheritdoc/>
        public void Insert(int index, IPlaylistSong item)
        {
            Songs.Insert(index, CreateFrom(item));
        }

        /// <inheritdoc/>
        public bool Remove(IPlaylistSong item)
        {
            bool songRemoved = false;
            if (item is T matchedType)
                songRemoved = Songs.Remove(matchedType);
            else
            {
                T song = Songs.FirstOrDefault(s => s.Equals(item));
                if (song != null)
                    songRemoved = Songs.Remove(song);
            }
            return songRemoved;
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            Songs.RemoveAt(index);
        }

        /// <inheritdoc/>
        public int RemoveAll(Func<IPlaylistSong, bool> match)
        {
            int songsRemoved = 0;
            T[]? toRemove = Songs.Where(s => match(s)).ToArray();
            foreach (T song in toRemove)
            {
                if (Songs.Remove(song))
                    songsRemoved++;
            }
            return songsRemoved;
        }

        /// <inheritdoc/>
        public virtual int RemoveAll(Func<T, bool> match)
        {
            int removedSongs = 0;
            if (match != null)
                removedSongs = Songs.RemoveAll(s => match(s));
            return removedSongs;
        }

        /// <inheritdoc/>
        public void RemoveDuplicates()
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        public bool TryRemoveByHash(string songHash)
        {
            songHash = songHash.ToUpper();
            return RemoveAll((IPlaylistSong s) => s.Hash == songHash) > 0;
        }

        /// <inheritdoc/>
        public bool TryRemoveByKey(string songKey)
        {
            songKey = songKey.ToLower();
            return RemoveAll((IPlaylistSong s) => s.Key == songKey) > 0;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Songs.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator<IPlaylistSong> IEnumerable<IPlaylistSong>.GetEnumerator()
        {
            foreach (var song in Songs)
            {
                yield return song;
            }
        }

        /// <inheritdoc/>
        void ICollection<IPlaylistSong>.Add(IPlaylistSong item)
        {
            Add(item);
        }
    }
}
