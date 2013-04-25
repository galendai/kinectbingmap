using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KinectBingMap.Models;
using KinectBingMap.Controller;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Interaction;
using Microsoft.Samples.Kinect.WpfViewers;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Design;

namespace KinectBingMap.ViewModels
{
    public class HomeScreenViewModel : ViewModelBase
    {                

        private KinectController _kc; // kinect controller
        private KinectHandUtilities _ku; // kinect utilities

        private double _distanceTwoHand; // track the distance between two hands           

        #region "Left Hand Cursor Properties"
        /// <summary>
        /// Left Hand Cursor Property
        /// </summary>
        private Hand _leftHand = new Hand();
        public Hand LeftHand
        {
            get { return this._leftHand; }
            set 
            {
                this._leftHand = value;
                OnPropertyChanged("LeftHand");
            }
        }

        /// <summary>
        /// Left Hand Cursor X Positions
        /// </summary>        
        public double LeftHandX
        {
            get
            { return LeftHand.PosLeft; }
            set
            {
                this.LeftHand.PosLeft = value;
                OnPropertyChanged("LeftHandX");
            }
        }

        /// <summary>
        /// Left Hand Cursor Y Positions
        /// </summary>        
        public double LeftHandY
        {
            get
            { return this.LeftHand.PosTop; }
            set
            {
                this.LeftHand.PosTop = value;
                OnPropertyChanged("LeftHandY");
            }
        }

        private InteractionHandEventType _leftHandInteraction;
        public InteractionHandEventType LeftHandInteraction
        {
            get { return this._leftHandInteraction; }
            set
            {
                this._leftHandInteraction = value;
                OnPropertyChanged("LeftHandInteraction");
            }
        }

        private bool _isLeftHandGrip;
        public bool IsLeftHandGrip
        {
            get { return this._isLeftHandGrip; }
            set
            {
                this._isLeftHandGrip = value;
                OnPropertyChanged("IsLeftHandGrip");
            }
        }

        private InteractionHandEventType _rightHandInteraction;
        public InteractionHandEventType RightHandInteraction
        {
            get { return this._rightHandInteraction; }
            set
            {
                this._rightHandInteraction = value;
                OnPropertyChanged("RightHandInteraction");
            }
        }
        private bool _isRightHandGrip;
        public bool IsRightHandGrip
        {
            get { return this._isRightHandGrip; }
            set
            {
                this._isRightHandGrip = value;
                OnPropertyChanged("IsRightHandGrip");
            }
        }

        #endregion

        #region "Right Hand Cursor Properties"
        /// <summary>
        /// Right Hand Cursor Property
        /// </summary>        
        private Hand _rightHand = new Hand();
        public Hand RightHand
        {
            get { return this._rightHand; }
            set
            {
                this._rightHand = value;
                OnPropertyChanged("RightHand");
            }
        }

        /// <summary>
        /// Left Hand Cursor X Positions
        /// </summary>        
        public double RightHandX
        {
            get
            { return this.RightHand.PosLeft; }
            set
            {
                this.RightHand.PosLeft = value;
                OnPropertyChanged("RightHandX");
            }
        }

        /// <summary>
        /// Left Hand Cursor Y Positions
        /// </summary>        
        public double RightHandY
        {
            get
            { return this.RightHand.PosTop; }
            set
            {
                this.RightHand.PosTop = value;
                OnPropertyChanged("RightHandY");
            }
        }
        #endregion

        #region "Kinect Sensor / SensorChooser properties"
        /// <summary>
        /// Kinect sensor chooser properties
        /// </summary>
        private KinectSensorChooser _sensorChooser;
        public KinectSensorChooser SensorChooser
        {
            get { return _sensorChooser; }
            set 
            { 
                this._sensorChooser = value;
                OnPropertyChanged("SensorChooser");
            }
        }

        /// <summary>
        /// Kinect Sensor properties
        /// </summary>
        private KinectSensor _sensor;
        public KinectSensor Sensor
        {
            get { return this._sensor; }
            set
            {
                this._sensor = value;
                OnPropertyChanged("Sensor");
            }
        }

        private KinectSensorManager _sensorManager;
        public KinectSensorManager SensorManager
        {
            get { return this._sensorManager; }
            set
            {
                this._sensorManager = value;
                OnPropertyChanged("SensorManager");
            }
        }
        #endregion

        #region "Map Component Properties"
        
        /// <summary>
        /// Componenet for Bing Map Control
        /// </summary>
        private BingMap _bingMap;
        public BingMap BingMap
        {
            get { return this._bingMap; }
            set
            {
                this._bingMap = value;
                OnPropertyChanged("BingMap");
            }
        }

        private Location _mapCenterPoint;
        public Location MapCenterPoint
        {
            get { return _mapCenterPoint; }
            set
            {
                _mapCenterPoint = value;
                OnPropertyChanged("MapCenterPoint");
            }
        }

        /// <summary>
        /// Properties for Map Zoom Level
        /// </summary>
        private double _mapZoomLevel;
        public double MapZoomLevel
        {
            get { return this._mapZoomLevel; }
            set
            {
                _mapZoomLevel = value;
                OnPropertyChanged("MapZoomLevel");
            }
        }

        #endregion

        #region "Test message properties"
        private string _testMessageLeftHand;
        public string TestMessageLeftHand
        {
            get { return this._testMessageLeftHand; }
            set 
            {
                this._testMessageLeftHand = value;
                OnPropertyChanged("TestMessageLeftHand");
            }
        }

        private string _testMessageRightHand;
        public string TestMessageRightHand
        {
            get { return this._testMessageRightHand; }
            set
            {
                this._testMessageRightHand = value;
                OnPropertyChanged("TestMessageRightHand");
            }
        }
        #endregion

        /// <summary>
        /// Constructor to initiliaze the components
        /// </summary>
        public HomeScreenViewModel()            
        { 
            // initialize kinect controller and hand manager utility         
            _kc = KinectController.Instance;
            _ku = KinectHandUtilities.Instance;
            _bingMap = new BingMap();
            
            // register property change event handler
            _kc.PropertyChanged += _kc_PropertyChanged;

            // initizlie the sensors
            SensorChooser = _kc.SensorChooser;
            Sensor = _kc.Sensor;
            this.SensorManager = new KinectSensorManager();            
            SensorManager.KinectSensor = Sensor;     
            
            // set the default center location to San Francisco
            BingMap.CenterLocation = new Location(37.806029, -122.407007);
            MapCenterPoint = BingMap.CenterLocation;
            
            // set the default zoom level
            this.MapZoomLevel = 16;

        }

        /// <summary>
        /// Update the hand status changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _kc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Update the hand joint and position
            LeftHand.HandJoint = _kc.LeftHandJoint;
            RightHand.HandJoint = _kc.RightHandJoint;
            Point[] points = _ku.GetHandsPostion(LeftHand.HandJoint, RightHand.HandJoint);
            LeftHandX = points[0].X;
            LeftHandY = points[0].Y;
            RightHandX = points[1].X;
            RightHandY = points[1].Y;            

            // update the hand interaction type
            this.LeftHandInteraction = _kc.LeftHandInteractionType;
            this.RightHandInteraction = _kc.RightHandInteractionType;
            this.IsLeftHandGrip = _kc.IsLeftHandGrip;
            this.IsRightHandGrip = _kc.IsRightHandGrip;

            double test = 0;
            // detect the hand gestures
            if (LeftHand.HandJoint.TrackingState == JointTrackingState.Tracked || RightHand.HandJoint.TrackingState == JointTrackingState.Tracked)
            {
                if (IsLeftHandGrip && IsRightHandGrip)
                {
                    // when both hand is gripped, update the zoom level otherwise only update the distance of between the two hand as reference
                    double zoomDistance = (Int32)_ku.GetTwoHandDistance(LeftHandX, LeftHandY, RightHandX, RightHandY);
                    if (zoomDistance > this._distanceTwoHand)
                    {
                        MapZoomLevel += 0.01;
                    }
                    else if (zoomDistance <= this._distanceTwoHand)
                    {
                        MapZoomLevel -= 0.01;
                    }
                }
                else if (IsRightHandGrip && !IsLeftHandGrip)
                {
                    // grab to drag the map with right hand
                    test = (double)RightHand.HandJoint.Position.Y / (Math.Pow(MapZoomLevel, 2));
                    BingMap.CenterLocation.Latitude -= (double)RightHand.HandJoint.Position.Y / (Math.Pow(MapZoomLevel, 3));
                    BingMap.CenterLocation.Longitude += (double)RightHand.HandJoint.Position.X / (Math.Pow(MapZoomLevel, 3));
                    MapCenterPoint = BingMap.CenterLocation;
                }
                else if (IsLeftHandGrip && !IsRightHandGrip)
                {
                    // grab to drag the map with left hand
                    BingMap.CenterLocation.Latitude -= (double)LeftHand.HandJoint.Position.Y / (Math.Pow(MapZoomLevel, 3));
                    BingMap.CenterLocation.Longitude += (double)LeftHand.HandJoint.Position.X / (Math.Pow(MapZoomLevel, 3));
                    MapCenterPoint = BingMap.CenterLocation;
                }
                else
                {
                    // update the distance between two hands
                    this._distanceTwoHand = (Int32)_ku.GetTwoHandDistance(LeftHandX, LeftHandY, RightHandX, RightHandY);
                }
            }
            // test message
            TestMessageLeftHand =
                "test = " + test * 1000
                + "\nArea Width = " + _ku.DisplayAreaWidth + "; Area Height = " + _ku.DisplayAreaHeight
                + "\nLeft Hand: Tracking State: "
                + LeftHand.HandJoint.TrackingState.ToString()
                + "; X = " + (Int32)LeftHandX
                + " ; Y = " + (Int32)LeftHandY
                + "; Interaction Type = " + this.LeftHandInteraction
                + "; Grip = " + this.IsLeftHandGrip;

            TestMessageRightHand =
                "Two hand distance = " + this._distanceTwoHand + "; ZoomLevel = " + MapZoomLevel
                + "; \nRight Hand: Tracking State: "
                + RightHand.HandJoint.TrackingState.ToString()
                + "; X = " + RightHand.HandJoint.Position.X
                + " ;Y = " + RightHand.HandJoint.Position.Y
                + "; Interaction Type = " + this.RightHandInteraction
                + "; Grip = " + this.IsRightHandGrip;

        }


    }
}
