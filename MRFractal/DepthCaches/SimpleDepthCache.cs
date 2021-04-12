using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using MoreLinq;

namespace MRFractal.DepthCaches
{
    class SimpleDepthCache : IContiniusAproximator<int, double>
    {
        TreeNode root = new TreeNode();
        public int this[double x, double y]
        {
            get
            {
                try
                {
                    return root.Get(x, y, 0);
                }
                catch
                {
                    return 0;
                }
            }
            set => root.Save(new Element(x, y, value), 0);
        }
    }

    class TreeNode
    {
        bool leaf = true;

        TreeNode HigherTree;
        TreeNode LowerTree;

        Element pivot;

        public TreeNode(Element value)
        {
            this.leaf = true;
            this.pivot = value;
        }
        public TreeNode()
        {
            this.leaf = true;
            this.pivot = null;
        }


        public int Get(double x, double y, int level)
        {
            if (leaf)
            {
                if (pivot == null)
                    return 0;
                return pivot.value;
            }

            double currentMark = level % 2 == 0 ? x : y;
            try
            {

                if (currentMark > (level % 2 == 0 ? pivot.x : pivot.y))
                    return HigherTree.Get(x, y, level + 1);
                else
                    return LowerTree.Get(x, y, level + 1);
            }
            catch(NullReferenceException e)
            {//node was in trasition from leaf to inner node....
                return pivot.value;
            }
        }

        public void Save(Element value, int level)
        {
            if (leaf) // we now need to be innerNode
            {
                if (pivot == null) //we are root
                {
                    pivot = value;
                    return;
                }
                leaf = false;
                if (level % 2 == 0) //check x
                {
                    if (value.x > pivot.x)
                    {
                        HigherTree = new TreeNode(value);
                        LowerTree = new TreeNode(pivot);
                    }
                    else
                    {
                        HigherTree = new TreeNode(pivot);
                        LowerTree = new TreeNode(value);
                        pivot = value;
                    }
                }
                else //check y
                {
                    if (value.y > pivot.y)
                    {
                        HigherTree = new TreeNode(value);
                        LowerTree = new TreeNode(pivot);
                    }
                    else
                    {
                        HigherTree = new TreeNode(pivot);
                        LowerTree = new TreeNode(value);
                        pivot = value;
                    }
                }
            }
            else
            {
                //cant save it localy
                if (level % 2 == 0) //check x
                {
                    if (value.x > pivot.x)
                    {
                        HigherTree.Save(value, level + 1);
                    }
                    else
                    {
                        LowerTree.Save(value, level + 1);
                    }

                }
                else //check y
                {
                    if (value.y > pivot.y)
                    {
                        HigherTree.Save(value, level + 1);
                    }
                    else
                    {
                        LowerTree.Save(value, level + 1);
                    }
                }
            }

        }
    }

    public class Element
    {
        public double x;
        public double y;
        public int value;

        public Element(double x, double y, int value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }

        public double diff(double x, double y) => Math.Abs(this.x - x) + Math.Abs(this.y - y);
    }
}
