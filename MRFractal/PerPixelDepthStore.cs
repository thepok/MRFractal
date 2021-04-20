using System;
using System.Collections.Generic;
using System.Text;

namespace MRFractal
{

    public struct PerPixelDepthStoreElement
    {
        public int SampleCount;
        public int SampleDepthSum;
        public bool IsRealData;
        public int RealDataDistance;
        public int Depth;

    }
    public class PerPixelDepthStore
    {
        public int pixelWidth;
        public int pixelHeigth;

        private PerPixelDepthStoreElement[,] Data;

        public int this[int x, int y]
        {
            get
            {
                return Data[x,y].Depth;
            }
        }


        public PerPixelDepthStore(int width, int height)
        {
            pixelWidth = width;
            pixelHeigth = height;

            Reset();
        }


        public void NewDepthData(int x, int y, int depth, bool BigPlot = false)
        {
            if(Data[x,y].IsRealData==false)
            {
                Data[x, y].IsRealData = true;
                Data[x, y].SampleCount = 1;
                Data[x, y].SampleDepthSum = depth;
                Data[x, y].RealDataDistance = 0;
                Data[x, y].Depth = depth;
            }
            else
            {
                Data[x, y].SampleCount += 1;
                Data[x, y].SampleDepthSum += depth;
                Data[x, y].Depth= Data[x,y].SampleDepthSum / Data[x, y].SampleCount;
                
            }

            depth = Data[x, y].Depth;

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

                            if (Data[new_x, new_y].IsRealData == false && Data[new_x, new_y].RealDataDistance > Math.Abs(dx) + Math.Abs(dy))
                            {
                                Data[new_x, new_y].Depth = depth;
                                Data[new_x, new_y].RealDataDistance = Math.Abs(dx) + Math.Abs(dy);
                            }
                                
                        }
                    }
                }
            }
        }

        public void Translate(int translateX, int translateY)
        {
            //var OldData = PixelDepthMap;
            //var OldIsRealData = isRealData;
            //var oldRealDistanceData = RealDataDistance;


            //this.PixelDepthMap = NewPixelDepthMap();
            //this.isRealData = NewIsRealData();
            //this.RealDataDistance = NewRealDataDistance();

            //for (int x = 0; x < pixelWidth; x++) //TODO Speed up by calculating boarders, not stupid testing if inside
            //{
            //    for (int y = 0; y < pixelHeigth; y++)
            //    {
            //        int sourceX = x + translateX;
            //        int sourceY = y + translateY;
            //        if (sourceX >= 0 && sourceX < pixelWidth && sourceY >= 0 && sourceY < pixelHeigth)
            //        {
            //            NewDepthData(x, y, OldData[sourceX, sourceY]);
            //            isRealData[x, y] = OldIsRealData[sourceX, sourceY];
            //            RealDataDistance[x, y] = oldRealDistanceData[sourceX, sourceY];
            //        }

            //    }
            //}
        }

        public void Zoom(double factor)
        {

            //var OldData = PixelDepthMap;
            //var OldIsRealData = isRealData;
            //var oldRealDistanceData = RealDataDistance;

            //this.PixelDepthMap = NewPixelDepthMap();
            //this.isRealData = NewIsRealData();
            //this.RealDataDistance = NewRealDataDistance();

            //for (double x = 0; x < pixelWidth; x++)
            //{
            //    for (double y = 0; y < pixelHeigth; y++)
            //    {
            //        double translateX = x - (pixelWidth / 2);
            //        double translateY = y - (pixelHeigth / 2);

            //        int copyx = (int)(x - (translateX / factor));
            //        int copyy = (int)(y - (translateY / factor));
            //        if (copyx >= 0 && copyx < pixelWidth && copyy >= 0 && copyy < pixelHeigth)
            //        {
            //            NewDepthData((int)x, (int)y, OldData[copyx, copyy]);
            //            isRealData[(int)x, (int)y] = OldIsRealData[copyx, copyy];
            //            RealDataDistance[(int)x,(int) y] = oldRealDistanceData[copyx, copyy];
            //        }

            //    }
            //}

            ////reseting, becaouse copied data is probably bad TODO calculate good RealDataDistance from old one
            //this.isRealData = NewIsRealData();
            //this.RealDataDistance = NewRealDataDistance();
        }

        internal void Reset()
        {
            Data = new PerPixelDepthStoreElement[pixelWidth, pixelHeigth];
            for(int x=0;x<pixelWidth;x++)
            {
                for (int y = 0; y < pixelHeigth; y++)
                {
                    Data[x, y].RealDataDistance = int.MaxValue;
                }
            }
        }
    }
}
