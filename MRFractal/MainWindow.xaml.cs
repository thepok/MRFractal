using System;
using System.Collections.Generic;
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


            model = new PixelMandelViewModel(100, 100);
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
                        model.UpdateColorPixelBitMap();
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

            if (e.Key == Key.Add)
            {
                model.NewMaxIteraition((int)(model.MaxIteration * 1.1) + 1);
            }

        }

        bool alive = true;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            alive = false;
        }

        private void MainImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.model = new PixelMandelViewModel((int)e.NewSize.Width, (int)e.NewSize.Height);
            this.DataContext = this.model;
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
                model.NewCenterByPixelPos(PosClickUp);
                //model.im_size /= 2;
                //model.re_size /= 2;
                model.Zoom(2);
            }
            else
            {

                var newLeftTop = new Cords() {real= model.XPixelToReal((int)MouseLeftDownPos.X), imaginar = model.YPixelToIm((int)MouseLeftDownPos.Y) };
                var newRightBottom= new Cords() { real = model.XPixelToReal((int)PosClickUp.X), imaginar = model.YPixelToIm((int)PosClickUp.Y) };
                model.re_rightbottom = newRightBottom.real;
                model.im_rightbottom = newRightBottom.imaginar;

                model.re_lefttop = newLeftTop.real;
                model.im_lefttop = newLeftTop.imaginar;

                model.ResetPerPixelDepthMap();
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
            Cords.Text = $"Re:{model.XPixelToReal((int)PosClick.X)} Im:{model.YPixelToIm((int)PosClick.Y)}";
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
    }
}
