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
                x = x.Truncate(25);
                y = y.Truncate(25);
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
    }
}
