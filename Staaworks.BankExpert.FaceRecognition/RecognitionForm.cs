using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Staaworks.BankExpert.Shared.Interfaces;
using Staaworks.BankExpert.Shared.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Staaworks.BankExpert.FaceRecognition
{
    public partial class RecognitionForm : Form, IImageRecognitionReceptor
    {
        private FixedArray<int> LastNRecognizedLabels { get; }
        public FaceRecognitionSchemes RecognitionScheme { get; }

        Image<Bgr, byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        HaarCascade eye;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels= new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name, names = null;

        public RecognitionForm (int noOfLabels, FaceRecognitionSchemes scheme)
        {
            InitializeComponent();
            LastNRecognizedLabels = new FixedArray<int>(noOfLabels, -1);
            RecognitionScheme = scheme;

            //Load haarcascades for face detection
            face = new HaarCascade("haarcascade_frontalface_default.xml");

        }

        public int GetResolvedLabel () => throw new NotImplementedException();
    }
}
