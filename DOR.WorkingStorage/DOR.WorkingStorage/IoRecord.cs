using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DOR.Core.Collections;

namespace DOR.WorkingStorage
{
	public delegate void IoAfterError();

	public enum IoStreamMode
	{
		Input,
		Output
	}

	public class IoRecord
	{
		public IBufferOffset[] Records
		{
			get;
			private set;
		}

		public string FileName
		{
			get;
			private set;
		}

		public IBufferOffset FileStatusRecord
		{
			get;
			private set;
		}

		public IoAfterError IoAfterErrorHandler
		{
			get;
			set;
		}

		public IoStreamMode Mode
		{
			get;
			private set;
		}

		public bool IsEof
		{
			get;
			private set;
		}

		public bool IsInvalidKey
		{
			get;
			private set;
		}

		protected Queue<IBufferOffset> Inputs = new Queue<IBufferOffset>();

		public Vector<IBufferOffset> Outputs = new Vector<IBufferOffset>(2);

		public IoRecord(IBufferOffset[] records, string fileName, IBufferOffset fileStatusRec)
		{
			Records = records;
			FileName = fileName;
			FileStatusRecord = fileStatusRec;
		}

		public void Add(IBufferOffset rec)
		{
			Inputs.Enqueue(rec);
		}

		public void Open(IoStreamMode mode)
		{
			Mode |= mode;
		}

		public void Close()
		{
		}

		public void Read()
		{
			if (Inputs.Count == 0)
			{
				IsEof = true;
				return;
			}

			IBufferOffset r = Inputs.Dequeue();
			for (int x = 0; x < Records.Length; x++)
			{
				Records[x].Set(r);
			}
		}

		public void Read(WsRecord rec)
		{
			for (int x = 0; x < Records.Length; x++)
			{
				if (rec.Name.Equals(Records[x].Name, StringComparison.InvariantCultureIgnoreCase))
				{
					Records[x].Set(Inputs.Dequeue());
					break;
				}
			}
		}

		public void Write()
		{
			Outputs.Add(Records[0]);
		}

		public void Write(WsRecord rec)
		{
			Outputs.Add(rec);
		}
	}
}
