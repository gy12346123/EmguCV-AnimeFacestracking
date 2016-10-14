using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeFacestracking
{
    public class FaceDetect
    {
        CudaCascadeClassifier cudaFace;
        CascadeClassifier cascadeFace;
        bool tryUseCuda = false;
        string faceFilePath;
        public FaceDetect(string faceFile,bool useCuda)
        {
            faceFilePath = faceFile;
            tryUseCuda = useCuda;
            if (tryUseCuda)
            {
                cudaFace = new CudaCascadeClassifier(faceFilePath);
            }else
            {
                cascadeFace = new CascadeClassifier(faceFilePath);
            }
        }

        ~FaceDetect()
        {
            if(cudaFace != null)
            {
                cudaFace.Dispose();
                cudaFace = null;
            }
            if (cascadeFace != null)
            {
                cascadeFace.Dispose();
                cascadeFace = null;
            }
        }

        public void detect(Mat image,ref List<Rectangle> faces)
        {
            #if !(__IOS__ || NETFX_CORE)
            if (tryUseCuda && CudaInvoke.HasCuda)
            {
                cudaFace.ScaleFactor = 1.1;
                cudaFace.MinNeighbors = 10;
                cudaFace.MinObjectSize = Size.Empty;
                using (CudaImage<Bgr, Byte> gpuImage = new CudaImage<Bgr, byte>(image))
                using (CudaImage<Gray, Byte> gpuGray = gpuImage.Convert<Gray, Byte>())
                using (GpuMat region = new GpuMat())
                {
                    cudaFace.DetectMultiScale(gpuGray, region);
                    Rectangle[] faceRegion = cudaFace.Convert(region);
                    faces.AddRange(faceRegion);
                }

            }
            else
            #endif
            {
                //Read the HaarCascade objects
                using (UMat ugray = new UMat())
                {
                    CvInvoke.CvtColor(image, ugray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

                    //normalizes brightness and increases contrast of the image
                    CvInvoke.EqualizeHist(ugray, ugray);

                    //Detect the faces  from the gray scale image and store the locations as rectangle
                    //The first dimensional is the channel
                    //The second dimension is the index of the rectangle in the specific channel

                    Rectangle[] facesDetected = cascadeFace.DetectMultiScale(
                       ugray,
                       1.1,
                       10,
                       new Size(20, 20));

                    faces.AddRange(facesDetected);
                }
            }
        }
    }
}
