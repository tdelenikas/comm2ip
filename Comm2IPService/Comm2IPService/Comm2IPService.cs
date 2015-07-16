using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Configuration.Install;
using System.Reflection;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Comm2IPService
{
	public partial class Comm2IPService : ServiceBase
	{
		public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private static int MAX_THREADS = 5;

		Comm2IP.Comm2IP[] handlers = new Comm2IP.Comm2IP[MAX_THREADS];
		Thread[] threads = new Thread[MAX_THREADS];

		public Comm2IPService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			log.Info("Starting...");
			for (int i = 0; i < MAX_THREADS; i++)
			{
				string parms = "";
				switch (i)
				{
					case 0: parms = Properties.Settings.Default.Binding1; break;
					case 1: parms = Properties.Settings.Default.Binding2; break;
					case 2: parms = Properties.Settings.Default.Binding3; break;
					case 3: parms = Properties.Settings.Default.Binding4; break;
					case 4: parms = Properties.Settings.Default.Binding5; break;
				}
				if (!String.IsNullOrEmpty(parms))
				{
					ServiceMapping sm = ParseParameters(parms.Split(new char[] { ' ' }));
					log.Info("Found service map: " + sm);
					handlers[i] = new Comm2IP.Comm2IP(sm.ipAddress, sm.ipPort, sm.commPort, sm.commRate);
					threads[i] = new Thread(new ThreadStart(handlers[i].Run));
					threads[i].Start();
				}
				else
				{
					handlers[i] = null;
					threads[i] = null;
				}
			}
		}

		protected override void OnStop()
		{
			log.Info("Stopping...");
			for (int i = 0; i < MAX_THREADS; i++)
			{
				if (threads[i] != null)
				{
					handlers[i].Stop();
					threads[i].Interrupt();
				}
			}
		}

		public ServiceMapping ParseParameters(string[] args)
		{
			ServiceMapping sm = new ServiceMapping();
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "-p") sm.ipPort = Int32.Parse(args[++i]);
				else if (args[i] == "-a")
				{
					string address = args[++i];
					string[] addr = address.Split(new char[] { '.' });
					for (int j = 0; j < 4; j++) sm.ipAddress[j] = (byte)Int32.Parse(addr[j]);
				}
				else if (args[i] == "-c") sm.commPort = args[++i];
				else if (args[i] == "-b") sm.commRate = Int32.Parse(args[++i]);
			}
			return sm;
		}

		public bool CheckParameters(ServiceMapping sm)
		{
			if ((sm.commRate == -1) || (sm.commPort == "") || (sm.ipPort == -1))
			{
				Console.WriteLine("Comm2IP command line / service interface.");
				Console.WriteLine("Copyright (c) 2002-2015, http://smslib.org");
				Console.WriteLine();
				Console.WriteLine("Service Usage:");
				Console.WriteLine("	Comm2IPService --install   : Install me as a Windows Service.");
				Console.WriteLine("	Comm2IPService --uninstall : Remove me from Windows Services.");
				Console.WriteLine("Command Line Usage:");
				Console.WriteLine("	Comm2IPService -a <bind address> -p <bind-port> -c <comm port name> -b <comm baud rate>");
				Console.WriteLine();
				Console.WriteLine("Example:");
				Console.WriteLine("	Comm2IPService -a 127.0.0.1 -p 12000 -c com1 -b 19200");
				Console.WriteLine("	i.e. connect to COM1@19200 and map traffic to localhost:12000.");
				return false;
			}
			else
			{
				log.Info(String.Format("Registering listener: {0}.{1}.{2}.{3}:{4} for {5}@{6}", sm.ipAddress[0], sm.ipAddress[1], sm.ipAddress[2], sm.ipAddress[3], sm.ipPort, sm.commPort, sm.commRate));
				return true;
			}
		}

		public static void Main(string[] args)
		{
			Comm2IPService service = new Comm2IPService();
			if (!Environment.UserInteractive)
			{
				ServiceBase.Run(service);
			}
			else
			{
				try
				{
					if (args.Length == 1)
					{
						if (args[0] == "--install") ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
						else if (args[0] == "--uninstall") ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
					}
					else
					{
						ServiceMapping sm = service.ParseParameters(args);
						if (service.CheckParameters(sm))
						{
							Console.WriteLine("Running in interactive mode, press Ctrl-C to abort...");
							Comm2IP.Comm2IP com = new Comm2IP.Comm2IP(sm.ipAddress, sm.ipPort, sm.commPort, sm.commRate);
							Thread listener = new Thread(new ThreadStart(com.Run));
							listener.Start();
							listener.Join();
						}
					}
				}
				catch (Exception e)
				{
					log.Fatal(e);
				}
			}
		}
	}
}
