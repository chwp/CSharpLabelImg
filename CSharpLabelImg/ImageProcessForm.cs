using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpLabelImg
{
    public partial class ImageProcessForm : Form
    {
        public ImageProcessForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] urls = Directory.GetFiles(this.textBox2.Text);
            string path = this.textBox3.Text;

            int i = Convert.ToInt32(this.textBox1.Text.Trim());

            foreach (var url in urls)
            {
                if (Path.GetExtension(url).ToLower() != ".jpg")
                {
                    Bitmap bitmap = new Bitmap(Image.FromFile(url));
                    bitmap.Save(path + "\\" + i.ToString().PadLeft(6, '0') + ".jpg", ImageFormat.Jpeg);
                }

                i++;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            string[] urls = Directory.GetFiles(this.textBox2.Text);
            string path = this.textBox3.Text;

            int i = Convert.ToInt32(this.textBox1.Text.Trim());

            foreach (var file in urls)
            {
                File.Move(file, path + "\\" + i.ToString().PadLeft(6, '0') + ".jpg");
                i++;
            }
            
        }
    }

}
