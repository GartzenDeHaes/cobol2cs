using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Net
{
	public interface IPacketListener
	{
		void IPacket_OnData(long i);
		void IPacket_OnData(int i);
		void IPacket_OnData(short i);
		void IPacket_OnData(char i);
		void IPacket_OnData(bool b);
		void IPacket_OnData(float f);
		void IPacket_OnData(double d);
		void IPacket_OnData(string str);
		void IPacket_OnData(byte[] data, int len);
		void IPacket_OnData(DateTime dtm);
		void IPacket_OnData(Date i);

		void IStreamRead_OnError(string msg);
		void IStreamRead_OnClose();
	}
}
