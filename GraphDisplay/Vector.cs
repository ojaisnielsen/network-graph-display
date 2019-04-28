using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphDisplay
{
    public class Vector
    {
        public double x;
        public double y;

        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(Vector.Dot(this, this));
            }
        }

        public Vector GetNormalized()
        {
            Vector normalized = new Vector(this.x, this.y);
            double l = normalized.Length;
            if (l > 0)
            {
                normalized = (1 / l) * normalized;
            }
            else
            {
                Random random = new Random();

                normalized.x = 1;
                normalized.y = 0;               
            }
            return normalized;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.x + b.x, a.y + b.y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.x - b.x, a.y - b.y);
        }

        public static Vector operator *(double a, Vector b)
        {
            return new Vector(a * b.x, a * b.y);
        }

        public static double Dot(Vector a, Vector b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static double Det(Vector a, Vector b)
        {
            return a.x * b.y - a.y * b.x;
        }
    }
}
