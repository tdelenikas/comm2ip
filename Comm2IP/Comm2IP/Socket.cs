using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Comm2IP
{
	class Socket
	{
		static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		System.Net.Sockets.Socket socket;
		SerialPort serialPort;
		byte[] inBuffer = new byte[1024];
		string comPort;
		int baudRate;

		public Socket(System.Net.Sockets.Socket socket, string comPort, int baudRate)
		{
			this.socket = socket;
			serialPort = null;
			this.comPort = comPort;
			this.baudRate = baudRate;
		}

		public void Run()
		{
			int inBufferCount;
			try
			{
				serialPort = Comm2IP.GetSerialPort(socket, comPort, baudRate);
				while (true)
				{
					inBufferCount = socket.Receive(inBuffer);
					if (inBufferCount == 0) break;
					serialPort.Send(inBuffer, inBufferCount);
				}
			}
			catch (Exception e)
			{
				log.Error(e);
			}
			finally
			{
				log.Info("Lost connection from: " + socket.RemoteEndPoint + " for: " + comPort + "@" + baudRate);
				Comm2IP.CloseSerialPort(serialPort);
				try { socket.Close(); }
				catch { }
			}
		}
	}
}