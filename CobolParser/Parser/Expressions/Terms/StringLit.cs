using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;

namespace CobolParser.Expressions.Terms
{
	public class StringLit : ITerm
	{
		public string QuotedText
		{
			get;
			private set;
		}

		public StringLit(string txt)
		{
			QuotedText = "\"" + txt + "\"";
		}

		public StringLit(Terminalize terms)
		{
			if (terms.Current.Str == "X" && terms.CurrentNext(1).Type == StringNodeType.Quoted)
			{
				terms.Next();
				QuotedText = "X" + terms.Current.Str;
				terms.Match(StringNodeType.Quoted);
			}
			else if (terms.CurrentNext(1).Type == StringNodeType.Quoted)
			{
				Vector<StringNode> nodes = new Vector<StringNode>();

				while (terms.Current.Type == StringNodeType.Quoted)
				{
					nodes.Add(terms.Current);
					terms.Match(StringNodeType.Quoted);
				}

				StringBuilder buf = new StringBuilder();
				buf.Append('"');
				buf.Append(StringHelper.StripQuotes(nodes[0].Str));

				for (int x = 1; x < nodes.Count - 1; x++)
				{
					// should be an embeded quote mark
					string txt = nodes[x].Str;
					if ((x % 2) == 0)
					{
						txt = StringHelper.StripQuotes(txt);
					}
					else
					{
						txt = txt.Replace("\"", "\\\"");
					}
					buf.Append(txt);
				}

				buf.Append(StringHelper.StripQuotes(nodes[nodes.Count - 1].Str));
				buf.Append('"');

				QuotedText = buf.ToString();
			}
			else
			{
				QuotedText = terms.Current.Str;
				terms.Match(StringNodeType.Quoted);
			}
		}

		public override string ToDocumentationString()
		{
			return QuotedText;
		}

		public override string ToString()
		{
			return QuotedText;
		}
	}
}
