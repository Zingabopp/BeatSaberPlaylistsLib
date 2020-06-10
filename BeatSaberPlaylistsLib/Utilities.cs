using System;
using System.IO;
using System.Reflection;

namespace BeatSaberPlaylistsLib
{
    /// <summary>
    /// Utilities
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Prefix in some base64 image strings.
        /// </summary>
        public static readonly string Base64Prefix = "base64,";
        /// <summary>
        /// Is set to true if <see cref="CurrentTime"/> throws an exception trying to return <see cref="DateTime.Now"/>.
        /// </summary>
        public static bool UseUtc = false;

        /// <summary>
        /// Returns the current local time. If <see cref="DateTime.Now"/> fails, uses <see cref="DateTime.UtcNow"/>.
        /// </summary>
        /// <remarks><see cref="DateTime.Now"/> can throw an exception with certain localities on certain platforms.</remarks>
        public static DateTime CurrentTime
        {
            get
            {
                try
                {
                    if (!UseUtc)
                        return DateTime.Now;
                    else
                        return DateTime.UtcNow;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception)
                {
                    UseUtc = true;
                    return DateTime.UtcNow;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        /// <summary>
        /// Converts a Base64 string to a byte array.
        /// </summary>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        /// <exception cref="FormatException">Thrown when the provided string isn't a valid Base64 string.</exception>
        public static byte[]? Base64ToByteArray(string base64Str)
        {
            if (string.IsNullOrEmpty(base64Str))
            {
                return null;
            }
            int tagIndex = base64Str.IndexOf(Base64Prefix);
            if (tagIndex >= 0)
            {
                int firstNonWhitespace = 0;
                int startIndex = 0;
                for (int i = 0; i <= tagIndex; i++)
                {
                    firstNonWhitespace = i;
                    if (!char.IsWhiteSpace(base64Str[i]))
                        break;
                }
                if (firstNonWhitespace == tagIndex)
                {
                    startIndex = tagIndex + Base64Prefix.Length;
                    for (int i = startIndex; i < base64Str.Length; i++)
                    {
                        startIndex = i;
                        if (!char.IsWhiteSpace(base64Str[i]))
                            break;
                    }
                    return Convert.FromBase64String(base64Str.Substring(startIndex));
                }
            }
            return Convert.FromBase64String(base64Str);
        }

        /// <summary>
        /// Converts a byte array to a base64 string.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static string ByteArrayToBase64(byte[]? byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return string.Empty;
            return Base64Prefix + Convert.ToBase64String(byteArray);
        }

        #region Image converting

        /// <summary>
        /// Converts an image at the given resource path to a base64 string.
        /// </summary>
        /// <param name="imageResourcePath"></param>
        /// <returns></returns>
        public static string ImageToBase64(string imageResourcePath)
        {
            try
            {
                byte[]? resource = GetResource(Assembly.GetCallingAssembly(), imageResourcePath);
                if (resource.Length == 0)
                {
                    //Logger.log?.Warn($"Unable to load image from path: {imagePath}");
                    return string.Empty;
                }
                return Convert.ToBase64String(resource);
            }
            catch (Exception)
            {
                throw;
                //Logger.log?.Warn($"Unable to load image from path: {imagePath}");
                //Logger.log?.Debug(ex);
            }
        }

        /// <summary>
        /// Gets a resource and returns it as a byte array.
        /// From https://github.com/brian91292/BeatSaber-CustomUI/blob/master/Utilities/Utilities.cs
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="ResourceName"></param>
        /// <returns></returns>
        public static byte[] GetResource(Assembly asm, string ResourceName)
        {
            try
            {
                using Stream stream = asm.GetManifestResourceStream(ResourceName);
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
                return data;
            }
            catch (NullReferenceException)
            {
                throw;
                //Logger.log?.Debug($"Resource {ResourceName} was not found.");
            }
        }
        #endregion
    }
}
