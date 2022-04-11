using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DOR.Core.Net
{
	public interface IHttpRequestBody
	{
		int ByteCount { get; }

		IHttpRequestBody Clone();

		void Write( Stream strm );

		string GetValue(string key);

		bool HasKey(string key);

		void Parse(byte[] cp, int pos, int len, int contentLen);
	}
}
