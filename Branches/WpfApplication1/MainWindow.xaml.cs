using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;


namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
      Polyline polyline1 = new Polyline();
      List<Branch> polyLineList = new List<Branch>();
      List<Branch> subPolyLineList = new List<Branch>();
      Random random = new Random();
      TransformGroup tgroup = new TransformGroup(); 
      int i;
      int seed;
      int branchSeed;
      int secondBranchSeed;
      int zagFactor = 300;
      float rotation = 0;
      RotateTransform rotateTransform = new RotateTransform();
      TranslateTransform translateTransform = new TranslateTransform();
      private int windingSpread = 3;
      private int segLength = 3;
      Point position = new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
      private int seedSpread = 150;


        public MainWindow()
        {
            InitializeComponent();
            // Create a Polyline.    
           // for ( i = growthStartPoint; i < 360; i += segLength )
           //{
           // zagFactor  += random.Next(windingSpread)-windingSpread/2;
           // polyline1.Points.Add(new Point(i, zagFactor));            
           //}

            seed = random.Next(seedSpread);
            polyline1.Stroke = Brushes.Blue;
            polyline1.StrokeThickness = 0;

            // Create a RotateTransform to rotate
            // the Polyline 45 degrees about its 
            // top-left corner.


            //polyline1.RenderTransform = rotateTransform1;
            // Create a Canvas to contain the Polyline.

            //canvas1.Width = 200;
            //canvas1.Height = 200;
            //Canvas.SetLeft(polyline1, 75);
            //Canvas.SetTop(polyline1, 50);
            //canvas1.Children.Add(polyline1);

            grid.Children.Add(polyline1);

            // Create a RotateTransform to rotate 
            // the Polyline 45 degrees about the 
            // point (25,50).


            // Create a Canvas to contain the Polyline.
            //Canvas canvas2 = new Canvas();
            //canvas2.Width = 200;
            //canvas2.Height = 200;
            //Canvas.SetLeft(polyline2, 75);
            //Canvas.SetTop(polyline2, 50);
       

            translateTransform.X = 0;
            translateTransform.Y = 0;
            tgroup.Children.Add(rotateTransform);
            tgroup.Children.Add(translateTransform);
        }


        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
             position = new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
            i += segLength;
           
             if (i < 900)
                {
                 zagFactor += random.Next(windingSpread) - windingSpread/2;
            // when i exceeds the seed value, add a polyline to the branch dictionary
                if (i >= seed && i <= seed +(segLength ))
                { 
                    
                    int posOrNeg = 0;
                    while (  posOrNeg == 0 )
                    {
                        posOrNeg = random.Next(3) - 1;
                    }
                   
                   int seedPointY = i;
                   int seedPointX = zagFactor;

                   Branch branch = new Branch(Brushes.Green, 0, new Point(zagFactor, i), i, zagFactor, posOrNeg);
                   polyLineList.Add(branch); 
                   
                   grid.Children.Add(branch.branch);
                   branch.branch.Points.Add(new Point(seedPointY, seedPointX));
                   branchSeed = random.Next(seedSpread) + i ;
               
                   seed = random.Next(seedSpread) +  i;                    
                } 
           
          
               // grow branches, spawn subBranches
                foreach ( Branch br in polyLineList)
                {

                    double seedPointX = br.seedPoint.X;
                    double seedPointY = br.seedPoint.Y;
                    // extracting a positive or negative 1 from the Y of the dictionary value because Point only holds two 
                    double posOrNeg = br.posOrNeg;
                    int seedPointi = br.seedPointi;
                    float seedPointZagFactor = br.seedPointZagFactor;
                    Point linePoint = new Point(seedPointi + zagFactor - seedPointZagFactor, seedPointZagFactor + (i - seedPointi) * posOrNeg);
                    br.branch.Points.Add(linePoint);

                    if (br.branch.StrokeThickness < 30)
                        br.branch.StrokeThickness += .5f;

                    // subBranch spawning

                    if((i  >= branchSeed && i  <= branchSeed + (segLength)) || (i  >= secondBranchSeed && i  <= secondBranchSeed + (segLength-1)))
                    {
                        int branchPosOrNeg = 0;
                        while (branchPosOrNeg == 0)
                        {
                            branchPosOrNeg = random.Next(3) - 1;
                        }

                       // double oldPosOrNeg = polyline.Points[polyline.Points.Count-1].Y/Math.Abs(polyline.Points[polyline.Points.Count-1].Y);
                        
                        Branch branch =new Branch(Brushes.Red, 0, linePoint, i, zagFactor, branchPosOrNeg);
// 
                        subPolyLineList.Add(branch);       /////new Point(polyline.Points[polyline.Points.Count - 1].X - i, polyline.Points[polyline.Points.Count - 1].Y - zagFactor));
                        grid.Children.Add(branch.branch);
                        //branch.Points.Add(polyline.Points[polyline.Points.Count-1]);
                        //branch.Points.Add(polyline.Points[polyline.Points.Count - 2]);
                        // branchSeed = random.Next(200) + 160;
                       secondBranchSeed = (int)(random.Next(seedSpread)  + i );
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
                       

                        if ( br.branch.StrokeThickness < 30)
                            br.branch.StrokeThickness += .5f;
                    }

                    // grow main brainch 
                    polyline1.Points.Add(new Point(i, zagFactor));
                    if (polyline1.StrokeThickness < 30)
                        polyline1.StrokeThickness += .5f;
                }
            //RotateTransform rotateTransform2 =
            //new RotateTransform(5);
            //rotateTransform2.CenterX = 25;
            //rotateTransform2.CenterY = 50;
       

            //polyline2.Points = polyline1.Points;
            //polyline2.Stroke = Brushes.Blue;
            //polyline2.StrokeThickness = 10;

               
         //   tgroup.Children.Add()
                //tgroup.Children.Add(new TranslateTransform(1 + pl, 1 + pl));
                //canvas1.Children[pl].RenderTransform = tgroup;

            foreach (Polyline child in grid.Children)
            child.RenderTransform = tgroup;
            rotation += .02f;
           // rotateTransform.Angle += rotation;
           position = new Point(System.Windows.Forms.Cursor.Position.X,System.Windows.Forms.Cursor.Position.Y);
           translateTransform.X = position.X-612;
           translateTransform.Y = position.Y-425;
          //  RotateTransform rotateTransform1 =
         //   new RotateTransform(rotation);
        //    polyline1.RenderTransform = rotateTransform1;
            
       //    polyline2.RenderTransform = rotateTransform1;

        }
    }
}
