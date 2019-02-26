using System;
using System.Windows.Forms;
using CefSharp.WinForms;

namespace CefSharp.MinimalExample.WinForms
{
    public partial class BrowserForm : Form
    {
        private readonly ChromiumWebBrowser browser;
        IniFiles ini = new IniFiles(Application.StartupPath + @"\配置文件.ini");

        public BrowserForm()
        {
            InitializeComponent();

            WindowState = FormWindowState.Maximized;

            //检测配置文件是否存在
            CheckIni();

            try
            {
                browser = new ChromiumWebBrowser(ini.IniReadValue("配置详情", "链接"))
                {
                    Dock = DockStyle.Fill,//填充方式
                };
                
                if (!Cef.IsInitialized)
                {
                    throw new InvalidOperationException("Cef::IsInitialized is false");
                }

                //禁用鼠标右键
                if (ini.IniReadValue("配置详情", "禁用鼠标右键") == "true")
                {
                    browser.MenuHandler = new MenuHandler();//禁用鼠标右键
                }

                toolStripContainer.ContentPanel.Controls.Add(browser);
                browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;//添加事件  
                browser.FrameLoadEnd += MyBrowserOnFrameLoadEnd;//加载事件


                //全屏
                if (ini.IniReadValue("配置详情", "全屏") == "true")
                {
                    AllScreen();
                }
                else
                {
                    this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;//自定义位置
                    this.Location = new System.Drawing.Point(0, 0);//屏幕起始位置
                    this.Width = int.Parse(ini.IniReadValue("配置详情", "窗口宽度"));
                    this.Height = int.Parse(ini.IniReadValue("配置详情", "窗口高度"));
                }

                HotKeys.RegHotKey(this.Handle, 247696405, 0, Keys.F5);
                HotKeys.RegHotKey(this.Handle, 247696411, 0, Keys.F11);
                HotKeys.RegHotKey(this.Handle, 247696412, (int)HotKeys.HotkeyModifiers.Alt, Keys.F12);
            }
            catch (Exception)
            {
                MessageBox.Show("配置错误，请检查配置文件.如需重置配置文件,请删除配置文件，重新打开软件即可。");
                throw;
            }
        }


        //热键消息循环
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696411) //判断热键
            {
                //全屏&取消全屏
                if (this.WindowState.ToString() == "Normal")
                {
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                }
                else {
                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    this.Location = new System.Drawing.Point(0, 0);//屏幕起始位置
                    this.Width = int.Parse(ini.IniReadValue("配置详情", "窗口宽度"));
                    this.Height = int.Parse(ini.IniReadValue("配置详情", "窗口高度"));
                    this.WindowState = System.Windows.Forms.FormWindowState.Normal;
                }  
            }
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696412) //判断热键
            {
                //调试
                browser.ShowDevTools();
            }
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == 247696405) //判断热键
            {
                //刷新窗口
                browser.Reload();
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// 开启调试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param> 
        private void OnIsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs args)
        {
            try
            {
                if (ini.IniReadValue("配置详情", "启用调试") == "true")
                {
                    //调试
                    browser.ShowDevTools();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("配置错误，请检查配置文件.如需重置配置文件,请删除配置文件，重新打开软件即可。");
                throw;
            }
        }

        /// <summary>
        /// 缩放   
        /// </summary>
        private void MyBrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs frameLoadEndEventArgs)
        {
            try
            {
                int ZoomLv = int.Parse(ini.IniReadValue("配置详情", "缩放"));
                if (ZoomLv > 100)
                {
                    browser.SetZoomLevel((Convert.ToDouble(browser.Tag) + (ZoomLv - 100)) / 25.0);
                }
                else {
                    browser.SetZoomLevel((Convert.ToDouble(browser.Tag) - (100 - ZoomLv)) / 25.0);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("配置错误，请检查配置文件.如需重置配置文件,请删除配置文件，重新打开软件即可。");
                throw;
            }

        }


        /// <summary>
        /// 检测配置文件
        /// </summary>
        private void CheckIni() {
            if (!ini.ExistINIFile())
            {
                //在这里写入配置信息
                ini.IniWriteValue("配置详情", "链接", "www.baidu.com");
                ini.IniWriteValue("配置详情", "全屏", "true");
                ini.IniWriteValue("配置详情", "窗口宽度", "1920");
                ini.IniWriteValue("配置详情", "窗口高度", "1080");
                ini.IniWriteValue("配置详情", "缩放", "100");
                ini.IniWriteValue("配置详情", "禁用鼠标右键", "true");
                ini.IniWriteValue("热键说明", "F5", "刷新");
                ini.IniWriteValue("热键说明", "F11", "全屏/退出全屏");
                ini.IniWriteValue("热键说明", "ALT+F12", "开启调试窗口");
                MessageBox.Show("检测到配置文件不存在，已初始化配置，请重新打开软件");
                System.Environment.Exit(0);
            }
        }

        /// <summary>
        /// 后退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackButtonClick(object sender, EventArgs e)
        {
            browser.Back();
        }

        /// <summary>
        /// 前进
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ForwardButtonClick(object sender, EventArgs e)
        {
            browser.Forward();
        }


        /// <summary>
        /// 加载链接
        /// </summary>
        /// <param name="url"></param>
        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                browser.Load(url);
            }
        }

        /// <summary>
        /// 全屏
        /// </summary>
        public void AllScreen()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }
    }

    /// <summary>
    /// 禁用鼠标右键  
    /// </summary>
    public class MenuHandler : CefSharp.IContextMenuHandler
    {

        void CefSharp.IContextMenuHandler.OnBeforeContextMenu(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IContextMenuParams parameters, CefSharp.IMenuModel model)
        {
            model.Clear();
        }

        bool CefSharp.IContextMenuHandler.OnContextMenuCommand(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IContextMenuParams parameters, CefSharp.CefMenuCommand commandId, CefSharp.CefEventFlags eventFlags)
        {
            //throw new NotImplementedException();
            return false;
        }

        void CefSharp.IContextMenuHandler.OnContextMenuDismissed(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame)
        {
            //throw new NotImplementedException();
        }

        bool CefSharp.IContextMenuHandler.RunContextMenu(CefSharp.IWebBrowser browserControl, CefSharp.IBrowser browser, CefSharp.IFrame frame, CefSharp.IContextMenuParams parameters, CefSharp.IMenuModel model, CefSharp.IRunContextMenuCallback callback)
        {
            return false;
        }
    }
}