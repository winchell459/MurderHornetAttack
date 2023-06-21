using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class Utility
    {
        public static int seed {
            get { return _seed; }
            set {
                _seed = value;
                random = new System.Random(seed);
            }
        }
        private static int _seed;
        static System.Random random;

        public static int Random(int min, int max)
        {
            if (random == null) random = new System.Random(seed);
            return random.Next(min, max);
        }
        public static float Random(float min, float max)
        {
            if (random == null) random = new System.Random(seed);
            return ((float)random.NextDouble() * (max - min) + min);
        }

        public static float GetTime()
        {
            return (System.DateTime.Now.Hour * 60 + System.DateTime.Now.Minute) * 60 + System.DateTime.Now.Second;
        }
        public static string FormatFloat(float value, int decimals)
        {

            int valueInt = (int)value;
            string valueStr = valueInt.ToString();
            if (decimals > 0) valueStr += ".";
            for (int i = 0; i < decimals; i += 1)
            {
                value -= valueInt;
                value *= 10;
                valueInt = (int)value;
                valueStr += valueInt.ToString();
            }
            return valueStr;
        }
        /// <summary>
        /// returns the shortest distance from a point to a line
        /// </summary>
        /// <param name="point"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static float PointDistanceToPath(Vector2 point, Vector2 start, Vector2 end)
        {
            float A = point.x - start.x;
            float B = point.y - start.y;
            float C = end.x - start.x;
            float D = end.y - start.y;

            float dot = A * C + B * D;
            float path = C * C + D * D;
            float check = -1;
            if (path != 0) check = dot / path;

            float xx, yy;

            if (check < 0)
            {
                xx = start.x;
                yy = start.y;
            }
            else if (check > 1)
            {
                xx = end.x;
                yy = end.y;
            }
            else
            {
                xx = start.x + check * C;
                yy = start.y + check * D;
            }
            float dx = point.x - xx;
            float dy = point.y - yy;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// returns the closets point on a line from another point not on at line
        /// </summary>
        /// <param name="point"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Vector2 ClosestPointOnLine(Vector2 point, Vector2 start, Vector2 end)
        {
            float A = point.x - start.x;
            float B = point.y - start.y;
            float C = end.x - start.x;
            float D = end.y - start.y;

            float dot = A * C + B * D;
            float path = C * C + D * D;
            float check = -1;
            if (path != 0) check = dot / path;

            Vector2 linePoint;
            if (check < 0)
            {
                linePoint = start;
            }
            else if (check > 1)
            {
                linePoint = end;
            }
            else
            {
                linePoint = start + (end - start) * check;
            }
            return linePoint;
        }

        public static bool CheckIntersecting(Vector2 p1_1, Vector2 p1_2, Vector2 p2_1, Vector2 p2_2)
        {
            bool intersects = false;
            /*find intersection of two lines
             * https://www.topcoder.com/community/competitive-programming/tutorials/geometry-concepts-line-intersection-and-its-applications/#:~:text=To%20find%20the%20point%20at,two%20unknowns%2C%20x%20and%20y.&text=Finally%2C%20divide%20both%20sides%20by,y%20can%20be%20derived%20similarly.

             */
            float A1 = p1_2.y - p1_1.y; //A1 = y1_2 - y1_1
            float B1 = p1_1.x - p1_2.x;
            float C1 = A1 * p1_1.x + B1 * p1_1.y;
            float A2 = p2_2.y - p2_1.y; //A2 = y2_2 - y2_1
            float B2 = p2_1.x - p2_2.x;
            float C2 = A2 * p2_1.x + B2 * p2_1.y;

            float det = A1 * B2 - A2 * B1;
            if (det != 0)
            {
                float x = (B2 * C1 - B1 * C2) / det;
                float y = (A1 * C2 - A2 * C1) / det;
                //Debug.Log(x + ", " + y);
                if (!((x > p1_1.x && x > p1_2.x) || (x < p1_1.x && x < p1_2.x)) && !((x < p2_1.x && x < p2_2.x) || (x > p2_1.x && x > p2_2.x))) intersects = true;


            }

            return intersects;
        }

        public static bool ShapeOverlappped(Circle circle, List<Circle> circles)
        {
            bool overlap = false;
            foreach (Circle c in circles)
            {
                if (Vector2.Distance(c.pos, circle.pos) < c.r + circle.r) overlap = true;
            }
            return overlap;
        }


    }
}

