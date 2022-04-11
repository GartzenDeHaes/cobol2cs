using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using DOR.Core;
using DOR.Core.Collections;
using DOR.Core.Data;
using DOR.Core.Data.Tandem;

namespace DOR.WorkingStorage
{
	public class ArgumentMap
	{
		public string Name
		{
			get;
			private set;
		}

		public string Value
		{
			get;
			private set;
		}

		public ArgumentMap(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}

	class CallRecordMap
	{
		public string MethodName
		{
			get;
			private set;
		}

		public Vector<ArgumentMap> Params = new Vector<ArgumentMap>();

		public TandemDataReader ResultSet
		{
			get;
			private set;
		}

		public CallRecordMap(string methodName, ArgumentMap[] args, TandemDataReader result)
		{
			MethodName = methodName;
			Params.AddRange(args);
			ResultSet = result;
		}
	}

	public class MockDataAccessBase
	{
		private Dictionary<string, List<CallRecordMap>> _maps = new Dictionary<string, List<CallRecordMap>>();

		public void Add(string methodName, string resultXml, params ArgumentMap[] args)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(resultXml);
			Add(methodName, doc, args);
		}

		public void Add(string methodName, XmlDocument xml, params ArgumentMap[] args)
		{
			if (!_maps.ContainsKey(methodName))
			{
				_maps.Add(methodName, new List<CallRecordMap>());
			}

			_maps[methodName].Add(new CallRecordMap(methodName, args, new TandemDataReader(xml)));
		}

		public int DispatchNonQuery(string methodName, params ArgumentMap[] args)
		{
			return Dispatch(methodName, args).RecordsAffected;
		}

		public TandemDataReader Dispatch(string methodName, params ArgumentMap[] args)
		{
			if (!_maps.ContainsKey(methodName))
			{
				return new TandemDataReader();
			}

			foreach (var cm in _maps[methodName])
			{
				if (args.Length != cm.Params.Count)
				{
					continue;
				}

				bool found = true;
				for (int x = 0; x < args.Length; x++)
				{
					if (args[x].Name != cm.Params[x].Name)
					{
						found = false;
						break;
					}
					if (args[x].Value != cm.Params[x].Value)
					{
						found = false;
						break;
					}
				}

				if (found)
				{
					return cm.ResultSet;
				}
			}

			return new TandemDataReader();
		}

		public void BeginTrans()
		{
		}

		public void CommitTrans()
		{
		}

		public void RollbackTrans()
		{
			throw new NotImplementedException();
		}
	}
}
