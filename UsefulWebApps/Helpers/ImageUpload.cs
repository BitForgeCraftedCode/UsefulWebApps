using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace UsefulWebApps.Helpers
{
    public static class ImageUpload
    {
        public static async Task<(bool Success, string? FilePathDb, string? ErrorMessage)> ProcessAndSaveImageAsync(
        IFormFile imageFile,
        string webRootPath,
        string relativeFolder,
        int maxWidth,
        bool preserveOriginalFormat = false,
        int jpegQuality = 75,
        int maxFileSize = 10 * 1024 * 1024)
        {
            // max image size 10MB

            if (imageFile == null ||
                imageFile.Length == 0 ||
                imageFile.Length > maxFileSize ||
                !imageFile.ContentType.StartsWith("image/"))
            {
                return (false, null, "Invalid image file. Please try again.");
            }
            // Determine format from content type
            /*
             * switch expression
             * The cast (IImageEncoder) on the first arm is needed because the compiler infers the tuple type from all arms together. 
             * Since PngEncoder, GifEncoder, etc. are all different types, the compiler needs a hint that they should all be treated 
             * as their shared interface IImageEncoder. Once the first arm establishes that type, the rest don't need the explicit cast.
             */
            string extension;
            IImageEncoder encoder;

            if (preserveOriginalFormat)
            {
                (extension, encoder) = imageFile.ContentType switch
                {
                    "image/png" => ("png", (IImageEncoder)new PngEncoder()),
                    "image/gif" => ("gif", new GifEncoder()),
                    "image/webp" => ("webp", new WebpEncoder { Quality = jpegQuality }),
                    _ => ("jpg", new JpegEncoder { Quality = jpegQuality }) // default jpeg for jpg, bmp, tiff, etc
                };
            }
            else
            {
                extension = "jpg";
                encoder = new JpegEncoder { Quality = jpegQuality };
            }
            // generate unique file name
            string fileName = $"{Guid.NewGuid()}.{extension}";
            string directory = Path.Combine(webRootPath, relativeFolder);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            //get filepath for physical storage location
            string storageFilePath = Path.Combine(directory, fileName);
            //get filepath for database
            // normalize slashes for DB path
            string filePathDb = "/" + Path.Combine(relativeFolder, fileName).Replace("\\", "/");

            try
            {
                //resize the image then save to storage location
                using (Image image = await Image.LoadAsync(imageFile.OpenReadStream()))
                {
                    image.Mutate(x => x.AutoOrient());

                    if (image.Width > maxWidth)
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new Size(maxWidth, 0), // 0 for height auto
                            Mode = ResizeMode.Max
                        }));
                    }
                    //upload image -- copy/save image to wwwroot
                    await image.SaveAsync(storageFilePath, encoder);
                }
            }
            catch
            {
                return (false, null, "Invalid or corrupted image. Please try again.");
            }

            return (true, filePathDb, null);
        }
    }
}
