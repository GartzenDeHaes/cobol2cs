using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using CobolParser.Expressions;
using CobolParser.Expressions.Terms;
using CobolParser.Verbs.Phrases;
using DOR.Core;

namespace CobolParser.Verbs
{
	public class Send : IVerb
	{
		public const string Lexum = "SEND";

		public ITerm MessageDataElement
		{
			get;
			private set;
		}

		public string ServerClassName
		{
			get;
			private set;
		}

		public StatementBlock OnError
		{
			get;
			private set;
		}

		public SendReplyYields OutputCases
		{
			get;
			private set;
		}

		public Send(Terminalize terms)
		: base(terms.Current)
		{
			Type = VerbType.Send;

			terms.Match(Lexum);

			terms.MatchOptional("MESSAGE");

			if (terms.CurrentNextEquals(1, "OF"))
			{
				MessageDataElement = new OffsetReference(terms);
			}
			else
			{
				MessageDataElement = new Id(terms);
			}

			CobolProgram.CurrentSymbolTable.AddSendReference(MessageDataElement, this);

			if (terms.CurrentEquals("TO"))
			{
				terms.Match("TO");

				ServerClassName = StringHelper.StripQuotes(terms.Current.Str);
				terms.Next();

				ImportManager.AddPathwaySend(terms.FileName, this);
			}

			while (true)
			{
				if (terms.CurrentEquals("TIMEOUT"))
				{
					terms.Next();
					terms.Next();
				}
				else if (terms.CurrentEquals("REPLY") || terms.CurrentEquals("CODE"))
				{
					OutputCases = new SendReplyYields(terms);
				}
				else if (terms.CurrentEquals("ON"))
				{
					terms.Match("ON");
					terms.Match("ERROR");
					OnError = new StatementBlock(terms);
				}
				else
				{
					break;
				}
			}
		}
	}
}
