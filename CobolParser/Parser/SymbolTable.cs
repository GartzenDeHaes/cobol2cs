using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CobolParser.Records;
using CobolParser.Expressions;
using CobolParser.Expressions.Terms;
using System.Diagnostics;
using CobolParser.Verbs;

namespace CobolParser.Parser
{
	public class SymbolTable
	{
		private Dictionary<string, Symbol> _idx = new Dictionary<string, Symbol>();

		public SymbolTable()
		{
		}

		private Symbol FromITerm(ITerm term)
		{
			if (term is Id)
			{
				return Find(((Id)term).Value.Str);
			}
			else if (term is OffsetReference)
			{
				return Find(((OffsetReference)term).OffsetChain[0].Value.Str, ((OffsetReference)term).OffsetChain[((OffsetReference)term).OffsetChain.Count - 1].Value.Str);
			}
			return null;
		}

		public Symbol AddReference(ITerm term)
		{
			Symbol s = FromITerm(term);
			if (s == null)
			{
				return null;
			}
			s.References.Add(term);
			return s;
		}

		public Symbol AddSendReference(ITerm term, Send send)
		{
			Symbol s = FromITerm(term);
			if (s == null)
			{
				return null;
			}
			s.SendReferences.Add(send);
			return s;
		}

		public Symbol AddScreenReference(ITerm term, ScreenField sf)
		{
			Symbol s = FromITerm(term);
			if (s == null)
			{
				return null;
			}
			s.ScreenReferences.Add(sf);
			return s;
		}

		public Symbol Add(INamedField rec, StringNode lex)
		{
			if (rec.Name.Equals("FILLER", StringComparison.InvariantCultureIgnoreCase))
			{
				return null;
			}

			Symbol s = null;
			string fqn = rec.FullyQualifiedName;
			if (_idx.ContainsKey(fqn))
			{
				s = _idx[fqn];
				// cobol allows dup record names, see s002cl1q lines 117 and 130.
				return s;
			}
			s = new Symbol(rec, lex);
			_idx.Add(fqn, s);
			return s;
		}

		public Symbol Lookup(string fullyQualifedName)
		{
			return _idx[fullyQualifedName];
		}

		public Symbol Find(string name)
		{
			return Find(name, null);
		}

		public Symbol Find(string name, string parent)
		{
			foreach (var s in _idx.Values)
			{
				if (name.Equals(s.Record.Name, StringComparison.InvariantCultureIgnoreCase))
				{
					if (parent == null || s.Record.HasParentNamed(parent))
					{
						return s;
					}
				}
			}

			return null;
		}
	}
}
