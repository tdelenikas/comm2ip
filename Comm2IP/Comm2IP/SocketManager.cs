using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Comm2IP
{
	class SocketManager
	{
		Socket socket;
		SerialPortManager portManager;
		byte[] inBuffer = new byte[1024];
		string comPort;
		int baudRate;

		public SocketManager(Socket socket, string comPort, int baudRate)
		{
			this.socket = socket;
			portManager = null;
			this.comPort = comPort;
			this.baudRate = baudRate;
		}

		public void Run()
		{
			int inBufferCount;
			portManager = new SerialPortManager(socket, comPort, baudRate);
			try
			{
				portManager.OpenPort();
				while (true)
				{
					inBufferCount = socket.Receive(inBuffer);
					if (inBufferCount == 0) break;
					portManager.Send(inBuffer, inBufferCount);
				}
			}
			catch (Exception e)
			{
				Comm2IP.log.Error(e);
			}
			finally
			{
				if (portManager != null) portManager.ClosePort();
				try { socket.Close(); }
				catch { }
				try { portManager.ClosePort(); }
				catch { }
			}
		}
	}
}
