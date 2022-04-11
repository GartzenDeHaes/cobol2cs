using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;

namespace CobolParser.Verbs
{
	public class CompilerDirective : IVerb
	{
		public const string Lexum = "?";
		private Vector<StringNode> _args = new Vector<StringNode>();

		public StringNode Directive
		{
			get;
			private set;
		}

		public string Arg1
		{
			get;
			private set;
		}

		public StringNode Arg2
		{
			get { return _args[0]; }
		}

		public CompilerDirective(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.CompilerDirective;

			terms.Match(StringNodeType.QuestionMark);

			Directive = terms.Current;
			terms.Next();

			if (Directive.LineNumber == terms.Current.LineNumber)
			{
				if (terms.Current.Type == StringNodeType.Comma)
				{
					terms.Next();
				}

				Arg1 = terms.Current.Str;
				if (terms.Current.Type == StringNodeType.Eq)
				{
					terms.Next();
					Arg1 += terms.Current.Str;
				}
				terms.Next();

				if (Directive.LineNumber == terms.Current.LineNumber)
				{
					if (terms.Current.Type == StringNodeType.LPar)
					{
						terms.Next();
					}

					if (terms.Current.Type != StringNodeType.Comma)
					{
						_args.Add(terms.Current);
					}
					terms.Next();

					if (terms.Current.Type == StringNodeType.RPar)
					{
						terms.Next();
					}

					while (Directive.LineNumber == terms.Current.LineNumber)
					{
						if (terms.Current.Type != StringNodeType.Comma)
						{
							_args.Add(terms.Current);
						}
						if (!terms.Next())
						{
							break;
						}
					}
				}
			}

			if (Directive.Str.Equals("SOURCE", StringComparison.CurrentCultureIgnoreCase))
			{
				Terminalize ins;
				if (Arg1[0] == '$')
				{
					ins = ImportManager.GetSection(Directive.FileName, new GuardianPath(Arg1), Arg2);
				}
				else
				{
					ins = ImportManager.GetSection(Directive.FileName, ImportManager.ResolveDefine(new GuardianDefine(Arg1)), Arg2);
				}
				if (null == ins)
				{
					// Assert in ImportManager
					return;
				}

				terms.Prev();
				terms.InjectAfterCurrent(ins);
				terms.Next();
			}
		}

		public StringNode ArgX(int pos)
		{
			return _args[pos];
		}

		public int ArgCount
		{
			get { return _args.Count; }
		}
	}
}
