using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Windows.Forms.DataVisualization.Charting;

namespace 随机作业收缴
{
    public partial class formMain : Form
    {
        public static FileStream fsConfig = new FileStream(@"./config.txt", FileMode.OpenOrCreate);
        public static FileStream fsRecord = new FileStream(@"./record.txt", FileMode.OpenOrCreate);

        int[] times = new int[10];

        public formMain()
        {
            InitializeComponent();

            listView1.Columns.Add("尾号", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("时间", 114, HorizontalAlignment.Center);

            chart1.ChartAreas[0].AxisY.Maximum = 10;
            for (int i = 0; i < 10; i++)
            {
                times[i] = 0;
            }
            updateChart();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            byte[] readbyte = new byte[1000];
            char[] content = new char[1000];
            Decoder d = Encoding.Default.GetDecoder();
            //读取配置文件
            fsConfig.Seek(0, SeekOrigin.Begin);
            fsConfig.Read(readbyte, 0, 100);

            d.GetChars(readbyte, 0, readbyte.Length, content, 0);
            if (content[0] != '\0')
            {
                formSet.pianchazhi = content[0] - 48;
                formSet.geshu = content[2] - 48;
            }
            //读取记录文件
            readbyte = new byte[1000];
            content = new char[1000];
            fsRecord.Seek(0, SeekOrigin.Begin);
            fsRecord.Read(readbyte, 0, 900);

            d.GetChars(readbyte, 0, readbyte.Length, content, 0);
            int n;
            for (int i = 0; i < content.Length; i = n + 1)
            {
                //计算本次读取尾
                for (n = i; n < content.Length; n++)
                    if (content[n] == '\n')
                        break;
                //判断是否到文件尾
                if (content[i] == '\0')
                {
                    break;
                }
                //将读取的数据插入listview
                listView1.BeginUpdate();
                ListViewItem lvi = new ListViewItem();
                //记录随机数出现次数
                for (int k = 0; content[i + k] != ' '; k++)
                {
                    if (content[i + k] - 48 < 0)
                        break;
                    times[content[i + k] - 48]++;
                }
                //将随机数插入listview
                int j;
                lvi.Text = "";
                for (j = i; content[j] != ' '; j++)
                {
                    lvi.Text += " " + content[j];
                }
                //将日期插入listview
                string t = "";
                for (j++; j < n - 1; j++)
                {
                    t += content[j];
                }
                lvi.SubItems.Add(t);
                listView1.Items.Add(lvi);
                listView1.EndUpdate();
            }
            updateChart();
            
        }

        private void updateChart()
        {
            Series n = new Series("出现次数");
            chart1.Series.Clear();
            n.IsVisibleInLegend = false;
            n.ChartType = SeriesChartType.Column;
            for (int j = 0; j < 10; j++)
            {
                n.Points.AddXY("尾号" + j, times[j]);
            }
            chart1.Series.Add(n);
        }

        //偏差值计算
        private int[] pianchaCalculator()
        {
            int max = times[0];
            int min = times[0];
            int[] m = new int[10];
            for (int i = 0; i < 10; i++)
                m[i] = 0;
            for (int i = 0; i < times.Length; i++)
            {
                max = max < times[i] ? times[i] : max;
                min = min > times[i] ? times[i] : min;
            }
            int j = 0;
            for (int i = 0; i < times.Length; i++)
            {
                if (times[i] == max)
                {
                    j++;
                    m[i] = 1;
                }
            }
            Random rd = new Random();
            if (10 - j < formSet.geshu)
                for (int i = 0; i < formSet.geshu - (10 - j); i++)
                {
                    int n = rd.Next(10);
                    while (m[n] != 1)
                        n = rd.Next(10);
                    m[n] = 0;
                }
            if (max - min >= formSet.pianchazhi)
                return m;
            else return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int[] temp = pianchaCalculator();

            Random rd = new Random();
            textBox1.Clear();
            int[] r = new int[10];
            for (int i = 0; i < 10; i++)
                r[i] = 0;
            for (int i = 0; i < formSet.geshu; i++)
            {
                int num = rd.Next(10);
                if (temp != null)
                    while (r[num] == 1 || temp[num] == 1)
                        num = rd.Next(10);
                else while (r[num] == 1)
                        num = rd.Next(10);
                textBox1.Text += "  " + num.ToString();
                r[num] = 1;
                times[num]++;
            }
            recordNum(r);
            updateChart();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form setForm = new formSet();
            setForm.ShowDialog();
        }

        private void recordNum(int[] num)
        {
            listView1.BeginUpdate();
            ListViewItem lvi = new ListViewItem();
            string t = "";
            for (int i = 0; i < 10; i++)
            {
                if (num[i] == 1)
                {
                    lvi.Text += " " + i.ToString();
                    t += i.ToString();
                }
            }
            lvi.SubItems.Add(DateTime.Now.ToLongDateString().ToString());
            listView1.Items.Add(lvi);
            listView1.EndUpdate();

            byte[] data = System.Text.Encoding.Default.GetBytes(t + " " + DateTime.Now.ToLongDateString().ToString() + "\r\n");
            formMain.fsRecord.Seek(0, System.IO.SeekOrigin.End);
            formMain.fsRecord.Write(data, 0, data.Length);
            formMain.fsRecord.Flush();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            fsConfig.Close();
            fsRecord.Close();
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form setForm = new formSet();
            setForm.ShowDialog();
        }
    }
}
