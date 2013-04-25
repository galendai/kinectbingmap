using KinectBingMap.Controller;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinectBingMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private KinectController _kc = KinectController.Instance;
        private KinectHandUtilities _ku = KinectHandUtilities.Instance;

        public MainWindow()
        {            
            InitializeComponent();                  
        } 

        /// <summary>
        /// Close the sensor when Window is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _kc.SensorChooser.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // set the content height and width
            _ku.DisplayAreaWidth = this.MainCanvas.ActualWidth;
            _ku.DisplayAreaHeight = this.MainCanvas.ActualHeight;
        }

    }
}
