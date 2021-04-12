using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRFractal
{
    class MandelFractal
    {
        public static int JuliaBigFloat(BigDecimal x, BigDecimal y, BigDecimal xadd, BigDecimal yadd, int max_iter)
        {
            int remain_iter = max_iter;
            BigDecimal xx = x * x;
            BigDecimal yy = y * y;
            BigDecimal xy = x * y;
            BigDecimal betrag_2 = xx + yy;

            while (betrag_2 <= 4 && remain_iter > 0)
            {
                remain_iter = remain_iter - 1;
                x = xx - yy + xadd;
                y = xy + xy + yadd;
                x=x.Truncate(25);
                y=y.Truncate(25);
                xx = x * x;
                yy = y * y;
                xy = x * y;
                betrag_2 = xx + yy;
            }

            return max_iter - remain_iter;
        }
        public static int Julia(double x, double y, double xadd, double yadd, int max_iter)
        {
            int remain_iter = max_iter;
            double xx = x * x;
            double yy = y * y;
            double xy = x * y;
            double betrag_2 = xx + yy;

            while (betrag_2 <= 4 && remain_iter > 0)
            {
                remain_iter = remain_iter - 1;
                x = xx - yy + xadd;
                y = xy + xy + yadd;
                xx = x * x;
                yy = y * y;
                xy = x * y;
                betrag_2 = xx + yy;
            }

            return max_iter - remain_iter;
        }

        public static DepthMap GetDepthMapByCenter(double im, double re, double size, int pixelWidth = 1000, int pixelHeigth = 1000)
        {
            return GetDepthMap(im - size, im + size, re - size, re + size, pixelWidth, pixelHeigth);
        }

        //public static LowBitMap GetLowBitmap(double im_min=-2, double im_max=2, double re_min=-2, double re_max=2, int pixelWidth =1000, int pixelHeigth=1000 )
        //{
        //    DepthMap depthMap = GetDepthMap(im_min, im_max, re_min, re_max, pixelWidth, pixelHeigth);
        //    return depthMap.GetLowBitMap();
        //}

        public static DepthMap GetDepthMap(double im_min, double im_max, double re_min, double re_max, int pixelWidth, int pixelHeigth)
        {
            DepthMap depthMap = new DepthMap(pixelWidth, pixelHeigth);

            int xpixels = pixelWidth;
            int ypixels = pixelHeigth;

            depthMap.GetPixelsCords().AsParallel().
                ForAll((t) =>
                {
                    var c_im = im_min + (im_max - im_min) * t.y / ypixels;
                    var c_re = re_min + (re_max - re_min) * t.x / xpixels;

                    depthMap[t.x, t.y] = Julia(c_re, c_im, c_re, c_im, 1000);
                    //depthMap[t.x, t.y] = Julia(c_re, c_im, -0.5, 0.5, 1000);
                });
            return depthMap;
        }
    }
}
