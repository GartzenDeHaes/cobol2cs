using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using CobolParser.Parser;

namespace CobolParser.SQL.Conds
{
	public class CondParam : ISqlCondToken
	{
		public Symbol Symbol
		{
			get;
			private set;
		}

		public string FieldName
		{
			get;
			private set;
		}

		public string RecordName
		{
			get;
			private set;
		}

		public string NewType
		{
			get;
			private set;
		}

		public string NewTypeQualifier
		{
			get;
			private set;
		}

		public CondParam NullIndicatorField
		{
			get;
			private set;
		}

		public ISqlCondToken AsTypeConversion
		{
			get;
			private set;
		}

		public CondParam(SqlLex lex)
		{
			RecordName = "";

			lex.Match(SqlToken.COLON);
			FieldName = lex.Lexum.Str;
			lex.Match(SqlToken.ID);

			string parent = null;

			while (!lex.IsEOF && (lex.Lexum.StrEquals("OF") || lex.Lexum.StrEquals("IN")))
			{
				if (lex.Lexum.StrEquals("IN") && lex.Lexum.Next.Type == StringNodeType.LPar)
				{
					break;
				}

				lex.Next();
				if (RecordName.Length > 0)
				{
					RecordName += ".";
				}
				RecordName += lex.Lexum.Str;
				parent = lex.Lexum.Str;
				lex.Match(SqlToken.ID);
			}

			Symbol = CobolProgram.CurrentSymbolTable.Find(FieldName, parent);

			if (!lex.IsEOF && lex.Lexum.StrEquals("TYPE"))
			{
				lex.Match("TYPE");
				lex.Match(SqlToken.AS);
				NewType = lex.Lexum.Str;
				lex.Next();

				if 
				(
					!lex.IsEOF && 
					(
						lex.Lexum.StrEquals("YEAR") || 
						lex.Lexum.StrEquals("MONTH") || 
						lex.Lexum.StrEquals("DAY") ||
						lex.Lexum.StrEquals("HOUR") ||
						lex.Lexum.StrEquals("FRACTION")
					)
				)
				{
					NewTypeQualifier = lex.Lexum.Str;
					lex.Next();

					if (lex.Token == SqlToken.LPAR)
					{
						lex.Next();
						NewTypeQualifier += "(" + lex.Lexum.Str + ")";
						lex.Next();
						lex.Match(SqlToken.RPAR);
					}
					else if (!lex.IsEOF && lex.Lexum.StrEquals("TO"))
					{
						lex.Match("TO");

						if (lex.Lexum.StrEquals("FRACTION"))
						{
							lex.Match("FRACTION");
							if (lex.Token == SqlToken.LPAR)
							{
								lex.Match(SqlToken.LPAR);
								NewTypeQualifier += " TO FRACTION(" + lex.Lexum.Str + ")";
								lex.Next();
								lex.Match(SqlToken.RPAR);
							}
							else
							{
								NewTypeQualifier += " TO FRACTION";
							}
						}
						else
						{
							NewTypeQualifier += " TO " + lex.Lexum.Str;
							lex.Next();
						}
					}
				}
			}

			if (!lex.IsEOF && lex.Lexum.StrEquals("INDICATOR"))
			{
				lex.Next();
				NullIndicatorField = new CondParam(lex);
				Debug.Assert(NewType == null);
				NewType = NullIndicatorField.NewType;
				NullIndicatorField.NewType = null;
				NewTypeQualifier = NullIndicatorField.NewTypeQualifier;
				NullIndicatorField.NewTypeQualifier = null;
			}

			if (lex.Token == SqlToken.AS)
			{
				lex.Next();
				if (lex.Token == SqlToken.INTERVAL)
				{
					AsTypeConversion = new CondInterval(lex);
				}
				else
				{
					AsTypeConversion = new SqlType(lex);
				}
			}
		}

		public string ParentRecordName()
		{
			int pos = RecordName.LastIndexOf('.');
			if (0 > pos)
			{
				return RecordName;
			}
			return RecordName.Substring(pos + 1);
		}

		public override void GetParameters(List<CondParam> lst)
		{
			lst.Add(this);

			if (NullIndicatorField != null)
			{
				lst.Add(NullIndicatorField);
			}
		}
	}
}
