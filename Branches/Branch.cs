using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Samples.Kinect.BranchingShadow
{
    class Branch
    {
        public Polyline branch;
        public Point seedPoint;
        public int posOrNeg;
        public float seedPointZagFactor;
        public int seedPointi;

        internal Branch(Brush brush, double thickness, Point seedPoint, int seedPointi, float seedPointZagFactor, int posOrNeg)
        {
            this.branch = new Polyline();
            branch.Stroke = brush;
            branch.StrokeThickness = thickness;
            this.seedPoint = seedPoint;
            this.posOrNeg = posOrNeg;
            this.seedPointZagFactor = seedPointZagFactor;
            this.seedPointi = seedPointi;
        }

    }
}
