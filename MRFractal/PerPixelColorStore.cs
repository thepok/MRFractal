using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MRFractal
{
    public class PerPixelColorStore
    {
        public int width;
        public int height;
        public int bytesperpixel = 3;
        public int stride;

        byte[] imgdata;
        public PerPixelColorStore(int width = 800, int height = 800)
        {
            this.width = width;
            this.height = height;
            stride = width * bytesperpixel;
            imgdata = new byte[width * height * bytesperpixel];

        }

        public void SetPixel(int x, int y, byte red, byte green, byte blue)
        {
            imgdata[y * stride + x * bytesperpixel + 0] = blue;
            imgdata[y * stride + x * bytesperpixel + 1] = green;
            imgdata[y * stride + x * bytesperpixel + 2] = red;
        }

        public IEnumerable<(int x, int y)> GetPixelsCords()
        {
         for(int x=0; x< width;x++)
            {
                for (int y = 0; y < height; y++)
                {
                    yield return (x, y);
                }
            }
        }


        public BitmapSource GetBitMapSource()
        {
            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr24, null, imgdata, stride);
        }
    }
}
