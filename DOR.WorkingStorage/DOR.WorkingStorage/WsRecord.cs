using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

using DOR.Core;
using DOR.Core.Collections;
using DOR.Core.ComponentModel;
using DOR.WorkingStorage.Pic;

namespace DOR.WorkingStorage
{
	[Serializable]
	public class WsRecord : IBufferOffset
	{
		private Dictionary<string, IBufferOffset> _idx = new Dictionary<string, IBufferOffset>();
		private Dictionary<string, int> _posIdx = new Dictionary<string, int>();
		private Vector<IBufferOffset> _bufs = new Vector<IBufferOffset>();

		public override string Redefines
		{
			get;
			protected set;
		}

		public int Count
		{
			get { return _idx.Count; }
		}

		public IBufferOffset this[string idx]
		{
			get { return _idx[idx]; }
		}

		private MemoryBuffer _buf;

		public override MemoryBuffer Buffer
		{
			get { return _buf; }
		}

		public override int Start
		{
			get;
			protected set;
		}

		public override int Length
		{
			get;
			protected set;
		}

		public override bool IsSpaces
		{
			get
			{
				return ToRawString().Trim().Length == 0;
			}
		}

		public bool IsSigned
		{
			get;
			protected set;
		}

		public string Default
		{
			get;
			private set;
		}

		public string DefaultValue
		{
			get;
			private set;
		}

		public override string[] Values
		{
			get;
			protected set;
		}

		public override int Occurances
		{
			get { return 1; }
		}

		public override string Name
		{
			get;
			protected set;
		}

		public override string Value
		{
			get { return ToString(); }
		}

		public override PicFormat Format
		{
			get;
			protected set;
		}

		public bool IsGroupRecord
		{
			get;
			protected set;
		}

		public WsRecord(string name, int len)
		: this(name, new MemoryBuffer(len))
		{
		}

		public WsRecord
		(
			string name, 
			MemoryBuffer buf, 
			int start, 
			int len
		)
		: this(buf, start, len, "", false, null, name, "", "BINARY-" + len)
		{
			IsGroupRecord = true;
		}

		public WsRecord(string name, MemoryBuffer buf)
		: this(name, buf, 0, buf.Length)
		{
			Redefines = "Yes";
		}

		public WsRecord
		(
			MemoryBuffer buf, 
			int start,
			int len,
			string defaultValue,
			bool isAll,
			string[] values,
			string name,
			string redefines,
			string format,
			bool isTemp = false
		)
		{
			_buf = buf;
			Start = start;
			Length = len;
			DefaultValue = defaultValue;
			Values = values;
			Name = name;
			Redefines = redefines;

			Format = PicFormat.Parse(format);

			Debug.Assert(Format.Length == Length);

			if (Start + Length > buf.Length)
			{
				throw new ArgumentException("Record exceeds buffer length");
			}

			if (!String.IsNullOrEmpty(redefines))
			{
				return;
			}

			if (isTemp)
			{
				return;
			}

			if (String.IsNullOrEmpty(defaultValue))
			{
				ClearTo(' ');
				return;
			}

			if (defaultValue == "ZEROES" || defaultValue == "ZEROS")
			{
				ClearTo('0');
				return;
			}

			if (defaultValue == "SPACE" || defaultValue == "SPACES")
			{
				defaultValue = " ";
			}

			if (isAll)
			{
				defaultValue = StringHelper.RepeatChar(defaultValue[0], Length);
			}

			Set(defaultValue);
		}

		public void Add(string name, IBufferOffset buf)
		{
			if (buf.Start + buf.Length > Buffer.Length)
			{
				throw new ArgumentException("Offset exceeds buffer size");
			}

			if (! _idx.ContainsKey(name))
			{
				// fillers can be dup
				_idx.Add(name, buf);
				_posIdx.Add(name, _bufs.Count);
			}
			_bufs.Add(buf);
		}

		public override void Initialize()
		{
			if (String.IsNullOrEmpty(Redefines))
			{
				return;
			}

			Set(Format.ToRawString(" "));

			for (int x = 0; x < _bufs.Count; x++)
			{
				_bufs[x].Initialize();
			}

			RaisePropertyChanged();
		}

		public override void Reset()
		{
			if (!String.IsNullOrEmpty(DefaultValue))
			{
				Set(DefaultValue);
			}
			else if (!String.IsNullOrEmpty(Default))
			{
				Set(Default);
			}
			else
			{
				Initialize();
			}
		}

		public override string ToDebugPrintString()
		{
			if (_idx.Keys.Count == 0)
			{
				return Buffer.ToString();
			}

			StringBuilder buf = new StringBuilder();

			foreach (string s in _idx.Keys)
			{
				IBufferOffset o = _idx[s];

				buf.Append(o.Name);
				buf.Append("=");
				buf.Append(o.ToString());
				buf.Append("\r\n");
			}

			return buf.ToString();
		}

		public void ClearTo(char b)
		{
			_buf.ClearTo(b, Start, Length);
			RaisePropertyChanged();
		}

		public override void Set(bool b)
		{
			Set(b ? 1 : 0);
		}

		public override void Set(int i)
		{
			_buf.Set(Format.ToRawString(i), Start, Length);
			RaisePropertyChanged();
		}

		public override void Set(long i)
		{
			_buf.Set(Format.ToRawString(i), Start, Length);
			RaisePropertyChanged();
		}

		public override void Set(string s)
		{
			_buf.Set(Format.ToRawString(s), Start, Length);
			RaisePropertyChanged();
		}

		public override void Set(double d)
		{
			///WARNING: Probably OK for our apps, not valid for high precision floating point
			Set((decimal)d);
		}

		public override void Set(decimal d)
		{
			_buf.Set(Format.ToRawString(d), Start, Length);
			RaisePropertyChanged();
		}

		public override void Set(NumericTemp i, bool round = false)
		{
			if (i.IsInt)
			{
				Set(i.ToInt());
			}
			else
			{
				decimal d = i.ToDecimal();
				if (round)
				{
					d = Math.Round(d, Format.Decimals);
				}
				Set(d);
			}
		}

		public override void Set(Zeroes zs)
		{
			ClearTo('0');
		}

		public override void Set(HighValues zs)
		{
			if (Format.IsNumeric)
			{
				Set(0);
			}
			else
			{
				Set(StringHelper.RepeatChar((char)0xFF, Format.Length));
			}
		}

		public override void Set(LowValues zs)
		{
			if (Format.IsNumeric)
			{
				Set(0);
			}
			else
			{
				Set(StringHelper.RepeatChar('\0', Format.Length));
			}
		}

		public override void Set(Spaces ss)
		{
			ClearTo(' ');
		}

		public override void Set(IBufferOffset ws)
		{
			if (Format.IsFloat && ws.Format.IsFloat)
			{
				Set(ws.ToDecimal());
			}
			else
			{
				Set(ws.ToRawString());
			}
		}

		public override long ToInt()
		{
			return Int64.Parse(Format.Format(_buf.ToString(Start, Length)).TrimStart());
		}

		public override long ToLong()
		{
			return Int64.Parse(Format.Format(_buf.ToString(Start, Length)).TrimStart());
		}

		public override int ToInt32()
		{
			return Int32.Parse(StringHelper.TrimLeading(Format.Format(_buf.ToString(Start, Length)), '0'));
		}

		public override int ToInt16()
		{
			return Int16.Parse(StringHelper.TrimLeading(Format.Format(_buf.ToString(Start, Length)), '0'));
		}

		public override decimal ToDecimal()
		{
			return Decimal.Parse(Format.Format(_buf.ToString(Start, Length)));
		}

		public override double ToDouble()
		{
			return Double.Parse(Format.Format(_buf.ToString(Start, Length)));
		}

		public override string ToString(string format)
		{
			return PicFormat.Parse(format).Format(ToString());
		}

		public override string ToString()
		{
			if (IsGroupRecord && _bufs.Count > 0)
			{
				StringBuilder buf = new StringBuilder();
				for (int x = 0; x < _bufs.Count; x++)
				{
					buf.Append(_bufs[x].ToString());
				}

				return buf.ToString();
			}
			else
			{
				return Format.Format(_buf.ToString(Start, Length));
			}
		}

		public override string ToRawString()
		{
			return _buf.ToString(Start, Length);
		}

		public override bool ToBool()
		{
			string value = ToString().Trim();

			if (value.Length == 0)
			{
				return false;
			}

			if (Zeroes.IsZeroes(value))
			{
				value = "0";
			}

			if (Values.Length > 0)
			{
				for (int x = 0; x < Values.Length; x++)
				{
					if (value == Values[x])
					{
						return true;
					}
				}
				return false;
			}

			return ToDecimal() != 0;
		}

		public override bool Equals(object obj)
		{
			if (obj is Boolean)
			{
				return ToBool() == (bool)obj;
			}

			if (obj is Spaces)
			{
				return String.IsNullOrWhiteSpace(ToRawString());
			}

			if (obj is IBufferOffset)
			{
				return this == (IBufferOffset)obj;
			}

			if (obj is int)
			{
				return this == (int)obj;
			}

			if (obj is decimal || obj is double)
			{
				return this == (decimal)obj;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return ToRawString().GetHashCode();
		}

		public override IBufferOffset CreateSubRange(int start, int len)
		{
			WsRecord b2 = new WsRecord(_buf, Start + start - 1, len, "", false, null, "TEMP", "", StringHelper.RepeatChar('X', len), true);
			return b2;
		}

		public override void Inc(long val)
		{
			if (Format.Decimals > 0)
			{
				Inc((decimal)val);
				return;
			}
			Set(ToInt() + val);
		}

		public override void Inc(decimal val)
		{
			Set(ToDecimal() + val);
		}

		public override void Inc(double val)
		{
			Set(ToDecimal() + (decimal)val);
		}

		public override void Inc(IBufferOffset val)
		{
			if (Format.IsFloat)
			{
				Set(ToDecimal() + val.ToDecimal());
			}
			else
			{
				Set(ToInt() + val.ToInt());
			}
		}

		public override void RaisePropertyChanged(string propertyName)
		{
			base.RaisePropertyChanged(propertyName);

			foreach (var r in _bufs)
			{
				r.RaisePropertyChanged();
			}
		}

		public override bool IsOneOf(params int[] values)
		{
			if (values == null || values.Length == 0)
			{
				return true;
			}

			int i = ToInt32();

			for (int x = 0; x < values.Length; x++)
			{
				if (values[x] == i)
				{
					return true;
				}
			}

			return false;
		}

		public override bool IsOneOf(params string[] values)
		{
			if (values == null || values.Length == 0)
			{
				return true;
			}

			string i = ToString().Trim();

			for (int x = 0; x < values.Length; x++)
			{
				if (values[x] == i)
				{
					return true;
				}
			}

			return false;
		}

		public override bool IsInRange(int low, int hi)
		{
			int i = ToInt32();
			return i >= low && i <= hi;
		}

		public override bool IsInRange(string low, string hi)
		{
			string s = ToString();

			return low.CompareTo(s) >= 0 && hi.CompareTo(s) <= 0;
		}
	}
}
