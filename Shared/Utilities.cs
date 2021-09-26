using BeatSaberPlaylistsLib.Types;
using System;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Drawing.Text;

namespace BeatSaberPlaylistsLib
{
    /// <summary>
    /// Utilities
    /// </summary>
    public static partial class Utilities
    {
        private static Stream? GetDefaultImageStream() =>
            Assembly.GetExecutingAssembly().GetManifestResourceStream("BeatSaberPlaylistsLib.Icons.FolderIcon.png");


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
        /// Opens a file for reading and returns the <see cref="FileStream"/>.
        /// If a '.bak' of the file exists, the original is replaced by the backup and opened.
        /// </summary>
        /// <param name="playlistFilePath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IOException"></exception>
        public static FileStream OpenFileRead(string playlistFilePath)
        {
            if (playlistFilePath == null) throw new ArgumentNullException(nameof(playlistFilePath));
            string backupFile = playlistFilePath + ".bak";
            bool originalExists = File.Exists(playlistFilePath);
            bool backupExists = File.Exists(backupFile);
            try
            {
                if (backupExists)
                {
                    try
                    {
                        if (originalExists) File.Delete(playlistFilePath);
                        File.Move(backupFile, playlistFilePath);
                    }
                    catch (IOException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new IOException(ex.Message, ex);
                    }
                }
                return File.OpenRead(playlistFilePath);
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new IOException(ex.Message, ex);
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
            int dataIndex = GetBase64DataStartIndex(base64Str);
            if (dataIndex > 0)
                return Convert.FromBase64String(base64Str.Substring(dataIndex));
            else
                return Convert.FromBase64String(base64Str);
        }



        /// <summary>
        /// Returns the index of <paramref name="base64Str"/> that the data starts at.
        /// </summary>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        public static int GetBase64DataStartIndex(string base64Str)
        {
            int tagIndex = Math.Max(0, base64Str.IndexOf(',') + 1);
            return tagIndex;
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
        /// <summary>
        /// Converts a <see cref="Stream"/> to a byte array.
        /// From: https://stackoverflow.com/a/44929033
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] ToArray(this Stream s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (!s.CanRead)
                throw new ArgumentException("Stream cannot be read");

            if (s is MemoryStream ms)
                return ms.ToArray();

            long pos = s.CanSeek ? s.Position : 0L;
            if (pos != 0L)
                s.Seek(0, SeekOrigin.Begin);

            byte[] result = new byte[s.Length];
            s.Read(result, 0, result.Length);
            if (s.CanSeek)
                s.Seek(pos, SeekOrigin.Begin);
            return result;
        }

        #region Image

        private static readonly DrawSettings defaultDrawSettings = new DrawSettings
        {
            Color = Color.White,
            DrawStyle = DrawStyle.Normal,
            Font = FindFont("Microsoft Sans Serif", 60, FontStyle.Regular),
            StringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            },
            MinTextSize = 30,
            MaxTextSize = 60,
            WrapWidth = 10
        };

        /// <summary>
        /// Returns a found system font or default font for a given family name.
        /// </summary>
        /// <param name="font"></param>
        /// <param name="emSize"></param>
        /// <param name="fontStyle"></param>
        /// <returns></returns>
        public static Font FindFont(string font, float emSize, FontStyle fontStyle)
        {
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            foreach(FontFamily fontFamily in installedFontCollection.Families)
            {
                if (fontFamily.Name == font)
                {
                    return new Font(fontFamily, emSize, fontStyle);
                }
            }
            return new Font(installedFontCollection.Families.FirstOrDefault(), emSize, fontStyle);
        }

        /// <summary>
        /// Generates a cover for a playlist.
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        public static Stream GenerateCoverForPlaylist(IPlaylist playlist)
        {
            using Stream? stream = GetDefaultImageStream();
            //Console.WriteLine($"Drawing string {playlist.Title}");
            string title = playlist.Title;
            if (title.Length > 110)
            {
                title = string.Join("", (from word in playlist.Title.Split() select word[0]));
            }
            Image img = ImageUtilities.DrawString(title, Image.FromStream(stream), defaultDrawSettings);
            MemoryStream ms = new MemoryStream();
            img.Save(ms, ImageFormat.Png);
            return ms;
        }

        /// <summary>
        /// Converts an image at the given resource path to a base64 string.
        /// </summary>
        /// <param name="imageResourcePath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string ImageToBase64(string imageResourcePath)
        {
            try
            {
                byte[] resource = GetResource(Assembly.GetCallingAssembly(), imageResourcePath);
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
        /// <param name="resourceName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] GetResource(Assembly asm, string resourceName)
        {
            if (asm == null)
                throw new ArgumentNullException(nameof(asm));
            if (string.IsNullOrEmpty(resourceName))
                throw new ArgumentException($"'{resourceName}' is not a valid resource name.", nameof(resourceName));

            using Stream? stream = asm.GetManifestResourceStream(resourceName)
                ?? throw new ArgumentException($"Could not load resource, '{resourceName}', from assembly '{asm.FullName}'", nameof(resourceName));
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }
        #endregion


    }
}
