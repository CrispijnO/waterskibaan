﻿
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
        DateTime startTime;
        DateTime endTime;

        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ///panel1.Visible = !panel1.Visible;
            string firstName = "John";
            string lastName = "de Boer";
            nameOutput.Text = "Welkom " + firstName + " " + lastName;
            nameOutput.BackColor = Color.Red;
            this.startTime = DateTime.Now;
            this.endTime = startTime.AddHours(1);
            labelTime.Text = "van " + startTime.Hour + ":" + startTime.Minute + " tot " + endTime.Hour + ":" + endTime.Minute;
        }

        private void pictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void labelTime_Click(object sender, EventArgs e)
        {

        }
    }
}
