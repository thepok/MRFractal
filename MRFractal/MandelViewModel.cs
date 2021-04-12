using MRFractal.DepthCaches;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MRFractal
{
    public class MandelViewModel
    {
        public BigDecimal im_center = 0;
        public BigDecimal re_center= -0;
        public BigDecimal size =1;

        int  pixelWidth = 400;
        int pixelHeigth = 400;



        ColorMapping colors = new ColorMapping();

        IContiniusAproximator<int, BigDecimal> DepthData;

        public LowBitMap bitmap;

        PixelDepthData PerPixelDepth;


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

        public int MaxIteration { get; set; } = 1000;

        public MandelViewModel(int width=400, int heigth=400)
        {
            pixelWidth = width;
            pixelHeigth = heigth;

            PerPixelDepth = new PixelDepthData(pixelWidth, pixelHeigth);

            bitmap = new LowBitMap(pixelWidth, pixelHeigth);
            //DepthData = new SimpleDepthCache();
            ResetDepthCache();
        }

        public void ResetDepthCache()
        {
            DepthData = new KDCache();
            DepthData[5, 5] = 0;
        }


        public void NewCenterByPixelPos(Point point)
        {
            {
                var x = point.X;
                var y = point.Y;

                re_center = re_center + ((2 * size) / pixelWidth) * (x - (pixelWidth / 2));
                im_center = im_center + ((2 * size) / pixelHeigth) * (y - (pixelHeigth / 2));
            }

            int translateX = (int)point.X - (pixelWidth / 2);
            int translateY = (int)point.Y - (pixelHeigth / 2);


            PerPixelDepth.Translate(translateX, translateY);

            
        }

        public void Zoom(double factor)
        {
            this.size /= factor;

            PerPixelDepth.Zoom(factor);
        }

        public void UpdateDepthMap(int count=100)
        {
            BigDecimal im_min = im_center - size;
            BigDecimal im_max = im_center + size;
            BigDecimal re_min = re_center - size;
            BigDecimal re_max = re_center + size;


            int xpixels = pixelWidth;
            int ypixels = pixelHeigth;
            var rnd = new Random();

            for (int i = 0; i < count; i++)
            {
                var c_im = im_min + (im_max - im_min) * rnd.NextDouble();
                var c_re = re_min + (re_max - re_min) * rnd.NextDouble();

                int depth= NativDoubleMode?MandelFractal.Julia((double)c_re, (double)c_im, (double)c_re, (double)c_im, MaxIteration) :  MandelFractal.JuliaBigFloat(c_re, c_im, c_re, c_im, MaxIteration);
                DepthData[c_re, c_im] = depth;
                //Console.WriteLine($"PointDone {depth}");
            }
        }

        public void ResetPerPixelDepthMap()
        {
            this.PerPixelDepth = new PixelDepthData(pixelWidth, pixelHeigth);
        }

        public void ResetRealData()
        {
            this.PerPixelDepth.ResetIsRealData();
        }


        public void UpdateBitMap()
        {
            for (int x = 0; x < pixelWidth; x++)
            {
                for (int y = 0; y < pixelHeigth; y++)
                {
                    
                    int depth = this.PerPixelDepth.PixelDepthMap[x, y];
                    bitmap.SetPixel(x, y, colors.GetRed(depth), colors.GetGreen(depth), colors.GetBlue(depth));
                }
            }
        }

        public void UpdatePixelDepthMap()
        {
            var rnd = new Random();
            for(int i=0; i<100;i++)
            {
                int x = rnd.Next(pixelWidth);
                int y = rnd.Next(pixelHeigth);
                if(this.PerPixelDepth.isRealData[x,y]==false)
                {
                    if (DirectMode)
                    {
                        var xx = XPixelToReal(x);
                        var yy = YPixelToIm(y);
                        //int depth = MandelFractal.Julia(xx, yy, xx, yy, 1000);
                        int depth = NativDoubleMode? MandelFractal.Julia((double)xx, (double)yy, (double)xx, (double)yy, MaxIteration) : MandelFractal.JuliaBigFloat(xx, yy, xx, yy, MaxIteration);
                        this.PerPixelDepth.NewDepthData(x, y, depth, true);
                    }
                    else
                    {
                        
                        int depth = DepthData[XPixelToReal(x), YPixelToIm(y)];
                        this.PerPixelDepth.NewDepthData(x, y, depth, true);
                    }
                }
            }
        }

        public void NewColorMapping()
        {
            this.colors = new ColorMapping(ColorMode.Random);
            UpdateBitMap();
        }

        public void NewMaxIteraition(int newMax)
        {
            this.MaxIteration = newMax;
            this.PerPixelDepth.ResetIsRealData();
        }

        BigDecimal XPixelToReal(int x) => re_center + ((2 * size) / pixelWidth) * (x - (pixelWidth / 2));
        BigDecimal YPixelToIm(int y) => im_center + ((2 * size) / pixelHeigth) * (y - (pixelHeigth / 2));

        public void ResetPos()
        {
            this.im_center = 0;
            this.re_center = 0;
            this.size = 2;
            ResetPerPixelDepthMap();
        }


    }
}
