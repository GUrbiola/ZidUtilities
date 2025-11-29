using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZidUtilities.CommonCode
{
    /// <summary>
    /// Provides helper methods for processing images, including cropping, scaling, encoding information retrieval,
    /// metadata extraction, and grayscale conversion using System.Drawing APIs.
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// Crops the specified <see cref="Image"/> using the provided rectangle and returns a new <see cref="Bitmap"/> containing the cropped area.
        /// </summary>
        /// <param name="originalImage">The source image to crop.</param>
        /// <param name="cropRectangle">The rectangle, in source image coordinates, defining the area to crop.</param>
        /// <returns>A new <see cref="Bitmap"/> containing the cropped pixels.</returns>
        /// <remarks>
        /// The returned bitmap is a new object. The <paramref name="originalImage"/> is not modified.
        /// </remarks>
        public Bitmap CropImage(Image originalImage, Rectangle cropRectangle)
        {
            Rectangle? destinationRectangle = new Rectangle(Point.Empty, cropRectangle.Size);

            var croppedImage = new Bitmap(destinationRectangle.Value.Width, destinationRectangle.Value.Height);

            using (var graphics = Graphics.FromImage(croppedImage))
            {
                graphics.DrawImage(originalImage, destinationRectangle.Value, cropRectangle, GraphicsUnit.Pixel);
            }
            return croppedImage;
        }

        /// <summary>
        /// Resizes the given bitmap to fit within the specified maximum width and height, preserving aspect ratio, and saves it as a JPEG with the given quality.
        /// </summary>
        /// <param name="image">The source <see cref="Bitmap"/> to resize.</param>
        /// <param name="maxWidth">The maximum width of the output image in pixels.</param>
        /// <param name="maxHeight">The maximum height of the output image in pixels.</param>
        /// <param name="quality">The JPEG quality (0–100) to use when saving.</param>
        /// <param name="filePath">The destination file path where the image will be saved.</param>
        /// <returns>Void. The method writes the resized image to <paramref name="filePath"/>.</returns>
        /// <remarks>
        /// Converts the image to 24bpp RGB to avoid issues with non-RGB color spaces (e.g., CMYK). The original image is not modified.
        /// </remarks>
        public void ScaleToMaxWidthOrHeight(Bitmap image, int maxWidth, int maxHeight, int quality, string filePath)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            // Get an ImageCodecInfo object that represents the JPEG codec.
            ImageCodecInfo imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);

            // Create an Encoder object for the Quality parameter.
            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object. 
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Save the image as a JPEG file with quality level.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, (long)quality);
            encoderParameters.Param[0] = encoderParameter;
            newImage.Save(filePath, imageCodecInfo, encoderParameters);
        }

        /// <summary>
        /// Resizes and saves the bitmap to a Workday-friendly square thumbnail (up to 200x200) as a JPEG.
        /// </summary>
        /// <param name="image">The source <see cref="Bitmap"/> to process.</param>
        /// <param name="filePath">The destination file path for the output image.</param>
        /// <param name="quality">The JPEG quality (0–100). Defaults to 100.</param>
        /// <returns>Void. The method writes the processed image to <paramref name="filePath"/>.</returns>
        public void SaveAsWorkdayFriendly(Bitmap image, string filePath, int quality = 100)
        {
            this.ScaleToMaxWidthOrHeight(image, 200, 200, quality, filePath);
        }

        /// <summary>
        /// Retrieves the encoder information for the specified image format.
        /// </summary>
        /// <param name="format">The <see cref="ImageFormat"/> for which to retrieve encoder information.</param>
        /// <returns>
        /// An <see cref="ImageCodecInfo"/> instance for the specified format if found; otherwise, <c>null</c>.
        /// </returns>
        public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

        /// <summary>
        /// Loads an image from the specified file and returns detailed bitmap information (pixel format, size, DPI, codec, etc.).
        /// </summary>
        /// <param name="FileName">The full path to the image file to analyze.</param>
        /// <returns>
        /// An <see cref="ImagingBitmapInfo"/> containing metadata and characteristics of the image.
        /// </returns>
        /// <remarks>
        /// Opens the file for read access and delegates processing to <see cref="BitmapPixelFormat(Stream)"/>.
        /// </remarks>
        public static ImagingBitmapInfo BitmapPixelFormat(string FileName)
        {
            using (FileStream stream = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return BitmapPixelFormat(stream);
            }
        }

        /// <summary>
        /// Reads an image from the provided stream and returns detailed bitmap information (pixel format, size, DPI, palette, codec, etc.).
        /// </summary>
        /// <param name="stream">The readable <see cref="Stream"/> from which the image will be loaded. The stream is not disposed by this method.</param>
        /// <returns>
        /// An <see cref="ImagingBitmapInfo"/> describing the image's properties and codec information.
        /// </returns>
        /// <remarks>
        /// The caller is responsible for managing the lifetime of <paramref name="stream"/>. This method inspects the image and extracts metadata such as pixel format, palette, DPI, and codec/mime information.
        /// </remarks>
        public static ImagingBitmapInfo BitmapPixelFormat(Stream stream)
        {
            ImagingBitmapInfo imageInfo = new ImagingBitmapInfo();
            Image image = Image.FromStream(stream);
            imageInfo.PixelFormat = image.PixelFormat;
            imageInfo.ImageSize = image.PhysicalDimension;
            imageInfo.HasPalette = (image.Palette.Entries.Length > 0) ? true : false;
            Guid[] FrameList = image.FrameDimensionsList;
            if (FrameList.Length > 0)
                imageInfo.HasAnimation = FrameList.Any(f => f == FrameDimension.Time.Guid);
            imageInfo.Palette = image.Palette;
            imageInfo.Dpi = new SizeF(image.HorizontalResolution, image.VerticalResolution);
            imageInfo.Flags = (ImageFlags)image.Flags;
            imageInfo.PixelSize = image.Size;
            imageInfo.ImageFormat = image.RawFormat;
            imageInfo.PixelFormatSize = Image.GetPixelFormatSize(image.PixelFormat);
            ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders()
                .FirstOrDefault(enc =>
                    enc.FormatID == image.RawFormat.Guid);
            imageInfo.ImageType = codec.FilenameExtension.ToLowerInvariant();
            imageInfo.MimeType = codec.MimeType;

            return imageInfo;
        }

        /// <summary>
        /// Creates a grayscale version of the provided bitmap using a luminance-based color matrix.
        /// </summary>
        /// <param name="original">The source <see cref="Bitmap"/> to convert to grayscale.</param>
        /// <returns>A new <see cref="Bitmap"/> containing the grayscale image.</returns>
        /// <remarks>
        /// The original bitmap is not modified. The caller is responsible for disposing the returned bitmap.
        /// </remarks>
        public static Bitmap MakeGrayScale(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        /// <summary>
        /// Creates and returns a resized bitmap scaled by the given percentage.
        /// </summary>
        /// <param name="original">The source <see cref="Bitmap"/> to resize.</param>
        /// <param name="percentageChange">Scaling factor applied to both width and height (e.g., 0.5 reduces size by half).</param>
        /// <returns>A new <see cref="Bitmap"/> instance with resized dimensions.</returns>
        public static Bitmap ResizeBitmap(Bitmap original, float percentageChange)
        {
            return new Bitmap(original, (int)(original.Width * percentageChange), (int)(original.Height * percentageChange));
        }

        /// <summary>
        /// Creates and returns a resized bitmap with explicit width and height.
        /// </summary>
        /// <param name="original">The source <see cref="Bitmap"/> to resize.</param>
        /// <param name="width">Desired output width in pixels.</param>
        /// <param name="height">Desired output height in pixels.</param>
        /// <returns>A new <see cref="Bitmap"/> instance with the requested dimensions.</returns>
        public static Bitmap ResizeBitmap(Bitmap original, int width, int height)
        {
            return new Bitmap(original, width, height);
        }

        /// <summary>
        /// Converts the provided bitmap to a 32bpp ARGB bitmap, preserving resolution.
        /// </summary>
        /// <param name="original">The source <see cref="Bitmap"/> to convert.</param>
        /// <returns>A new <see cref="Bitmap"/> in <see cref="PixelFormat.Format32bppArgb"/> format containing the original image data.</returns>
        public static Bitmap ConvertToRGB(Bitmap original)
        {
            Bitmap newImage = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            newImage.SetResolution(original.HorizontalResolution, original.VerticalResolution);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.DrawImageUnscaled(original, 0, 0);
            }
            return newImage;
        }

        /// <summary>
        /// Converts the provided bitmap to a bitonal (1bpp indexed) image using a fixed luminance threshold.
        /// </summary>
        /// <param name="original">The source <see cref="Bitmap"/> to convert. If not 32bpp ARGB, the method will convert it temporarily for processing.</param>
        /// <returns>A new <see cref="Bitmap"/> in <see cref="PixelFormat.Format1bppIndexed"/> format representing a black-and-white version of the source image.</returns>
        /// <remarks>
        /// This conversion uses a simple threshold-based algorithm to decide whether a pixel becomes black or white.
        /// The returned bitmap is a new object; the original bitmap is not modified.
        /// </remarks>
        public static Bitmap ConvertToBitonal(Bitmap original)
        {
            Bitmap source = null;

            // If original bitmap is not already in 32 BPP, ARGB format, then convert
            if (original.PixelFormat != PixelFormat.Format32bppArgb)
            {
                source = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
                source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
                using (Graphics g = Graphics.FromImage(source))
                {
                    g.DrawImageUnscaled(original, 0, 0);
                }
            }
            else
            {
                source = original;
            }

            // Lock source bitmap in memory
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Copy image data to binary array
            int imageSize = sourceData.Stride * sourceData.Height;
            byte[] sourceBuffer = new byte[imageSize];
            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);

            // Unlock source bitmap
            source.UnlockBits(sourceData);

            // Create destination bitmap
            Bitmap destination = new Bitmap(source.Width, source.Height, PixelFormat.Format1bppIndexed);
            destination.SetResolution(original.HorizontalResolution, original.VerticalResolution);

            // Lock destination bitmap in memory
            BitmapData destinationData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

            // Create destination buffer
            imageSize = destinationData.Stride * destinationData.Height;
            byte[] destinationBuffer = new byte[imageSize];

            int sourceIndex = 0;
            int destinationIndex = 0;
            int pixelTotal = 0;
            byte destinationValue = 0;
            int pixelValue = 128;
            int height = source.Height;
            int width = source.Width;
            int threshold = 500;

            // Iterate lines
            for (int y = 0; y < height; y++)
            {
                sourceIndex = y * sourceData.Stride;
                destinationIndex = y * destinationData.Stride;
                destinationValue = 0;
                pixelValue = 128;

                // Iterate pixels
                for (int x = 0; x < width; x++)
                {
                    // Compute pixel brightness (i.e. total of Red, Green, and Blue values) - Thanks murx
                    //                           B                             G                              R
                    pixelTotal = sourceBuffer[sourceIndex] + sourceBuffer[sourceIndex + 1] + sourceBuffer[sourceIndex + 2];
                    if (pixelTotal > threshold)
                    {
                        destinationValue += (byte)pixelValue;
                    }
                    if (pixelValue == 1)
                    {
                        destinationBuffer[destinationIndex] = destinationValue;
                        destinationIndex++;
                        destinationValue = 0;
                        pixelValue = 128;
                    }
                    else
                    {
                        pixelValue >>= 1;
                    }
                    sourceIndex += 4;
                }
                if (pixelValue != 128)
                {
                    destinationBuffer[destinationIndex] = destinationValue;
                }
            }

            // Copy binary image data to destination bitmap
            Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize);

            // Unlock destination bitmap
            destination.UnlockBits(destinationData);

            // Dispose of source if not originally supplied bitmap
            if (source != original)
            {
                source.Dispose();
            }

            // Return
            return destination;
        }
    }
    /// <summary>
    /// Represents detailed information about an image/bitmap including format, pixel characteristics, palette, DPI, and codec metadata.
    /// </summary>
    public class ImagingBitmapInfo
    {
        private ImageFlags flags;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagingBitmapInfo"/> class with default values.
        /// </summary>
        public ImagingBitmapInfo()
        {
            this.flags = ImageFlags.None;
            this.BitmapData = new BitmapDataInfo();
        }

        /// <summary>
        /// Gets or sets additional interpreted flag-based metadata about the bitmap.
        /// </summary>
        public BitmapDataInfo BitmapData { get; set; }

        /// <summary>
        /// Gets or sets the image format (e.g., JPEG, PNG).
        /// </summary>
        public ImageFormat ImageFormat { get; set; }

        /// <summary>
        /// Gets or sets the filename extension(s) representing the image type (e.g., ".jpg;.jpeg").
        /// </summary>
        public string ImageType { get; set; }

        /// <summary>
        /// Gets or sets the mime type of the image (e.g., "image/jpeg").
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the physical dimension of the image in document units.
        /// </summary>
        public SizeF ImageSize { get; set; }

        /// <summary>
        /// Gets or sets the horizontal and vertical DPI of the image.
        /// </summary>
        public SizeF Dpi { get; set; }

        /// <summary>
        /// Gets or sets the pixel size of the image (width and height in pixels).
        /// </summary>
        public Size PixelSize { get; set; }

        /// <summary>
        /// Gets or sets the number of bits per pixel for the pixel format.
        /// </summary>
        public int PixelFormatSize { get; set; }

        /// <summary>
        /// Gets or sets the effective bits per pixel value (may mirror <see cref="PixelFormatSize"/>).
        /// </summary>
        public int BitsPerPixel { get; set; }

        /// <summary>
        /// Gets or sets the GDI+ pixel format of the image.
        /// </summary>
        public PixelFormat PixelFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the image has a color palette.
        /// </summary>
        public bool HasPalette { get; set; }

        /// <summary>
        /// Gets or sets the color palette associated with the image, if any.
        /// </summary>
        public ColorPalette Palette { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the image contains animation frames (e.g., GIF).
        /// </summary>
        public bool HasAnimation { get; set; }

        /// <summary>
        /// Gets or sets the raw <see cref="ImageFlags"/> value that describes characteristics of the image.
        /// Setting this property updates <see cref="BitmapData"/> derived fields.
        /// </summary>
        public ImageFlags Flags
        {
            get { return this.flags; }
            set
            {
                this.BitmapData.SetValues(value);
                this.flags = value;
            }
        }

        /// <summary>
        /// Determines whether the image is grayscale by using default (shallow) detection.
        /// </summary>
        /// <returns><c>true</c> if the image is grayscale; otherwise, <c>false</c>.</returns>
        public bool IsGrayScale()
        { return CheckIfGrayScale(false); }

        /// <summary>
        /// Determines whether the image is grayscale, optionally performing a deep palette scan for indexed formats.
        /// </summary>
        /// <param name="DeepScan">If <c>true</c>, performs a palette analysis for indexed formats to verify grayscale.</param>
        /// <returns><c>true</c> if the image is grayscale; otherwise, <c>false</c>.</returns>
        public bool IsGrayScale(bool DeepScan)
        { return CheckIfGrayScale(DeepScan); }

        /// <summary>
        /// Internal method to check grayscale status based on flags, pixel format, and optionally palette contents.
        /// </summary>
        /// <param name="DeepScan">If <c>true</c>, inspects the palette for indexed formats to ensure all entries are gray.</param>
        /// <returns><c>true</c> if grayscale is detected; otherwise, <c>false</c>.</returns>
        private bool CheckIfGrayScale(bool DeepScan)
        {
            bool result = false;

            if ((this.BitmapData.ColorSpaceGRAY == true) || (this.PixelFormat == PixelFormat.Format16bppGrayScale))
            {
                result = true;
            }
            else if (this.PixelFormat == PixelFormat.Format8bppIndexed || this.PixelFormat == PixelFormat.Format4bppIndexed || this.PixelFormat == PixelFormat.Format1bppIndexed)
            {
                DeepScan = true;
            }

            if (DeepScan & this.Palette != null)
            {
                List<Color> IndexedColors = this.Palette.Entries.ToList();
                result = IndexedColors.All(rgb => (rgb.R == rgb.G && rgb.G == rgb.B && rgb.B == rgb.R));
            }
            return result;
        }

    }
    /// <summary>
    /// Contains interpreted metadata derived from <see cref="ImageFlags"/> describing transparency, scalability, and color spaces.
    /// </summary>
    public class BitmapDataInfo
    {
        /// <summary>
        /// Gets a value indicating whether any flags are present.
        /// </summary>
        public bool HasFlags { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the image has an alpha channel.
        /// </summary>
        public bool HasAlpha { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the image contains translucent pixels.
        /// </summary>
        public bool IsTranslucent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the image is fully scalable.
        /// </summary>
        public bool IsScalable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the image is partially scalable.
        /// </summary>
        public bool IsPartiallyScalable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the image uses the RGB color space.
        /// </summary>
        public bool ColorSpaceRGB { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the image uses the CMYK color space.
        /// </summary>
        public bool ColorSpaceCMYK { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the image uses the GRAY color space.
        /// </summary>
        public bool ColorSpaceGRAY { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the image uses the YCbCr color space.
        /// </summary>
        public bool ColorSpaceYCBCR { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the image uses the YCCK color space.
        /// </summary>
        public bool ColorSpaceYCCK { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the image has real DPI information.
        /// </summary>
        public bool HasRealDPI { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the image has real pixel size information.
        /// </summary>
        public bool HasRealPixelSize { get; private set; }

        /// <summary>
        /// Updates the interpreted properties from the specified <see cref="ImageFlags"/>.
        /// </summary>
        /// <param name="Flags">The <see cref="ImageFlags"/> value to interpret.</param>
        /// <returns>Void. Properties of this instance are updated based on the provided flags.</returns>
        internal void SetValues(ImageFlags Flags)
        {
            this.HasFlags = (Flags > 0);
            if (Flags > 0)
            {
                this.HasAlpha = ((Flags & ImageFlags.HasAlpha) > 0);
                this.HasRealDPI = ((Flags & ImageFlags.HasRealDpi) > 0);
                this.HasRealPixelSize = ((Flags & ImageFlags.HasRealPixelSize) > 0);
                this.IsTranslucent = ((Flags & ImageFlags.HasTranslucent) > 0);
                this.IsPartiallyScalable = ((Flags & ImageFlags.PartiallyScalable) > 0);
                this.IsScalable = ((Flags & ImageFlags.Scalable) > 0);
                this.ColorSpaceCMYK = ((Flags & ImageFlags.ColorSpaceCmyk) > 0);
                this.ColorSpaceGRAY = ((Flags & ImageFlags.ColorSpaceGray) > 0);
                this.ColorSpaceRGB = ((Flags & ImageFlags.ColorSpaceRgb) > 0);
                this.ColorSpaceYCBCR = ((Flags & ImageFlags.ColorSpaceYcbcr) > 0);
                this.ColorSpaceYCCK = ((Flags & ImageFlags.ColorSpaceYcck) > 0);
            }
        }
    }

}
