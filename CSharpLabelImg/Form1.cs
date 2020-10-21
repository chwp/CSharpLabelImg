using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CSharpLabelImg
{
    public partial class Form1 : Form
    {
        private Point p1, p2;//定义两个点（启点，终点）
        private static bool drawing = false;//设置一个启动标志
        private List<Tuple<Point, Point>> allRects;

        private string dir;
        private string fileName;

        public Bitmap myBitmap = null;
        
        public Form1()
        {
            InitializeComponent();
            allRects = new List<Tuple<Point, Point>>();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {

        }

        private void btnOpenDir_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择样本所在文件夹";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }

                dir = dialog.SelectedPath;

                DirectoryInfo root = new DirectoryInfo(dialog.SelectedPath);
                FileInfo[] files = root.GetFiles();

                foreach (var file in files)
                {
                    this.listBox1.Items.Add(file.Name);
                }

            }
            
        }

        private void btnNextImage_Click(object sender, EventArgs e)
        {

        }

        private void btnPrevImage_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(Path.GetFileNameWithoutExtension(this.fileName) + ".txt", FileMode.Create);
            //获得字节数组
            
            string width = this.myBitmap.Width.ToString();
            string height = this.myBitmap.Height.ToString();

            double wid = (double)(this.myBitmap.Width);
            double hei = (double)(this.myBitmap.Height);

            var xmldoc = new XmlDocument();
            //加入XML的声明段落,<?xml version="1.0" encoding="gb2312"?>
            //XmlDeclaration xmldecl;
            //xmldecl = xmldoc.CreateXmlDeclaration("1.0", "gb2312", null);
            //xmldoc.AppendChild(xmldecl);

            //加入一个根元素
            var xmlelem = xmldoc.CreateElement("", "annotation", "");
            xmldoc.AppendChild(xmlelem);
            
            XmlNode root = xmldoc.SelectSingleNode("annotation");//查找<annotation>

            XmlElement folder = xmldoc.CreateElement("folder");
            folder.InnerText = "AircraftHatch";

            root.AppendChild(folder);

            XmlElement filename = xmldoc.CreateElement("filename");
            filename.InnerText = this.fileName;
            root.AppendChild(filename);
            
            XmlNode size = xmldoc.CreateElement("size");
            
            XmlElement xewidth = xmldoc.CreateElement("width");
            XmlElement xeheight = xmldoc.CreateElement("height");
            XmlElement xedepth = xmldoc.CreateElement("depth");
            xewidth.InnerText = width;
            xeheight.InnerText = height;
            xedepth.InnerText = "3";

            size.AppendChild(xewidth);
            size.AppendChild(xeheight);
            size.AppendChild(xedepth);

            root.AppendChild(size);

            XmlNode source = xmldoc.CreateElement("source");
            XmlElement xedatabase = xmldoc.CreateElement("database");
            xedatabase.InnerText = "HatchDoorDB";
            XmlElement xeannotation = xmldoc.CreateElement("annotation");
            xeannotation.InnerText = "HatchDoor VOC";
            XmlElement xeimage = xmldoc.CreateElement("image");
            xeimage.InnerText = "flickr";
            XmlElement xeflickrid = xmldoc.CreateElement("flickrid");
            xeflickrid.InnerText = Path.GetFileNameWithoutExtension(this.fileName);

            source.AppendChild(xedatabase);
            source.AppendChild(xeannotation);
            source.AppendChild(xeimage);
            source.AppendChild(xeflickrid);

            root.AppendChild(source);
            
            XmlElement xesegmented = xmldoc.CreateElement("segmented");
            xesegmented.InnerText = "0";
            root.AppendChild(xesegmented);

            foreach (var rect in allRects)
            {
                XmlNode objectNode = xmldoc.CreateElement("object");
                XmlElement xeobjectname = xmldoc.CreateElement("name");
                xeobjectname.InnerText = "aircrafthatch";

                objectNode.AppendChild(xeobjectname);

                XmlElement xeobjectpose = xmldoc.CreateElement("pose");
                xeobjectpose.InnerText = "Unspecified";
                objectNode.AppendChild(xeobjectpose);

                XmlElement xeobjecttruncated = xmldoc.CreateElement("truncated");
                xeobjecttruncated.InnerText = "0";

                objectNode.AppendChild(xeobjecttruncated);

                XmlElement xeobjectdifficult = xmldoc.CreateElement("difficult");
                xeobjectdifficult.InnerText = "0";

                objectNode.AppendChild(xeobjectdifficult);

                XmlNode bndboxNode = xmldoc.CreateElement("bndbox");

                XmlElement xexmin = xmldoc.CreateElement("xmin");
                XmlElement xeymin = xmldoc.CreateElement("ymin");
                XmlElement xexmax = xmldoc.CreateElement("xmax");
                XmlElement xeymax = xmldoc.CreateElement("ymax");

                xexmin.InnerText = rect.Item1.X.ToString();
                xeymin.InnerText = rect.Item1.Y.ToString();
                xexmax.InnerText = rect.Item2.X.ToString();
                xeymax.InnerText = rect.Item2.Y.ToString();

                bndboxNode.AppendChild(xexmin);
                bndboxNode.AppendChild(xeymin);
                bndboxNode.AppendChild(xexmax);
                bndboxNode.AppendChild(xeymax);

                objectNode.AppendChild(bndboxNode);

                root.AppendChild(objectNode);

              
                byte[] data = System.Text.Encoding.Default.GetBytes(Convert(0, wid,hei, rect.Item1.X, rect.Item1.Y, rect.Item2.X, rect.Item2.Y));

                //开始写入
                fs.Write(data, 0, data.Length);
                //清空缓冲区、关闭流
               
            }
            
            //保存创建好的XML文档
            xmldoc.Save(Path.GetFileNameWithoutExtension(this.fileName) + ".xml");


            //生成txt文件
            fs.Flush();
            fs.Close();

            allRects.Clear();


        }

        private string Convert(int classid, double width, double height, int x1, int y1, int x2, int y2)
        {
            double dw = 1.0 / width;
            double dh = 1.0 / height;

            double x = (x1 + x2) / 2.0 - 1;
            double y = (y1 + y2) / 2.0 - 1;
            double w = x2 - x1;
            double h = y2 - y1;

            x = x * dw;
            w = w * dw;
            y = y * dh;
            h = h * dh;

            x = Math.Round(x, 6);
            w = Math.Round(w, 6);
            y = Math.Round(y, 6);
            h = Math.Round(h, 6);

            return string.Format("{0} {1} {2} {3} {4}\n",classid, x, y, w, h);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            p1 = new Point(e.X, e.Y);
           
            drawing = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Graphics g = pictureBox1.CreateGraphics();

            if (drawing)
            {

                Point currentPoint = new Point(e.X, e.Y);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;//消除锯齿

                p2.X = currentPoint.X;
                p2.Y = currentPoint.Y;

                g.DrawRectangle(new Pen(Color.Blue, 2), new Rectangle { X = p1.X, Y = p1.Y, Width = p2.X - p1.X, Height = p2.Y - p1.Y });
                //pictureBox1.Invalidate();
            }

            p2 = new Point(e.X, e.Y);

            allRects.Add(new Tuple<Point, Point>(p1, p2));

            drawing = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.fileName = this.listBox1.SelectedItem.ToString();

            if (!string.IsNullOrWhiteSpace(this.fileName))
            {
                pictureBox1.Image = Image.FromFile(Path.Combine(dir, this.fileName));
                this.myBitmap = new Bitmap(Path.Combine(dir, this.fileName));
            }
            
        }

       
    }
}
