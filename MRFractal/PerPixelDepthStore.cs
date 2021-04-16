using System;
using System.Collections.Generic;
using System.Text;

namespace MRFractal
{
    class PerPixelDepthStore
    {
        public int pixelWidth;
        public int pixelHeigth;


        public int[,] PixelDepthMap;
        public bool[,] isRealData;
        public int[,] RealDataDistance;

        public int this[int x, int y]
        {
            get
            {
                return PixelDepthMap[x, y];
            }
        }


        public PerPixelDepthStore(int width, int height)
        {
            pixelWidth = width;
            pixelHeigth = height;

            PixelDepthMap = NewPixelDepthMap();
            isRealData = NewIsRealData();
            RealDataDistance = NewRealDataDistance();

        }

        private int[,] NewRealDataDistance()
        {
            var newDAta= new int[pixelWidth, pixelHeigth];
            for(int x=0;x<pixelWidth;x++)
            {
                for (int y = 0; y < pixelHeigth; y++)
                {
                    newDAta[x, y] = int.MaxValue;
                }
            }
            return newDAta;
        }

        public bool[,] NewIsRealData()
        {
            return new bool[pixelWidth, pixelHeigth];
        }

        public int[,] NewPixelDepthMap()
        {
            return new int[pixelWidth, pixelHeigth];
        }



        public void NewDepthData(int x, int y, int depth, bool BigPlot = false)
        {
            if(isRealData[x,y]==false)
            {
                isRealData[x, y] = true;
                PixelDepthMap[x, y] = depth;
                RealDataDistance[x, y] = 0;
            }
            else
            {
                PixelDepthMap[x, y]= (PixelDepthMap[x, y]+depth)/2;
            }

            if (BigPlot)
            {
                for (int dx = -20; dx <= 20; dx++)
                {
                    for (int dy = -20; dy <= 20; dy++)
                    {
                        var new_x = x + dx;
                        var new_y = y + dy;
                        if (new_x >= 0 && new_x < pixelWidth && new_y >= 0 && new_y < pixelHeigth)
                        {

                            if (isRealData[new_x, new_y] == false && RealDataDistance[new_x, new_y] > Math.Abs(dx) + Math.Abs(dy))
                            {
                                PixelDepthMap[new_x, new_y] = depth;
                                RealDataDistance[new_x, new_y] = Math.Abs(dx) + Math.Abs(dy);
                            }
                                
                        }
                    }
                }
            }
        }

        public void Translate(int translateX, int translateY)
        {
            var OldData = PixelDepthMap;
            var OldIsRealData = isRealData;
            var oldRealDistanceData = RealDataDistance;


            this.PixelDepthMap = NewPixelDepthMap();
            this.isRealData = NewIsRealData();
            this.RealDataDistance = NewRealDataDistance();

            for (int x = 0; x < pixelWidth; x++) //TODO Speed up by calculating boarders, not stupid testing if inside
            {
                for (int y = 0; y < pixelHeigth; y++)
                {
                    int sourceX = x + translateX;
                    int sourceY = y + translateY;
                    if (sourceX >= 0 && sourceX < pixelWidth && sourceY >= 0 && sourceY < pixelHeigth)
                    {
                        NewDepthData(x, y, OldData[sourceX, sourceY]);
                        isRealData[x, y] = OldIsRealData[sourceX, sourceY];
                        RealDataDistance[x, y] = oldRealDistanceData[sourceX, sourceY];
                    }

                }
            }
        }

        public void Zoom(double factor)
        {

            var OldData = PixelDepthMap;
            var OldIsRealData = isRealData;
            var oldRealDistanceData = RealDataDistance;

            this.PixelDepthMap = NewPixelDepthMap();
            this.isRealData = NewIsRealData();
            this.RealDataDistance = NewRealDataDistance();

            for (double x = 0; x < pixelWidth; x++)
            {
                for (double y = 0; y < pixelHeigth; y++)
                {
                    double translateX = x - (pixelWidth / 2);
                    double translateY = y - (pixelHeigth / 2);

                    int copyx = (int)(x - (translateX / factor));
                    int copyy = (int)(y - (translateY / factor));
                    if (copyx >= 0 && copyx < pixelWidth && copyy >= 0 && copyy < pixelHeigth)
                    {
                        NewDepthData((int)x, (int)y, OldData[copyx, copyy]);
                        isRealData[(int)x, (int)y] = OldIsRealData[copyx, copyy];
                        RealDataDistance[(int)x,(int) y] = oldRealDistanceData[copyx, copyy];
                    }

                }
            }

            //reseting, becaouse copied data is probably bad TODO calculate good RealDataDistance from old one
            this.isRealData = NewIsRealData();
            this.RealDataDistance = NewRealDataDistance();
        }

        internal void ResetIsRealData()
        {
            this.isRealData = NewIsRealData();
        }
    }
}
