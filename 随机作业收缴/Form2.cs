using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 随机作业收缴
{
    public partial class formSet : Form
    {
        public static int pianchazhi = 2;
        public static int geshu = 3;

        public formSet()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pianchazhi = (int)numericUpDown1.Value;
            geshu = (int)numericUpDown2.Value;
            saveToConfig();
            this.Close();
        }

        private void saveToConfig()
        {

            byte[] data = System.Text.Encoding.Default.GetBytes(pianchazhi.ToString() + " " + geshu.ToString() + " ");
            formMain.fsConfig.Seek(0, System.IO.SeekOrigin.Begin);
            formMain.fsConfig.Write(data, 0, data.Length);
            formMain.fsConfig.Flush();
        }

        private void formSet_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = pianchazhi;
            numericUpDown2.Value = geshu;
        }

    }
}
