#if BeatSaber
extern alias BeatSaber;
using BeatSaber::UnityEngine;
using System;
using System.IO;
using System.Reflection;

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

    }
}
#endif