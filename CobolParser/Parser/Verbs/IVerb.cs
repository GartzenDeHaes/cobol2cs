using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser
{
	public abstract class IVerb
	{
		public VerbType Type
		{
			get;
			protected set;
		}

		public StringNode VerbLexum
		{
			get;
			protected set;
		}

		public IVerb(StringNode lex)
		{
			VerbLexum = lex;
		}

		public string CommentProbe()
		{
			if (VerbLexum == null)
			{
				return null;
			}

			if (VerbLexum.Prev.Type == StringNodeType.Comment)
			{
				StringBuilder buf = new StringBuilder();
				StringNode node = VerbLexum.Prev;

				while (node.Type == StringNodeType.Comment)
				{
					node = node.Prev;
				}
				node = node.Next;
				while (node.Type == StringNodeType.Comment)
				{
					buf.Append(node.Str);
					if (node.Next.Type == StringNodeType.Comment)
					{
						buf.Append("\r\n");
					}
					node = node.Next;
				}

				string cmt = buf.ToString();
				if (cmt == "/")
				{
					return "";
				}
				return cmt;
			}

			return null;
		}
	}
}
