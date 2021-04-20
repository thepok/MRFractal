using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MRFractal
{
    public struct BigComplex
    {
        public BigDecimal real;
        public BigDecimal imaginar;
        public static BigComplex operator +(BigComplex a, BigComplex b) => new BigComplex() { imaginar = a.imaginar + b.imaginar, real =a.real + b.real };
        public static BigComplex operator -(BigComplex a, BigComplex b) => new BigComplex() { imaginar = a.imaginar - b.imaginar, real = a.real - b.real };

        public static BigComplex operator /(BigComplex a, double b) => new BigComplex() { imaginar = a.imaginar / b, real = a.real / b };
        
    }
}
