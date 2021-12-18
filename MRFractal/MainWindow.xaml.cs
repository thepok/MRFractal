using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MRFractal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public PixelMandelViewModel model;
        public MainWindow()
        {


            var window = this;
            //window.WindowStyle = WindowStyle.None;
            //window.ResizeMode = ResizeMode.NoResize;
            
            //window.Left = 0;
            //window.Top = 0;
            
            //window.Width = SystemParameters.VirtualScreenWidth/2;
            //window.Height = SystemParameters.VirtualScreenHeight/2;
            //window.Topmost = true;

            BigDecimal.AlwaysTruncate = true;
            BigDecimal.Precision = 10;



            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {


            model = new PixelMandelViewModel(800, 600);
            this.DataContext = model;
            //{
            //    for (int i = 0; i < 1; i++)
            //    {
            //        var Updater = new System.Threading.Thread(() =>
            //        {
            //            while (true)
            //            {

            //            }
            //        });
            //        Updater.Start();
            //    }
            //}

            {
                for (int i = 0; i < 8; i++)
                {
                    var Updater = new System.Threading.Thread(() =>
                    {
                        while (alive)
                        {
                            if (model.DirectMode == false)
                            {
                               // model.UpdateDepthMap(100);
                            }
                            else
                            {
                                model.UpdatePixelDepthMap();
                            }
                            Console.WriteLine("Calcs done");
                        }
                    });
                    Updater.Start();
                }
            }

            //for (int i = 0; i < 6; i++)
            {
                var Refresher = new System.Threading.Thread(() =>
                {
                    while (alive)
                    {
                        try
                        {
                            model.UpdateColorPixelBitMap();
                        }
                        catch { };
                        //model.NewColorMapping();
                        try
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                MainImage.Source = model.PerPixelColorStore.GetBitMapSource();
                                Console.WriteLine("New Pic Calculated");
                            }
                            );
                        }
                        catch
                        {
                            return;
                        }
                        System.Threading.Thread.Sleep(200);
                    }
                });
                Refresher.Start();
            }

            //MainImage.Source = MandelFractal.GetDepthMapByCenter(model.im_center, model.re_center, model.size).GetLowBitMap().GetBitMapSource();
            //model.UpdateDepthMap();
            //model.UpdateLowBitMap();
            //var lowbitmap = model.GetLowBitMap();
            //lowbitmap.SetPixel(10, 10, 255, 255, 255);
            //var bitmap = lowbitmap.GetBitMapSource();
            //MainImage.Source = bitmap;
        }


        private async void  MainImage_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var PosClick = e.GetPosition((IInputElement)sender);
            PosClick.X /= ZoomSlider.Value;
            PosClick.Y /= ZoomSlider.Value;
            model.NewCenterByPixelPos(PosClick);

            //Console.WriteLine($"{model.size} {model.re_center} {model.im_center}");
            //MainImage.Source = dm.GetLowBitMap().GetBitMapSource();
            //Task.Run(() => MainImage.Source = model.GetLowBitMap().GetBitMapSource());
            //MainImage.Source = model.GetLowBitMap().GetBitMapSource();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {

            if(e.Key==Key.C)
            {
                model.NewColorMapping();
                //MainImage.Source = model.GetLowBitMap().GetBitMapSource();
            }

            if(e.Key==Key.R)
            {
                model.ResetPos();
                model.ResetPerPixelDepthMap();
            }

            if(e.Key==Key.M)
            {
                model.ResetPerPixelDepthMap();
            }

            if(e.Key==Key.N)
            {
                model.NativDoubleMode = !model.NativDoubleMode;
            }

            if (e.Key == Key.D)
            {
                model.DirectMode = !model.DirectMode;
            }

            if (e.Key == Key.O)
            {
                model.Zoom(0.5);
            }

            if (e.Key == Key.Add || e.Key==Key.OemPlus)
            {
                model.NewMaxIteraition((int)(model.MaxIteration * 1.1) + 1);
                MaxIteration.Text = model.MaxIteration.ToString();
            }

        }

        bool alive = true;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            alive = false;
        }

        private void MainImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //this.model.pixelHeigth = (int)e.NewSize.Height;
            //this.model.pixelWidth = (int)e.NewSize.Width;
            //this.model.ResetStores();
            
            return;
        }



        #region MOUSE

        private Point PointAdjustedByZoom(Point ZoomedPoint)
        {
            return new Point(ZoomedPoint.X / ZoomSlider.Value, ZoomedPoint.Y / ZoomSlider.Value);
        }

        System.Windows.Point MouseLeftDownPos;
        bool LeftMouseButtonDown = false;

        private async void MainImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            MouseLeftDownPos = e.GetPosition((IInputElement)sender);
            LeftMouseButtonDown = true;
        }

        private void MainImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LeftMouseButtonDown = false;
            SelectionRectangle.Visibility = Visibility.Collapsed;
            var PosClickUp = e.GetPosition((IInputElement)MainImage);

            if (PosClickUp == MouseLeftDownPos)
            {
                model.NewCenterByPixelPos(PointAdjustedByZoom(PosClickUp));
                //model.im_size /= 2;
                //model.re_size /= 2;
                model.Zoom(2);
            }
            else
            {
                var MouseLeftDownAdjustedByZoom = PointAdjustedByZoom(MouseLeftDownPos);
                var MouseLeftUpAdjustedByZoom = PointAdjustedByZoom(PosClickUp);

                var newLeftTop = new BigComplex() {real= model.XPixelToReal((int)MouseLeftDownAdjustedByZoom.X), imaginar = model.YPixelToIm((int)MouseLeftDownAdjustedByZoom.Y) };
                var newRightBottom= new BigComplex() { real = model.XPixelToReal((int)MouseLeftUpAdjustedByZoom.X), imaginar = model.YPixelToIm((int)MouseLeftUpAdjustedByZoom.Y) };
                model.RightBottom = newRightBottom;
                

                model.LeftTop = newLeftTop;
                

                model.ResetStores();
            }

            //Console.WriteLine($"{model.size} {model.re_center} {model.im_center}");
            //MainImage.Source = dm.GetLowBitMap().GetBitMapSource();
            //model.UpdateDepthMap();
            //MainImage.Source = model.GetLowBitMap().GetBitMapSource();
            //MainImage.Source = model.GetLowBitMap().GetBitMapSource();
        }

        private void MainImage_MouseMove(object sender, MouseEventArgs e)
        {
            var PosClick = e.GetPosition((IInputElement)MainImage);
            var PosClickAdjustedByZoom = PointAdjustedByZoom(PosClick);

            try
            {
                Cords.Text = $"Re:{model.XPixelToReal((int)PosClickAdjustedByZoom.X)} Im:{model.YPixelToIm((int)PosClickAdjustedByZoom.Y)} Iterations:{model.PerPixelDepthStore[(int)PosClickAdjustedByZoom.X, (int)PosClickAdjustedByZoom.Y]}";
            }
            catch { }
            try
            {
                if (LeftMouseButtonDown)
                {
                    SelectionRectangle.Visibility = Visibility.Visible;
                    SelectionRectangle.Margin = new Thickness(MouseLeftDownPos.X, MouseLeftDownPos.Y, 0, 0);

                    

                    SelectionRectangle.Width = PosClick.X - MouseLeftDownPos.X;
                    SelectionRectangle.Height = PosClick.Y - MouseLeftDownPos.Y;
                }
            }
            catch { }
        }

        #endregion

        #region Menu

        private void ResolutionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int width = int.Parse(((string)((MenuItem)sender).Tag).Split(" ")[0]);
            int height = int.Parse(((string)((MenuItem)sender).Tag).Split(" ")[1]);
            
            MainImage.Width = width/ZoomSlider.Value;
            MainImage.Height = height / ZoomSlider.Value;

            this.model.pixelHeigth = height;
            this.model.pixelWidth = width;
            this.model.ResetStores();
        }

        private void FitWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainImage.Width = 1;
            MainImage.Height = 1;
            this.model.pixelHeigth = (int)MainImage.Height;
            this.model.pixelWidth = (int)MainImage.Width;

            MainImage.Width = FracScollVIew.ActualWidth;
            MainImage.Height = FracScollVIew.ActualHeight;
            this.model.pixelHeigth = (int)MainImage.Height;
            this.model.pixelWidth = (int)MainImage.Width;

            //FracScollVIew.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //FracScollVIew.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            this.model.ResetStores();
            return;
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            var pic = this.model.PerPixelColorStore.GetBitMapSource();

            //tried Jpg...its worse than png
            if (((string)((MenuItem)sender).Tag) == "PNG")
            {
                using (var fileStream = new FileStream("test.png", FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(pic));
                    encoder.Save(fileStream);
                }
            }
        }

        #endregion

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MainImage != null)
            {
                MainImage.Width = e.NewValue * this.model.pixelWidth;
                MainImage.Height = e.NewValue* this.model.pixelHeigth;
            }
        }
    }
}
