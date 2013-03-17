using System;
using System.Collections.Generic;
using System.Text;

namespace Comm2IPService
{
	public class ServiceMapping
	{
		private int _ipPort = -1;
		private byte[] _ipAddress = new byte[4] { 0, 0, 0, 0 };
		private string _commPort = "";
		private int _commRate = -1;

		public int ipPort { get { return _ipPort; } set { _ipPort = value; } }
		public byte[] ipAddress { get { return _ipAddress; } set { _ipAddress = value; } }
		public string commPort { get { return _commPort; } set { _commPort = value; } }
		public int commRate { get { return _commRate; } set { _commRate = value; } }

		public override string ToString()
		{
			return String.Format("IF: {0}@{1} <-> {2}:{3}", _commPort, _commRate, _ipAddress, _ipPort);
		}
	}
}
