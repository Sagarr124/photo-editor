using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace PhotoEditor
{
    public partial class frmPhotoEditor : Form
    {

        private Image Img;
        private Size OriginalImageSize;
        private Size ModifiedImageSize;

        int cropX;
        int cropY;
        int cropWidth;
        int cropHeight;
        public Pen cropPen;

        public DashStyle cropDashStyle = DashStyle.DashDot;
        public bool MakeSelection = false;
        public bool ImgSaved = false;
        public bool CreateText = false;

        public frmPhotoEditor()
        {
            InitializeComponent();
        }

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPEG Files (*.jpg;*.jpeg)|*.jpg;*.jpeg";
            openFileDialog.Title = "Select Image";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Img = Image.FromFile(openFileDialog.FileName);
                LoadImage();
                SetResizeInfo();
                this.trackBar1.Enabled = true;
                this.trackBar2.Enabled = true;
                this.trackBar3.Enabled = true;
                this.trackBar6.Enabled = true;
                this.trackBar4.Enabled = true;
                this.trackBar5.Enabled = true;
                this.NumericUpDown1.Enabled = true;
            }
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JPEG Files (*.jpg;*.jpeg)|*.jpg;*.jpeg";
                saveFileDialog.Title = "Save Image";
                if(saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    PictureBox1.Image.Save(saveFileDialog.FileName);
                    ImgSaved = true;
                    MessageBox.Show("Image saved successfully.");
                }
            }
            else
            {
                MessageBox.Show("Open an image.", "Error");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ImgSaved)
            {
                Application.Exit();
            }
            else
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;
                DialogResult result = MessageBox.Show("Photo Editor", "Do you want to save your work?", buttons);
                if (result == DialogResult.Yes)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "JPEG Files (*.jpg;*.jpeg)|*.jpg;*.jpeg";
                    saveFileDialog.Title = "Save Image";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        Img.Save(saveFileDialog.FileName);
                        Application.Exit();
                    }
                }
                else if(result == DialogResult.No)
                {
                    Application.Exit();
                }
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            if (Img == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "JPEG Files (*.jpg;*.jpeg)|*.jpg;*.jpeg";
                openFileDialog.Title = "Select Image";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Img = Image.FromFile(openFileDialog.FileName);
                    LoadImage();
                    SetResizeInfo();
                    this.trackBar1.Enabled = true;
                    this.trackBar2.Enabled = true;
                    this.trackBar3.Enabled = true;
                    this.trackBar6.Enabled = true;
                    this.trackBar4.Enabled = true;
                    this.trackBar5.Enabled = true;
                    this.NumericUpDown1.Enabled = true;

                }
            }
        }

        private void PictureBoxLocation()
        {
            int _x = 0;
            int _y = 0;
            if (splitContainer1.Panel1.Width > PictureBox1.Width)
            {
                _x = (splitContainer1.Panel1.Width - PictureBox1.Width) / 2;
            }
            if (splitContainer1.Panel1.Height > PictureBox1.Height)
            {
                _y = (splitContainer1.Panel1.Height - PictureBox1.Height) / 2;
            }
            PictureBox1.Location = new Point(_x, _y);
        }

        private void splitContainer1_Panel1_Resize(object sender, EventArgs e)
        {
            PictureBoxLocation();
        }

        private void LoadImage()
        {
            //we set the picturebox size according to image, we can get image width and height with the help of Image.Width and Image.height properties.
            int imgWidth = Img.Width;
            int imghieght = Img.Height;
            PictureBox1.Width = imgWidth;
            PictureBox1.Height = imghieght;
            PictureBox1.Image = Img;
            PictureBoxLocation();
            OriginalImageSize = new Size(imgWidth, imghieght);
            SetResizeInfo();
        }

        private void SetResizeInfo()
        {
            lblOriginalSize.Text = OriginalImageSize.ToString();
            lblModifiedSize.Text = ModifiedImageSize.ToString();
        }

        #region Tools
        private void btnRotateLeft_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                PictureBox1.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                PictureBox1.Refresh();
            }
        }

        private void btnRotateRight_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                PictureBox1.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                PictureBox1.Refresh();
            }
        }

        private void btnRotateVertical_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                PictureBox1.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                PictureBox1.Refresh();
            }
        }

        private void btnRotateHorizontal_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                PictureBox1.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                PictureBox1.Refresh();
            }
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int percentage = 0;
            try
            {
                percentage = Convert.ToInt32(NumericUpDown1.Value);
                ModifiedImageSize = new Size((OriginalImageSize.Width * percentage) / 100, (OriginalImageSize.Height * percentage) / 100);
                SetResizeInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Percentage\n" + ex);
                return;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                Bitmap bm_source = new Bitmap(PictureBox1.Image);
                // Make a bitmap for the result.
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(ModifiedImageSize.Width), Convert.ToInt32(ModifiedImageSize.Height));
                // Make a Graphics object for the result Bitmap.
                Graphics gr_dest = Graphics.FromImage(bm_dest);
                // Copy the source image into the destination bitmap.
                gr_dest.DrawImage(bm_source, 0, 0, bm_dest.Width + 1, bm_dest.Height + 1);
                // Display the result.
                PictureBox1.Image = bm_dest;
                PictureBox1.Width = bm_dest.Width;
                PictureBox1.Height = bm_dest.Height;
                PictureBoxLocation();
            }
        }

        private void btnMakeSelection_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                MakeSelection = true;
                btnCrop.Enabled = true;
            }
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                Cursor = Cursors.Default;

                try
                {
                    if (cropWidth < 1)
                    {
                        return;
                    }
                    Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
                    //First we define a rectangle with the help of already calculated points
                    Bitmap OriginalImage = new Bitmap(PictureBox1.Image, PictureBox1.Width, PictureBox1.Height);
                    //Original image
                    Bitmap _img = new Bitmap(cropWidth, cropHeight);
                    // for cropinf image
                    Graphics g = Graphics.FromImage(_img);
                    // create graphics
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    //set image attributes
                    g.DrawImage(OriginalImage, 0, 0, rect, GraphicsUnit.Pixel);

                    PictureBox1.Image = _img;
                    PictureBox1.Width = _img.Width;
                    PictureBox1.Height = _img.Height;
                    PictureBoxLocation();
                    btnCrop.Enabled = false;
                    MakeSelection = false;
                    Img = _img;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Img != null)
            {
                if (tabControl.SelectedIndex == 1)
                {
                    Point TextStartLocation = e.Location;
                    if (CreateText)
                    {
                        Cursor = Cursors.IBeam;
                    }
                }
                else
                {
                    Cursor = Cursors.Default;
                    if (MakeSelection)
                    {
                        try
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                Cursor = Cursors.Cross;
                                cropX = e.X;
                                cropY = e.Y;

                                cropPen = new Pen(Color.Black, 1);
                                cropPen.DashStyle = DashStyle.DashDotDot;


                            }
                            PictureBox1.Refresh();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Img != null)
            {
                if (tabControl.SelectedIndex == 1)
                {
                    Point TextStartLocation = e.Location;
                    if (CreateText)
                    {
                        Cursor = Cursors.IBeam;
                    }
                }
                else
                {
                    Cursor = Cursors.Default;
                    if (MakeSelection)
                    {
                        try
                        {
                            if (PictureBox1.Image == null)
                                return;


                            if (e.Button == MouseButtons.Left)
                            {
                                PictureBox1.Refresh();
                                cropWidth = e.X - cropX;
                                cropHeight = e.Y - cropY;
                                PictureBox1.CreateGraphics().DrawRectangle(cropPen, cropX, cropY, cropWidth, cropHeight);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (Img != null)
            {
                if (MakeSelection)
                {
                    Cursor = Cursors.Default;
                }
            }
        }
        #endregion


        #region Enhance
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            NumericUpDown2.Value = trackBar1.Value;

            if (Img != null)
            {
                float value = trackBar1.Value * 0.01f;

                float[][] colorMatrixElements = { 
                    new float[] { 1, 0, 0, 0, 0 },
                    new float[] { 0, 1, 0, 0, 0 },
                    new float[] { 0, 0, 1, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { value, value, value, 0, 1 } 
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            NumericUpDown3.Value = trackBar2.Value;

            if (Img != null)
            {
                float value = trackBar2.Value * 0.01f;
                float t = (0.1f - value) / 2.0f;

                float[][] colorMatrixElements = {
                    new float[] { value, 0, 0, 0, 0 },
                    new float[] { 0, value, 0, 0, 0 },
                    new float[] { 0, 0, value, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { t, t, t, 0, 1 }
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            NumericUpDown4.Value = trackBar3.Value;

            if (Img != null)
            {
                float value = trackBar3.Value * 0.01f;
                float sr = (1 - value) * 0.3086f;
                float sg = (1 - value) * 0.6094f;
                float sb = (1 - value) * 0.0820f;

                float[][] colorMatrixElements = {
                    new float[] { sr + value, sr, sr, 0, 0 },
                    new float[] { sg, sg + value, sg, 0, 0 },
                    new float[] { sb, sb, sb + value, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 } 
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            NumericUpDown5.Value = trackBar4.Value;

            if (Img != null)
            {
                float value = trackBar4.Value * 0.1f;

                float[][] colorMatrixElements = {
                    new float[] { value, 0, 0, 0, 0 },
                    new float[] { 0, 1, 0, 0, 0 },
                    new float[] { 0, 0, 1, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 } 
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            NumericUpDown6.Value = trackBar5.Value;

            if (Img != null)
            {
                float value = trackBar5.Value * 0.1f;

                float[][] colorMatrixElements = {
                    new float[] { 1, 0, 0, 0, 0 },
                    new float[] { 0, value, 0, 0, 0 },
                    new float[] { 0, 0, 1, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 } 
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            NumericUpDown7.Value = trackBar6.Value;

            if (Img != null)
            {
                float value = trackBar6.Value * 0.01f;

                float[][] colorMatrixElements = {
                    new float[] { 1, 0, 0, 0, 0 },
                    new float[] { 0, 1, 0, 0, 0 },
                    new float[] { 0, 0, value, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }
        #endregion
        

        #region Filters
        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                float[][] colorMatrixElements = {
                    new float[] { 1.438f, -0.062f, -0.062f, 0, 0 },
                    new float[] { -0.122f, 1.378f, -0.122f, 0, 0 },
                    new float[] { -0.016f, -0.016f, 1.483f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { -0.03f, 0.05f, -0.02f, 0, 1 }
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }

        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                float[][] colorMatrixElements = {
                    new float[] { 0, 0, 1, 0, 0 },
                    new float[] { 0, 1, 0, 0, 0 },
                    new float[] { 1, 0, 0, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }

        private void guna2CirclePictureBox3_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                float[][] colorMatrixElements = {
                    new float[] { 1.5f, 1.5f, 1.5f, 0, 0 },
                    new float[] { 1.5f, 1.5f, 1.5f, 0, 0 },
                    new float[] { 1.5f, 1.5f, 1.5f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { -1, -1, -1, 0, 1 }
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }

        private void guna2CirclePictureBox4_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                float[][] colorMatrixElements = {
                    new float[] { 0.393f, 0.349f, 0.272f, 0, 0 },
                    new float[] { 0.769f, 0.686f, 0.534f, 0, 0 },
                    new float[] { 0.189f, 0.168f, 0.131f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }

        private void guna2CirclePictureBox5_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                float[][] colorMatrixElements = {
                    new float[] { 0.3f, 0.3f, 0.3f, 0, 0 },
                    new float[] { 0.6f, 0.6f, 0.6f, 0, 0 },
                    new float[] { 0.1f, 0.1f, 0.1f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }

        private void guna2CirclePictureBox6_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                float[][] colorMatrixElements = {
                    new float[] { -1, 0, 0, 0, 0 },
                    new float[] { 0, -1, 0, 0, 0 },
                    new float[] { 0, 0, -1, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 1, 1, 1, 0, 1 }
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            if (Img != null)
            {
                float[][] colorMatrixElements = {
                    new float[] { 1, 0, 0, 0, 0 },
                    new float[] { 0, 1, 0, 0, 0 },
                    new float[] { 0, 0, 1, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                };

                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                ImageAttributes imageAttributes = new ImageAttributes();


                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                Image _img = Img;
                Graphics _g = default(Graphics);
                Bitmap bm_dest = new Bitmap(Convert.ToInt32(_img.Width), Convert.ToInt32(_img.Height));
                _g = Graphics.FromImage(bm_dest);
                _g.DrawImage(_img, new Rectangle(0, 0, bm_dest.Width + 1, bm_dest.Height + 1), 0, 0, bm_dest.Width + 1, bm_dest.Height + 1, GraphicsUnit.Pixel, imageAttributes);
                PictureBox1.Image = bm_dest;
            }
        }
        #endregion

    }
}
