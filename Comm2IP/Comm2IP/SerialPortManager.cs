using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Net.Sockets;

namespace Comm2IP
{
	class SerialPortManager
	{
		private static readonly int BUFFER_SIZE = 16384;
		private static readonly int TIMEOUT = 15000;

		private SerialPort port;
		private SerialDataReceivedEventHandler onReceiveHandler;
		Socket socket;
		string comPort;
		int baudRate;

		protected internal SerialPortManager(Socket socket, string comPort, int baudRate)
		{
			this.socket = socket;
			this.comPort = comPort;
			this.baudRate = baudRate;
		}

		protected internal void SetHandshake(Handshake handshake)
		{
			port.Handshake = handshake;
		}

		protected internal void OpenPort()
		{
			port = new SerialPort(comPort, baudRate, Parity.None, 8, StopBits.One);
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

		protected internal void ClosePort()
		{
			port.DataReceived -= onReceiveHandler;
			port.Close();
		}

		protected internal void Send(string s)
		{
			port.Write(s);
		}

		protected internal void Send(byte[] buffer, int size)
		{
			port.Write(buffer, 0, size);
		}

		protected internal int Read()
		{
			return port.ReadByte();
		}

		protected internal bool PortHasData() { return (port.BytesToRead > 0); }

		private void OnReceive(Object sender, SerialDataReceivedEventArgs e)
		{
			List<byte> buffer = new List<byte>();

			while (PortHasData()) buffer.Add((byte)Read());
			socket.Send(buffer.ToArray());
		}
	}
}
