using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;


namespace Labirynt
{
    class WormPart
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Ellipse Ell { get; private set; }

        public WormPart(int x, int y)
        {
            X = x;
            Y = y;
            Ell = new Ellipse();
            Ell.Width = 8;
            Ell.Height = 8;
            Ell.Fill = Brushes.DarkSalmon;
        }
    }
}
