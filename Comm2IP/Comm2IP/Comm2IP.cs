using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Comm2IP
{
	public class Comm2IP
	{
		static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		static Dictionary<string, SerialPort> ports = new Dictionary<string, SerialPort>();

		byte[] localHost;
		int port;
		string comPort;
		int baudRate;
		TcpListener tcpListener;
		bool shouldListen;

		public Comm2IP(byte[] localHost, int port, string comPort, int baudRate)
		{
			this.localHost = localHost;
			this.port = port;
			this.comPort = comPort;
			this.baudRate = baudRate;
			tcpListener = new TcpListener(new IPAddress(localHost), port);
			shouldListen = true;
		}

		public void Run()
		{
			tcpListener.Start();
			while (shouldListen)
			{
				try
				{
					System.Net.Sockets.Socket socket = tcpListener.AcceptSocket();
					Socket m = new Socket(socket, comPort, baudRate);
					log.Info("Got connection from: " + socket.RemoteEndPoint + " for: " + comPort + "@" + baudRate);
					Thread t = new Thread(new ThreadStart(m.Run));
					t.Start();
				}
				catch (Exception e)
				{
					log.Warn(e);
				}
			}
		}

		public void Start()
		{
			shouldListen = true;
		}

		public void Stop()
		{
			shouldListen = false;
			tcpListener.Stop();
		}

		public static SerialPort GetSerialPort(System.Net.Sockets.Socket socket, string portName, int baudRate)
		{
			lock (ports)
			{
				SerialPort sp = null;
				string key = portName.ToUpper();
				try
				{
					sp = ports[key];
					if (sp.InUse()) throw new Exception("Port is already in use!");
					sp.SetSocket(socket);
				}
				catch (KeyNotFoundException)
				{
					sp = new SerialPort(socket, portName, baudRate);
					sp.OpenPort();
					ports.Add(key, sp);
				}
				return sp;
			}
		}

		public static void CloseSerialPort(SerialPort serialPort)
		{
			lock (ports)
			{
				if (serialPort != null) serialPort.SetSocket(null);
			}
		}
	}
}