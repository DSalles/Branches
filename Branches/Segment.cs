using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace swell //Microsoft.Samples.Kinect.BranchingShadow
{

    class Segment : Shape
    {
        public Pen pen;
        Line line;
        internal Segment()
        { }
        internal Segment(Pen pen, float curve, float length)
        {
            this.curve = curve;
            this.length = length;
            this.pen = pen;
            line.Stroke = pen.Brush;
            line.StrokeThickness = pen.Thickness;
            line.PointToScreen(new Point(0,0));           
        }

        internal virtual void Draw(DrawingContext drawingContext, Point point0, float LimitDrawSegments)
        {
            // drawingContext.DrawLine(pen, point0, new Point(point0.X + length, point0.Y));
            TransformGroup tgroup = new TransformGroup();
            tgroup.Children.Add(new RotateTransform(curve));
            tgroup.Children.Add(new TranslateTransform(point0.X, point0.Y));
            this.line.RenderTransform = tgroup;
        }

        public float curve;
        public float length;
    }

    class BranchSegment : Segment
    {
        public Pen pen;
            internal BranchSegment()
        { }
        internal BranchSegment(Pen pen, float curve, float length)
        {           
            this.curve = curve; 
            this.length = length;   
            this.pen = pen;
            this.Stroke = pen.Brush;
            this.StrokeThickness = pen.Thickness;
        }

        internal override void Draw(DrawingContext drawingContext, Point point0, float LimitDrawSegments) 
        {
            drawingContext.DrawLine(pen, point0, new Point(point0.X + length, point0.Y));                
        }
        public float curve;
        public float length;
        }

    class Seed : Segment
    {

      public  Branch branch;
      internal Seed(Pen pen, float curve, float length, int numBranchSegments, int color, int curveSpread)
      {
          List<Brush> brushes = BranchManager.brushes;
          // get the remainder of color int over crushes count to get the current color
          int colorIndex = color % brushes.Count;
          // add to the color int for the next branch
          color++;
          this.curve = curve;
          this.length = length;
          this.pen = pen;
          this.Stroke = pen.Brush;
          this.StrokeThickness = pen.Thickness;
          int segmentLength = (int)(length * 1.5);
          branch =  new Branch(numBranchSegments, (int)(numBranchSegments * .16), brushes , new Pen(brushes[colorIndex], this.pen.Thickness), (int)(curveSpread * .8), length, color, curve);
        }

      internal override void Draw(DrawingContext drawingContext, Point point0, float limitDrawSegments)
      {
          BranchManager.GetDefaultManager().DrawSubBranch(drawingContext, point0, limitDrawSegments, branch);
          
       //   drawingContext.DrawLine(pen, point0, new Point(point0.X + length, point0.Y));
      }

    }
}
