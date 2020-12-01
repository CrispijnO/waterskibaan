using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpringCard.PCSC;
using SpringCard.LibCs;
using SpringCard.PCSC.CardLibraries;

namespace waterskibaan
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getReaders();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            scanScherm.Visible = !scanScherm.Visible;
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            ///panel1.Visible = !panel1.Visible;
            string firstName = "John";
            string lastName = "de Boer";
            nameOutput.Text = "Welkom " + firstName + " " + lastName;
        }

        private void getReaders()
        {
            try
            {
                string[] readers = SCARD.Readers;
                foreach (string reader in readers)
                {
                    textBox1.AppendText(reader + " yes");
                }
            }
            catch (Exception)
            {
                textBox1.AppendText("geen readers");
            }

            ///SCARD.Connect();
        }
    }
}
