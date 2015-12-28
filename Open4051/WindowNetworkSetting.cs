using System;
using Gtk;
using System.IO;

namespace Open4051
{
    public delegate void ApplyButtonClicked(string[] IPtoSet);
    public partial class WindowNetworkSetting : Gtk.Window
    {
        //Pango.FontDescription fontsmall = Pango.FontDescription.FromString("Arial 10");
        Pango.FontDescription fontmid = Pango.FontDescription.FromString("Arial 14");
        Pango.FontDescription fontlarge = Pango.FontDescription.FromString("Arial 26");

        private string[] IP = new string[4];
        private string[] address = new string[4];


        public event ApplyButtonClicked ChangeIP;

        public WindowNetworkSetting()
            : base(Gtk.WindowType.Toplevel)
        {
            this.Build();



            MainWindow.SetFont(labelSetup, fontlarge);
            MainWindow.SetFont(label15, fontmid);
            MainWindow.SetFont(label16, fontmid);
            MainWindow.SetFont(label17, fontmid);
            MainWindow.SetFont(label18, fontmid);
            MainWindow.SetFont(labelPriority, fontmid);
            MainWindow.SetFont(labelGroup, fontmid);
            MainWindow.SetFont(btnBackbone, fontmid);
            MainWindow.SetFont(btnCustom, fontmid);
            MainWindow.SetFont(btnSave, fontmid);
            MainWindow.SetFont(btnCancel, fontmid);
            MainWindow.SetFont(btnClear, fontmid);
            MainWindow.SetFont(btnDefault, fontmid);
            MainWindow.SetFont(entryGateway, fontmid);
            MainWindow.SetFont(entryGroup, fontmid);
            MainWindow.SetFont(entryAddress, fontmid);
            MainWindow.SetFont(entryMask, fontmid);
            MainWindow.SetFont(entryPriority, fontmid);
            MainWindow.SetFont(entryRemote, fontmid);

            //Default ip address from file
            IP = MainWindow.GetIpAddress();
            address = IP[0].Split('.');
           
            SetUI();

            entryGroup.Changed += OnEntryGroupChanged;
            entryPriority.Changed += OnEntryPriorityChanged;


			
        }

        private void SetUI()
        {
            address = IP[0].Split('.');

            //Backbone IP address
            if (IP[0].Substring(0, 7).Equals("172.16."))
            {
                btnBackbone.Sensitive = false;
                btnCustom.Sensitive = true;
                btnClear.Sensitive = false;

                labelPriority.Sensitive = true;
                labelGroup.Sensitive = true;
                entryPriority.Sensitive = true;
                entryGroup.Sensitive = true;

                entryAddress.Sensitive = false;
                entryMask.Sensitive = false;
                entryGateway.Sensitive = false;
                entryRemote.Sensitive = false;

                entryPriority.Text = (int.Parse(address[3]) - 200).ToString();
                entryGroup.Text = address[2];
            }
            else
            {
                btnBackbone.Sensitive = true;
                btnCustom.Sensitive = false;
                btnClear.Sensitive = true;

                labelPriority.Sensitive = false;
                labelGroup.Sensitive = false;
                entryPriority.Sensitive = false;
                entryGroup.Sensitive = false;

                entryAddress.Sensitive = true;
                entryMask.Sensitive = true;
                entryGateway.Sensitive = true;
                entryRemote.Sensitive = true;
            }

            entryAddress.Text = IP[0];
            entryMask.Text = IP[1];
            entryGateway.Text = IP[2];
            entryRemote.Text = IP[3];
        }



        private void BackboneAddress()
        {
            IP[0] = "172.16." + entryGroup.Text + "." + (int.Parse(entryPriority.Text) + 200).ToString();
            IP[1] = "255.255.0.0";
            IP[2] = "172.16.1.2";
            IP[3] = "172.16.1.2";

            SetUI();
        }

        private bool CheckIntNumber(string input)
        {
            int tmp;
            return int.TryParse(input, out tmp);
        }

        private bool CheckIntNumber(string input, int limit)
        {
            int tmp;
            return int.TryParse(input, out tmp) && tmp >= 0 && tmp <= limit;
        }

        private string GetIPFromEntry(Entry input)
        {
            System.Net.IPAddress tmp;
            if (System.Net.IPAddress.TryParse(input.Text, out tmp))
                return input.Text;
            else
                return "Invalid";
        }

        protected void OnBtnBackboneClicked(object sender, EventArgs e)
        {
            BackboneAddress();
        }

        protected void OnBtnCustomClicked(object sender, EventArgs e)
        {
            btnBackbone.Sensitive = true;
            btnCustom.Sensitive = false;
            btnClear.Sensitive = true;

            labelPriority.Sensitive = false;
            labelGroup.Sensitive = false;
            entryPriority.Sensitive = false;
            entryGroup.Sensitive = false;

            entryAddress.Sensitive = true;
            entryMask.Sensitive = true;
            entryGateway.Sensitive = true;
            entryRemote.Sensitive = true;
        }

        protected void OnEntryGroupChanged(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;

            if (entry.Text == "")
                return;

            if (!CheckIntNumber(entry.Text, 99))
            {
                SetUI();
            }
            else
                BackboneAddress();
        }

        protected void OnEntryPriorityChanged(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;

            if (entry.Text == "")
                return;

            if (!CheckIntNumber(entry.Text, 10))
            {
                SetUI();
            }
            else
                BackboneAddress();
        }

        protected void OnBtnCancelClicked(object sender, EventArgs e)
        {
            this.Destroy();
        }

        protected void OnBtnDefaultClicked(object sender, EventArgs e)
        {
            IP[0] = "172.16.1.200";
            IP[1] = "255.255.0.0";
            IP[2] = "172.16.1.2";
            IP[3] = "172.16.1.2";

            SetUI();
        }

        protected void OnBtnClearClicked(object sender, EventArgs e)
        {
            entryAddress.Text = "";
            entryGateway.Text = "";
            entryMask.Text = "";
            entryRemote.Text = "";
        }

        protected void OnBtnSaveClicked(object sender, EventArgs e)
        {
            IP[0] = GetIPFromEntry(entryAddress);
            IP[1] = GetIPFromEntry(entryMask);
            IP[2] = GetIPFromEntry(entryGateway);
            IP[3] = GetIPFromEntry(entryRemote);
            ChangeIP(IP);
            this.Destroy();

        }

    }
}

