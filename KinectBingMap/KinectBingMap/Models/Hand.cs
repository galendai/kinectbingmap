using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Kinect.Toolkit.Interaction;
using Microsoft.Kinect;

namespace KinectBingMap.Models
{
    public class Hand
    {

        /// <summary>
        /// Hand's X position
        /// </summary>
        private double _posTop;
        public double PosTop
        {
            get { return this._posTop; }
            set { this._posTop = value; }
        }


        /// <summary>
        /// Hand's Y position
        /// </summary>
        private double _posLeft;
        public double PosLeft
        {
            get { return this._posLeft; }
            set { this._posLeft = value; }
        }

        /// <summary>
        /// Hand's Kinect Skeleton Joint
        /// </summary>
        private Joint _handJoint;
        public Joint HandJoint
        {
            get { return this._handJoint; }
            set { this._handJoint = value; }
        }
    }
}
