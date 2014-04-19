//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BranchingShadow
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using Microsoft.Kinect;
    using System.Diagnostics;
    using System.Globalization;
   

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 12;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly System.Windows.Media.Brush centerPointBrush = System.Windows.Media.Brushes.Red;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly System.Windows.Media.Brush trackedJointBrush = System.Windows.Media.Brushes.Red;

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly System.Windows.Media.Brush inferredJointBrush = System.Windows.Media.Brushes.Red; //Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly System.Windows.Media.Pen trackedBonePen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Red, 56);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly System.Windows.Media.Pen inferredBonePen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.Red, 56);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

               /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;
        private byte[] colorPixels;

      //  BranchManager manager = BranchManager.GetDefaultManager();
        public Random random;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            random = new Random();
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                     System.Windows.Media.Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                     System.Windows.Media.Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                     System.Windows.Media.Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                     System.Windows.Media.Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)

        {
            //Debug.Assert(System.Windows.Forms.SystemInformation.MonitorCount > 1);

            //System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.AllScreens[1].WorkingArea;
            //this.Left = workingArea.Left;
            //this.Top = workingArea.Top;
            //this.Width = workingArea.Width;
            //this.Height = workingArea.Height;
            //this.WindowState = WindowState.Maximized;
            //this.WindowStyle = WindowStyle.None;
            //this.Topmost = true;
            //this.Show();
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {

                // Turn on the color stream to receive color frames
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
           

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.ColorFrameReady += this.SensorColorFrameReady;
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
        /*    using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);
                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }


            /// color mapping research to match projected image with camera image
            //for (int i = 0; i < colorPixels.Length; i += 4)
            //{
            //    if (colorPixels[i] > 0X0FA && colorPixels[i+1] < 0X0BE && colorPixels[i+2] < 0X096)
            //    {
            //        int b = colorPixels[i];
            //        int g = colorPixels[i + 1];
            //        int r = colorPixels[i + 2];

            //        int X = (i / 4) % 640;
            //        int Y = (i / 4) / 640;
            //        int jointX = 640 - (int)jointPoint.X;
            //        int jointY = (int)jointPoint.Y;
            //        if (X < 127 || X > 596 || Y < 127 || Y > 455)
            //        //  create a png bitmap encoder which knows how to save a .png file
            //        {   Console.WriteLine("X: " + X + " Y: " + Y);
            //            BitmapEncoder encoder = new PngBitmapEncoder();

            //            // create frame from the writable bitmap and add to encoder
            //            encoder.Frames.Add(BitmapFrame.Create(this.colorBitmap));

            //            string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            //            string myPhotos = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            //            string path = Path.Combine(myPhotos, "KinectSnapshot-" + time + ".png");

            //            // write the new file to disk
            //            try
            //            {
            //                using (FileStream fs = new FileStream(path, FileMode.Create))
            //                {
            //                    encoder.Save(fs);
            //                }

            //            }
            //            catch (IOException)
            //            {
            //            }
            //        }
                
            //      //Console.WriteLine("diff Light From RHand X: " + (X - jointX) +"diff light from RHand Y: " + (Y - jointY));
            //        //this.layoutGrid.Margin = new Thickness(X - (this.ellipse.Margin.Left), Y - (this.ellipse.Margin.Top), 0, 0);
            //        //this.layoutGrid.Width = 752;

            //        break;
            //    }

            //}*/

        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {            
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
              
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {                
                // Draw a transparent background to set the render size
                dc.DrawRectangle(System.Windows.Media.Brushes.DarkCyan, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            //               if(limitDrawSegments < 70)           
                        //    limitDrawSegments += .3f;
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
           ////// Render Torso
           // this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
           // this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
           // this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
           // this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
           // this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
           // this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
           // this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);
            
           //// // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

           //// // Left Leg
           // this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
           // this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
           // this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

           //////  Right Leg
           // this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
           // this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
           // this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

           //  Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
              /*  if (joint == skeleton.Joints[JointType.HandRight] || joint == skeleton.Joints[JointType.HandLeft])
                
                {
                    Brush drawBrush = null;

                    if (joint.TrackingState == JointTrackingState.Tracked)
                    {
                        drawBrush = this.trackedJointBrush;
                    }
                    else if (joint.TrackingState == JointTrackingState.Inferred)
                    {
                        drawBrush = this.inferredJointBrush;
                    }

                    if (drawBrush != null)
                    {
                        jointPoint = this.SkeletonPointToScreen(joint.Position);
                        drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                    }
                }*/
           }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
        const double Rad2Deg = 180.0 / Math.PI;
        const double Deg2Rad = Math.PI / 180.0;
  
        //private int numBranchSegments = 32;
        //private float segmentLength = 20;
        //private float segmentThickness = 20;


        /// <summary>
        /// Calculates angle in radians between two points and x-axis.
        /// </summary>
        //private double Angle(Point start, Point end)
        //{
        //    return Math.Atan2(start.Y - end.Y, end.X - start.X) *Rad2Deg;
        //}

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }
           
            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }
                   
            Point joint0Point = this.SkeletonPointToScreen(joint0.Position);
            Point joint1Point = this.SkeletonPointToScreen(joint1.Position);
            drawingContext.DrawLine(drawPen, joint0Point, this.SkeletonPointToScreen(joint1.Position));
            drawingContext.DrawLine(new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 36), this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
                        
            // create trunk branch

            Point segmentStartPoint = new Point(joint0Point.X, joint0Point.Y);  //(joint1Point.X - joint0Point.X) / 2 + joint0Point.X, (joint1Point.Y - joint0Point.Y) / 2 + joint0Point.Y);

            //for(int i = 0; i < BranchManager.jointDictionaries.Count; i++)
            //{
            //    if(!BranchManager.jointDictionaries[i].ContainsKey(jointType0))
            //        // send joint and index of dictionary
            //         manager.AddBranch(jointType0, i);
            //}

            /// draw trunk branch
            //for (int jd = 0; jd < BranchManager.jointDictionaries.Count ; jd++)
            //{
               if (!BranchManager.BranchDictionary.ContainsKey(jointType0))
               {
            //Add branch which instantiates a new branch and stores it in the branch manager
                  Branches branches = new Branches(this);
                  BranchManager.BranchDictionary.Add(jointType0, branches);
               }
            else
            {

            // draw trunkbranch which contains a number of seeds which contain branches which contain a number of seeds
                    RotateTransform rotate = new RotateTransform();
                    float jointAngle = (float)(Math.Atan2(segmentStartPoint.Y - joint1Point.Y, joint1Point.X - segmentStartPoint.X) * -180 / Math.PI);
                    rotate.Angle = jointAngle ;
                    rotate.CenterX = segmentStartPoint.X;
                    rotate.CenterY = segmentStartPoint.Y;
                BranchManager.BranchDictionary[jointType0].Update(segmentStartPoint, rotate);

                   //drawingContext.PushTransform(rotate);
                    
                   // manager.DrawBranch(drawingContext, new Point(segmentStartPoint.X, segmentStartPoint.Y), limitDrawSegments, jointType0);
                    //for (int i = 0; i < Math.Min(branch.Count, limitDrawSegments); i++)
                    //    drawingContext.Pop();
                  //  drawingContext.Pop();
            //    }
           }
       }

        /// <summary>
        /// Handles the checking or unchecking of the seated mode combo box
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void CheckBoxSeatedModeChanged(object sender, RoutedEventArgs e)
        {
            if (null != this.sensor)
            {
                if (this.checkBoxSeatedMode.IsChecked.GetValueOrDefault())
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                }
                else
                {
                    this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;   //Default;
                }
            }
        }
    }
}