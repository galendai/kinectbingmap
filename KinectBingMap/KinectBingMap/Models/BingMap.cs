using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectBingMap.Models
{
    public class BingMap
    {
        private Location _centerLocation;
        public Location CenterLocation
        {
            get { return this._centerLocation; }
            set
            {
                this._centerLocation = value;
                return;
            }
        }
    }
}
