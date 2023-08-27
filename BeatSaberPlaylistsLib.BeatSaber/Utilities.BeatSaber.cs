#if BeatSaber
extern alias BeatSaber;
using BeatSaber::UnityEngine;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using IPA.Loader;
using CustomPreviewBeatmapLevel = BeatSaber::CustomPreviewBeatmapLevel;
using Graphics = System.Drawing.Graphics;

namespace BeatSaberPlaylistsLib
{
    public static partial class Utilities
    {
        private static Lazy<Sprite?> _defaultSpriteLoader = new Lazy<Sprite?>(() =>
        {
            Logger?.Invoke("Loading default sprite.", null);
            using Stream? stream = GetDefaultImageStream();
            if (stream == null)
                throw new InvalidOperationException("Couldn't get image stream from resources.");
            Logger?.Invoke($"Manifest stream is {stream.Length} bytes.", null);
            return GetSpriteFromStream(stream, 100.0f, false);
        });
        /// <summary>
        /// Default playlist cover, loaded on first access.
        /// </summary>
        public static Sprite? DefaultSprite => _defaultSpriteLoader.Value;
        /// <summary>
        /// Logger for debugging sprite loads.
        /// </summary>
        public static Action<string?, Exception?>? Logger;
        /// <summary>
        /// Creates a <see cref="Sprite"/> from an image <see cref="Stream"/>.
        /// </summary>
        /// <param name="imageStream"></param>
        /// <param name="pixelsPerUnit"></param>
        /// <param name="returnDefaultOnFail"></param>
        /// <returns></returns>
        public static Sprite? GetSpriteFromStream(Stream imageStream, float pixelsPerUnit = 100.0f, bool returnDefaultOnFail = true)
        {
            Sprite? ReturnDefault(bool useDefault)
            {
                if (useDefault)
                    return DefaultSprite;
                return null;
            }
            try
            {
                Logger?.Invoke($"imageStream is {imageStream?.Length ?? -1} bytes.", null);
                if (imageStream == null || (imageStream.CanSeek && imageStream.Length == 0))
                {
                    //Logger?.Invoke($"imageStream seems to be null or empty.", null);
                    return ReturnDefault(returnDefaultOnFail) ?? throw new ArgumentNullException(nameof(imageStream));
                }
                Texture2D texture = new Texture2D(2, 2);
                byte[]? data = null;
                if (imageStream is MemoryStream memStream)
                {
                    //Logger?.Invoke($"imageStream is a MemoryStream", null);
                    data = memStream.ToArray();
                }
                else
                {
                    data = imageStream.ToArray();
                }
                //Logger?.Invoke($"data is {data?.Length ?? -1} bytes long.", null);
                if (data == null || data.Length == 0)
                {
                    //Logger?.Invoke($"data seems to be null or empty.", null);
                    return ReturnDefault(returnDefaultOnFail);
                }
                texture.LoadImage(data);
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), pixelsPerUnit);
            }
            catch (Exception ex)
            {
                Logger?.Invoke($"Caught unhandled exception", ex);
                return ReturnDefault(returnDefaultOnFail);
            }

        }

        /// <summary>
        /// Gets a <see cref="Stream"/> from a <see cref="Sprite"/>
        /// </summary>
        /// <param name="previewBeatmapLevel"></param>
        /// <returns></returns>
        public static Stream? GetStreamFromBeatmap(BeatSaber.IPreviewBeatmapLevel? previewBeatmapLevel)
        {
            if (previewBeatmapLevel is CustomPreviewBeatmapLevel customPreviewBeatmapLevel)
            {
                var fileName = customPreviewBeatmapLevel.standardLevelInfoSaveData.coverImageFilename;
                return new FileStream(Path.Combine(customPreviewBeatmapLevel.customLevelPath, fileName), FileMode.Open, FileAccess.Read, FileShare.Read, 0x4096, true);
            }
            return GetDefaultImageStream();
        }

        /// <summary>
        /// Downscales <paramref name="original"/> to <paramref name="imageSize"/>
        /// </summary>
        /// <param name="original"></param>
        /// <param name="imageSize"></param>
        /// <returns></returns>
        public static Stream DownscaleImage(Stream original, int imageSize)
        {
            var ms = new MemoryStream();
            try
            {
                var originalImage = Image.FromStream(original);

                if (originalImage.Width <= imageSize && originalImage.Height <= imageSize)
                {
                    return original;
                }

                var resizedRect = new Rectangle(0, 0, imageSize, imageSize);
                var resizedImage = new Bitmap(imageSize, imageSize);

                resizedImage.SetResolution(originalImage.HorizontalResolution, originalImage.VerticalResolution);

                using (var graphics = Graphics.FromImage(resizedImage))
                {
                    using var wrapMode = new ImageAttributes();
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(originalImage, resizedRect, 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, wrapMode);
                }

                resizedImage.Save(ms, ImageFormat.Png);
                return ms;
            }
            catch (Exception)
            {
                return original;
            }
        }

        internal static bool ImageSharpLoaded()
        {
            var imageSharp = PluginManager.GetPluginFromId("SixLabors.ImageSharp");
            return imageSharp != null;
        }
    }
}
#endif