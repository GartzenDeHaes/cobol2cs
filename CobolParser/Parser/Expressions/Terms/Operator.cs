using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOR.Core;

namespace CobolParser.Expressions.Terms
{
	public class Operator : ITerm
	{
		public string Lexum
		{
			get;
			private set;
		}

		public Operator(Terminalize terms)
		{
			Lexum = "";

			while (IsOperator(terms.Current))
			{
				if (Lexum.Length > 0 && terms.Current.Str.Length > 1)
				{
					Lexum += " ";
				}
				Lexum += terms.Current.Str;

				if (terms.CurrentEquals("EQUAL"))
				{
					terms.Next();
					terms.MatchOptional("TO");
				}
				else
				{
					terms.Next();
				}
			}
		}

		public override string ToDocumentationString()
		{
			return Lexum;
		}

		public static bool IsOperator(StringNode node)
		{
			if (node.Str.Equals("EQUAL", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (node.Str.Equals("NOT", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (node.Str.Equals("OR", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (node.Str.Equals("AND", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (node.Str.Equals("IS", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (node.Str.Equals("LESS", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (node.Str.Equals("GREATER", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (node.Str.Equals("THAN", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (node.Str.Equals("WITHIN", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (node.Str == "<=")
			{
				return true;
			}
			if (node.Str == ">=")
			{
				return true;
			}
			if (node.Str.Length > 1)
			{
				return false;
			}
			switch (node.Str[0])
			{
				case '+':
				case '-':
				case '=':
				case '<':
				case '>':
				case '/':
				case '*':
					return true;
				default:
					return false;
			}
		}

		public override string ToString()
		{
			if (Lexum.Equals("EQUAL", StringComparison.InvariantCultureIgnoreCase))
			{
				return "==";
			}
			if (Lexum.Equals("NOT", StringComparison.InvariantCultureIgnoreCase))
			{
				return "!";
			}
			if (Lexum.Equals("OR", StringComparison.InvariantCultureIgnoreCase))
			{
				return "||";
			}
			if (Lexum.Equals("AND", StringComparison.InvariantCultureIgnoreCase))
			{
				return "&&";
			}
			if (Lexum.Equals("IS", StringComparison.InvariantCultureIgnoreCase))
			{
				return "==";
			}
			if (Lexum.Equals("LESS", StringComparison.InvariantCultureIgnoreCase))
			{
				return "<";
			}
			if (Lexum.Equals("GREATER", StringComparison.InvariantCultureIgnoreCase))
			{
				return ">";
			}
			if (Lexum.Equals("THAN", StringComparison.InvariantCultureIgnoreCase))
			{
				return "";
			}
			if (Lexum.Equals("WITHIN", StringComparison.InvariantCultureIgnoreCase))
			{
				return ".IndexOf";
			}
			if (Lexum == "<=" || Lexum == ">=")
			{
				return Lexum;
			}
			if (Lexum.Equals("NOT=", StringComparison.InvariantCultureIgnoreCase))
			{
				return "!=";
			}
			if (Lexum.Equals("NOT EQUAL", StringComparison.InvariantCultureIgnoreCase))
			{
				return "!=";
			}
			if (Lexum.Equals("LESS THAN OR EQUAL", StringComparison.InvariantCultureIgnoreCase))
			{
				return "<=";
			}
			if (Lexum.Equals("GREATER THAN OR EQUAL", StringComparison.InvariantCultureIgnoreCase))
			{
				return ">=";
			}
			if (Lexum.Equals("LESS THAN", StringComparison.InvariantCultureIgnoreCase))
			{
				return "<";
			}
			if (Lexum.Equals("GREATER THAN", StringComparison.InvariantCultureIgnoreCase))
			{
				return ">";
			}
			if (Lexum.Equals("NOT GREATER THAN", StringComparison.InvariantCultureIgnoreCase))
			{
				return "<=";
			}
			if (Lexum.Equals("AND NOT", StringComparison.InvariantCultureIgnoreCase))
			{
				return "&& !";
			}
			if (Lexum.Equals("IS NOT EQUAL", StringComparison.InvariantCultureIgnoreCase))
			{
				return "!=";
			}
			if (Lexum.Equals("OR NOT", StringComparison.InvariantCultureIgnoreCase))
			{
				return "|| !";
			}
			if (Lexum.Equals("IS EQUAL", StringComparison.InvariantCultureIgnoreCase))
			{
				return "==";
			}
			if (Lexum.Equals("NOT<", StringComparison.InvariantCultureIgnoreCase))
			{
				return ">=";
			}
			if (Lexum.Equals("NOT>", StringComparison.InvariantCultureIgnoreCase))
			{
				return "<=";
			}
			if (Lexum.Equals("OR>", StringComparison.InvariantCultureIgnoreCase))
			{
				return "|| >";
			}
			if (Lexum.Equals("IS>=", StringComparison.InvariantCultureIgnoreCase))
			{
				return ">=";
			}
			if (Lexum.Equals("AND<=", StringComparison.InvariantCultureIgnoreCase))
			{
				return "<=";
			}
			if (Lexum.Equals("IS>", StringComparison.InvariantCultureIgnoreCase))
			{
				return ">";
			}
			if (Lexum.Length > 1)
			{
				throw new Exception("Internal error");
			}
			switch (Lexum[0])
			{
				case '=':
					return "==";
				case '+':
				case '-':
				case '<':
				case '>':
				case '/':
				case '*':
					return Lexum;
				default:
					throw new Exception("Internal error");
			}
		}
	}
}
