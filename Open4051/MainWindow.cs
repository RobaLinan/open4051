using System;
using Gtk;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Diagnostics;


namespace Open4051
{
  public partial class MainWindow: Gtk.Window
  {
    string browser = @"";
    bool fullscreen = false;
    string interfaceName = @"";
    ToolBarWindow tbw = new ToolBarWindow();
    WindowNetworkSetting wns = new WindowNetworkSetting();

    Pango.FontDescription fontsmall = Pango.FontDescription.FromString("Arial 10");
    Pango.FontDescription fontmid = Pango.FontDescription.FromString("Arial 14");
    Pango.FontDescription fontlarge = Pango.FontDescription.FromString("Arial 26");

    Thread CheckCableThread;

    public MainWindow()
      : base(Gtk.WindowType.Toplevel)
    {
      Build();
      OnLoad(this, new EventArgs());
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
      CheckCableThread.Abort();
      Application.Quit();
      a.RetVal = true;
    }

    private void ReadConfig()
    {
      WriteLog("Reading application configuration.");
      StreamReader sr = new StreamReader("open4051.cfg");

      string line;
      while (!sr.EndOfStream)
        {
          line = sr.ReadLine();

          if (line.StartsWith("browser="))
            {
              browser = line.Substring(line.IndexOf("=") + 1);
            }

          if (line.StartsWith("fullscreen="))
            {
              fullscreen = bool.Parse(line.Substring(line.IndexOf("=") + 1));
            }

          if (line.StartsWith("interface="))
            {
              interfaceName = line.Substring(line.IndexOf("=") + 1);
            }
        }
      sr.Close();

      if (browser == "")
        {
          WriteLog("Please set browser in Open4051.cfg");
          btnContinue.Sensitive = false;
        } else
        WriteLog("Browser: " + browser);

      if (interfaceName == "")
        {
          WriteLog("Please set interface in Open4051.cfg");
          btnNetworkSetting.Sensitive = false;
        }
    }

    public static void SetFont(Label label, Pango.FontDescription font)
    {
      label.ModifyFont(font);
    }

    public static void SetFont(Entry tv, Pango.FontDescription font)
    {
      tv.ModifyFont(font);
    }

    public static void SetFont(Button btn, Pango.FontDescription font)
    {
      btn.Child.ModifyFont(font);
    }

    private void WriteLog(string log)
    {
      textviewLog.Buffer.Text += "> " + log + "\n";
    }

    public void SetIpAddress(string[] IPSetting)
    {
      WriteLog("Writing IP settings.");
      string IP = "";


      try
        {
          //Set ip for windows
          Process setipProcess = new Process();
          setipProcess.StartInfo.FileName = "setip_windows.bat ";
          setipProcess.StartInfo.Arguments = interfaceName + " " + IPSetting[0] + " " + IPSetting[1] + " " + IPSetting[2];
          setipProcess.StartInfo.CreateNoWindow = true;
          setipProcess.StartInfo.UseShellExecute = false;
          //string setipcmd = "setip_windows.bat " + interfaceName + " " + IPSetting[0] + " " + IPSetting[1] + " " + IPSetting[2];
          setipProcess.Start();
        } catch (Exception e)
        {
          WriteLog(e.ToString());
        }


      //Write to file IPSetting.txt
      foreach (string s in IPSetting)
        {
          IP += s + "\n";
        }
      try
        {
          StreamWriter sw = new StreamWriter("IPSetting.txt");
          sw.Write(IP);
          sw.Close();
        } catch (Exception e)
        {
          WriteLog(e.ToString());
        }

    }

    public static string[] GetIpAddress()
    {
      //WriteLog("Reading IP settings.");
      StreamReader sr = new StreamReader("IPSetting.txt");
      string[] lines = new string[4];
      int i = 0;
      while (!sr.EndOfStream)
        {
          lines[i] = sr.ReadLine();
          i++;
        }
      sr.Close();
      return lines;
    }

    private void LoadIPSetting()
    {
      //Show IP Setting
      string[] IPSetting = GetIpAddress();
      labelIP.Text = IPSetting[0];
      labelMask.Text = IPSetting[1];
      labelGateway.Text = IPSetting[2];
      labelRemote.Text = IPSetting[3];
    }

    private void CheckCablePlugged()
    {
      string imageFileName = null;
      bool? cablePlugged = false;
      bool? cableLastCycle = null;
      do
        {
          cablePlugged = NetworkInterface.GetIsNetworkAvailable();
          if (cableLastCycle != cablePlugged)
            {
              if ((bool)cablePlugged)
                {
                  labelConnection.Text = "YES";
                  imageFileName = "cabel_plugged.gif";
                  WriteLog("Ethernet cable connected.");
                  WriteLog("Executing network echo test:");
                  CheckRemoteConnection();
                } else
                {
                  labelConnection.Text = "NO";
                  imageFileName = "cabel_unplugged.gif";
                  WriteLog("Ethernet cable disconnected.");
                }
              var imageBuffer = File.ReadAllBytes(imageFileName);
              var pixbuf = new Gdk.Pixbuf(imageBuffer);
              imageConnected.Pixbuf = pixbuf;
            }
          cableLastCycle = cablePlugged;

          if (!wns.Activate())
            tbw.KbdPositoin = "middle";
          Thread.Sleep(2000);
        } while(true);
    }

    private void CheckRemoteConnection()
    {

      string log = null;
      Ping p = new Ping();
      PingReply pr = null;
      try
        {
          pr = p.Send(labelRemote.Text);

          if (pr.Status == IPStatus.Success)
            {

              log = "Echo reply from " + labelRemote.Text + " received!";
            } else
            {
              log = pr.Status.ToString();
            }


        } catch (Exception e)
        {
          log = "No reply from " + labelRemote.Text + " !";    

          //I don't want the complier to show a warning that e is not used.
          e.Equals("null");
        } finally
        {
          WriteLog(log);
        }


    }

    protected void OnLoad(object sender, EventArgs e)
    {
      WriteLog("Applicatoin started.");
      textviewLog.ModifyFont(fontsmall);

      //Set font for labels
      SetFont(labelMain, fontlarge);
      SetFont(label1, fontsmall);
      SetFont(label2, fontsmall);
      SetFont(label3, fontsmall);
      SetFont(label4, fontsmall);
      SetFont(label5, fontsmall);
      SetFont(label6, fontsmall);
      SetFont(labelIP, fontsmall);
      SetFont(labelMask, fontsmall);
      SetFont(labelGateway, fontsmall);
      SetFont(labelRemote, fontsmall);
      SetFont(labelConnection, fontsmall);
      SetFont(labelCalibrated, fontsmall);

      //Set font for buttons
      SetFont(btnNetworkSetting, fontmid);
      SetFont(btnContinue, fontmid);
      SetFont(btnCalibrate, fontmid);
      SetFont(btnNetworkTest, fontsmall);

      //Config
      ReadConfig();
      LoadIPSetting();

      //Others
      labelCalibrated.Text = "YES";
      tbw = new ToolBarWindow();
      tbw.Browser = browser;
      tbw.KbdPositoin = "middle";
      tbw.Show();

      CheckCableThread = new Thread(new ThreadStart(CheckCablePlugged));
      CheckCableThread.Start();
      WriteLog("Starting to monitor ethernet cable.");


      if (fullscreen)
        this.Fullscreen();
    }

    protected void OnBtnCalibrateClicked(object sender, EventArgs e)
    {

    }

    protected void OnBtnNetworkSettingClicked(object sender, EventArgs e)
    {
      wns = new WindowNetworkSetting();
      wns.ChangeIP += Wns_ChangeIP;
      wns.ShowAll();


      tbw.KbdPositoin = "right";

      if (fullscreen)
        wns.Fullscreen();
    }

    void Wns_ChangeIP(string[] IPtoSet)
    {
      foreach (string s in IPtoSet)
        {
          if (s.Contains("Invalid"))
            {
              WriteLog("IP setting invalid. Aborting.");
              MessageDialog md = 
                new MessageDialog(this, DialogFlags.Modal, MessageType.Error, 
                  ButtonsType.Ok, "Invalid IP Address!", new object());

              md.Run();
              md.Destroy();
              return;
            }
        }
      //string ipaddress = IPtoSet[0] + "\n" + IPtoSet[1] + "\n" + IPtoSet[2] + "\n" + IPtoSet[3];
      SetIpAddress(IPtoSet);
      LoadIPSetting();
      CheckRemoteConnection();
    }

    protected void OnBtnNetworkTestClicked(object sender, EventArgs e)
    {
      CheckRemoteConnection();       
    }

    private int countDown;
    System.Timers.Timer timer = new System.Timers.Timer(1000);

    protected void OnBtnContinueClicked(object sender, EventArgs e)
    {
      btnContinue.Sensitive = false;
      WriteLog("Loading remote display... please wait");
      //Thread.Sleep(1000);

      timer.AutoReset = true;
      timer.Enabled = true;
      timer.Elapsed += (x, y) => CountDown(timer);
      timer.Start();

      countDown = 15;


      Process browserProcess = new Process();
      browserProcess.StartInfo.FileName = browser;
      browserProcess.StartInfo.Arguments = labelRemote.Text;
      browserProcess.StartInfo.CreateNoWindow = true;
      browserProcess.StartInfo.UseShellExecute = false;
      try
        {
          browserProcess.Start();
          //browserProcess.WaitForExit();
        } catch (Exception fe)
        {
          WriteLog(fe.ToString());
        }
    }

    private void CountDown(System.Timers.Timer timer)
    {
      countDown--;

      btnContinue.Label = countDown.ToString();
      SetFont(btnContinue, fontmid);
      if (countDown == 0)
        {
          btnContinue.Label = "Continue";
          btnContinue.Sensitive = true;
          SetFont(btnContinue, fontmid);
          timer.Stop();
        }
    }


  }
}
