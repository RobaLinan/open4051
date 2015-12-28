using System;
using System.Diagnostics;

namespace Open4051
{
    public partial class ToolBarWindow : Gtk.Window
    {
        private string browser;
        private string kbdPosition = "middle";

        public string KbdPositoin
        {
            get{ return kbdPosition; }
            set{ kbdPosition = value; }
        }

        public string Browser
        {
            get{ return browser; }
            set{ browser = value; }
        }

        public ToolBarWindow()
            : base(Gtk.WindowType.Toplevel)
        {
            this.Build();
            this.KeepAbove = true;

            int width, height;
            this.GetSize(out width, out height);

            this.Move(Screen.Width - width, 0);

        }

        protected void OnBtnCloseAllClicked(object sender, EventArgs e)
        {
            foreach (Process p in Process.GetProcessesByName("ScreenKeyboard"))
            {
                p.Kill();            
            }

            CloseBrowser();

            this.ParentWindow.Destroy();
        }

        protected void OnBtnKeyboardClicked(object sender, EventArgs e)
        {
            KeyboardCtrl();
        }

        public void CloseBrowser()
        {
           
            
            string processname = browser.Substring(1 + browser.LastIndexOf("\\"));
            processname = processname.Substring(0, processname.IndexOf(".exe"));

            foreach (Process p in Process.GetProcessesByName(processname))
            {
                p.Kill();            
            }
        }

        public void KeyboardCtrl()
        {
            foreach (Process p in Process.GetProcessesByName("ScreenKeyboard"))
            {
                p.Kill();            
                return;
            }


            Process kbdProcess = new Process();
            kbdProcess.StartInfo.Arguments = kbdPosition;
            kbdProcess.StartInfo.FileName = "ScreenKeyboard.exe";
            kbdProcess.Start();
        }

        protected void OnBtnCloseBrowserClicked(object sender, EventArgs e)
        {
            CloseBrowser();
        }
    }
}

