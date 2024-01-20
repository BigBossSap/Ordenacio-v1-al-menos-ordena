using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ordenacio.Model
{
    public class PositionedRectangle
    {
        public Rectangle Rectangle { get; set; }

        public int Number { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Brush Fill { get; set; }
        public bool IsElipse { get; set; }
        public UIElement RectangleElement { get; set; }
        public int ZIndex { get; set; }
    }
}