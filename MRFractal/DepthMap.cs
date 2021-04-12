using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MRFractal
{
    class DepthMap
    {
        int[,] DepthData;
        public int min = int.MaxValue;
        public int max = int.MinValue;

        int width;
        int height;
        public DepthMap(int width, int height)
        {
            this.width = width;
            this.height = height;

            DepthData = new int[width, height];
        }


        public IEnumerable<(int x, int y)> GetPixelsCords()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    yield return (x, y);
                }
            }
        }

        public PerPixelColorStore GetLowBitMap()
        {
            var bitmap = new PerPixelColorStore(width, height);
            
            ColorMapping colors = new ColorMapping();
            
            foreach(var(x,y) in bitmap.GetPixelsCords())
            {
                int range = max - min;
                int depth = DepthData[x, y];
                bitmap.SetPixel(x, y, colors.GetRed(depth), colors.GetGreen(depth), colors.GetBlue(depth));

                //bitmap.SetPixel(x, y,
                //    (byte)((DepthData[x, y] / ((double)max)) * 255), 0, 0);

                //bitmap.SetPixel(x,y,
                //    (byte)(((DepthData[x,y]-min)/((double)max-min))*255),0,0);

                //bitmap.SetPixel(x,y,
                //    (byte)(DepthData[x,y]%255),0,0);
            }
            return bitmap;
        }

        public int this[int x, int y]
        {
            get
            {
                return DepthData[x, y];
            }
            set
            {
                if(value>max)
                {
                    max = value;
                }
                if(value<min)
                {
                    min = value;
                }
                DepthData[x, y] = value;
            }

        }
    }
}
