using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace Microsoft.Samples.Kinect.BranchingShadow
{
    class Branches
    {
        Polyline polyline1 = new Polyline();
        Random random;
        int zagFactor;
        private int windingSpread = 12;
        private int segLength = 12;
        int i;
        int slowDownFactor = 35;
        int frameSpeed;
        int seed = -10;
        Point position = new Point(0,0);
        TranslateTransform translateTransform = new TranslateTransform();
        TransformGroup tgroup = new TransformGroup(); 
        RotateTransform rotateTransform = new RotateTransform();
        List<Branch> polyLineList = new List<Branch>();
        List<Branch> subPolyLineList = new List<Branch>();
        List<Branch> subSubPolyLineList = new List<Branch>();
        Grid layoutGrid;
        int branchSeed=-11;
        private int seedSpread = 10;
        int secondBranchSeed=-30;
        private int totaliLimit = 50;

        internal Branches(MainWindow window)
        {
            random = window.random;
            polyline1.Stroke = Brushes.Black;
            polyline1.StrokeThickness = 20;
            this.layoutGrid = window.layoutGrid;
            this.layoutGrid.Children.Add(polyline1);
            translateTransform.X = 0;
            translateTransform.Y = 0;
            tgroup.Children.Add(rotateTransform);
            tgroup.Children.Add(translateTransform);
            polyline1.RenderTransform = tgroup;
        }

        public void Update(Point position, RotateTransform rotateTransform)
        { 
            this.position = position;
            this.rotateTransform.Angle = rotateTransform.Angle*-1;
            translateTransform.X = (position.X )*-2.25 + 1300 ;
            translateTransform.Y = position.Y*2  ;
            frameSpeed++;
            if(frameSpeed % slowDownFactor ==0)
            {           
                if (i > totaliLimit*-1)
                {               
                    i --;
                    zagFactor += random.Next(windingSpread) - windingSpread / 2;
                
                    // to limit The length of trunk branch
                    if (i > (int)(totaliLimit / 2) * -1)
                        {
                        // thicken trunk 
                        if (polyline1.StrokeThickness < 120)
                            polyline1.StrokeThickness += .5f;
                        // grow trunk 
                        polyline1.Points.Add(new Point(i*segLength, zagFactor));
                        // get ready to branch
                        if (i == seed ) //&& i >= seed -1)
                                
                            { 
                                int seedPointX = i * segLength;
                                int seedPointY = zagFactor;
                                Branch branch = new Branch(Brushes.Black, 0, new Point(seedPointX, seedPointY), i, zagFactor,random,0);
                                branch.branch.RenderTransform = tgroup;
                                polyLineList.Add(branch);
                                layoutGrid.Children.Add(branch.branch);
                                seedSpread = (int)(seedSpread * .8f);
                                seed = random.Next(seedSpread)*-1 + i;
                            }
                        }
                    if (i == branchSeed)
                    {
                        foreach (Branch br in polyLineList)
                        {
                            Branch branch = new Branch(Brushes.Black, 0, br.branch.Points.Last(), i, zagFactor, random, br.angle);
                            branch.branch.RenderTransform = tgroup;
                            subPolyLineList.Add(branch);
                            layoutGrid.Children.Add(branch.branch);
                        }
                            branchSeed = (random.Next(seedSpread) +3) * -1 + i;                        
                    }

                    // grow branches
                    foreach (Branch br in polyLineList)
                    {
                        double seedPointX = br.seedPoint.X;
                        double seedPointY = br.seedPoint.Y;
                        float zagFactorX = (random.Next(250) - 30)*.01f;
                        float zagFactorY = (random.Next(220) - 30) * .01f;
                        int seedPointi = br.seedPointi;
                        double Xfactor = Math.Cos(br.angle);
                        double Yfactor = Math.Sin(br.angle);
                        Point linePoint = new Point(br.branch.Points.Last().X + (Xfactor*segLength * zagFactorX), br.branch.Points.Last().Y - (Yfactor*segLength*zagFactorY));
                        br.branch.Points.Add(linePoint);
                        // grow branches
                        if (br.branch.StrokeThickness < 30)
                        br.branch.StrokeThickness += .2f;
                        // subBranch spawning
                        
                       
                    }
                    
                       

                    /// grow subBranches
                    foreach (Branch br in subPolyLineList)
                    {
                        double seedPointX = br.seedPoint.X;
                        double seedPointY = br.seedPoint.Y;
                        float zagFactorX = (random.Next(260) - 30) * .01f;
                        float zagFactorY = (random.Next(200) - 30) * .01f;
                        double Xfactor = Math.Cos(br.angle);
                        double Yfactor = Math.Sin(br.angle);
                        int seedPointi = br.seedPointi;
                        Point linePoint = new Point(br.branch.Points.Last().X + (Xfactor * segLength * zagFactorX), br.branch.Points.Last().Y - (Yfactor * segLength * zagFactorY));
                        br.branch.Points.Add(linePoint);
                        if (br.branch.StrokeThickness < 10)
                            br.branch.StrokeThickness += .1f;
                        // spawn sub sub branches
                        if (i == secondBranchSeed)
                        {                           
                            Branch branch = new Branch(Brushes.Black, 0, linePoint, i, zagFactor, random, br.angle);
                            branch.branch.RenderTransform = tgroup;
                            subSubPolyLineList.Add(branch);
                            layoutGrid.Children.Add(branch.branch);
                            secondBranchSeed = random.Next(seedSpread) * -1 + i;
                        }
                    }

                    // grow subSubBranches
                  foreach (Branch br in subSubPolyLineList)
                  {
                      double seedPointX = br.seedPoint.X;
                      double seedPointY = br.seedPoint.Y;
                      float zagFactorX = (random.Next(260) - 30) * .01f;
                      float zagFactorY = (random.Next(200) - 30) * .01f;
                      double Xfactor = Math.Cos(br.angle);
                      double Yfactor = Math.Sin(br.angle);
                      int seedPointi = br.seedPointi;
                      Point linePoint = new Point(br.branch.Points.Last().X + (Xfactor * segLength * zagFactorX), br.branch.Points.Last().Y - (Yfactor * segLength * zagFactorY));
                      br.branch.Points.Add(linePoint);
                      if (br.branch.StrokeThickness < 8)
                          br.branch.StrokeThickness += .1f;
                  }
             }
           }
        }
    }
}
