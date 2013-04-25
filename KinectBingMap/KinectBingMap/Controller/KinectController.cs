using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System.Windows.Input;
using Microsoft.Kinect.Toolkit.Interaction;
using KinectBingMap.ViewModels;

namespace KinectBingMap.Controller
{
    public class KinectController : ViewModelBase
    {

        /// <summary>
        /// Component that manages finding a Kinect sensor
        /// </summary>
        private readonly KinectSensorChooser _sensorChooser = new KinectSensorChooser();
        public KinectSensorChooser SensorChooser
        {
            get { return _sensorChooser; }            
        }


        /// <summary>
        /// Active Kinect senor properties
        /// </summary>
        private KinectSensor _sensor;
        public KinectSensor Sensor
        {
            get { return this._sensor; }
            set { this._sensor = value; }            
        }

        // Skeleton information        
        private Skeleton[] _frameSkeletons = null;

        // The data stream contain the interaction information.        
        private InteractionStream _interactionStream;

        // Intermediate storage for the user information received from interaction stream.
        private UserInfo[] _userInfos;

        /// <summary>
        /// Left hand joint properties
        /// </summary> 
        private Joint _leftHandJoint;
        public Joint LeftHandJoint
        {
            get { return this._leftHandJoint; }
            set 
            { 
                this._leftHandJoint = value;
                OnPropertyChanged("LeftHandJoint");
            }
        }

        /// <summary>
        /// Right hand joint
        /// </summary>
        private Joint _rightHandJoint;
        public Joint RightHandJoint
        {
            get { return this._rightHandJoint; }
            set
            {
                this._rightHandJoint = value;
                OnPropertyChanged("RightHandJoint");
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

        private InteractionHandEventType _leftHandInteractionType;
        public InteractionHandEventType LeftHandInteractionType
        {
            get { return this._leftHandInteractionType; }
            set
            {
                this._leftHandInteractionType = value;
                OnPropertyChanged("LeftHandInteractionType");
            }
        }

        private InteractionHandEventType _rightHandInteractionType;
        public InteractionHandEventType RightHandInteractionType
        {
            get { return this._rightHandInteractionType; }
            set
            {
                this._rightHandInteractionType = value;
                OnPropertyChanged("RightHandInteractionType");
            }
        }

        #region "Constructor using singlton"
        /// <summary>
        /// public Constructor using singleton pattern to avoid multiple kienct controller instance created
        /// </summary>
        public static KinectController _instance;
        public static KinectController Instance
        {
            get
            {
                if (_instance == null)
                { _instance = new KinectController(); }
                return _instance;
            }
        }
        public KinectController()
        {
            // initalize the sensor            
            this.SensorChooser.KinectChanged += SensorChooserOnKinectChanged;                        

            // start the sensor            
            this.SensorChooser.Start();
           
            // turn on the skeleton stream to receive skeleton frames            
            this.Sensor = SensorChooser.Kinect;
            if (Sensor != null)
            {               
                //this.Sensor.SkeletonStream.Enable();

                // add an event handler to be called whenever there is new color frame data
                Sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                //// initial the interation stream
                InteractionAdapter interactionAdapter = new InteractionAdapter();
                this._interactionStream = new InteractionStream(this.Sensor, interactionAdapter);
                this._interactionStream.InteractionFrameReady += new EventHandler<InteractionFrameReadyEventArgs>(HandsInteractionFrameReady);
                this._userInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];

                //// initialize the depth stream
                this.Sensor.DepthStream.Enable();
                this.Sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(SensorSkeletonDepthFrameReady);
                
                this._frameSkeletons = new Skeleton[this.Sensor.SkeletonStream.FrameSkeletonArrayLength];

            }
            
        }        
        #endregion

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    frame.CopySkeletonDataTo(this._frameSkeletons);
                    Skeleton skeleton = GetPrimarySkeleton(this._frameSkeletons);
                    if (skeleton != null)
                    {                        
                        // use for scratch
                        var accelerometerReading = this.Sensor.AccelerometerGetCurrentReading();

                        // Hand data to Interaction framework to be processed.
                        this._interactionStream.ProcessSkeleton(this._frameSkeletons, accelerometerReading, frame.Timestamp);

                        // get left Hand and right hand joint
                        this.LeftHandJoint = skeleton.Joints[JointType.HandLeft];
                        this.RightHandJoint = skeleton.Joints[JointType.HandRight];                        
                    }
                }                
            }
        }

        private void HandsInteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            using (InteractionFrame interactionFrame = e.OpenInteractionFrame())
            {
                if (interactionFrame != null)
                {
                    interactionFrame.CopyInteractionDataTo(this._userInfos);
                }
                else
                {
                    return;
                }
            }

            foreach (UserInfo userInfo in this._userInfos)
            {
                foreach (InteractionHandPointer interactionHandPointer in userInfo.HandPointers)
                {
                    // track if the left hand or right hand is griped
                    if (interactionHandPointer.HandType == InteractionHandType.Left)
                    {
                        switch (interactionHandPointer.HandEventType)
                        { 
                            case InteractionHandEventType.Grip:
                                this.LeftHandInteractionType = interactionHandPointer.HandEventType;
                                this.IsLeftHandGrip = true;
                                break;
                            case InteractionHandEventType.GripRelease:
                                this.LeftHandInteractionType = interactionHandPointer.HandEventType;
                                this.IsLeftHandGrip = false;
                                break;                            
                        }
                        
                    }
                    else if (interactionHandPointer.HandType == InteractionHandType.Right)
                    {
                        switch (interactionHandPointer.HandEventType)
                        {
                            case InteractionHandEventType.Grip:
                                this.RightHandInteractionType = interactionHandPointer.HandEventType;
                                this.IsRightHandGrip = true;
                                break;
                            case InteractionHandEventType.GripRelease:
                                this.RightHandInteractionType = interactionHandPointer.HandEventType;
                                this.IsRightHandGrip = false;
                                break;
                        }
                    }
                }
            }

        }
        
        private void SensorSkeletonDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            // Even though we un-register all our event handlers when the sensor
            // changes, there may still be an event for the old sensor in the queue
            // due to the way the KinectSensor delivers events.  So check again here.
            if (this.Sensor != sender)
            {
                return;
            }

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (null != depthFrame)
                {
                    try
                    {
                        // Hand data to Interaction framework to be processed.
                        this._interactionStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
                    }
                    catch (InvalidOperationException)
                    {
                        // DepthFrame functions may throw when the sensor gets
                        // into a bad state.  Ignore the frame in that case.
                    }
                }
            }
        }

        /// <summary>
        /// get the skeleton closest to the Kinect sensor
        /// </summary>
        /// <param name="skeletons">all avaliable skeletons</param>
        /// <returns></returns>
        private  Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            Skeleton skeleton = null;
            if (skeletons != null)
            {
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            if (skeleton.Position.Z > skeletons[i].Position.Z)
                            {
                                skeleton = skeletons[i];
                            }
                        }
                    }
                }
            }
            return skeleton;
        }

        /// <summary>
        /// Called when the KinectSensorChooser gets a new sensor
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="args">event arguments</param>
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            bool error = false;

            //MessageBox.Show(args.NewSensor == null ? "No Kinect" : args.NewSensor.Status.ToString());

            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                    error = true;
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                        error = true;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                    error = true;
                }
            }

            if (!error)
            {
                //kinectRegion.KinectSensor = args.NewSensor;       
                this.Sensor = args.NewSensor;
            }

        }

        #region "InteractionAdapter helper class"
        class InteractionAdapter : IInteractionClient
        {
            public InteractionInfo GetInteractionInfoAtLocation(int skeletonTrackingId, InteractionHandType handType, double x, double y)
            {
                var interactionInfo = new InteractionInfo
                {
                    IsPressTarget = false,
                    IsGripTarget = false,
                };

                return interactionInfo;
            }
        }
        #endregion


    }
}
