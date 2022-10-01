using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Columns
{
    public class SpecialText
    {
        public readonly int X;
        public readonly int Y;
        public readonly Point Offset;
        public readonly string Text;

        public SpecialText(int x, int y, Point offset, string text)
        {
            X = x;
            Y = y;
            Offset = offset;
            Text = text;
        }
    }
}
