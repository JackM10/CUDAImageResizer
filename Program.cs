using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GPUImageResizer
{
    class Program
    {
        static void Main()
        {
            var cudaDeviceInfo = new CudaDeviceInfo();

            if (cudaDeviceInfo.IsCompatible) //ToDo: realize IDisposable pattern, check all performance issues
            {
                ResizeImage();
            }
            else
            {
                Console.WriteLine("No CUDA compatible harware found");
            }
        }

        private static void ResizeImage(string imageName = "111.jpg", string pathToImages = @"D:\Temp", int width = 400, int height = 299)
        {
            var newFileName = imageName + "_AfterResize.jpg";
            var imageFileNamePath = Path.Combine(pathToImages, imageName);
            var newImageFilePathName = Path.Combine(pathToImages, newFileName);

            using (var image = new Bitmap(imageFileNamePath))
            {
                var newImage = new Bitmap(image);
                Image<Bgr, Byte> img = new Image<Bgr, Byte>(newImage);
                Size newSize = new Size(width, height);
                ResizeBitmapWithCuda(img, ref newSize);
                img = img.Resize(400, 299, Inter.Nearest);
                img.ToBitmap().Save(newFileName, ImageFormat.Jpeg);
            }
        }

        private static Bitmap ResizeBitmapWithCuda(Image<Bgr, Byte> sourceBM, ref Size newSize)
        {
            // Initialize Emgu Image object
            CudaImage<Bgr, Byte> img = new CudaImage<Bgr, Byte>(sourceBM);            
            // Resize using liniear interpolation
            img.Resize(newSize, Inter.Linear);

            // Return .NET Bitmap object
            return img.Bitmap;
        }
    }
}
