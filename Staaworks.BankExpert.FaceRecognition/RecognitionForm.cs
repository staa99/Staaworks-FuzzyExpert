using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Staaworks.BankExpert.Shared.Interfaces;
using Staaworks.BankExpert.Shared.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static Staaworks.BankExpert.Shared.Interfaces.FaceRecognitionSchemes;


namespace Staaworks.BankExpert.FaceRecognition
{
    public partial class RecognitionForm : Form, IImageRecognitionReceptor
    {
        private FixedArray<string> LastNRecognizedLabels { get; }
        public (FaceRecognitionSchemes scheme, dynamic data) SchemeInfo { get; }
        public Action OnComplete { set; get; }
        public string BaseDirectory { get; }
        public string TrainedFacesDirectory { get; }
        private EventHandler Handler { get; set; }

        private int errorCount = 0;
        Image<Bgr, byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        //HaarCascade eye;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels= new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        string name;

        public RecognitionForm (int noOfLabels, (FaceRecognitionSchemes scheme, object data) schemeInfo)
        {
            InitializeComponent();
            LastNRecognizedLabels = new FixedArray<string>(noOfLabels, null);
            SchemeInfo = schemeInfo;
            BaseDirectory = Application.StartupPath + "/Config/Face";
            TrainedFacesDirectory = BaseDirectory + "/TrainedFaces";

            Handler = new EventHandler(FrameGrabber);
            Directory.CreateDirectory(BaseDirectory);
            Directory.CreateDirectory(TrainedFacesDirectory);

            Shown += (o, e) =>
            {
                Initialize();
            };

            FormClosed += (o, e) =>
            {
                Application.Idle -= Handler;
                face = null;
                grabber.Dispose();
                grabber = null;
            };
        }

        public void Initialize()
        {
            //Load haarcascades for face detection
            face = new HaarCascade("haarcascade_frontalface_default.xml");

            // load training data
            //Initialize the capture device
            grabber = new Capture();
            grabber.QueryFrame();

            try
            {
                //Load previous trained faces and labels for each image
                var Labelsinfo = File.ReadAllText(BaseDirectory + "/TrainedLabels.txt");
                var Labels = Labelsinfo.Split('%');
                NumLabels = Convert.ToInt16(Labels[0]);
                ContTrain = NumLabels;
                string LoadFaces;

                for (var tf = 1; tf < NumLabels + 1; tf++)
                {
                    LoadFaces = "face" + tf + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(TrainedFacesDirectory + "/" + LoadFaces));
                    labels.Add(Labels[tf]);
                }
            }
            catch
            {
                Console.Beep();
            }

            //Initialize the FrameGraber event
            Application.Idle += Handler;


            switch (SchemeInfo.scheme)
            {
                case AuthenticationScheme:
                    Controls.Remove(RegBtn);
                    break;
            }
        }

        public string GetResolvedLabel () => LastNRecognizedLabels.GetMostPopularLabel();


        private void AddFaceButton_Click (object sender, EventArgs e)
        {
            ContTrain++;

            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(320, 240, INTER.CV_INTER_CUBIC);

            gray = grabber.QueryGrayFrame().Resize(320, 240, INTER.CV_INTER_CUBIC);

            //Face Detector
            var facesDetected = gray.DetectHaarCascade(
                face,
                1.2,
                10,
                HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                new Size(20, 20));


            if (facesDetected[0].Any())
            {
                var f = facesDetected[0][0];
                TrainedFace = currentFrame.Copy(f.rect).Convert<Gray, byte>();
            }

            try
            {
                //resize face detected image for force to compare the same size with the 
                //test image with cubic interpolation type method
                TrainedFace = TrainedFace.Resize(100, 100, INTER.CV_INTER_CUBIC);
                trainingImages.Add(TrainedFace);
                labels.Add(SchemeInfo.data.Email);

                //Write the number of triained faces in a file text for further load
                File.WriteAllText(BaseDirectory + "/TrainedLabels.txt", trainingImages.ToArray().Length.ToString() + "%");

                for (var i = 1; i < trainingImages.ToArray().Length + 1; i++)
                {
                    trainingImages.ToArray()[i - 1].Save(TrainedFacesDirectory + "/face" + i + ".bmp");
                    File.AppendAllText(BaseDirectory + "/TrainedLabels.txt", labels.ToArray()[i - 1] + "%");
                }

                SchemeInfo.data.Successful = true;
                OnComplete();
            }
            catch
            {
                FeedbackLabel.ForeColor = Color.DarkRed;
                FeedbackLabel.Text = string.Format("Could not capture face. Trials: {0}", ++errorCount);
                SchemeInfo.data.Successful = false;
            }
        }



        private void FrameGrabber (object sender, EventArgs e)
        {
            ContTrain++;

            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(320, 240, INTER.CV_INTER_CUBIC);

            gray = grabber.QueryGrayFrame().Resize(320, 240, INTER.CV_INTER_CUBIC);

            //Face Detector
            var facesDetected = gray.DetectHaarCascade(
                face,
                1.2,
                10,
                HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                new Size(20, 20));


            if (SchemeInfo.scheme == AuthenticationScheme)
            {
                foreach (var f in facesDetected[0])
                {
                    t = t + 1;
                    result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);

                    //draw the face detected in the 0th (gray) channel with blue color
                    currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);

                    var arrTrainingImages = trainingImages.ToArray();
                    if (arrTrainingImages.Length != 0)
                    {
                        //TermCriteria for face recognition with numbers of trained images like maxIteration
                        var termCrit = new MCvTermCriteria(ContTrain, 0.001);

                        //Eigen face recognizer
#pragma warning disable CS0436 // Type conflicts with imported type
                        var recognizer = new EigenObjectRecognizer(
                           trainingImages.ToArray(),
                           labels.ToArray(),
                           3000,
                           ref termCrit);
#pragma warning restore CS0436 // Type conflicts with imported type

                        name = recognizer.Recognize(result);
                        LastNRecognizedLabels.Add(name);
                    }

                    if (LastNRecognizedLabels.IsFull)
                    {
                        OnComplete();
                        Dispose(true);
                        Close();
                    }
                }
            }

            imageBoxFrameGrabber.Image = currentFrame;
        }
    }
}
