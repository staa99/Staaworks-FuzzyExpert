using System;
using System.Diagnostics;
using Emgu.CV.Structure;

namespace Emgu.CV
{
    /// <summary>
    /// An object recognizer using PCA (Principle Components Analysis)
    /// </summary>
    [Serializable]
    public class EigenObjectRecognizer
    {
        private Image<Gray, float>[] _eigenImages;
        private Image<Gray, float> _avgImage;

        /// <summary>
        /// Get the eigen vectors that form the eigen space
        /// </summary>
        /// <remarks>The set method is primary used for deserialization, do not attemps to set it unless you know what you are doing</remarks>
        public Image<Gray, float>[] EigenImages
        {
            get => _eigenImages;
            set => _eigenImages = value;
        }

        /// <summary>
        /// Get or set the labels for the corresponding training image
        /// </summary>
        public string[] Labels { get; set; }

        /// <summary>
        /// Get or set the eigen distance threshold.
        /// The smaller the number, the more likely an examined image will be treated as unrecognized object. 
        /// Set it to a huge number (e.g. 5000) and the recognizer will always treated the examined image as one of the known object. 
        /// </summary>
        public double EigenDistanceThreshold { get; set; }

        /// <summary>
        /// Get the average Image. 
        /// </summary>
        /// <remarks>The set method is primary used for deserialization, do not attemps to set it unless you know what you are doing</remarks>
        public Image<Gray, float> AverageImage
        {
            get => _avgImage;
            set => _avgImage = value;
        }

        /// <summary>
        /// Get the eigen values of each of the training image
        /// </summary>
        /// <remarks>The set method is primary used for deserialization, do not attemps to set it unless you know what you are doing</remarks>
        public Matrix<float>[] EigenValues { get; set; }

        private EigenObjectRecognizer ()
        {
        }


        /// <summary>
        /// Create an object recognizer using the specific tranning data and parameters, it will always return the most similar object
        /// </summary>
        /// <param name="images">The images used for training, each of them should be the same size. It's recommended the images are histogram normalized</param>
        /// <param name="termCrit">The criteria for recognizer training</param>
        public EigenObjectRecognizer (Image<Gray, byte>[] images, ref MCvTermCriteria termCrit)
           : this(images, GenerateLabels(images.Length), ref termCrit)
        {
        }

        private static string[] GenerateLabels (int size)
        {
            var labels = new string[size];
            for (var i = 0; i < size; i++)
            {
                labels[i] = i.ToString();
            }

            return labels;
        }

        /// <summary>
        /// Create an object recognizer using the specific tranning data and parameters, it will always return the most similar object
        /// </summary>
        /// <param name="images">The images used for training, each of them should be the same size. It's recommended the images are histogram normalized</param>
        /// <param name="labels">The labels corresponding to the images</param>
        /// <param name="termCrit">The criteria for recognizer training</param>
        public EigenObjectRecognizer (Image<Gray, byte>[] images, string[] labels, ref MCvTermCriteria termCrit)
           : this(images, labels, 0, ref termCrit)
        {
        }

        /// <summary>
        /// Create an object recognizer using the specific tranning data and parameters
        /// </summary>
        /// <param name="images">The images used for training, each of them should be the same size. It's recommended the images are histogram normalized</param>
        /// <param name="labels">The labels corresponding to the images</param>
        /// <param name="eigenDistanceThreshold">
        /// The eigen distance threshold, (0, ~1000].
        /// The smaller the number, the more likely an examined image will be treated as unrecognized object. 
        /// If the threshold is &lt; 0, the recognizer will always treated the examined image as one of the known object. 
        /// </param>
        /// <param name="termCrit">The criteria for recognizer training</param>
        public EigenObjectRecognizer (Image<Gray, byte>[] images, string[] labels, double eigenDistanceThreshold, ref MCvTermCriteria termCrit)
        {
            Debug.Assert(images.Length == labels.Length, "The number of images should equals the number of labels");
            Debug.Assert(eigenDistanceThreshold >= 0.0, "Eigen-distance threshold should always >= 0.0");

            CalcEigenObjects(images, ref termCrit, out _eigenImages, out _avgImage);

            /*
            _avgImage.SerializationCompressionRatio = 9;

            foreach (Image<Gray, float> img in _eigenImages)
                //Set the compression ration to best compression. The serialized object can therefore save spaces
                img.SerializationCompressionRatio = 9;
            */

            EigenValues = Array.ConvertAll(images,
                delegate (Image<Gray, byte> img)
                {
                    return new Matrix<float>(EigenDecomposite(img, _eigenImages, _avgImage));
                });

            Labels = labels;

            EigenDistanceThreshold = eigenDistanceThreshold;
        }

        #region static methods
        /// <summary>
        /// Caculate the eigen images for the specific traning image
        /// </summary>
        /// <param name="trainingImages">The images used for training </param>
        /// <param name="termCrit">The criteria for tranning</param>
        /// <param name="eigenImages">The resulting eigen images</param>
        /// <param name="avg">The resulting average image</param>
        public static void CalcEigenObjects (Image<Gray, byte>[] trainingImages, ref MCvTermCriteria termCrit, out Image<Gray, float>[] eigenImages, out Image<Gray, float> avg)
        {
            var width = trainingImages[0].Width;
            var height = trainingImages[0].Height;

            var inObjs = Array.ConvertAll(trainingImages, delegate(Image<Gray, byte> img) { return img.Ptr; });

            if (termCrit.max_iter <= 0 || termCrit.max_iter > trainingImages.Length)
            {
                termCrit.max_iter = trainingImages.Length;
            }

            var maxEigenObjs = termCrit.max_iter;

            #region initialize eigen images
            eigenImages = new Image<Gray, float>[maxEigenObjs];
            for (var i = 0; i < eigenImages.Length; i++)
            {
                eigenImages[i] = new Image<Gray, float>(width, height);
            }

            var eigObjs = Array.ConvertAll(eigenImages, delegate(Image<Gray, float> img) { return img.Ptr; });
            #endregion

            avg = new Image<Gray, float>(width, height);

            CvInvoke.cvCalcEigenObjects(
                inObjs,
                ref termCrit,
                eigObjs,
                null,
                avg.Ptr);
        }

        /// <summary>
        /// Decompose the image as eigen values, using the specific eigen vectors
        /// </summary>
        /// <param name="src">The image to be decomposed</param>
        /// <param name="eigenImages">The eigen images</param>
        /// <param name="avg">The average images</param>
        /// <returns>Eigen values of the decomposed image</returns>
        public static float[] EigenDecomposite (Image<Gray, byte> src, Image<Gray, float>[] eigenImages, Image<Gray, float> avg) => CvInvoke.cvEigenDecomposite(
                src.Ptr,
                Array.ConvertAll<Image<Gray, float>, IntPtr>(eigenImages, delegate (Image<Gray, float> img) { return img.Ptr; }),
                avg.Ptr);
        #endregion

        /// <summary>
        /// Given the eigen value, reconstruct the projected image
        /// </summary>
        /// <param name="eigenValue">The eigen values</param>
        /// <returns>The projected image</returns>
        public Image<Gray, byte> EigenProjection (float[] eigenValue)
        {
            var res = new Image<Gray, byte>(_avgImage.Width, _avgImage.Height);
            CvInvoke.cvEigenProjection(
                Array.ConvertAll<Image<Gray, float>, IntPtr>(_eigenImages, delegate (Image<Gray, float> img) { return img.Ptr; }),
                eigenValue,
                _avgImage.Ptr,
                res.Ptr);
            return res;
        }

        /// <summary>
        /// Get the Euclidean eigen-distance between <paramref name="image"/> and every other image in the database
        /// </summary>
        /// <param name="image">The image to be compared from the training images</param>
        /// <returns>An array of eigen distance from every image in the training images</returns>
        public float[] GetEigenDistances (Image<Gray, byte> image)
        {
            using (var eigenValue = new Matrix<float>(EigenDecomposite(image, _eigenImages, _avgImage)))
            {
                return Array.ConvertAll<Matrix<float>, float>(EigenValues,
                    delegate (Matrix<float> eigenValueI)
                    {
                        return (float) CvInvoke.cvNorm(eigenValue.Ptr, eigenValueI.Ptr, Emgu.CV.CvEnum.NORM_TYPE.CV_L2, IntPtr.Zero);
                    });
            }
        }

        /// <summary>
        /// Given the <paramref name="image"/> to be examined, find in the database the most similar object, return the index and the eigen distance
        /// </summary>
        /// <param name="image">The image to be searched from the database</param>
        /// <param name="index">The index of the most similar object</param>
        /// <param name="eigenDistance">The eigen distance of the most similar object</param>
        /// <param name="label">The label of the specific image</param>
        public void FindMostSimilarObject (Image<Gray, byte> image, out int index, out float eigenDistance, out string label)
        {
            index = 0;
            var dist = GetEigenDistances(image);
            eigenDistance = dist[0];
            for (var i = 1; i < dist.Length; i++)
            {
                if (dist[i] < eigenDistance)
                {
                    index = i;
                    eigenDistance = dist[i];
                }
            }
            label = Labels[index];
        }

        /// <summary>
        /// Try to recognize the image and return its label
        /// </summary>
        /// <param name="image">The image to be recognized</param>
        /// <returns>
        /// string.Empty, if not recognized;
        /// Label of the corresponding image, otherwise
        /// </returns>
        public string Recognize (Image<Gray, byte> image)
        {
            FindMostSimilarObject(image, out var index, out var eigenDistance, out var label);

            return (EigenDistanceThreshold <= 0 || eigenDistance < EigenDistanceThreshold) ? Labels[index] : string.Empty;
        }
    }
}
