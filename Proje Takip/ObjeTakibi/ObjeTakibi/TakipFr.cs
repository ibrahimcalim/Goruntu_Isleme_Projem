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
using System.IO.Ports;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Controls;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Math.Geometry;
using AForge.Math;




namespace ObjeTakibi
{
    public partial class TakipFr : Form
    {
        private FilterInfoCollection kameralar;
        private VideoCaptureDevice kameram;
        public TakipFr()
        {
            InitializeComponent();
        }
        
        int R, G, B;
        int gen, yuk;

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            trackBar1.Maximum = 255;
            trackBar1.Minimum = 0;
            trackBar1.TickFrequency = 1;
            trackBar1.LargeChange = 1;
            trackBar1.SmallChange = 1;
            label1.Text = "Kırmızı: " + trackBar1.Value.ToString();
            R = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            trackBar2.Maximum = 255;
            trackBar2.Minimum = 0;
            trackBar2.TickFrequency = 1;
            trackBar2.LargeChange = 1;
            trackBar2.SmallChange = 1;
            label2.Text = "Yeşil: " + trackBar2.Value.ToString();
            G = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            trackBar3.Maximum = 255;
            trackBar3.Minimum = 0;
            trackBar3.TickFrequency = 1;
            trackBar3.LargeChange = 1;
            trackBar3.SmallChange = 1;
            label3.Text = "Mavi: " + trackBar3.Value.ToString();
            B = trackBar3.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            {
                if (kameram.IsRunning)
                {
                    kameram.Stop();
                }
            }
        }

     

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = 0;
            serialPort1.BaudRate = 9600;
            serialPort1.Open();
          
        }

        private void button5_Click(object sender, EventArgs e)
        {
            serialPort1.Close(); 
        }

      

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox2.DataSource = SerialPort.GetPortNames();
            serialPort1.PortName = comboBox2.SelectedItem.ToString();

            kameralar = new FilterInfoCollection(FilterCategory.VideoInputDevice); 
            foreach (FilterInfo item in kameralar)
            {
                comboBox1.Items.Add(item.Name);
            }
            

            comboBox1.SelectedIndex = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void dosyaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                serialPort1.Write("0");

                if (kameram.IsRunning)
                {
                    kameram.Stop();
                }
                this.Close();
            
        }
    }

        private void button1_Click(object sender, EventArgs e)
        {
            kameram = new VideoCaptureDevice(kameralar[comboBox1.SelectedIndex].MonikerString);
            kameram.NewFrame += new NewFrameEventHandler(kameram_NewFrame);  
            kameram.Start();
        }

        private void kameram_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone(); 
            Bitmap image2 = (Bitmap)eventArgs.Frame.Clone();
            Mirror ayna = new Mirror(false,true);
            ayna.ApplyInPlace(image1);

            
           
                EuclideanColorFiltering oklid = new EuclideanColorFiltering();
                oklid.CenterColor = new RGB(Color.FromArgb(R, G, B)); 
                oklid.Radius = 100; 
                oklid.ApplyInPlace(image1);
                BitmapData objectsData = image1.LockBits(new Rectangle(0, 0, image1.Width, image1.Height), ImageLockMode.ReadOnly, image1.PixelFormat);
                Grayscale grifiltresi = new Grayscale(0.2125, 0.7154, 0.0721);
                UnmanagedImage grayImage = grifiltresi.Apply(new UnmanagedImage(objectsData));
                image1.UnlockBits(objectsData);

                BlobCounter parcalar = new BlobCounter(); 
                parcalar.MinHeight = 25; 
                parcalar.MinWidth = 25; 
                parcalar.FilterBlobs = true; 
                parcalar.ObjectsOrder = ObjectsOrder.Size;
                parcalar.ProcessImage(grayImage); 
                Rectangle[] rects = parcalar.GetObjectsRectangles(); 
                pictureBox2.Image = image2;
                foreach (Rectangle recs in rects)
                {
                Graphics g = Graphics.FromImage(image1);

                if (rects.Length > 0) 
                    {
                        Rectangle objectRect = rects[0]; 
                    gen = objectRect.X + (objectRect.Width / 2); 
                    yuk = objectRect.Y + (objectRect.Height / 2);
                    if (gen < 210 && yuk < 160)
                    {
                        serialPort1.Write("1");
                    }
                    if ((210 < gen && gen < 420) && yuk < 160)

                    {
                        serialPort1.Write("2");
                    }
                    if (420 < gen && gen < 630 && yuk < 160)
                    {
                        serialPort1.Write("3");
                    }
                    if ((gen < 210 && (160 < yuk && yuk < 320)))
                    {
                        serialPort1.Write("4");
                    }
                    if ((210 < gen && gen < 420) && (160 < yuk && yuk < 320))
                    {
                        serialPort1.Write("5");
                    }
                    if ((420 < gen && gen < 630) && (yuk < 320 && yuk > 160))
                    {
                        serialPort1.Write("6");
                    }
                    if (gen < 210 && (320 < yuk && yuk < 480))
                    {
                        serialPort1.Write("7");
                    }
                    if ((210 < gen && gen < 420) && (320 < yuk && yuk < 480))
                    {
                        serialPort1.Write("8");
                    }
                    if ((420 < gen && gen < 630) && (320 < yuk && yuk < 480))
                    {
                        serialPort1.Write("9");
                    }

                    using (Pen pen = new Pen(Color.FromArgb(10, 200, 10), 2))
                    {

                        g.DrawRectangle(pen, objectRect);


                    }

                    g.DrawString("x Kor: "+gen.ToString() + "Y Kor: "+yuk.ToString(), new Font("Arial", 12), Brushes.Red, new System.Drawing.Point(gen, yuk));

                    g.Dispose();



                    }


                    
                }
            pictureBox1.Image = image1;





        }

      
    }
}
