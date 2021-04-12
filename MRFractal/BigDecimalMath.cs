using KdTree.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRFractal
{
    class BigDecimalMath : TypeMath<BigDecimal>
    {
        public override BigDecimal MinValue => new BigDecimal(10000,-1000000000);

        public override BigDecimal MaxValue => new BigDecimal(10000, 1000000000);

        public override BigDecimal Zero => 0;

        public override BigDecimal NegativeInfinity => new BigDecimal(10000, -1000000000);

        public override BigDecimal PositiveInfinity => new BigDecimal(10000, 1000000000);

        public override BigDecimal Add(BigDecimal a, BigDecimal b)
        {
            return a+b;
        }

        public override bool AreEqual(BigDecimal a, BigDecimal b)
        {
            return a==b;
        }

        public override int Compare(BigDecimal a, BigDecimal b)
        {
            return a.CompareTo(b);
        }

        public override BigDecimal DistanceSquaredBetweenPoints(BigDecimal[] a, BigDecimal[] b)
        {
            BigDecimal Sum = 0;
            for(int i=0; i< a.Length;i++)
            {
                var diff = a[i] - b[i];
                Sum += (diff * diff);
            }
            return Sum;
        }

        public override BigDecimal Multiply(BigDecimal a, BigDecimal b)
        {
            return a*b;
        }

        public override BigDecimal Subtract(BigDecimal a, BigDecimal b)
        {
            return a-b;
        }
    }
}
