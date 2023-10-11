﻿#if BeatSaber
extern alias BeatSaber;
using BeatSaber::UnityEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static BeatSaber::SliderController.Pool;
using IBeatmapLevelCollection = BeatSaber::IBeatmapLevelCollection;

namespace BeatSaberPlaylistsLib.Types
{
    /// <summary>
    /// Base class for a Playlist.
    /// </summary>
    public abstract partial class Playlist : IStagedSpriteLoad, IPlaylist
    {
        /// <summary>
        /// Maximum width and height of the small cover image
        /// </summary>
        public const int kSmallImageSize = 128;

        /// <summary>
        /// Queue of <see cref="Action"/>s to load playlist sprites.
        /// </summary>
        protected static readonly Queue<Action> SpriteQueue = new Queue<Action>();

        /// <summary>
        /// Instance of the playlist cover sprite.
        /// </summary>
        protected Sprite? _sprite;
        /// <summary>
        /// Instance of the previous cover sprite.
        /// </summary>
        protected Sprite? _previousSprite;

        /// <summary>
        /// Instance of the downscaled playlist cover sprite.
        /// </summary>
        protected Sprite? _smallSprite;
        /// <summary>
        /// Instance of the previous downscaled cover sprite.
        /// </summary>
        protected Sprite? _previousSmallSprite;

        /// <summary>
        /// Returns true if the sprite for the playlist is already queued.
        /// </summary>
        protected bool SpriteLoadQueued;

        /// <summary>
        /// Returns true if the small sprite for the playlist is already queued.
        /// </summary>
        protected bool SmallSpriteLoadQueued;

        private static readonly object _loaderLock = new object();
        private static bool CoroutineRunning = false;

        /// <summary>
        /// Adds a playlist to the sprite load queue.
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="downscaleImage"></param>
        protected async static void QueueLoadSprite(Playlist playlist, bool downscaleImage)
        {
            var stream = playlist.HasCover ? playlist.GetCoverStream() : await playlist.GetDefaultCoverStream();

            if (stream == null)
            {
                var sprite = Utilities.DefaultSprite;
                playlist._sprite = sprite;
                playlist._smallSprite = sprite;
                OnSpriteLoaded(playlist);
            }

            if (downscaleImage)
            {
                var downscaleStream = stream != null && stream != Stream.Null ? await Task.Run(() => Utilities.DownscaleImage(stream, kSmallImageSize)) : stream;
                SpriteQueue.Enqueue(() =>
                {
                    var sprite = Utilities.GetSpriteFromStream(downscaleStream ?? Stream.Null);
                    playlist._smallSprite = sprite;
                    OnSmallSpriteLoaded(playlist);
                    downscaleStream?.Dispose();
                    stream?.Dispose();
                });
            }
            else
            {
                SpriteQueue.Enqueue(() =>
                {
                    var sprite = Utilities.GetSpriteFromStream(stream ?? Stream.Null);
                    playlist._sprite = sprite;
                    playlist._smallSprite = sprite;
                    OnSpriteLoaded(playlist);
                    stream?.Dispose();
                });
            }

            if (!CoroutineRunning)
                SharedCoroutineStarter.instance.StartCoroutine(SpriteLoadCoroutine());
        }

        private static void OnSpriteLoaded(Playlist playlist)
        {
            playlist.SmallSpriteWasLoaded = true;
            playlist.SpriteWasLoaded = true;
            playlist.SpriteLoaded?.Invoke(playlist, null);
            playlist._previousSprite = null;
            playlist._previousSmallSprite = null;
            playlist.SpriteLoadQueued = false;
            playlist.SmallSpriteLoadQueued = false;
        }

        private static void OnSmallSpriteLoaded(Playlist playlist)
        {
            playlist.SmallSpriteWasLoaded = true;
            playlist.SpriteLoaded?.Invoke(playlist, null);
            playlist._previousSmallSprite = null;
            playlist.SmallSpriteLoadQueued = false;
        }

        /// <summary>
        /// Wait <see cref="YieldInstruction"/> between sprite loads.
        /// </summary>
        public static YieldInstruction LoadWait = new WaitForEndOfFrame();

        /// <summary>
        /// Coroutine to load sprites in the queue.
        /// </summary>
        /// <returns></returns>
        protected static IEnumerator<YieldInstruction> SpriteLoadCoroutine()
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
                SharedCoroutineStarter.instance.StartCoroutine(SpriteLoadCoroutine());
        }

        #region IStagedSpriteLoad

        /// <inheritdoc/>
        public event EventHandler? SpriteLoaded;

        /// <inheritdoc/>
        public bool SpriteWasLoaded { get; protected set; }

        /// <inheritdoc/>
        public Sprite? Sprite
        {
            get
            {
                if (_sprite != null)
                    return _sprite;
                _sprite = _previousSprite ? _previousSprite : Utilities.DefaultSprite;
                if (!SpriteLoadQueued)
                {
                    SpriteLoadQueued = true;
                    QueueLoadSprite(this, false);
                }
                return _sprite;
            }
        }

        /// <inheritdoc/>
        public bool SmallSpriteWasLoaded { get; protected set; }

        /// <inheritdoc/>
        public Sprite? SmallSprite
        {
            get
            {
                if (_smallSprite != null)
                    return _smallSprite;
                _smallSprite = _previousSmallSprite ? _previousSmallSprite : Utilities.DefaultSprite;
                if (!SmallSpriteLoadQueued)
                {
                    SmallSpriteLoadQueued = true;
                    QueueLoadSprite(this, true);
                }
                return _smallSprite;
            }
        }

        /// <summary>
        /// Resets the sprite for...reasons.
        /// </summary>
        partial void ResetSprite()
        {
            _previousSprite = _sprite;
            _previousSmallSprite = _smallSprite;
            _sprite = null;
            _smallSprite = null;
        }

        #endregion

        /// <summary>
        /// Name of the collection, uses <see cref="Playlist.Title"/>.
        /// </summary>
        string BeatSaber.IAnnotatedBeatmapLevelCollection.collectionName => Regex.Replace(Title, @"\t|\n|\r", " ");

        /// <summary>
        /// Cover image sprite.
        /// </summary>
        Sprite? BeatSaber.IAnnotatedBeatmapLevelCollection.coverImage => Sprite;

        /// <summary>
        /// Cover image sprite.
        /// </summary>
        Sprite? BeatSaber.IAnnotatedBeatmapLevelCollection.smallCoverImage => SmallSprite;

        /// <summary>
        /// BeatmapLevelPack ID.
        /// </summary>
        public string packID => BeatSaber.CustomLevelLoader.kCustomLevelPackPrefixId + playlistID;

        /// <summary>
        /// BeatmapLevelPack Name, same as name of the collection.
        /// </summary>
        public string packName => Regex.Replace(Title, @"\t|\n|\r", " ");

        /// <summary>
        /// BeatmapLevelPack Short Name, same as name of the collection.
        /// </summary>
        public string shortPackName => Regex.Replace(Title, @"\t|\n|\r", " ");

        /// <summary>
        /// BeatmapLevelPack content rating.
        /// </summary>
        public BeatSaber.PlayerSensitivityFlag contentRating => BeatSaber.PlayerSensitivityFlag.Safe;

        /// <summary>
        /// Returns itself.
        /// </summary>
        BeatSaber.IBeatmapLevelCollection BeatSaber.IAnnotatedBeatmapLevelCollection.beatmapLevelCollection => this;
        /// <summary>
        /// Returns a new array of IPreviewBeatmapLevels in this playlist.
        /// </summary>
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        IReadOnlyList<BeatSaber.IPreviewBeatmapLevel> BeatSaber.IBeatmapLevelCollection.beatmapLevels
        {
            get
            {
                foreach (var song in this)
                {
                    song.RefreshFromSongCore();

                }
                return this.Where(s => s.PreviewBeatmapLevel != null).Select(s => s.PreviewBeatmapLevel).ToArray();
            }
        }
        /// <summary>
        /// Returns a new array of PlaylistSongs (cast as IPreviewBeatmapLevels) in this playlist.
        /// </summary>
        public BeatSaber.IPreviewBeatmapLevel[] BeatmapLevels
        {
            get
            {
                foreach (var song in this)
                {
                    song.RefreshFromSongCore();
                }
                return this.Where(s => s.PreviewBeatmapLevel != null).Cast<BeatSaber.IPreviewBeatmapLevel>().ToArray();
            }
        }

#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        /// <inheritdoc/>
        public IPlaylistSong? Add(BeatSaber.IPreviewBeatmapLevel beatmap)
        {
            if (beatmap == null)
                return null;
            IPlaylistSong? song = Add((ISong)CreateFromByLevelId(beatmap.levelID, beatmap.songName, null, beatmap.levelAuthorName));
            song?.SetPreviewBeatmap(beatmap);
            return song;
        }

        /// <inheritdoc/>
        public IPlaylistSong? Add(BeatSaber.IDifficultyBeatmap beatmap)
        {
            if (beatmap == null)
                return null;

            Difficulty difficulty = new Difficulty();
            difficulty.BeatmapDifficulty = beatmap.difficulty;
            difficulty.Characteristic = beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName;
            IPlaylistSong? song = Add(beatmap.level);
            if (song != null)
                song.Difficulties = new List<Difficulty> { difficulty };

            return song;
        }


        #region Default Cover

        private static SemaphoreSlim _defaultCoverSemaphore = new SemaphoreSlim(1, 1);

        /// <inheritdoc cref="IPlaylist.GetDefaultCoverStream" />
        public async Task<Stream?> GetDefaultCoverStream()
        {
            if (_defaultCoverData != null)
            {
                return new MemoryStream(_defaultCoverData);
            }

            if (!Utilities.ImageSharpLoaded() || BeatmapLevels.Length == 0)
            {
                return null;
            }

            await _defaultCoverSemaphore.WaitAsync();
            var ms = new MemoryStream();

            try
            {
                var beatmapLevels = ((IBeatmapLevelCollection) this).beatmapLevels;

                if (beatmapLevels.Count == 1)
                {
                    using var coverStream = Utilities.GetStreamFromBeatmap(beatmapLevels[0]);
                    if (coverStream != null) await coverStream.CopyToAsync(ms);
                }
                else if (beatmapLevels.Count == 2)
                {
                    using var imageStream1 = Utilities.GetStreamFromBeatmap(beatmapLevels[0]);
                    using var imageStream2 = Utilities.GetStreamFromBeatmap(beatmapLevels[1]);
                    using var coverStream = await ImageUtilities.GenerateCollage(imageStream1 ?? Stream.Null, imageStream2 ?? Stream.Null);
                    await coverStream.CopyToAsync(ms);
                }
                else if (beatmapLevels.Count == 3)
                {
                    using var imageStream1 = Utilities.GetStreamFromBeatmap(beatmapLevels[0]);
                    using var imageStream2 = Utilities.GetStreamFromBeatmap(beatmapLevels[1]);
                    using var imageStream3 = Utilities.GetStreamFromBeatmap(beatmapLevels[2]);
                    using var coverStream = await ImageUtilities.GenerateCollage(imageStream1 ?? Stream.Null, imageStream2 ?? Stream.Null, imageStream3 ?? Stream.Null);
                    await coverStream.CopyToAsync(ms);
                }
                else
                {
                    using var imageStream1 = Utilities.GetStreamFromBeatmap(beatmapLevels[0]);
                    using var imageStream2 = Utilities.GetStreamFromBeatmap(beatmapLevels[1]);
                    using var imageStream3 = Utilities.GetStreamFromBeatmap(beatmapLevels[2]);
                    using var imageStream4 = Utilities.GetStreamFromBeatmap(beatmapLevels[3]);
                    using var coverStream = await ImageUtilities.GenerateCollage(imageStream1 ?? Stream.Null, imageStream2 ?? Stream.Null, imageStream3 ?? Stream.Null, imageStream4 ?? Stream.Null);
                    await coverStream.CopyToAsync(ms);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                _defaultCoverSemaphore.Release();
            }

            _defaultCoverData = ms.ToArray();
            return ms;
        }
        #endregion
    }



}
#endif