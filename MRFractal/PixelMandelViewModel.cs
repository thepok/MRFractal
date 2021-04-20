using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MRFractal
{
    public class PixelMandelViewModel
    {
        public BigDecimal im_lefttop = -2;
        public BigDecimal re_lefttop = -2;

        public BigDecimal im_rightbottom = 2;
        public BigDecimal re_rightbottom = 2;

        public int  pixelWidth = 400;
        public int pixelHeigth = 400;



        ColorMapping colors = new ColorMapping();


        public PerPixelColorStore PerPixelColorStore;

        public PerPixelDepthStore PerPixelDepthStore;


        bool directMode = true;

        public bool DirectMode
        {
            get
            {
                return directMode;
            }
            set
            {
                directMode = value;
            }
        }
        public bool NativDoubleMode { get; set; } = true;

        public int MaxIteration { get; set; } = 100000;

        public PixelMandelViewModel(int width=400, int heigth=400)
        {
            pixelWidth = width;
            pixelHeigth = heigth;
            ResetStores();
            //DepthData = new SimpleDepthCache();

        }

        public void ResetStores()
        {
            ResetPerPixelDepthMap();
            PerPixelColorStore = new PerPixelColorStore(pixelWidth, pixelHeigth);
        }


        Cords Center
        {
            get
            {
                return new Cords()
                {
                    imaginar = im_lefttop + Size.imaginar / 2,
                    real = re_lefttop + Size.real / 2
                };
            }
        }

        Cords Size
        {
            get
            {
                return new Cords()
                {
                    imaginar = (im_rightbottom - im_lefttop),
                    real = (re_rightbottom - re_lefttop)
                };
            }
        }

        Cords PixelSize
        {
            get
            {
                return new Cords()
                {
                    imaginar = Size.imaginar / pixelHeigth,
                    real = Size.real / pixelWidth
                };
            }
        }



        public void NewCenterByPixelPos(Point point)
        {
            {
                var x = point.X;
                var y = point.Y;

                var re_size = (re_rightbottom - re_lefttop)/2;
                var im_size = (im_rightbottom - im_lefttop)/2;


                var re_newCenter = XPixelToReal((int)point.X);
                var im_newCenter = YPixelToIm((int)point.Y);

                re_lefttop = re_newCenter - re_size;
                im_lefttop = im_newCenter - im_size;

                re_rightbottom = re_newCenter + re_size;
                im_rightbottom = im_newCenter + im_size;
            }

            ResetStores();

            return;

            int translateX = (int)point.X - (pixelWidth / 2);
            int translateY = (int)point.Y - (pixelHeigth / 2);


            PerPixelDepthStore.Translate(translateX, translateY);

            
        }

        public void Zoom(double factor)
        {
            var center = Center;
            var size = Size;
            var newSize = new Cords() { imaginar = Size.imaginar / factor, real = Size.real / factor };

            this.re_rightbottom = center.real + (newSize.real / 2);
            this.im_rightbottom = center.imaginar + (newSize.imaginar / 2);

            this.re_lefttop = center.real - (newSize.real / 2);
            this.im_lefttop = center.imaginar - (newSize.imaginar / 2);


            PerPixelDepthStore.Zoom(factor);
        }


        public void ResetPerPixelDepthMap()
        {
            this.PerPixelDepthStore = new PerPixelDepthStore(pixelWidth, pixelHeigth);
        }


        public void UpdateColorPixelBitMap()
        {
            for (int x = 0; x < pixelWidth; x++)
            {
                for (int y = 0; y < pixelHeigth; y++)
                {
                    
                    int depth = this.PerPixelDepthStore[x, y];
                    PerPixelColorStore.SetPixel(x, y, colors.GetRed(depth), colors.GetGreen(depth), colors.GetBlue(depth));
                }
            }
        }

        public void UpdatePixelDepthMap()
        {
            try
            {
                var rnd = new Random();
                for (int i = 0; i < 100; i++)
                {
                    var x = rnd.Next(pixelWidth);
                    var y = rnd.Next(pixelHeigth);
                    //if (this.PerPixelDepthStore.isRealData[x, y] == false)
                    {
                        var xx = XPixelToReal(x) + (rnd.NextDouble()-0.5)*PixelSize.real;
                        var yy = YPixelToIm(y) + (rnd.NextDouble() - 0.5) * PixelSize.imaginar;
                        //int depth = MandelFractal.Julia(xx, yy, xx, yy, 1000);
                        int depth = NativDoubleMode ? MandelFractal.Julia((double)xx, (double)yy, (double)xx, (double)yy, MaxIteration) : MandelFractal.JuliaBigFloat(xx, yy, xx, yy, MaxIteration);
                        this.PerPixelDepthStore.NewDepthData(x, y, depth, true);

                    }
                }
            }
            catch { };
        }

        public void NewColorMapping()
        {
            this.colors = new ColorMapping(ColorMode.Random);
            UpdateColorPixelBitMap();
        }

        public void NewMaxIteraition(int newMax)
        {
            this.MaxIteration = newMax;
            this.PerPixelDepthStore.Reset();
        }

        public BigDecimal XPixelToReal(BigDecimal x) => re_lefttop + x * PixelSize.real;
        public BigDecimal YPixelToIm(BigDecimal y) => im_lefttop + y * PixelSize.imaginar;

        public void ResetPos()
        {
            this.im_lefttop = -2;
            this.re_lefttop = -2;
            this.re_rightbottom = 2;
            this.im_rightbottom = 2;
            ResetPerPixelDepthMap();
        }


    }
}
