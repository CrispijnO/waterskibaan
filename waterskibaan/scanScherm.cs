using System;
using System.Timers;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpringCard.PCSC;
using waterskibaan;
using Newtonsoft.Json;

namespace groenschermfrom
{   

    public partial class scanScherm : Form
    {
        

        SCardReader cardReader;
        SCardChannel channel;

        private bracelet braceletCode = new bracelet();
    
        public scanScherm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getReaders();
            GroenPanel.Visible = false;
            roodPanel.Visible = false;
            panel1.Visible = true;
            //SetTimer();

            /*myTimer.Stop();
            myTimer.Dispose();*/
        }
        

        private void IntervalClick()
        {
            Console.WriteLine("the timer is active.");
            int timeLeft = tijd.get_time_left();
            if (errorCheck.Checked == true)
            {
                if (roodPanel.Visible == false)
                {
                    roodPanel.Visible = true;
                }
                else if (roodPanel.Visible == true)
                {
                    roodPanel.Visible = false;
                }
            }
            else
            {
                if (GroenPanel.Visible == false)
                {
                    GroenPanel.Visible = true;
                }
                else if (GroenPanel.Visible == true)
                {
                    GroenPanel.Visible = false;
                }
            };


            /*braceletCode.braceletCode =  0;
            string name = GetUser.FirstName + " " + GetUser.LastName + "\n" + dates.bookingStart + " tot " + dates.bookingEnd;
            nameOutput.Text = "Welkom " + GetUser.FirstName + " " + GetUser.LastName;
            labelTime.Text = "time left: " + timeLeft.ToString() + " min";
            textBox1.Text = name;
            RESTClient rClient = new RESTClient();
            rClient.endPoint = "https://demo.recras.nl/api2/contacten/9034/afbeelding";
            string response = rClient.makeRequest();
            Console.WriteLine(response);*/
        }

        
        private async void button1_Click(object sender, EventArgs e)
        {
            //SetTimer();
            //IntervalClick();
            /*IntervalClick();
            await Task.Delay(3000);
            IntervalClick();*/
            //testTHIS(8418);
            handleApiRequests(56);
        }

        public void getReaders()
        {

            try
            {
                string[] readers = SCARD.Readers;
                if (readers.Length == 0)
                {
                    textBox1.AppendText("Geen readers");
                }
                cardReader = new SCardReader(0, readers[0]);
                textBox1.AppendText(cardReader.StatusAsString);
                cardReader.StartMonitor(onCardChange);
                ///SCARD.Connect(SCARD.PCI_RAW(), readers[0], 0x00000003, 0x00000002, Handle,);
                ///SCARD.ListCards();
                ///SCARD.ListReaders(textBox1.Text);
            }
            catch (Exception)
            {
                textBox1.AppendText(SCARD.ErrorToMessage(0));
            }

            
        }

        delegate void onCardChangeInvoker(uint readerState, CardBuffer cardAtr);

        public void onCardChange(uint readerState, CardBuffer cardAtr)
        {
            if (cardAtr != null)
            {
                if (InvokeRequired)
                {
                    this.BeginInvoke(new onCardChangeInvoker(onCardChange), readerState, cardAtr);
                    return;
                }
                channel = new SCardChannel(cardReader);
                if (!channel.Connect())
                {
                    return;
                }
                CAPDU capdu = new CAPDU(0xff, 0xca, 0x00, 0x00);
                RAPDU rapdu = channel.Transmit(capdu);
                if (rapdu.SW != 0x9000)
                {
                    textBox1.Text = "Get UID APDU failed!";
                    return;
                }
                byte[] rapduB = rapdu.data.GetBytes();
                string hexadecimalResult = BitConverter.ToString(rapduB);
                textBox1.Text = hexadecimalResult;

                if (hexadecimalResult == "04-BA-B7-82-E0-60-80")
                {
                    //Image img = Image.FromFile(@"C:\Users\milan\source\repos\waterskibaan\waterskibaan\images\9A-91 wallpaper1.jpg");
                    //profileImage.Image = img;
                }

                switch (hexadecimalResult)
                {
                    case "04-BA-B7-82-E0-60-80":
                        braceletCode.braceletCode = 56;
                        // rood
                        break;
                    case "04-31-BF-5A-91-5B-80":
                        braceletCode.braceletCode = 8418;
                        // groen
                        break;
                    case "04-17-BD-5A-91-5B-80":
                        braceletCode.braceletCode = 8422;
                        // groen (tot 3 uur)
                        break;
                    default:
                       // textBox1.Text = "Unknown code";
                        Console.WriteLine(hexadecimalResult);
                        return;
                }
                /*string response = string.Empty;
                RESTClient rClient = new RESTClient();
                rClient.endPoint = "https://demo.recras.nl/api2/klanten/" + braceletCode.braceletCode;
                response = rClient.makeRequest();
                klanten responseKlant = JsonConvert.DeserializeObject<klanten>(response);
                rClient.endPoint = "https://demo.recras.nl/api2/boekingen?klant.id=" + braceletCode.braceletCode;
                response = rClient.makeRequest();
                boekingen responseBoeking = JsonConvert.DeserializeObject<boekingen>(response);*/

                handleApiRequests(braceletCode.braceletCode);
                channel.Disconnect();
                channel = null;
                cardReader.StopMonitor();

                ///SCARD.Connect(SCARD.PCI_RAW(), cardReader, 0x00000003, 0x00000002, Handle, ));
            }
        }
        private void handleApiRequests(int braceletcode)
        {
            string response = string.Empty;
            RESTClient rClient = new RESTClient();
            rClient.endPoint = $"https://demo.recras.nl/api2/klanten/{braceletcode}";
            response = rClient.makeRequest();
            klanten responseKlant = JsonConvert.DeserializeObject<klanten>(response);
            rClient.endPoint = $"https://demo.recras.nl/api2/boekingen?klant.id={braceletcode}&embed=boekingsregels";
            response = rClient.makeRequest();
            List<boekingen> responseBoeking = JsonConvert.DeserializeObject<List<boekingen>>(response);
            int index = 0;
            bool found = false;
            DateTime DateNow = DateTime.Now;
            responseBoeking.ForEach(async boeking =>
            {
                for (int i = 0; i < boeking.boekingsregels.Count - 1; i++)
                {
                    var boekingsregelW = boeking.boekingsregels[i];
                    
                    // begin tijd moet al geweest zijn en de eind tijd moet nog komen.
                    if (boekingsregelW.begin < DateNow && boekingsregelW.eind > DateNow)
                    {
                        index = i;
                        found = true;
                        break;
                    }
                }
                Console.WriteLine("index: " + index);
                DateTime eerstVolgendeBoeking = boeking.boekingsregels[0].begin;
                if (!found)
                {
                    Console.WriteLine("This is getting executed.");
                    for (int i = 0; i < boeking.boekingsregels.Count - 1; i++)
                    {
                        var boekingsregelW = boeking.boekingsregels[i];
                        // looping over elke boekingsregel en kijken welke boeking het eerst aan de beurt is.
                        // ook checken of het niet al geweest is.

                        Console.WriteLine("test.");
                        Console.WriteLine(boekingsregelW.begin + " - " + eerstVolgendeBoeking);
                        if (eerstVolgendeBoeking > boekingsregelW.begin && DateTime.Now < boekingsregelW.begin)
                        {
                            Console.WriteLine("this is getting set.");
                            eerstVolgendeBoeking = boekingsregelW.begin;
                            index = i;
                        }
                    }
                }
                var boekingsregel = boeking.boekingsregels[index];
                TimeSpan t = timeLeft(boekingsregel.begin, boekingsregel.eind);
                if (!found)
                // time for start is in eerstVolgendeBoeking en de index is in index.
                {
                    richTextBox2.Text = $"Hallo {responseKlant.displaynaam},\n\nU heeft op did moment geen boeking.\nUw eerst volgende boeking is: {eerstVolgendeBoeking}";
                    GroenPanel.Visible = true;
                    roodPanel.Visible = true;
                    profileImage.ImageLocation = "https://i.imgur.com/JNJGSs7.png";
                    await Task.Delay(5000);
                    roodPanel.Visible = false;
                    GroenPanel.Visible = false;
                    richTextBox2.Text = "";
                    profileImage.ImageLocation = "";
                } else
                {
                    nameOutput.Text = $"Hallo {responseKlant.displaynaam}!";
                    labelTime.Text = $"U heeft nog {formatTime(t)} tegaan!";
                    GroenPanel.Visible = true;
                    await Task.Delay(5000);
                    GroenPanel.Visible = false;
                    labelTime.Text = "";
                    nameOutput.Text = "";
                }
                string formattedTimeString = formatTime(t);
                Console.WriteLine(formattedTimeString);
            });
        }
        public TimeSpan timeLeft(DateTime start, DateTime end)
        {
            DateTime now = DateTime.Now;
            //Console.WriteLine("[NOW | END] " + now + " | " + end);
            TimeSpan result = end - now;
            return result;
        }

        public string formatTime(TimeSpan time)
        {
            string formattedTime = string.Empty;
            bool days = time.Days > 0;
            bool hours = time.Hours > 0;
            bool minutes = time.Minutes > 0;
            bool seconds = time.Seconds > 0;
            if (days)
            {
                if (time.Days == 1)
                {
                    formattedTime += string.Format("{0} dag, ", time.Days);
                }
                else
                {
                    formattedTime += string.Format("{0} dagen, ", time.Days);
                }
            }
            if (days || hours)
            {
                if (time.Hours == 1)
                {
                    formattedTime += string.Format("{0} uur, ", time.Hours);
                }
                else
                {
                    formattedTime += string.Format("{0} uren, ", time.Hours);
                }
            }
            if (days || hours || minutes)
            {
                if (time.Minutes == 1)
                {
                    formattedTime += string.Format("{0} minuut, ", time.Minutes);
                } else
                {
                    formattedTime += string.Format("{0} minuten, ", time.Minutes);
                }
            }
            if (days || hours || minutes || seconds)
            {
                if (time.Seconds == 1)
                {
                    formattedTime += string.Format("{0} seconde", time.Seconds);
                }
                else
                {
                    formattedTime += string.Format("{0} secondes", time.Seconds);
                }
            }
            return formattedTime;
        }
        private void errorButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void errorButton_CheckedChanged_1(object sender, EventArgs e)
        {

        }

    }
}
