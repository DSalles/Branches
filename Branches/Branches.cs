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
        private int segLength = 6;
        int i;
        int seed;
        Point position = new Point(0,0);
        TranslateTransform translateTransform = new TranslateTransform();
        TransformGroup tgroup = new TransformGroup(); 
        RotateTransform rotateTransform = new RotateTransform();
        List<Branch> polyLineList = new List<Branch>();
        List<Branch> subPolyLineList = new List<Branch>();
        List<Branch> subSubPolyLineList = new List<Branch>();
        Grid layoutGrid;
        int branchSeed;
        private int seedSpread = 80;
        int secondBranchSeed;
        private int totaliLimit = 400;

        internal Branches(MainWindow window)
        {
            random = window.random;
            polyline1.Stroke = Brushes.Blue;
            polyline1.StrokeThickness = 0;
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
            if (i > totaliLimit*-1)
            {
                i -= segLength;
                zagFactor += random.Next(windingSpread) - windingSpread / 2;
                if(i > (int) (totaliLimit/2)*-1)
                {

                polyline1.Points.Add(new Point(i, zagFactor));
               
                  if (i <= seed && i >= seed -(segLength ))
                { 
                      int posOrNeg = 0;
                while (posOrNeg == 0)
                {
                    posOrNeg = random.Next(3) - 1;
                }

                int seedPointY = i;
                int seedPointX = zagFactor;

                Branch branch = new Branch(Brushes.Green, 0, new Point(zagFactor, i), i, zagFactor, posOrNeg);
                branch.branch.RenderTransform = tgroup;
                polyLineList.Add(branch);

                layoutGrid.Children.Add(branch.branch);
                branch.branch.Points.Add(new Point(seedPointY, seedPointX));
                branchSeed = random.Next(seedSpread)*-1 + i;

                seed = random.Next(seedSpread)*-1 + i;
                }
                }
                  foreach (Branch br in polyLineList)
                  {

                      double seedPointX = br.seedPoint.X;
                      double seedPointY = br.seedPoint.Y;
                     double posOrNeg = br.posOrNeg;
                      int seedPointi = br.seedPointi;
                      float seedPointZagFactor = br.seedPointZagFactor;
                      Point linePoint = new Point(seedPointi + zagFactor - seedPointZagFactor, seedPointZagFactor + (i + seedPointi) * posOrNeg);
                      br.branch.Points.Add(linePoint);
                      // grow branches
                      if (br.branch.StrokeThickness < 30)
                          br.branch.StrokeThickness += .2f;
                                // subBranch spawning

                    if((i  >= branchSeed && i  <= branchSeed + (segLength)))// || (i  >= secondBranchSeed && i  <= secondBranchSeed + (segLength-1)))
                    {
                        int branchPosOrNeg = 0;
                        while (branchPosOrNeg == 0)
                        {
                            branchPosOrNeg = random.Next(3) - 1;
                        }

                          
                        Branch branch =new Branch(Brushes.Red, 0, linePoint, i, zagFactor, branchPosOrNeg);
                        branch.branch.RenderTransform = tgroup;
                        subPolyLineList.Add(branch);     
                        layoutGrid.Children.Add(branch.branch);
                        secondBranchSeed = random.Next(seedSpread) * -1 + i;
                    }
                  }
                   /// grow subBranches

                  foreach (Branch br in subPolyLineList)
                  {
                      double seedPointX = br.seedPoint.X;
                      double seedPointY = br.seedPoint.Y;
                      int seedPointi = br.seedPointi;
                      double seedPointZag = br.seedPointZagFactor;
                      // extracting a positive or negative 1 from the Y of the dictionary value because Point only holds two 
                      double posOrNeg = br.posOrNeg;

                      Point linePoint = new Point(seedPointX + (i - seedPointi) * posOrNeg, zagFactor + seedPointY - seedPointZag);
                      br.branch.Points.Add(linePoint);


                      if (br.branch.StrokeThickness < 10)
                          br.branch.StrokeThickness += .1f;
                   /*   if ((i >= secondBranchSeed && i <= branchSeed + (segLength)) )//|| (i >= secondBranchSeed && i <= secondBranchSeed + (segLength - 1)))
                      {
                          int branchPosOrNeg = 0;
                          while (branchPosOrNeg == 0)
                          {
                              branchPosOrNeg = random.Next(3) - 1;
                          }


                          Branch branch = new Branch(Brushes.White, 0, linePoint, i, zagFactor, branchPosOrNeg);
                          branch.branch.RenderTransform = tgroup;
                          subSubPolyLineList.Add(branch);
                          layoutGrid.Children.Add(branch.branch);
                         // secondBranchSeed = random.Next(seedSpread) * -1 + i;
                      }*/
                  }

                  foreach (Branch br in subSubPolyLineList)
                  {
                      double seedPointX = br.seedPoint.X;
                      double seedPointY = br.seedPoint.Y;
                      int seedPointi = br.seedPointi;
                      double seedPointZag = br.seedPointZagFactor;
                      
                      double posOrNeg = br.posOrNeg;
                      Point linePoint = new Point(seedPointi + zagFactor - seedPointZag, seedPointZag + (i + seedPointi) * posOrNeg);
                    
                      br.branch.Points.Add(linePoint);


                      if (br.branch.StrokeThickness < 10)
                          br.branch.StrokeThickness += .1f;
                  }
            }
            if
                (polyline1.StrokeThickness < 30)
                polyline1.StrokeThickness += .5f;
        }
    }
}
