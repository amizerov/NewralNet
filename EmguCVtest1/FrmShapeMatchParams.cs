using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace EmguCVtest1
{
    public partial class FrmShapeMatchParams : Form
    {
        public delegate void DelegateApplyShapeMatching(Image<Bgr, byte> imgTemplate,
            double threshold = 0.00001, double area = 1000, ContoursMatchType matchType = ContoursMatchType.I2);
        public event DelegateApplyShapeMatching OnShapeMatching;

        public FrmShapeMatchParams()
        {
            InitializeComponent();
        }

        private void FrmShapeMatchParams_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(pictureBox1.Image == null)
                {
                    throw new Exception("Select a template image");
                }

                var img = new Bitmap(pictureBox1.Image).ToImage<Bgr, byte>();
                ContoursMatchType matchType = ContoursMatchType.I2;
                
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        matchType = ContoursMatchType.I1;
                        break;
                    case 1:
                        matchType = ContoursMatchType.I2;
                        break;
                    case 2:
                        matchType= ContoursMatchType.I3;
                        break;
                }

                double areaThreshold = 1000;
                double.TryParse(tbMinArea.Text, out areaThreshold);
                double threshold = 0.001;
                double.TryParse(tbDistTheshold.Text, out threshold);
            
                if(OnShapeMatching != null)
                {
                    OnShapeMatching(img, threshold, areaThreshold, matchType);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp;)|*.jpg;*.jpeg;*.png;*.bmp;|All Files (*.*)|*.*;";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var img = new Image<Bgr, byte>(dialog.FileName);
                    pictureBox1.Image = img.ToBitmap();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void rotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var img = new Bitmap(pictureBox1.Image)
                    .ToImage<Bgr, byte>()
                    .Rotate(15, new Bgr(255, 255, 255));
                pictureBox1.Image = img.ToBitmap();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void scaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var img = new Bitmap(pictureBox1.Image)
                    .ToImage<Bgr, byte>()
                    .Resize(1.25, Inter.Cubic);
                pictureBox1.Image = img.ToBitmap();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
