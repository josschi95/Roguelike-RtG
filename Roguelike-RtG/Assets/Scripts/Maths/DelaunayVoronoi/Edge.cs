using System;
using UnityEngine;

namespace DelaunayVoronoi
{
    public class Edge
    {
        public Point Point1 { get; }
        public Point Point2 { get; }

        private float length = -1;
        public float Length
        {
            get
            {
                if (length < 0)
                {
                    float dx = (float)Point1.X - (float)Point2.X;
                    float dy = (float)Point1.Y - (float) Point2.Y;
                    length = Mathf.Sqrt(dx * dx + dy * dy);
                }
                return length;
            }
        }

        public Edge(Point point1, Point point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            var edge = obj as Edge;

            var samePoints = Point1 == edge.Point1 && Point2 == edge.Point2;
            var samePointsReversed = Point1 == edge.Point2 && Point2 == edge.Point1;
            return samePoints || samePointsReversed;
        }

        public override int GetHashCode()
        {
            int hCode = (int)Point1.X ^ (int)Point1.Y ^ (int)Point2.X ^ (int)Point2.Y;
            return hCode.GetHashCode();
        }

        public int LengthComparison(Edge other)
        {
            if (Mathf.Approximately(Length, other.Length)) return 0;
            else if (Length > other.length) return 1;
            return -1;
        }

        public static int LengthComparison(Edge a, Edge b)
        {
            if (Mathf.Approximately(a.Length, b.Length)) return 0;
            else if (a.Length > b.length) return 1;
            return -1;
        }
    }
}