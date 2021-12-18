using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MRFractal
{
    public class PixelMandelViewModel
    {
        public BigComplex LeftTop=new BigComplex() {imaginar=-2, real=-2 };
        public BigComplex RightBottom = new BigComplex() { imaginar = 2, real = 2 };

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
            WorkProvider = GetNextWorkCenterbased();
        }


        BigComplex Center
        {
            get
            {
                return new BigComplex()
                {
                    imaginar = LeftTop.imaginar + Size.imaginar / 2,
                    real = LeftTop.real + Size.real / 2
                };
            }
        }

        BigComplex Size
        {
            get
            {
                return new BigComplex()
                {
                    imaginar = (RightBottom.imaginar - LeftTop.imaginar),
                    real = (RightBottom.real - LeftTop.real)
                };
            }
        }

        BigComplex PixelSize
        {
            get
            {
                return new BigComplex()
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


                BigComplex Size = (RightBottom - LeftTop) / 2;

                var newCenter = PixelPosToComplex((int)x, (int)y);

                LeftTop = newCenter-Size;
                RightBottom = newCenter + Size;
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
            var newSize = new BigComplex() { imaginar = Size.imaginar / factor, real = Size.real / factor };

            RightBottom = new BigComplex() { real = center.real + (newSize.real / 2), imaginar = center.imaginar + (newSize.imaginar / 2) };

            LeftTop = new BigComplex() { real = center.real - (newSize.real / 2), imaginar = center.imaginar - (newSize.imaginar / 2) };

            ResetStores();
            //PerPixelDepthStore.Zoom(factor);

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

        object WorkLock = new object();
        IEnumerator<(int x, int y)> WorkProvider;


        public IEnumerator<(int x, int y)> GetNextWorkCenterbased()
        {
            while (true)
            {
                int centerX = pixelWidth / 2;
                int centerY = pixelHeigth / 2;
                yield return new(centerX, centerY);

                for(int ring=1;ring<Math.Max(pixelWidth/2+1, pixelHeigth/2+1);ring ++)
                {
                    for(int dx=-ring; dx<=ring;dx++)
                    {
                        int nextX = centerX - dx;
                        if (nextX >= 0 && nextX < pixelWidth)
                        {
                            if(centerY - ring>=0)
                                yield return new(nextX, centerY - ring);
                            if(centerY + ring<pixelHeigth)
                                yield return new(nextX, centerY + ring);
                        }
                    }
                    for (int dy = -ring; dy <= ring; dy++)
                    {
                        int nextY = centerY - dy;
                        if (nextY >= 0 && nextY < pixelHeigth)
                        {
                            if (centerX - ring >= 0)
                                yield return new(centerX - ring, nextY);
                            if (centerX + ring < pixelWidth)
                                yield return new(centerX + ring, nextY);
                        }
                    }
                }
            }
        }

        public IEnumerator<(int x, int y)> GetNextWork()
        {
            while (true)
            {
                var rnd = new Random();
                {
                    foreach (var ele in PerPixelColorStore.GetPixelsCords())
                    {
                        yield return ele;
                    }
                }
            }
        }


        public void UpdatePixelDepthMap()
        {
            try
            {
                var rnd = new Random();
                //for (int i = 0; i < 100; i++)
                {
                    var x = rnd.Next(pixelWidth);
                    var y = rnd.Next(pixelHeigth);
                    Stack<(int x, int y)> WorkBatch = new Stack<(int x, int y)>();
                    lock (WorkLock)
                    {
                        for (int k = 0; k < 1000; k++)
                        {
                            WorkProvider.MoveNext();


                                x = WorkProvider.Current.x;
                            y = WorkProvider.Current.y;
                            WorkBatch.Push(new(x, y));
                        }
                    }
                    
                    foreach(var ele in WorkBatch)
                    {
                        //for (int i = 0; i < 30; i++) //Supersample on Spot?
                        {
                            x = ele.x;
                            y = ele.y;
                            BigDecimal xx;
                            BigDecimal yy;
                            if (PerPixelDepthStore.Data[x, y].IsRealData==false)
                            {
                                xx = XPixelToReal(x);
                                yy = YPixelToIm(y);
                            }
                            else//supersample
                            {
                                xx = XPixelToReal(x) + ((rnd.NextDouble() - 0.5) / 1) * PixelSize.real;
                                yy = YPixelToIm(y) + ((rnd.NextDouble() - 0.5) / 1) * PixelSize.imaginar;
                            }
                            //int depth = MandelFractal.Julia(xx, yy, xx, yy, 1000);
                            int depth = NativDoubleMode ? MandelFractal.Julia((double)xx, (double)yy, (double)xx, (double)yy, MaxIteration) : MandelFractal.JuliaBigFloat(xx, yy, xx, yy, MaxIteration);
                            this.PerPixelDepthStore.NewDepthData(x, y, depth, false);
                        }
                    }
                }
            }
            catch (Exception e ){
                Console.WriteLine(e.Message);
            };
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

        public BigDecimal XPixelToReal(BigDecimal x) => LeftTop.real + x * PixelSize.real;
        public BigDecimal YPixelToIm(BigDecimal y) => LeftTop.imaginar + y * PixelSize.imaginar;

        public BigComplex PixelPosToComplex(int x, int y) => new BigComplex() { real = XPixelToReal(x), imaginar = YPixelToIm(y) };


        public void ResetPos()
        {

            LeftTop = new BigComplex() { imaginar = -2, real = -2 };
            RightBottom = new BigComplex() { imaginar = 2, real = 2 };

            ResetPerPixelDepthMap();
        }


    }
}
