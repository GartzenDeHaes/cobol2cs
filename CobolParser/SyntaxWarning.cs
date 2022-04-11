using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser
{
	public class SyntaxWarning
	{
		public int LineNumber
		{
			get { return Token.LineNumber; }
		}

		public int CharacterPosition
		{
			get { return Token.CharPos; }
		}

		public string Message
		{
			get;
			private set;
		}

		public StringNode Token
		{
			get;
			private set;
		}

		public SyntaxWarning(StringNode token, string msg)
		{
			Message = msg;
			Token = token;
		}
	}
}
