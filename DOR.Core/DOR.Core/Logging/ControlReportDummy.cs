using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.Logging
{
	public class ExceptionReportDummy : ExceptionReport
	{
		private StringBuilder _output = new StringBuilder();

		public ExceptionReportDummy()
		: base()
		{
		}

		protected override void Write(string s)
		{
			_output.Append(s);
		}

		public override string ToString()
		{
			return _output.ToString();
		}

		public void Clear()
		{
			_output.Clear();
		}
	}

	public class ControlReportDummy : ControlReport
	{
		private StringBuilder _output = new StringBuilder();

		public ControlReportDummy()
		: base(new ExceptionReportDummy())
		{
		}

		protected override void Write(string s)
		{
			_output.Append(s);
		}

		public override string ToString()
		{
			return (_output.ToString() + " " + ExceptionReport.ToString()).Trim();
		}

		public void Clear()
		{
			_output.Clear();
			((ExceptionReportDummy)ExceptionReport).Clear();
		}
	}
}
