using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataCapturingClient
{
    /// <summary>
    /// Interaction logic for PictureCaptureView.xaml
    /// </summary>
    public partial class PictureCaptureView : Window, INotifyPropertyChanged
    {
       
        #region Public properties

        public ObservableCollection<FilterInfo> VideoDevices { get; set; }
        MovePoint FirstPoint;
        Bitmap last_bitmap;
        public FilterInfo CurrentDevice
        {
            get { return _currentDevice; }
            set { _currentDevice = value; this.OnPropertyChanged("CurrentDevice"); }
        }
        private FilterInfo _currentDevice;

        public bool Original
        {
            get { return _original; }
            set { _original = value; this.OnPropertyChanged("Original"); }
        }
        private bool _original;

        public bool Grayscaled
        {
            get { return _grayscale; }
            set { _grayscale = value; this.OnPropertyChanged("Grayscaled"); }
        }
        private bool _grayscale;

        public bool Thresholded
        {
            get { return _thresholded; }
            set { _thresholded = value; this.OnPropertyChanged("Thresholded"); }
        }
        private bool _thresholded;


        public int Threshold
        {
            get { return _threshold; }
            set { _threshold = value; this.OnPropertyChanged("Threshold"); }
        }
        private int _threshold;

        #endregion


        #region Private fields

        private IVideoSource _videoSource;

        #endregion
        private Profile profilepage;
        public PictureCaptureView(Profile profilepage)
        {
            InitializeComponent();
            this.DataContext = this;
            GetVideoDevices();
            Threshold = 127;
            Original = true;
            this.Closing += PictureCaptureView_Closing;
            this.profilepage = profilepage;
        }

        private void PictureCaptureView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopCamera();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartCamera();
            btnStart.Visibility= Visibility.Hidden ;
        }

        private void video_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bi;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    if (Grayscaled)
                    {
                        using (var grayscaledBitmap = Grayscale.CommonAlgorithms.BT709.Apply(bitmap))
                        {
                            bi = grayscaledBitmap.ToBitmapImage();
                        }
                    }
                    else if (Thresholded)
                    {
                        using (var grayscaledBitmap = Grayscale.CommonAlgorithms.BT709.Apply(bitmap))
                        using (var thresholdedBitmap = new Threshold(Threshold).Apply(grayscaledBitmap))
                        {
                            bi = thresholdedBitmap.ToBitmapImage();
                        }
                    }
                    else // original
                    {
                        bi = bitmap.ToBitmapImage();
                    }
                    last_bitmap = bitmap;
                }
                bi.Freeze(); // avoid cross thread operations and prevents leaks
                Dispatcher.BeginInvoke(new ThreadStart(delegate { videoPlayer.Source = bi;  }));
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCamera();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
           // StopCamera();

            // create filter
           double x=  Canvas.GetLeft(RectViewPoint);
            double y = Canvas.GetTop(RectViewPoint);
            double x1 = Canvas.GetLeft(videoPlayer);
            double y1 = Canvas.GetTop(videoPlayer);
            Crop filter = new Crop(new System.Drawing.Rectangle((int)x, (int)y, (int)RectViewPoint.Width,(int)RectViewPoint.Height));
            // apply the filter
            BitmapImage bi = (BitmapImage)videoPlayer.Source;
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(bi.StreamSource);
            Bitmap newImage = filter.Apply( new Bitmap(bitmap));
            if (imgcapture1.Source == null)
            {
                imgcapture1.Source = newImage.ToBitmapImage();
            }
            else if (imgcapture2.Source == null)
            {
                imgcapture2.Source = newImage.ToBitmapImage();
                btnCapture.IsEnabled = false;
            }
            else
            {

            }
        }

        private void GetVideoDevices()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            foreach (FilterInfo filterInfo in new FilterInfoCollection(FilterCategory.VideoInputDevice))
            {
                VideoDevices.Add(filterInfo);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
            else
            {
                MessageBox.Show("No video sources found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartCamera()
        {
            if (CurrentDevice != null)
            {
                _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();

            }
        }

        private void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= new NewFrameEventHandler(video_NewFrame);
            }
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

        private void videoPlayer_MouseMove(object sender, MouseEventArgs e)
        {
           
        }
        private void videoPlayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
          //  FirstPoint = new MovePoint();

          //  FirstPoint.X = e.X;

          //  FirstPoint.Y = e.Y;
        }
        private struct MovePoint

        {

            public double X;

            public double Y;

        }

        private void RectViewPoint_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                int initialX = 0, initialY = 0;//e.GetPosition(videoPlayer).X;
               // RectViewPoint.
               //RectViewPoint.TranslatePoint()
              // RectViewPoint.Margin= new Thickness( RectViewPoint.Margin.Left - 5,RectViewPoint.Margin.Top,RectViewPoint.Margin.Right,RectViewPoint.Margin.Bottom);
                // for example. 
               // System.Windows.Shapes.Rectangle      
                                               //  System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
               
                                                  //  RectViewPoint.X = (e.GetPosition.i.X - FirstPoint.X) + initialX;
                                                // RectViewPoint.Y = (e.Y - FirstPoint.Y) + initialY;
                                                // videoPlayer.Invalidate();
            }
        }

        private void canvasArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void canvasArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Canvas.SetLeft(RectViewPoint, e.GetPosition(canvasArea).X);
                Canvas.SetTop(RectViewPoint, e.GetPosition(canvasArea).Y);
            }
        }

        private void canvasArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
           // FirstPoint = new MovePoint();

           // FirstPoint.X = e.GetPosition(canvasArea).X;

           // FirstPoint.Y = e.GetPosition(canvasArea).Y;
        }

        private void btnkeepimg1_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bimg = (BitmapImage)imgcapture1.Source;
            profilepage.imgboxCapture.Source = bimg;
            this.Hide();
        }

        private void btnkeepimg2_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bimg = (BitmapImage)imgcapture2.Source;
            profilepage.imgboxCapture.Source = bimg;
            this.Close();
        }
    }

    static class BitmapHelpers
    {
        public static BitmapImage ToBitmapImage(this System.Drawing.Bitmap bitmap)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }
    }
}
