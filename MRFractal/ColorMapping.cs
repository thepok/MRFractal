using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRFractal
{

    enum ColorMode
    {
        Random,
        Fluent
    }

    class ColorMapping
    {
        byte[] Reds;
        byte[] Greens;
        byte[] Blues;

        public ColorMapping(ColorMode mode=ColorMode.Fluent)
        {
            switch (mode)
            {
                case ColorMode.Fluent:
                    FluentFill();
                    break;
                case ColorMode.Random:
                    RandomFill();
                    break;
            }
        }

        private void FluentFill()
        {
            List<(byte r, byte g, byte b)> ColorMappingList = new List<(byte r, byte g, byte b)>();

            //getRed
            for(byte i=0; i<255; i+=5)
            {
                ColorMappingList.Add((i, 0, 0));
            }

            //get RED GREEN
            for (byte i = 0; i < 255; i+=5)
            {
                ColorMappingList.Add((255, i, 0));
            }
            for (byte i = 255; i > 0; i -= 5)
            {
                ColorMappingList.Add((i, 255, 0));
            }

            //get RED GREEN BLUE
            for (byte i = 0; i < 255; i+=5)
            {
                ColorMappingList.Add((0, 255, i));
            }

            Reds = ColorMappingList.Select(e => e.r).ToArray();
            Greens = ColorMappingList.Select(e => e.g).ToArray();
            Blues = ColorMappingList.Select(e => e.b).ToArray();
        }

        private void RandomFill()
        {
            int Size = 1000;
            var rnd = new MyRng(Math.Abs( (int)System.DateTime.Now.Ticks % int.MaxValue));

            Reds = new byte[Size];
            Greens = new byte[Size];
            Blues = new byte[Size];

            Reds[0] = (byte)rnd.NextInt(255);
            Greens[0] = (byte)rnd.NextInt(255);
            Blues[0] = (byte)rnd.NextInt(255);

            for (int i = 1; i < Size; i++)
            {
                Reds[i] = Normalize(Reds[i - 1] + 5 * (rnd.NextInt(11) - 5));
                Greens[i] = Normalize(Greens[i - 1] + 5 * (rnd.NextInt(11) - 5));
                Blues[i] = Normalize(Blues[i - 1] + 5 * (rnd.NextInt(11) - 5));

                //Reds[i] = (byte)rnd.Next(255);
                //Greens[i] = (byte)rnd.Next(255);
                //Blues[i] = (byte)rnd.Next(255);

            }
        }

        static byte Normalize(int value)
        {
            return (byte) Math.Max(0, Math.Min(255, value));
        }

        public byte GetRed(int depth)
        {
            return Reds[depth % Reds.Length];
        }
        public byte GetGreen(int depth)
        {
            return Greens[depth % Greens.Length];
        }
        public byte GetBlue(int depth)
        {
            return Blues[depth % Blues.Length];
        }
    }
}
