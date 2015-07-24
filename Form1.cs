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
    public partial class Test_Image : Form
    {
        public Test_Image()
        {
            InitializeComponent();
        }
        private Image<Bgr, Byte> photo;


        private void button1_Click(object sender, EventArgs e)
        {
            //1.lode image
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Photo File(**.jpg)|**.jpg";
            openFile.Title = "เลือกไฟล์ภาพ";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFile.FileName);
                photo = new Image<Bgr, byte>((Bitmap)pictureBox1.Image);

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //2.convert gray scale
            Image<Bgr, Byte> photo2;
            photo2 = new Image<Bgr, byte>((Bitmap)pictureBox1.Image);
            Image<Gray, Byte> Grayphoto = photo2.Convert<Gray,byte>();
            pictureBox2.Image = Grayphoto.ToBitmap();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

            //3 smoothing
           
            
                Image<Bgr, Byte> photo3;
                photo3 = new Image<Bgr, byte>((Bitmap)pictureBox1.Image);
                Image<Bgr, Byte> smoothed = photo3.SmoothGaussian(7);
                pictureBox3.Image = smoothed.ToBitmap();

        
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //4.sobel
            Image<Bgr, Byte> photo4;
            photo4 = new Image<Bgr, byte>((Bitmap)pictureBox1.Image);
            Image<Gray, Single> img = photo4.Convert<Gray, Single>();
            Image<Gray, Single> img_final = (img.Sobel(1, 0, 1));
            pictureBox4.Image = img_final.ToBitmap();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //3.1 contrast
            Image<Bgr, Byte> photo3_1;
            photo3_1 = new Image<Bgr, byte>((Bitmap)pictureBox1.Image);
            photo3_1._EqualizeHist();
            photo3_1._GammaCorrect(1.8d);
            pictureBox5.Image = photo3_1.ToBitmap();
           
        }

        private void button6_Click(object sender, EventArgs e)
        {
            PerformShapeDetection();
        }
        /*
        //------------------------------------------------//
        public static Image<Gray, float> GetGradientMagnitude(Image<Gray, float> gX, Image<Gray, float> gY)
        {
            Image<Gray, float> gradient = new Image<Gray, float>(gX.Width, gX.Height);
            for (int i = 0; i < gradient.Height; i++)
            {
                for (int j = 0; j < gradient.Width; j++)
                {
                    float gradVal = (float)Math.Sqrt(Math.Pow(gX[i, j].Intensity, 2.0) + Math.Pow(gY[i, j].Intensity, 2.0));
                    gradient[i, j] = new Gray(gradVal);
                }
            }

            return gradient;
        }
        //------
        public static Image<Gray, float> GetGradientX(Image<Gray, byte> gray)
        {
            var grayFloat = gray.Convert<Gray, float>();
            return grayFloat.Sobel(1, 0, 3);//To obtain gY: grayFloat.Sobel(0, 1, 3)
        }
        //------
        //-------------------------
        public static Image<Gray, float> GetGradientDirection(Image<Gray, byte> gray)
        {
            var gradientDirection = new Image<Gray, float>(gray.Width, gray.Height);
            var gX = GetGradientX(gray);
            var gY = GetGradientY(gray);

            for (int i = 0; i < gradientDirection.Height; i++)
            {
                for (int j = 0; j < gradientDirection.Width; j++)
                {
                   // gradientDirection[i, j] = new Gray(Math.Atan2(gY[i, j].Intensity, gX[i, j].Intensity));
                }
            }
            return gradientDirection;
        }

        private static object GetGradientY(Image<Gray, byte> gray)
        {
            throw new NotImplementedException();
        }

        //-------------------------
        //-----------------------------------------------//
        */

        public void PerformShapeDetection()
        {
            
                //StringBuilder msgBuilder = new StringBuilder("Performance: ");
                Gray cannyThreshold = new Gray(180);
                Gray cannyThresholdLinking = new Gray(120);
                Gray circleAccumulatorThreshold = new Gray(120);
                Image<Bgr, Byte> photo5;
                photo5 = new Image<Bgr, byte>((Bitmap)pictureBox1.Image);
                Image<Gray, Byte> gray = photo5.Convert<Gray, byte>();
                CircleF[] circles = gray.HoughCircles(
                    cannyThreshold,
                    circleAccumulatorThreshold,
                    5.0, //Resolution of the accumulator used to detect centers of the circles
                    10.0, //min distance 
                    5, //min radius
                    0 //max radius
                    )[0]; //Get the circles from the first channel

                Image<Gray, Byte> cannyEdges = gray.Canny(180, 120);
                LineSegment2D[] lines = cannyEdges.HoughLinesBinary(
                    1, //Distance resolution in pixel-related units
                    Math.PI / 45.0, //Angle resolution measured in radians.
                    20, //threshold
                    30, //min Line width
                    10 //gap between lines
                    )[0]; //Get the lines from the first channel

                #region Find triangles and rectangles
                List<Triangle2DF> triangleList = new List<Triangle2DF>();
                List<MCvBox2D> boxList = new List<MCvBox2D>();
            //double largestarea = 0;
            using (MemStorage storage = new MemStorage()) //allocate storage for contour approximation
                    for (Contour<Point> contours = cannyEdges.FindContours(); contours != null; contours = contours.HNext)
                    {
                        Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, storage);
                    double largestarea = contours.Area;
                    string p = largestarea.ToString();  // จะแสดงค่าขนาดพื้นที่ของรูปภาพแต่ทำไม่ได้ 
                    textBox1.Text = p;
                    //MessageBox.Show(largestarea);

                    if (contours.Area > 250) //only consider contours with area greater than 250
                       //MessageBox.Show(contours.Area);

                    {
                            if (currentContour.Total == 3) //The contour has 3 vertices, it is a triangle
                            {
                                Point[] pts = currentContour.ToArray();
                                triangleList.Add(new Triangle2DF(
                                   pts[0],
                                   pts[1],
                                   pts[2]
                                   ));
                           // String text = _ocr.GetText(); หาขนาดแล้วไปแสดงใน text box
                           // ocrTextBox.Text = text;
                        
                        }
                            else if (currentContour.Total == 4) //The contour has 4 vertices.
                            {
                                #region determine if all the angles in the contour are within the range of [80, 100] degree
                                bool isRectangle = true;
                                Point[] pts = currentContour.ToArray();
                                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                                for (int i = 0; i < edges.Length; i++)
                                {
                                    double angle = Math.Abs(
                                       edges[(i + 1) % edges.Length].GetExteriorAngleDegree(edges[i]));
                                    if (angle < 80 || angle > 100)
                                    {
                                        isRectangle = false;
                                        break;
                                    }
                                }
                                #endregion

                                if (isRectangle) boxList.Add(currentContour.GetMinAreaRect());
                            }
                        }
                    }
            #endregion
            //pictureBox6.Image = photo5.ToBitmap();
            pictureBox6.Image = cannyEdges.ToBitmap();

            
                       #region draw triangles and rectangles
                       Image<Bgr, Byte> triangleRectangleImage = photo5.CopyBlank();
                       foreach (Triangle2DF triangle in triangleList)
                           triangleRectangleImage.Draw(triangle, new Bgr(Color.DarkBlue), 2);
                       foreach (MCvBox2D box in boxList)
                           triangleRectangleImage.Draw(box, new Bgr(Color.DarkOrange), 2);
                           pictureBox7.Image = triangleRectangleImage.ToBitmap();
                        #endregion

                        #region draw circles
                        Image<Bgr, Byte> circleImage = photo5.CopyBlank();
                       foreach (CircleF circle in circles)
                           circleImage.Draw(circle, new Bgr(Color.Brown), 2);
                      // circleImageBox.Image = circleImage;
                      pictureBox8.Image = circleImage.ToBitmap();
                        #endregion
            /*
                       #region draw lines
                       Image<Bgr, Byte> lineImage = img.CopyBlank();
                       foreach (LineSegment2D line in lines)
                           lineImage.Draw(line, new Bgr(Color.Green), 2);
                       lineImageBox.Image = lineImage;
                       #endregion
             */
        }

        private void button7_Click(object sender, EventArgs e)
        {
           // MessageBox.Show("contours.Area");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
       
}

