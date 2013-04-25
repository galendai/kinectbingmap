using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;

namespace KinectBingMap.Controller
{
    public class KinectHandUtilities
    {                

        public AverageFilter filterLeftHandX = new AverageFilter(10);
        public AverageFilter filterLeftHandY = new AverageFilter(10);
        public AverageFilter filterRightHandX = new AverageFilter(10);
        public AverageFilter filterRightHandY = new AverageFilter(10);

        public double DisplayAreaWidth;
        public double DisplayAreaHeight;

        private static KinectHandUtilities _instance;
        public static KinectHandUtilities Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new KinectHandUtilities();
                }
                return _instance;
            }
        }
        public KinectHandUtilities()
        { 
        
        }

        public Point[] GetHandsPostion(Joint jointLeftHand, Joint jointRighHand)
        {
            Point leftHandPos = this.SkeletonPositionOnKinectCoordinateToScreen(jointLeftHand.Position);
            Point rightHandPos = this.SkeletonPositionOnKinectCoordinateToScreen(jointRighHand.Position);
            Point[] twoHands = new Point[] { leftHandPos, rightHandPos };

            twoHands[0].X = filterLeftHandX.FilterInStep(twoHands[0].X);
            twoHands[0].Y = filterLeftHandY.FilterInStep(twoHands[0].Y);
            twoHands[1].X = filterRightHandX.FilterInStep(twoHands[1].X);
            twoHands[1].Y = filterRightHandY.FilterInStep(twoHands[1].Y);

            return twoHands;
        }

        public double GetTwoHandDistance(double x1, double y1, double x2, double y2)
        {
            double distance = 0;

            distance = Math.Sqrt(Math.Abs(Math.Pow((x2-x1),2) + Math.Abs(Math.Pow((y2-y1),2))));

            return distance;
        }

        public Point SkeletonPositionOnKinectCoordinateToScreen(SkeletonPoint skeletonPoint)
        {
            //ColorImagePoint depthImagePoint = this.kinectSensor.CoordinateMapper.MapSkeletonPointToColorPoint(skeletonPoint, ColorImageFormat.RawBayerResolution1280x960Fps12);
            //return new Point(depthImagePoint.X, depthImagePoint.Y);
            double proport = 1000;
            return new Point(this.DisplayAreaWidth / 2 + skeletonPoint.X * proport, this.DisplayAreaHeight / 2 - skeletonPoint.Y * proport);
        }
    }
}
