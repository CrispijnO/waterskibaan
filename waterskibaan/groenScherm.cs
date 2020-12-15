using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace groenschermfrom
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int timeLeft = tijd.get_time_left();
            ///panel1.Visible = !panel1.Visible;
            string firstName = "John";
            string lastName = "de Boer";
            nameOutput.Text = "Welkom " + firstName + " " + lastName;
            labelTime.Text = "time left: " + timeLeft.ToString() + " min";
        }
    }
}
