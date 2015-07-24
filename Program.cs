using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.ML;
using Emgu.CV.UI;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using System.Diagnostics;
namespace WindowsFormsApplication4
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Test_Image());
        }
    }

    class MergePolygon
    {
       public List<Contour<Point>> contourList = new List<Contour<Point>>();

        public MergePolygon(Contour<Point> initContour)
        {
            contourList.Add(initContour);
            //Image<Gray, Byte> cannyEdges = gray.Canny(180, 120);
            //LineSegment2D[] lines = cannyEdges.HoughLinesBinary(
            //    1, //Distance resolution in pixel-related units
            //    Math.PI / 45.0, //Angle resolution measured in radians.
            //    20, //threshold
            //    30, //min Line width
            //    10 //gap between lines
            //    )[0]; //Get the lines from the first channel
            //for (Contour<Point> contours = cannyEdges.FindContours(); contours != null; contours = contours.HNext)
            //{
            //    contourList.Add(contours);

            //}
        }
        public void AddPolygon(Contour<Point> c)
        {
            this.contourList.Add(c);
        }
        //public float FindArea() {

        //}
        public enum Intersecresult
        {
        Notintersec,
            Intersec,
            MergeInto,
        }
        public Intersecresult IsIntersectOrUnion(Contour<Point> c)
        {
            //double sum = 0;
           // double Avg = 0;
            //double number = 0;
            int j = contourList.Count;
            for (int i = 0; i < j; ++i)
            {
                var c1 = contourList[i];
                if (FindContourIntersec(c1, c))
                {
                  //  sum = sum + c.Area;
                   
                    return Intersecresult.Intersec;

                }
                else if (IsUnion(c1, c))
                {
                 //   sum = sum + c.Area;
                  //  Console.WriteLine("Sum contour" + sum + "-" + number);
                    return Intersecresult.MergeInto;
                }
              
              //  number++;
            }
           // Console.WriteLine("Sum contour" + sum + "-" + number);
            return Intersecresult.Notintersec;
        }
        //public bool FindUnion(Contour<Point> c, Contour<Point> c1)
        //{

        //    Point[] pts3 = c.ToArray();
        //    Point[] pts4 = c1.ToArray();

        //    //int len1 = pts3.Length;
        //    //int len2 = pts4.Length;
        //    //Point s1 = pts4[0];

        //    //if (IsInPolygon(pts3, s1))
        //    //{

        //    //    return true;
        //    //}
        //   // int j = contourList.Count;
        //    // bool isU = false;
        //    for (int i = 0; i < j; ++i)
        //    {
        //        var c1 = contourList[i];
        //        if (IsUnion(c1, c))
        //        {
        //            ///isU = true;
        //            return true;
        //        }
        //        else
        //        {
        //            //  bool union = IsUnion(c1, c);

        //        }
        //    }
        //    return false;
        //}


        public static PointF FindLineIntersection(PointF start1, PointF end1, PointF start2, PointF end2)
        {
            float denom = ((end1.X - start1.X) * (end2.Y - start2.Y)) - ((end1.Y - start1.Y) * (end2.X - start2.X));

            //  AB & CD are parallel 
            if (denom == 0)
                return PointF.Empty;

            float numer = ((

                start1.Y - start2.Y) * (end2.X - start2.X)) - ((start1.X - start2.X) * (end2.Y - start2.Y));

            float r = numer / denom;

            float numer2 = ((start1.Y - start2.Y) * (end1.X - start1.X)) - ((start1.X - start2.X) * (end1.Y - start1.Y));

            float s = numer2 / denom;

            if ((r < 0 || r > 1) || (s < 0 || s > 1))
                return PointF.Empty;

            // Find intersection point
            PointF result = new PointF();
            result.X = start1.X + (r * (end1.X - start1.X));
            result.Y = start1.Y + (r * (end1.Y - start1.Y));
            // MessageBox.Show(result.X.ToString());
            //MessageBox.Show(result.Y.ToString());
            //  Console.WriteLine(result);
            return result;
        }
        bool FindContourIntersec(Contour<Point> c1, Contour<Point> c2)
        {
            Point[] pts1 = c1.ToArray();
            Point[] pts2 = c2.ToArray();
            int len1 = pts1.Length;
            int len2 = pts2.Length;
            for (int i = 0; i <= len1 - 1; i++)
            {
                Point s1;
                Point e1;
                if (i == len1 - 1)
                {
                    s1 = pts1[len1 - 1];
                    e1 = pts1[0];
                }
                else
                {
                    s1 = pts1[i];
                    e1 = pts1[i + 1];
                }
                for (int j = 0; j < len2 - 1; j++)
                {
                    Point s2 = pts2[j];
                    Point e2 = pts2[j + 1];
                    PointF result = FindLineIntersection(s1, e1, s2, e2);

                    if (!result.IsEmpty)
                    {
                        //  Console.WriteLine("point" + result);
                        return true;
                    }
                }
                {
                    Point s2 = pts2[len2 - 1];
                    Point e2 = pts2[0];
                    PointF result = FindLineIntersection(s1, e1, s2, e2);

                    if (!result.IsEmpty)
                    {
                        return true;
                    }
                }

            }
            return false;
        }
        float CalculatePolygonArea(Point[] pts)
        {
            //int i = 0;
            //  MessageBox.Show(pts[0+1].Y.ToString());
            int i = 0;
            int len = pts.Length;
            float num = 0;
            float number;

            // MessageBox.Show(pts[0].ToString());
            for (i = 0; i < len - 1; i++)
            {

                float Xn = pts[i].X;//MessageBox.Show(ptsวว[j].X.ToString());                                    
                float Yn = pts[i].Y; //MessageBox.Show(pts[j].X.ToString());                                 
                float Xn1 = pts[i + 1].X;// MessageBox.Show(pts[j++].Y.ToString()); 
                float Yn1 = pts[i + 1].Y; //MessageBox.Show(pts[--j].Y.ToString());

                num = num + ((Xn * Yn1) - (Yn * Xn1));

                // Console.Write("(" + Xn.ToString() + "," + Yn + ")" + "," + "(" + Xn1 + "," + Yn1 + ")");
            }
            {
                float Xn = pts[len - 1].X;
                float Yn = pts[len - 1].Y;
                float Xn1 = pts[0].X;
                float Yn1 = pts[0].Y;
                num = num + ((Xn * Yn1) - (Yn * Xn1));

                //  Console.Write("(" + Xn.ToString() + "," + Yn + ")" + "," + "(" + Xn1 + "," + Yn1 + ")");// MessageBox.Show(A.ToString());
            }


            num = Math.Abs(num);
            number = num / 2;
            // Console.WriteLine("num" + num);
            // Console.WriteLine("number"+number);        
            return number;
        }
        //----------------------------------------------------------------------------------------------------------------------
        //int orientation(PointF p, PointF q, PointF r)
        //{
        //    int val = (q.Y - p.y) * (r.x - q.x) -
        //              (q.x - p.x) * (r.y - q.y);

        //    if (val == 0) return 0;  // colinear
        //    return (val > 0) ? 1 : 2; // clock or counterclock wise
        //}
        //------------------------------------------------------------------------------------------------------------------------
        //public static PointF FindLineUnion(PointF s1, PointF s2, PointF s3)
        //{
        //    //int n = len3;
        //    PointF denom = new PointF();
        //     denom.X = s1.X - s2.X ;
        //     denom.Y = s1.Y - s2.Y;
        //    int count = 0, i = 0;
        //    do
        //    {
        //      //  int next = (i + 1) % n;
        //       PointF resual= orientation(s1, s2,s3);
        //    } while (i != 0);

        //    return denom;
        //}
        bool IsUnion(Contour<Point> c1, Contour<Point> c2)
        {
            
           
                if (c1.Area > c2.Area)
                {
                    Point[] pts1 = c1.ToArray();
                    Point[] pts2 = c2.ToArray();
                    Point s1 = pts2[0];
                    if (IsInPolygon(pts1, s1))
                    {
                        return true;
                    }
                }
                else
                {
                    Point[] pts1 = c1.ToArray();
                    Point[] pts2 = c2.ToArray();
                    Point s1 = pts1[0];
               // Console.WriteLine("mint"+s1);
                    if (IsInPolygon(pts2, s1))
                    {
                        return true;
                    }
                }
            
            //    for (int i = 0; i <= len1 - 1; i++)
            //{
            //    Point s1 = pts3[i];
            //    // Point s2 = pts3[i + 1];


            //    for (int j = 0; j < len2 - 1; j++)
            //    {
            //        Point s3 = pts4[0];
            //        //if (IsInPolygon(pts3, s3))
            //        //{
            //        //    return true;
            //        //}
            //    }
            //}
            return false;

        }

        //public static bool IsInPolygon(Point[] poly, Point point)
        //{
        //    var coef = poly.Skip(1).Select((p, i) =>
        //                                    (point.Y - poly[i].Y) * (p.X - poly[i].X)
        //                                  - (point.X - poly[i].X) * (p.Y - poly[i].Y))
        //                            .ToList();

        //    if (coef.Any(p => p == 0))
        //        return true;

        //    for (int i = 1; i < coef.Count(); i++)
        //    {
        //        if (coef[i] * coef[i - 1] < 0)
        //            return false;
        //    }
        //    return true;
        //}


        public static bool IsInPolygon(Point[] poly, Point p)
        {
            Point p1, p2;


            bool inside = false;


            if (poly.Length < 3)
            {
                return inside;
            }


            var oldPoint = new Point(
                poly[poly.Length - 1].X, poly[poly.Length - 1].Y);


            for (int i = 0; i < poly.Length; i++)
            {
                var newPoint = new Point(poly[i].X, poly[i].Y);


                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;

                    p2 = newPoint;
                }

                else
                {
                    p1 = newPoint;

                    p2 = oldPoint;
                }


                if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                    && (p.Y - (long)p1.Y) * (p2.X - p1.X)
                    < (p2.Y - (long)p1.Y) * (p.X - p1.X))
                {
                    inside = !inside;
                }


                oldPoint = newPoint;
            }


            return inside;
        }
    }


    //static class PolygonTools  float CalculatePolygonArea(Point[] pts)
    //{
    //    //int i = 0;
    //    //  MessageBox.Show(pts[0+1].Y.ToString());
    //    int i = 0;
    //    int len = pts.Length;
    //    float num = 0;
    //    float number;

    //    // MessageBox.Show(pts[0].ToString());
    //    for (i = 0; i < len - 1; i++)
    //    {

    //        float Xn = pts[i].X;//MessageBox.Show(ptsวว[j].X.ToString());                                    
    //        float Yn = pts[i].Y; //MessageBox.Show(pts[j].X.ToString());                                 
    //        float Xn1 = pts[i + 1].X;// MessageBox.Show(pts[j++].Y.ToString()); 
    //        float Yn1 = pts[i + 1].Y; //MessageBox.Show(pts[--j].Y.ToString());

    //        num = num + ((Xn * Yn1) - (Yn * Xn1));

    //        // Console.Write("(" + Xn.ToString() + "," + Yn + ")" + "," + "(" + Xn1 + "," + Yn1 + ")");
    //    }
    //    {
    //        float Xn = pts[len - 1].X;
    //        float Yn = pts[len - 1].Y;
    //        float Xn1 = pts[0].X;
    //        float Yn1 = pts[0].Y;
    //        num = num + ((Xn * Yn1) - (Yn * Xn1));

    //        //  Console.Write("(" + Xn.ToString() + "," + Yn + ")" + "," + "(" + Xn1 + "," + Yn1 + ")");// MessageBox.Show(A.ToString());
    //    }


    //    num = Math.Abs(num);
    //    number = num / 2;
    //    // Console.WriteLine("num" + num);
    //    // Console.WriteLine("number"+number);        
    //    return number;
    //}


}


