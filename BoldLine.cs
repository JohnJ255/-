using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Columns
{
    public class BoldLine
    {
        public Point From;
        public Point To;
        public BoldLineType BLType;

        public BoldLine(Point from, Point to, BoldLineType blType)
        {
            From = from;
            To = to;
            BLType = blType;
        }
    }
}
