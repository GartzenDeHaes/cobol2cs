using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core.Collections;
using DOR.Core;
using CobolParser.Expressions;
using CobolParser.Expressions.Terms;

namespace CobolParser.Records
{
	public class OffsetAttributes
	{
		public Picture Pic
		{
			get;
			set;
		}

		public IExpr DefaultValue
		{
			get;
			private set;
		}

		/// <summary>
		/// 10  W20-CTL-LINE-TEXT             PIC X(35) VALUE ALL "X".
		/// </summary>
		public bool ValueExpandToSize
		{
			get;
			private set;
		}

		/// <summary>
		/// From the 88 field
		/// </summary>
		public List<FieldOption88> ValueChoices
		{
			get;
			private set;
		}

		public List<StringNode> ValueConstraints
		{
			get;
			private set;
		}

		public string IndexedBy
		{
			get;
			private set;
		}

		public ValueList AscendingKey
		{
			get;
			private set;
		}

		public Occurances Occures
		{
			get;
			set;
		}

		public bool IsSignLeadingSeparate
		{
			get;
			private set;
		}

		public bool IsBlankWhenZero
		{
			get;
			private set;
		}

		public string Justification
		{
			get;
			private set;
		}

		public string Heading
		{
			get;
			private set;
		}

		public string Comments
		{
			get;
			set;
		}

		public OffsetAttributes()
		{
			ValueChoices = new List<FieldOption88>();
			ValueConstraints = new List<StringNode>();
			Occures = new Occurances();
		}

		public void Parse(string name, Terminalize terms)
		{
			while(true)
			{
				if (Picture.IsPictureKeyword(terms))
				{
					Pic = new Picture(terms);
					if (DefaultValue == null)
					{
						if (Pic.IsNativeBinary || Pic.PicFormat.IsNumeric)
						{
							DefaultValue = new Expr(new Number(0));
						}
						else
						{
							DefaultValue = new Expr(new StringLit(""));
						}
					}
				}
				else if (terms.CurrentEquals("TYPE"))
				{
					terms.Match("TYPE");
					if (terms.Current.Type == StringNodeType.Star)
					{
						Offset myType = ImportManager.GetDllDef(name);
						if (null == myType)
						{
							terms.Match(StringNodeType.Star);
							continue;
						}
						Pic = myType.Attributes.Pic;
						if (null == Comments)
						{
							Comments = myType.Comments;
						}
						terms.Match(StringNodeType.Star);
					}
					else
					{
						Offset o = ImportManager.GetDllDef(terms.Current.Str);
						if (o != null)
						{
							Pic = o.Attributes.Pic;
						}
						terms.Next();
					}
				}
				else if (terms.CurrentEquals("VALUES"))
				{
					terms.Match("VALUES");
					terms.MatchOptional("ARE");

					while 
					(
						terms.Current.Type == StringNodeType.Quoted || 
						terms.Current.Type == StringNodeType.Number ||
						terms.CurrentEquals("ZEROES") ||
						terms.CurrentEquals("ZEROS") ||
						terms.CurrentEquals("ZERO")
					)
					{
						ValueConstraints.Add(terms.Current);
						terms.Next();
						terms.MatchOptional(",");
					}
				}
				else if
				(
					terms.CurrentEquals("VALUE")
				)
				{
					terms.Next();

					terms.MatchOptional("ARE");

					if (terms.CurrentEquals("ALL"))
					{
						terms.Next();
						ValueExpandToSize = true;
					}
					terms.MatchOptional("IS");
					DefaultValue = IExpr.Parse(terms);
				}
				else if (terms.CurrentEquals("OCCURS"))
				{
					Occures = new Occurances(terms);
				}
				else if (terms.CurrentEquals("INDEXED"))
				{
					terms.Match("INDEXED");
					terms.MatchOptional("BY");
					IndexedBy = terms.Current.Str;
					terms.Next();
				}
				else if (terms.CurrentEquals("ASCENDING"))
				{
					terms.Match("ASCENDING");
					terms.MatchOptional("KEY");
					terms.MatchOptional("IS");
					AscendingKey = new ValueList(terms);
				}
				else if (terms.CurrentEquals("COMP"))
				{
					terms.Next();
					terms.MatchOptional("SYNC");
				}
				else if (terms.CurrentEquals("BLANK"))
				{
					terms.Match("BLANK");
					terms.Match("WHEN");
					terms.Match("ZERO");
					IsBlankWhenZero = true;
				}
				else if (terms.CurrentEquals("JUSTIFIED"))
				{
					terms.Match("JUSTIFIED");
					Justification = terms.Current.Str;
					terms.Next();
				}
				else if (terms.CurrentEquals("SIGN"))
				{
					terms.Match("SIGN");
					terms.MatchOptional("LEADING");
					terms.MatchOptional("SEPARATE");
					Pic.IsSignLeadingSeperate = true;
				}
				else if (terms.CurrentEquals("COMP-3"))
				{
					terms.Next();
				}
				else if (terms.CurrentEquals("HEADING"))
				{
					terms.Next();
					Heading = StringHelper.StripQuotes(terms.Current.Str);
					terms.Next();
				}
				else
				{
					return;
				}
			}
		}
	}
}
