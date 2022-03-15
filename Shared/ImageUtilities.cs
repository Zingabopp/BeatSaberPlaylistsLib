using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace BeatSaberPlaylistsLib
{
    /// <summary>
    /// Utilities for modifying images.
    /// </summary>
    public static class ImageUtilities
    {
        public const int kImageSize = 256;
        
        /// <summary>
        /// Generate a collage of 2 images
        /// </summary>
        /// <param name="imageStream1"></param>
        /// <param name="imageStream2"></param>
        /// <returns></returns>
        public static async Task<Stream> GenerateCollage(Stream imageStream1, Stream imageStream2)
        {
            var image = new Image<Rgba32>(kImageSize, kImageSize);
            using var image1 = await Image.LoadAsync(imageStream1);
            using var image2 = await Image.LoadAsync(imageStream2);

            await Task.Run(() =>
            {
                image.Mutate(i =>
                {
                    image1.Mutate(i1 =>
                    {
                        i1.Resize(kImageSize, kImageSize);
                        i1.Crop(new Rectangle(kImageSize / 8, 0, kImageSize / 2, kImageSize));
                    });
                    i.DrawImage(image1, Point.Empty, 1.0f);

                    image2.Mutate(i2 =>
                    {
                        i2.Resize(kImageSize, kImageSize);
                        i2.Crop(new Rectangle(kImageSize / 8, 0, kImageSize / 2, kImageSize));
                    });
                    i.DrawImage(image2, new Point(kImageSize / 2, 0), 1.0f);
                });
            });

            var imageStream = new MemoryStream();
            await image.SaveAsPngAsync(imageStream);
            if (imageStream.CanSeek)
            {
                imageStream.Seek(0, SeekOrigin.Begin);
            }
            return imageStream;
        }
        
        /// <summary>
        /// Generate a collage of 3 images
        /// </summary>
        /// <param name="imageStream1"></param>
        /// <param name="imageStream2"></param>
        /// <param name="imageStream3"></param>
        /// <returns></returns>
        public static async Task<Stream> GenerateCollage(Stream imageStream1, Stream imageStream2, Stream imageStream3)
        {
            var image = new Image<Rgba32>(kImageSize, kImageSize);
            using var image1 = await Image.LoadAsync(imageStream1);
            using var image2 = await Image.LoadAsync(imageStream2);
            using var image3 = await Image.LoadAsync(imageStream3);

            await Task.Run(() =>
            {
                image.Mutate(i =>
                {
                    image1.Mutate(i1 =>
                    {
                        i1.Resize(kImageSize, kImageSize);
                        i1.Crop(new Rectangle(0, kImageSize / 8, kImageSize, kImageSize / 2));
                    });
                    i.DrawImage(image1, Point.Empty, 1.0f);

                    image2.Mutate(i2 =>
                    {
                        i2.Resize(kImageSize / 2, kImageSize / 2);
                    });
                    i.DrawImage(image2, new Point(0, kImageSize / 2), 1.0f);

                    image3.Mutate(i3 =>
                    {
                        i3.Resize(kImageSize / 2, kImageSize / 2);
                    });
                    i.DrawImage(image3, new Point(kImageSize / 2, kImageSize / 2), 1.0f);
                });
            });

            var imageStream = new MemoryStream();
            await image.SaveAsPngAsync(imageStream);
            if (imageStream.CanSeek)
            {
                imageStream.Seek(0, SeekOrigin.Begin);
            }
            return imageStream;
        }
        
        /// <summary>
        /// Generate a collage of 4 images
        /// </summary>
        /// <param name="imageStream1"></param>
        /// <param name="imageStream2"></param>
        /// <param name="imageStream3"></param>
        /// <param name="imageStream4"></param>
        /// <returns></returns>
        public static async Task<Stream> GenerateCollage(Stream imageStream1, Stream imageStream2, Stream imageStream3, Stream imageStream4)
        {
            var image = new Image<Rgba32>(kImageSize, kImageSize);
            using var image1 = await Image.LoadAsync(imageStream1);
            using var image2 = await Image.LoadAsync(imageStream2);
            using var image3 = await Image.LoadAsync(imageStream3);
            using var image4 = await Image.LoadAsync(imageStream4);

            await Task.Run(() =>
            {
                image.Mutate(i =>
                {
                    image1.Mutate(i1 =>
                    {
                        i1.Resize(kImageSize / 2, kImageSize / 2);
                    });
                    i.DrawImage(image1, Point.Empty, 1.0f);
                
                    image2.Mutate(i2 =>
                    {
                        i2.Resize(kImageSize / 2, kImageSize / 2);
                    });
                    i.DrawImage(image2, new Point(kImageSize / 2, 0), 1.0f);
                
                    image3.Mutate(i3 =>
                    {
                        i3.Resize(kImageSize / 2, kImageSize / 2);
                    });
                    i.DrawImage(image3, new Point(0, kImageSize / 2), 1.0f);
                
                    image4.Mutate(i4 =>
                    {
                        i4.Resize(kImageSize / 2, kImageSize / 2);
                    });
                    i.DrawImage(image4, new Point(kImageSize / 2, kImageSize / 2), 1.0f);
                });
            });

            var imageStream = new MemoryStream();
            await image.SaveAsPngAsync(imageStream);
            if (imageStream.CanSeek)
            {
                imageStream.Seek(0, SeekOrigin.Begin);
            }
            return imageStream;
        }
    }
}
