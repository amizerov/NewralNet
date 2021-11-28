using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace EmguCVtest1
{
    public partial class Form1 : Form
    {
        Dictionary<string, Image<Bgr,byte>> IMGDict;
        public Form1()
        {
            InitializeComponent();
            IMGDict = new Dictionary<string, Image<Bgr,byte>>();
        }

        private void îòêðûòüToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp;)|*.jpg;*.jpeg;*.png;*.bmp;|All Files (*.*)|*.*;";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var img = new Image<Bgr,byte>(dialog.FileName);
                    pictureBox1.Image = img.ToBitmap();
                    if (IMGDict.ContainsKey("input"))
                    {
                        IMGDict.Remove("input");
                    }
                    IMGDict.Add("input", img);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void shapeMatchingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmShapeMatchParams form = new FrmShapeMatchParams();
                form.OnShapeMatching += ApplyShapeMatching;
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ApplyShapeMatching(Image<Bgr, byte> imgTemplate, 
            double threshold = 0.00001, double area = 1000, ContoursMatchType matchType = ContoursMatchType.I2)
        {
            try
            {
                if(IMGDict["input"] == null)
                {
                    MessageBox.Show("Select an image");
                }
                var img = IMGDict["input"].Clone();
                var imgSource = img.Convert<Gray, byte>()
                    .SmoothGaussian(3)
                    .ThresholdBinaryInv(new Gray(240), new Gray(255));
                var imgTarget = imgTemplate.Convert<Gray, byte>()
                    .SmoothGaussian(3)
                    .ThresholdBinaryInv(new Gray(240), new Gray(255));

                var imgSourceContours = CalculateContours(imgSource, area);
                var imgTargetContours = CalculateContours(imgTarget, area);

                if(imgSourceContours.Size == 0 || imgTargetContours.Size == 0)
                {
                    throw new Exception("Not enogth contours.");
                }

                for(int i = 0; i < imgSourceContours.Size; i++)
                {
                    var distance = CvInvoke.MatchShapes(imgSourceContours[i], 
                        imgTargetContours[0], matchType);

                    if(distance <= threshold)
                    {
                        var rect = CvInvoke.BoundingRectangle(imgSourceContours[i]);
                        img.Draw(rect, new Bgr(0, 255, 0), 4);
                        CvInvoke.PutText(img, distance.ToString("F6"), new Point(rect.X, rect.Y+20),
                            FontFace.HersheyPlain, 2, new MCvScalar(0, 0, 0));                    }
                }

                pictureBox1.Image = img.ToBitmap();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private VectorOfVectorOfPoint CalculateContours(Image<Gray, byte> img, double thresholdarea = 1000)
        {
            try
            {
                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                Mat h = new Mat();

                CvInvoke.FindContours(img, contours, h, RetrType.External,
                    ChainApproxMethod.ChainApproxSimple);

                VectorOfVectorOfPoint filteredContours = new VectorOfVectorOfPoint();
                for(int i=0; i< contours.Size; i++)
                {
                    var area = CvInvoke.ContourArea(contours[i]);
                    if(area > thresholdarea)
                    {
                        filteredContours.Push(contours[i]);
                    }
                }
                    
                return contours;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}