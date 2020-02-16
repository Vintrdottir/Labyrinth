using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Labirynt
{
    class MyWorm
    {
        public WormPart Head { get; set; }
        public List<WormPart> Parts { get; private set; }

        public MyWorm()
        {
            Head = new WormPart(4, 10);
            Head.Ell.Width = Head.Ell.Height = 10;
            Head.Ell.Fill = System.Windows.Media.Brushes.DimGray;
            Parts = new List<WormPart>();
            Parts.Add(new WormPart(3, 10));
            Parts.Add(new WormPart(2, 10));
            Parts.Add(new WormPart(1, 10));
        }


        public void RedrawWorm()
        {
            Grid.SetColumn(Head.Ell, Head.X);
            Grid.SetRow(Head.Ell, Head.Y);
            foreach (WormPart wormPart in Parts)
            {
                Grid.SetColumn(wormPart.Ell, wormPart.X);
                Grid.SetRow(wormPart.Ell, wormPart.Y);
            }
        }
    }
}
