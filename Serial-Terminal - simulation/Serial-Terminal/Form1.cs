using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace Serial_Terminal
{
    public partial class Form1 : Form
    {
        SerialPort Com;
        byte[] SerBuf = new byte[10];

        string textFile =  "C:\\Users\\Admin\\Desktop\\solar\\Voltronic protocol\\sent.txt";
        string[] lines;
        int lines_no;
        int current_line_no;
        
        public Form1()
        {
            InitializeComponent();

            listBox1.Items.Clear();

            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length == 0)
            {
                MessageBox.Show("No Communecation port was detected!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Thread.CurrentThread.Abort();
                this.Close();
            }

            foreach (string port in ports)
            {
                listBox1.Items.Add(port);
            }

        }

        void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string portNo = (string)listBox1.GetItemText(listBox1.SelectedItem);
            if (portNo == "")
            {
                MessageBox.Show("No Communecation port was Selected!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }

            listBox1.Visible = false;

            Com = new SerialPort(portNo);

            Com.Open();

            Com.BaudRate = 2400; //115200;
            Com.DataBits = 8;
            Com.DataReceived += Com_DataReceived;

            Com.Disposed += Com_Disposed;
        }

        void Com_Disposed(object sender, EventArgs e)
        {
            MessageBox.Show("Serial Port has been terminated!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(-1);
        }

        //bool Query_being_recived = false;
        void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int size = Com.BytesToRead;
            byte[] X = new byte[size];
            Com.Read(X, 0, size);
            String STR = "";
            
            string STR2 = System.Text.Encoding.ASCII.GetString(X);
            STR2 = STR2.Replace(' ', '\n');
          //  string[] STRs = STR2.Split(' ');
            CrossThreadShow2(STR2, true);

            for (int i = 0; i < size; i++)
            {
                STR += X[i].ToString("X")+" ";
                if (X[i] == 0xd) STR += Environment.NewLine;
            }
            CrossThreadShow(STR, true);

            Send_Qeury(textBox1.Text);
            //send_string(textBox1.Text);
        }

       void Scroll_to_careat1()
        {
            // set the current caret position to the end
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            // scroll it automatically
            richTextBox1.ScrollToCaret();
        }

       void Scroll_to_careat2()
       {
           // set the current caret position to the end
           richTextBox2.SelectionStart = richTextBox2.Text.Length;
           // scroll it automatically
           richTextBox2.ScrollToCaret();
       }
        void CrossThreadShow(String str, bool Add)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                { 
                    if (Add) richTextBox1.Text += str;
                    else richTextBox1.Text = str;
                    richTextBox1.SelectionStart
                        = richTextBox1.Text.Length;
                    Scroll_to_careat1();
                }));
            }
            else
            {
                if (Add) richTextBox1.Text += str;
                else richTextBox1.Text = str;
                richTextBox1.SelectionStart
                    = richTextBox1.Text.Length;
                Scroll_to_careat1();
            }
        }


        void CrossThreadShow2(String str, bool Add)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    if (Add) richTextBox2.Text += str;
                    else richTextBox2.Text = str;
                    richTextBox2.SelectionStart
                        = richTextBox2.Text.Length;
                    Scroll_to_careat2();
                }));
            }
            else
            {
                if (Add) richTextBox2.Text += str;
                else richTextBox2.Text = str;
                richTextBox2.SelectionStart
                    = richTextBox2.Text.Length;
                Scroll_to_careat2();
            }
        }
        
        void Send_Qeury(string str)
        {
            string s = str + "\r";   //\r (0x0d) End of input character
           
            char[] characters = s.ToCharArray();
            Byte[] encodedBytes = System.Text.Encoding.ASCII.GetBytes(s);
            if(checkBox1.Checked == true)Com.Write(encodedBytes, 0, encodedBytes.Length);
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            richTextBox2.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //QPIRI<cr>: Device Rating Information inquiry
            Send_Qeury("QPIRI");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //QVFW2<cr> :Another CPU Firmware version inquiry
            Send_Qeury("QVFW2");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /*
             Device Rating Information inquiry
            51 50 49 52 49 F8 54 D
            QPIRI?T
            (220.0 18.1 220.0 50.0 18.1 4000 4000 24.0 23.0 21.0 28.2 27.0 0 030 030 0 1 2 1 01 0 0 27.0 0 1?
             */
            SerBuf[0] = 0x51;
            SerBuf[1] = 0x50;
            SerBuf[2] = 0x49;
            SerBuf[3] = 0x52;
            SerBuf[4] = 0x49;
            SerBuf[5] = 0xf8;
            SerBuf[6] = 0x54;
            SerBuf[7] = 0xd;

            Com.Write(SerBuf, 0, 8);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            /*
             51 4D 4E BB 64 D
            QMN?d
            (VMIII-4000??
             */
            SerBuf[0] = 0x51;
            SerBuf[1] = 0x4d;
            SerBuf[2] = 0x4e;
            SerBuf[3] = 0xbb;
            SerBuf[4] = 0x64;
            SerBuf[5] = 0xd;         
            Com.Write(SerBuf, 0, 6);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Send_Qeury("QPIGS");//Device general status parameters inquiry
        }

        private void button7_Click(object sender, EventArgs e)
        {
            /*
             Device general status parameters inquiry
             51 50 49 47 53 B7 A9 D
             QPIGS??
             (202.3 48.9 202.3 48.9 0809 0726 020 425 28.20 015 095 0038 06.1 142.2 00.00 00000 00010110 00 00 00839 011 0 00 0000}?
             */
            SerBuf[0] = 0x51;
            SerBuf[1] = 0x50;
            SerBuf[2] = 0x49;
            SerBuf[3] = 0x47;
            SerBuf[4] = 0x53;
            SerBuf[5] = 0xb7;
            SerBuf[6] = 0xa9;
            SerBuf[7] = 0x0d;

            Com.Write(SerBuf, 0, 8);
        }

      

        void send_line(string str)
        {
            string STR = str;
            string[] ARRAY = STR.Split(' ');
            byte[] ary = new byte[ARRAY.Length];
            for (int i = 0; i < ARRAY.Length; i++)
            {
                ary[i] = Convert.ToByte(ARRAY[i], 16);

            }
            Com.Write(ary, 0, ary.Length);

            string STR2 = System.Text.Encoding.ASCII.GetString(ary);

            CrossThreadShow2(Environment.NewLine + STR + Environment.NewLine + STR2, true);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            string STR = textBox1.Text;
            send_line(STR);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Read a text file line by line.
            lines = File.ReadAllLines(textFile);
            lines_no = lines.Length;
            current_line_no = 0;

             
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string str = lines[current_line_no];
            str = str.Substring(0, str.Length-1);
            send_line(str);
            current_line_no++;
            progressBar1.Value = (int)((100*current_line_no) / lines_no);
            label1.Text = progressBar1.Value.ToString();
        }

    }
}
/*
QPI<cr>: Device Protocol ID Inquiry
51 50 49 B6 AC D
QPI??
(PI30?
 
51 47 4D 4E 49 29 D
QGMNI)
(055??
 
51 4D 4E BB 64 D
QMN?d
(VMIII-4000??

Device Rating Information inquiry
51 50 49 52 49 F8 54 D
QPIRI?T
(220.0 18.1 220.0 50.0 18.1 4000 4000 24.0 23.0 2

51 4D 4E BB 64 D
QMN?d
1.0 28.2 27.0 0 030 030 0 1 2 1 01 0 0 27.0 0 1?

The device ID inquiry
51 53 49 44 BB 5 D
QSID?
(1496322206105690005535~K

51 44 4F 50 85 E5 D
QDOP??
(1 3 2 0 00.0 21.0 015 40 010 030 000 000 00 23k?
 
Main CPU Firmware version inquiry
51 56 46 57 62 99 D
QVFWb?
(VERFW:00056.02d
 
Device general status parameters inquiry
51 50 49 47 53 B7 A9 D
QPIGS??
(202.3 48.9 202.3 48.9 0809 0726 020 425 28.20 015 095 0038 06.1 142.2 00.00 00000 00010110 00 00 00839 011 0 00 0000}?

51 42 45 51 49 2E A9 D
QBEQI.?
(0 060 030 030 030 29.20 000 120 0 0000VX

Device Rating Information inquiry
51 50 49 52 49 F8 54 D
QPIRI?T
(220.0 18.1 220.0 50.0 18.1 4000 4000 24.0 23.0 21.0 28.2 27.0 0 030 030 0 1 2 1 01 0 0 27.0 0 1?
 
51 46 4C 41 47 98 74 D
QFLAG?t
(EbkuvDadjxyz\

The default setting value information
51 44 49 71 1B D
QDIq
(230.0 50.0 0030 21.0 27.0 28.2 23.0 60 0 0 2 0 0 0 0 0 1 1 1 0 1 0 27.0 0 1)F

Device Warning Status inquiry
51 50 49 57 53 B4 DA D
QPIWS??
(000000000000000001000000000000000000??

 
*/