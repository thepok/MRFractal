using System;
using System.Collections.Generic;
using System.Text;

namespace MRFractal
{
    interface IContiniusAproximator<tParam, iParam> where iParam : IComparable<iParam>
    {
        tParam this[iParam x, iParam y]
        {
            get;
            set;
        }
    }
}
