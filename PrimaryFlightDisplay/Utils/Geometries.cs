using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PrimaryFlightDisplay
{
    public static class Geometries
    {
        public static GeometryCollection Minus
        {
            get
            {
                return new GeometryCollection
                {
                    new LineGeometry(new Point(0.20, 0.75), new Point(0.30, 0.75))
                };
            }
        }
        public static GeometryCollection[] Numbers
        {
            get
            {
                return new GeometryCollection[]
                {
                    Zero,
                    One,
                    Two,
                    Three,
                    Four,
                    Five,
                    Six,
                    Seven,
                    Eight,
                    Nine
                };
            }
        }

        private static GeometryCollection GetArcGeometry(Point center, double radius, double startAngle, double endAngle, int segments, bool clockwise = true)
        {
            var geometryCollection = new GeometryCollection();

            double angleDifference = 0;
            if (clockwise)
            {
                if (endAngle <= startAngle)
                    endAngle += 360;

                angleDifference = endAngle - startAngle;
            }
            else
            {
                if (startAngle <= endAngle)
                    startAngle += 360;

                angleDifference = startAngle - endAngle;
            }

            for (int i = 0; i < segments; i++)
            {
                double segmentStartAgnle = (startAngle + i * angleDifference / segments) * Math.PI / 180.0;
                double segmentEndAngle = (startAngle + (i + 1) * angleDifference / segments) * Math.PI / 180.0;

                Point startPoint = center + new Vector(Math.Cos(segmentStartAgnle), Math.Sin(segmentStartAgnle)) * radius;
                Point endPoint = center + new Vector(Math.Cos(segmentEndAngle), Math.Sin(segmentEndAngle)) * radius;

                geometryCollection.Add(new LineGeometry(startPoint, endPoint));
            }

            return geometryCollection;
        }

        private static GeometryCollection Zero
        {
            get
            {
                return new GeometryCollection
                {
                    new LineGeometry(new Point(0.08, 0.92), new Point(0.16, 1.00)),
                    new LineGeometry(new Point(0.16, 1.00), new Point(0.34, 1.00)),
                    new LineGeometry(new Point(0.34, 1.00), new Point(0.42, 0.92)),
                    new LineGeometry(new Point(0.42, 0.92), new Point(0.50, 0.76)),
                    new LineGeometry(new Point(0.50, 0.76), new Point(0.50, 0.24)),
                    new LineGeometry(new Point(0.50, 0.24), new Point(0.42, 0.08)),
                    new LineGeometry(new Point(0.42, 0.08), new Point(0.34, 0.00)),
                    new LineGeometry(new Point(0.34, 0.00), new Point(0.16, 0.00)),
                    new LineGeometry(new Point(0.16, 0.00), new Point(0.08, 0.08)),
                    new LineGeometry(new Point(0.08, 0.08), new Point(0.00, 0.24)),
                    new LineGeometry(new Point(0.00, 0.24), new Point(0.00, 0.76)),
                    new LineGeometry(new Point(0.00, 0.76), new Point(0.08, 0.92))
                };
            }
        }
        private static GeometryCollection One
        {
            get
            {
                return new GeometryCollection
                {
                    new LineGeometry(new Point(0.12, 0.18), new Point(0.25, 0.00)),
                    new LineGeometry(new Point(0.25, 0.00), new Point(0.25, 1.00)),
                    new LineGeometry(new Point(0.12, 1.00), new Point(0.38, 1.00))
                };
            }
        }
        private static GeometryCollection Two
        {
            get
            {
                var geometryCollection = new GeometryCollection();

                geometryCollection.AddRange(GetArcGeometry(new Point(0.25, 0.25), 0.25, 199, 30, 6, true));
                geometryCollection.Add(new LineGeometry(new Point(0.467, 0.375), new Point(0.00, 1.00)));
                geometryCollection.Add(new LineGeometry(new Point(0.00, 1.00), new Point(0.50, 1.00)));

                return geometryCollection;
            }
        }
        private static GeometryCollection Three
        {
            get
            {
                var geometryCollection = new GeometryCollection();

                geometryCollection.AddRange(GetArcGeometry(new Point(0.25, 0.24), 0.24, 210, 78, 7, true));
                geometryCollection.AddRange(GetArcGeometry(new Point(0.23, 0.73), 0.27, 285, 147, 7, true));
                geometryCollection.Add(new LineGeometry(new Point(0.16, 0.47), new Point(0.31, 0.47)));

                return geometryCollection;
            }
        }
        private static GeometryCollection Four
        {
            get
            {
                return new GeometryCollection
                {
                    new LineGeometry(new Point(0.40, 1.00), new Point(0.40, 0.00)),
                    new LineGeometry(new Point(0.40, 0.00), new Point(0.00, 0.66)),
                    new LineGeometry(new Point(0.00, 0.66), new Point(0.50, 0.66))
                };
            }
        }
        private static GeometryCollection Five
        {
            get
            {
                GeometryCollection geometryCollection = new GeometryCollection();

                geometryCollection.AddRange(GetArcGeometry(new Point(0.20, 0.69), 0.31, 230, 130, 7, true));
                geometryCollection.Add(new LineGeometry(new Point(0.00, 0.455), new Point(0.00, 0.00)));
                geometryCollection.Add(new LineGeometry(new Point(0.50, 0.00), new Point(0.00, 0.00)));

                return geometryCollection;
            }
        }
        private static GeometryCollection Six
        {
            get
            {
                var geometryCollection = new GeometryCollection();

                geometryCollection.AddRange(GetArcGeometry(new Point(0.38, 0.38), 0.387, 190, 288, 7, true));
                geometryCollection.AddRange(GetArcGeometry(new Point(0.25, 0.65), 0.26, 196, 344, 7, true));
                geometryCollection.AddRange(GetArcGeometry(new Point(0.25, 0.75), 0.26, 16, 164, 7, true));
                geometryCollection.Add(new LineGeometry(new Point(0.00, 0.31), new Point(0.00, 0.825)));
                geometryCollection.Add(new LineGeometry(new Point(0.50, 0.575), new Point(0.50, 0.825)));

                return geometryCollection;
            }
        }
        private static GeometryCollection Seven
        {
            get
            {
                return new GeometryCollection
                {
                    new LineGeometry(new Point(0.50, 0.00), new Point(0.00, 0.00)),
                    new LineGeometry(new Point(0.14, 1.00), new Point(0.50, 0.00))
                };
            }
        }
        private static GeometryCollection Eight
        {
            get
            {
                var geometryCollection = new GeometryCollection();

                geometryCollection.AddRange(GetArcGeometry(new Point(0.25, 0.24), 0.24, 0, 0, 12, true));
                geometryCollection.AddRange(GetArcGeometry(new Point(0.25, 0.74), 0.26, 0, 0, 12, true));

                return geometryCollection;
            }
        }
        private static GeometryCollection Nine
        {
            get
            {
                var geometryCollection = new GeometryCollection();

                geometryCollection.AddRange(GetArcGeometry(new Point(0.25, 0.25), 0.25, 180, 0, 7, true));
                geometryCollection.AddRange(GetArcGeometry(new Point(0.25, 0.30), 0.25, 0, 180, 7, true));
                geometryCollection.AddRange(GetArcGeometry(new Point(-0.16, 0.36), 0.67, 10, 73, 7, true));
                geometryCollection.Add(new LineGeometry(new Point(0.00, 0.25), new Point(0.00, 0.30)));
                geometryCollection.Add(new LineGeometry(new Point(0.50, 0.25), new Point(0.50, 0.48)));

                return geometryCollection;
            }
        }
    }
}