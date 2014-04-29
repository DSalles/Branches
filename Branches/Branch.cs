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
        public float seedPointZagFactor;
        public int seedPointi;
        public double angle;

        internal Branch(Brush brush, double thickness, Point seedPoint, int seedPointi, float seedPointZagFactor, Random random, double parentAngle)
        {
            int branchPosOrNeg = 0;
            while (branchPosOrNeg == 0)
            {
                branchPosOrNeg = random.Next(3) - 1;
            }            
            this.branch = new Polyline();
            this.branch.Points.Add(seedPoint);
            branch.Stroke = brush;
            branch.StrokeThickness = thickness;
            this.seedPoint = seedPoint;
            float divisor = ((random.Next(6) + 1));
            if (parentAngle == 0)
                this.angle = Math.PI * .55 + Math.PI * divisor / 8;
            else
                this.angle = Math.PI * .8 + Math.PI * divisor / 16;
            this.seedPointZagFactor = seedPointZagFactor;
            this.seedPointi = seedPointi;
        }

    }
}
