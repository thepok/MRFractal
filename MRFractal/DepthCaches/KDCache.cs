using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MoreLinq;
using KdTree;
using KdTree.Math;

namespace MRFractal.DepthCaches
{
    class KDCache : IContiniusAproximator<int, BigDecimal>
    {
        KdTree<BigDecimal,int> root = new KdTree<BigDecimal, int>(2, new BigDecimalMath());
        public int this[BigDecimal x, BigDecimal y]
        {
            get
            {
                try
                {
                    return root.GetNearestNeighbours(new[] { x, y},1)[0].Value;
                }
                catch
                {
                    return 0;
                }
            }
            set => root.Add(new[] { x, y }, value);
        }
    }

}
