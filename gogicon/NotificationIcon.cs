/*
 * Created by SharpDevelop.
 * User: nmo
 * Date: 04.11.2012
 * Time: 20:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Net;

namespace gogicon
{
	
	
	public sealed class NotificationIcon
	{
		
		private ContextMenu notificationMenu;
		public static NotifyIcon notifyIcon;
		#region Initialize icon and menu
		public NotificationIcon()
		{
			notifyIcon = new NotifyIcon();
			notificationMenu = new ContextMenu(InitializeMenu());
			
			notifyIcon.DoubleClick += IconDoubleClick;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotificationIcon));
			//	notifyIcon.Icon = (Icon)resources.GetObject("$this.Icon");
			notifyIcon.Icon = GetIcon("...");
			notifyIcon.ContextMenu = notificationMenu;
		}
		
		private MenuItem[] InitializeMenu()
		{
			MenuItem[] menu = new MenuItem[] {
					new MenuItem("About", menuAboutClick),
				new MenuItem("Exit", menuExitClick)
			};
			return menu;
		}
		#endregion
		
		
		
		
		public static Icon GetIcon(string text)
		{
			//Create bitmap, kind of canvas
			Bitmap bitmap = new Bitmap(16, 16);

			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotificationIcon));

			//  Icon icon = new Icon(@"tf2_2.ico");
			Icon icon = (Icon)resources.GetObject("$this.Icon16");
			System.Drawing.Font drawFont = new System.Drawing.Font("Tahoma", 8, FontStyle.Bold);
			System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

			System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);

			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
			graphics.DrawIcon(icon, 0, 0);
			graphics.DrawString(text, drawFont, drawBrush, 1, 2);

			//To Save icon to disk
			//bitmap.Save("icon.ico", System.Drawing.Imaging.ImageFormat.Icon);

			Icon createdIcon = Icon.FromHandle(bitmap.GetHicon());

			drawFont.Dispose();
			drawBrush.Dispose();
			graphics.Dispose();
			bitmap.Dispose();

			return createdIcon;
		}
		
		
		
		
		#region Main - Program entry point
		/// <summary>Program entry point.</summary>
		/// <param name="args">Command Line Arguments</param>
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			bool isFirstInstance;
			// Please use a unique name for the mutex to prevent conflicts with other programs
			using (Mutex mtx = new Mutex(true, "gogicon", out isFirstInstance)) {
				if (isFirstInstance) {
					NotificationIcon notificationIcon = new NotificationIcon();
					notifyIcon.Visible = true;
					addtimer();
					updateIcon();
					Application.Run();
					notifyIcon.Dispose();
				} else {
					// The application is already running
					// TODO: Display message box or change focus to existing application instance
				}
			} // releases the Mutex
		}
		#endregion
		
		
		public static void addtimer()
		{
			System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
			timer1.Enabled = true;
			timer1.Interval = 20000;
			timer1.Tick += new System.EventHandler(Timer1Tick);
		}
		
		#region Event Handlers
		private void menuAboutClick(object sender, EventArgs e)
		{
			MessageBox.Show("Another programming crime by nomo.\nSupposed to show number of players on GoG TF2 server...\nExpect this to break as soon as hlstatsx changes their statpage code.\n");
		}
		
		private static void updateIcon()
		{
			
			try
			{
			WebClient client = new WebClient();
			byte[] arr = client.DownloadData("http://thegrumpyoldgits.hlstatsx.com/status/");
			
			string tempstr="";
			
			for(int i=0;i<arr.Length;i++)
			{
				tempstr+=Convert.ToChar(arr[i]);
				if(Convert.ToChar(arr[i])=='\n')
				{
					if(tempstr.Contains("srv_empty") || tempstr.Contains("srv_nempty"))
					{
							string[] substrs = tempstr.Split('>');
							
						string numstring =substrs[3];
						
						string[] numsubstrs = numstring.Split('/');
						
						string thenumber = numsubstrs[0];
						
						if(thenumber != "0")
						{
						notifyIcon.Icon = GetIcon(thenumber);
						}
						else
						{
						notifyIcon.Icon = GetIcon("");
						}
						

					}
					tempstr="";
				}
			}
			

			
			}
			catch
			{
			
			}
		}
		
		private	static void Timer1Tick(object sender, EventArgs e)
		{
			updateIcon();
		}
		
		private void menuExitClick(object sender, EventArgs e)
		{
			Application.Exit();
		}
		
		private void IconDoubleClick(object sender, EventArgs e)
		{
			//MessageBox.Show("The icon was double clicked");
		}
		#endregion
	}
}
