using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectBingMap.Controller
{
    public class AverageFilter
    {
        private double[] buffer;
        private int filtNumber;
        private int countIndex;

        public double[] TheBuffer
        {
            get
            {
                return buffer;
            }
            set
            {
                if (value.Length != filtNumber)
                {
                    throw new Exception("The length of the value should be the same with the length of the buffer");
                }
                else
                {
                    buffer = value;
                }
            }
        }

        public int FiltNumber
        {
            get
            {
                return filtNumber;
            }
        }

        public AverageFilter(int theBufferNumber)
        {
            this.filtNumber = theBufferNumber;
            this.buffer = new double[theBufferNumber];
            this.countIndex = 0;

            for (int i = 0; i < theBufferNumber; i++)
            {
                buffer[i] = new double();
            }
        }

        public double FilterInStep(double value)
        {
            this.buffer[countIndex] = value;
            double sum = 0;

            for (int i = 0; i < this.FiltNumber; i++)
            {
                sum += buffer[i];
            }
            sum /= this.filtNumber;

            this.countIndex++;

            if (this.countIndex == this.filtNumber)
            {
                countIndex = 0;
            }

            return sum;
        }

        public double FilterAllInBuffer()
        {
            double sum = 0;

            for (int i = 0; i < this.FiltNumber; i++)
            {
                sum += buffer[i];
            }
            sum /= this.filtNumber;

            return sum;
        }
    }
}
