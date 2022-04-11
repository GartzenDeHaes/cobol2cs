using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core
{
	public class ValidationMessage
	{
		/// <summary>
		/// True if this is a hard application error, not a validation message.
		/// </summary>
		public bool IsException
		{
			get;
			set;
		}

		public string Field
		{
			get;
			set;
		}

		public int MessaegCode
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public string StackTrace
		{
			get;
			set;
		}

		public ValidationMessage()
		{
			Field = "";
			Message = "";
			StackTrace = "";
		}

		public ValidationMessage
		(
			bool isExcpt, 
			string field, 
			int code, 
			string message, 
			string stack
		)
		{
			IsException = isExcpt;
			Field = field;
			MessaegCode = code;
			Message = message;
			StackTrace = stack;
		}
	}
}
