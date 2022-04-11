/*
 *   This file is part of the Maximum library.
 *
 *   Maximum is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   Maximum is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with Maximum.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace DOR.Core.Config
{
	/// <summary>
	/// Parse commandline argument.  UN*X or windows style swtiches can be handled.
	/// </summary>
	public class ProgramArguments
	{
		private List<ConfigurationParameter> m_switchList = new List<ConfigurationParameter>();
		private Dictionary<string, ConfigurationParameter> m_switchIdx = new Dictionary<string, ConfigurationParameter>();
		private string[] m_params = new string[0];
		private string m_allowedSwitches;

		private void Init(string[] args, string allowedSwitches)
		{
			m_allowedSwitches = allowedSwitches;

			if (args.Length == 0)
			{
				return;
			}
			List<string> parms = new List<string>();
			bool win32ParamStyle = true;
			int splitCharPos;
			int nameStart;

			if ( args.Length > 0 )
			{
				if (args[0][0] != '/')
				{
					if (args[0].Length > 1 && args[0][0] == '"' && args[0][1] == '/')
					{
						win32ParamStyle = true;
					}
					else
					{
						win32ParamStyle = false;
					}
				}
			}

			for (int x = 0; x < args.Length; x++)
			{
				ConfigurationParameter arg = new ConfigurationParameter();
				string part = UnQuote(args[x]);
				if (win32ParamStyle)
				{
					// /switch:value
					if (part[0] != '/')
					{
						parms.Add(part);
						continue;
					}
					splitCharPos = part.IndexOf(':');
					nameStart = 1;
				}
				else
				{
					if (part.Length == 0 )
					{
						continue;
					}
					if (part[0] != '-')
					{
						parms.Add(part);
						continue;
					}
					if (part[1] == '-')
					{
						nameStart = 2;
					}
					else
					{
						nameStart = 1;
					}
					splitCharPos = part.IndexOf('=');
				}
				if (-1 == splitCharPos)
				{
					// bare switch
					arg.Name = part.Substring(nameStart);
					arg.Value = "";
				}
				else
				{
					arg.Name = part.Substring(nameStart, splitCharPos - nameStart);
					arg.Value = UnQuote(part.Substring(splitCharPos + 1));
				}
				if (null != allowedSwitches && allowedSwitches.IndexOf("|" + arg.Name + "|") < 0)
				{
					throw new ConfigurationException("Unknown switch " + arg.Name);
				}
				m_switchList.Add(arg);
				m_switchIdx.Add(arg.Name, arg);
			}
			m_params = parms.ToArray();
		}

		/// <summary>
		/// Parse the arguments and throw an exception if a switch is not in the allowedSwtiches list.
		/// </summary>
		/// <param name="args">Command line args.</param>
		/// <param name="allowedSwitches">Pipe delimited switch list ( fe: "|?|help|" ).</param>
		public ProgramArguments(string[] args, string allowedSwitches)
		{
			Init(args, allowedSwitches);
		}

		public ProgramArguments(string[] args)
		{
			Init(args, null);
		}

		private string UnQuote(string str)
		{
			if (str.Length == 0 || str[0] != '"')
			{
				return str;
			}
			if (!str.EndsWith("\""))
			{
				return str;
			}
			return str.Substring(1, str.Length - 2);
		}

		public int Count
		{
			get { return m_switchList.Count; }
		}

		public string this[int idx]
		{
			get { return (string)m_switchList[idx].Value; }
		}

		public string this[string key]
		{
			get 
			{
				if (!String.IsNullOrEmpty(m_allowedSwitches) && m_allowedSwitches.IndexOf("|" + key + "|") < 0)
				{
					throw new ArgumentException("Invalid switch " + key);
				}
				if (m_switchIdx.ContainsKey(key))
				{
					return (string)m_switchIdx[key].Value;
				}
				return null;
			}
		}

		public bool HasSwitch(string key)
		{
			if (! String.IsNullOrEmpty(m_allowedSwitches) && m_allowedSwitches.IndexOf("|" + key + "|") < 0)
			{
				throw new ArgumentException("Invalid switch " + key);
			}
			return m_switchIdx.ContainsKey(key);
		}

		public bool HasAnyOfSwitches(string keys, char delim)
		{
			string[] sws = keys.Split(new char[] { delim });
			for (int x = 0; x < sws.Length; x++)
			{
				if ( HasSwitch(sws[x]) )
				{
					return true;
				}
			}
			return false;
		}

		public string SwitchAt(int idx)
		{
			return m_switchList[idx].Name;
		}

		public string[] Parameters
		{
			get { return m_params; }
		}
	}
}
