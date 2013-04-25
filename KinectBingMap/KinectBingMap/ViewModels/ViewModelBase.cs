using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;

namespace KinectBingMap.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class
        /// </summary>
        protected ViewModelBase()
        {
        }

        /// Event that is signaled when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Signals the PropertyChanged event with the given property name
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            //VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        /// <summary>
        /// Debug only method that verifies that a property exists on this view model.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        //[Conditional("DEBUG")]
        //[DebuggerStepThrough]
        //private void VerifyPropertyName(string propertyName)
        //{
        //    if (TypeDescriptor.GetProperties(this)[propertyName] == null)
        //    {
        //        throw new ArgumentException("Invalid property name: " + propertyName);
        //    }
        //}


    }
}
