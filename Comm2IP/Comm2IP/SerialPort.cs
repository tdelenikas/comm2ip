using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Net.Sockets;

namespace Comm2IP
{
	public class SerialPort
	{
		static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private static readonly int BUFFER_SIZE = 16384;
		private static readonly int TIMEOUT = 15000;

		private System.IO.Ports.SerialPort port;
		private SerialDataReceivedEventHandler onReceiveHandler;
		System.Net.Sockets.Socket socket;
		string comPort;
		int baudRate;

		public SerialPort(System.Net.Sockets.Socket socket, string comPort, int baudRate)
		{
			this.socket = socket;
			this.comPort = comPort;
			this.baudRate = baudRate;
		}

		public void SetHandshake(Handshake handshake)
		{
			port.Handshake = handshake;
		}

		public void OpenPort()
		{
			port = new System.IO.Ports.SerialPort(comPort, baudRate, Parity.None, 8, StopBits.One);
			port.DtrEnable = true;
			port.DiscardNull = true;
			port.ReadBufferSize = BUFFER_SIZE;
			port.WriteBufferSize = BUFFER_SIZE;
			port.ReadTimeout = TIMEOUT;
			port.WriteTimeout = TIMEOUT;
			port.Handshake = Handshake.RequestToSend;
			port.ReceivedBytesThreshold = 1;
			port.DiscardNull = true;
			port.Open();
			onReceiveHandler = new SerialDataReceivedEventHandler(OnReceive);
			port.DataReceived += onReceiveHandler;
		}

		public void ClosePort()
		{
			port.DataReceived -= onReceiveHandler;
			port.Close();
		}

		public void SetSocket(System.Net.Sockets.Socket s)
		{
			socket = s;
		}

		public bool InUse()
		{
			return (socket != null);
		}

		public void Send(string s)
		{
			port.Write(s);
		}

		public void Send(byte[] buffer, int size)
		{
			port.Write(buffer, 0, size);
		}

		public int Read()
		{
			return port.ReadByte();
		}

		public bool PortHasData() { return (port.BytesToRead > 0); }

		void OnReceive(Object sender, SerialDataReceivedEventArgs e)
		{
			List<byte> buffer = new List<byte>();

			while (PortHasData()) buffer.Add((byte)Read());
			if (socket != null) socket.Send(buffer.ToArray());
		}
	}
}