using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace FoxOne.Core
{
    public static class ImageHelper
    {
        public static string CompressImage(string sourceImagePath, int ThumbnailImageHeight, int ThumbnailImageWidth)
        {
            string targetPath = GetTargetImagePath(sourceImagePath, ThumbnailImageWidth, ThumbnailImageHeight);
            System.Drawing.Image image = System.Drawing.Image.FromFile(sourceImagePath);
            int srcX = 0, srcY = 0;
            int thumbWidth = 0, thumbHeight = 0;
            int width = image.Width;
            int height = image.Height;
            decimal a = Math.Round((decimal)ThumbnailImageWidth / ThumbnailImageHeight, 2);
            decimal b = Math.Round((decimal)width / height, 2);
            if (a >= b)
            {
                thumbWidth = width;
                thumbHeight = (width * ThumbnailImageHeight) / ThumbnailImageWidth;
                srcX = 0;
                srcY = (height - thumbHeight) / 2;
            }
            else
            {
                thumbHeight = height;
                thumbWidth = (height * ThumbnailImageWidth) / ThumbnailImageHeight;
                srcY = 0;
                srcX = (width - thumbWidth) / 2;
            }
            Bitmap bitmap = new Bitmap(ThumbnailImageWidth, ThumbnailImageHeight);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.DrawImage(image, new Rectangle(0, 0, ThumbnailImageWidth, ThumbnailImageHeight), srcX, srcY, thumbWidth, thumbHeight, GraphicsUnit.Pixel);
            image.Dispose();
            FileInfo dirInfo = new FileInfo(targetPath);
            if (!dirInfo.Directory.Exists)
            {
                dirInfo.Directory.Create();
            }
            try
            {
                ImageCodecInfo encoder = GetEncoderInfo("image/jpeg");
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);
                bitmap.Save(targetPath, encoder, encoderParams);
                encoderParams.Dispose();
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                bitmap.Dispose();
                graphics.Dispose();
            }
            return targetPath;
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="sourceImagePath">原图片路径(相对路径)</param>
        /// <param name="thumbnailImagePath">生成的缩略图路径,如果为空则保存为原图片路径(相对路径)</param>
        /// <param name="thumbnailImageWidth">缩略图的宽度（高度与按源图片比例自动生成）</param>
        public static void GetThumbnailImages(string SourceImagePath, int ThumbnailImageWidth, int ThumbnailImageHeight)
        {
            string targetPath = GetTargetImagePath(SourceImagePath, ThumbnailImageWidth, ThumbnailImageHeight);
            System.Drawing.Image image = System.Drawing.Image.FromFile(SourceImagePath);
            int num = 0;
            int startX = 0, startY = 0;
            int thumbWidth = 0, thumbHeight = 0;
            int width = image.Width;
            int height = image.Height;
            if (width >= height)
            {
                num = height * ThumbnailImageWidth / width;
                startY = (ThumbnailImageHeight - num) / 2;
                thumbWidth = ThumbnailImageWidth;
                thumbHeight = num;
            }
            else
            {
                num = width * ThumbnailImageWidth / height;
                startX = (ThumbnailImageWidth - num) / 2;
                thumbHeight = ThumbnailImageWidth;
                thumbWidth = num;
            }
            Bitmap bitmap = new Bitmap(ThumbnailImageWidth, ThumbnailImageHeight);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.DrawImage(image, new Rectangle(startX, startY, thumbWidth, thumbHeight));
            image.Dispose();
            FileInfo dirInfo = new FileInfo(targetPath);
            if (!dirInfo.Directory.Exists)
            {
                dirInfo.Directory.Create();
            }
            try
            {
                ImageCodecInfo encoder = GetEncoderInfo("image/jpeg");
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)100);
                bitmap.Save(targetPath, encoder, encoderParams);
                encoderParams.Dispose();
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                bitmap.Dispose();
                graphics.Dispose();
            }
        }

        public static string GetTargetImagePath(string imageUrl, int width, int height)
        {
            int index = imageUrl.LastIndexOf('\\');
            string part1 = imageUrl.Substring(0, index);
            string part2 = imageUrl.Substring(index);
            imageUrl = string.Format("{0}\\{1}{2}", part1, string.Format("{0}_{1}", width, height), part2);
            return imageUrl;
        }

        public static string GetImageDisplayPath(string imageUrl, int width, int height)
        {
            if (string.IsNullOrEmpty(imageUrl)) return imageUrl;
            int index = imageUrl.LastIndexOf('/');
            string part1 = imageUrl.Substring(0, index);
            string part2 = imageUrl.Substring(index);
            imageUrl = string.Format("{0}/{1}{2}", part1, string.Format("{0}_{1}", width, height), part2);
            return imageUrl;
        }

        public static void CompressImage(string sourcePath, string targetPath, long value)
        {
            Bitmap bitmap = new Bitmap(sourcePath);
            ImageCodecInfo encoder = GetEncoderInfo("image/jpeg");
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, value);
            bitmap.Save(targetPath, encoder, encoderParams);
            encoderParams.Dispose();
            bitmap.Dispose();
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo result = null;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < encoders.Length; i++)
            {
                if (encoders[i].MimeType == mimeType)
                {
                    result = encoders[i];
                    break;
                }
            }
            return result;
        }
    }
}
