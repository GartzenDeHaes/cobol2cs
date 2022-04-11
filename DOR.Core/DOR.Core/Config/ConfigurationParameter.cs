using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using DOR.Core.ComponentModel;

namespace DOR.Core.Config
{
	public class ConfigurationParameter : NotifyPropertyChangedBase, IConfigurationParameter
	{
		#region Primitive Properties

		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					RaisePropertyChanged("Name");
				}
			}
		}
		private string _name;

		public string TypeName
		{
			get { return _typeName; }
			set
			{
				if (_typeName != value)
				{
					_typeName = value;
					RaisePropertyChanged("TypeName");
				}
			}
		}
		private string _typeName;

		public bool Required
		{
			get { return _required; }
			set
			{
				if (_required != value)
				{
					_required = value;
					RaisePropertyChanged("Required");
				}
			}
		}
		private bool _required;

		public string DefaultValue
		{
			get { return _defaultValue; }
			set
			{
				if (_defaultValue != value)
				{
					_defaultValue = value;
					RaisePropertyChanged("DefaultValue");
				}
			}
		}
		private string _defaultValue;

		public bool IsDefault
		{
			get { return _isDefault; }
			set
			{
				if (_isDefault != value)
				{
					_isDefault = value;
					RaisePropertyChanged("IsDefault");
				}
			}
		}
		private bool _isDefault;

		#endregion

		#region C'tor

		public ConfigurationParameter()
		{
			TypeName = "string";
		}

		public ConfigurationParameter
		(
			string name,
			bool required,
			string defaultValue,
			bool isDefault,
			string typeName,
			object value
		)
		{
			Name = name;
			Required = required;
			DefaultValue = defaultValue;
			IsDefault = isDefault;
			TypeName = typeName;
			_value = value;
		}

		public IConfigurationParameter Clone()
		{
			ConfigurationParameter p = new ConfigurationParameter();

			p.Name = Name;
			p.Required = Required;
			p.DefaultValue = DefaultValue;
			p.IsDefault = IsDefault;
			p.TypeName = TypeName;
			p._value = _value;

			return p;
		}

		#endregion

		private object _value;
		public object Value
		{
			get { return _value; }
			set
			{
				if (null == value)
				{
					_value = null;
					return;
				}
				switch (this.TypeName.ToLower())
				{
					case "int":
						if (!(value is int))
						{
							int i;
							if (!Int32.TryParse(value.ToString(), out i))
							{
								throw new InvalidCastException("Cannot convert " + value.ToString() + " to int");
							}
						}
						break;
					case "date":
						if (!Date.IsDate(value))
						{
							throw new InvalidCastException("Cannot convert " + value.ToString() + " to Date");
						}
						break;
					case "string":
						break;
					case "datetime":
						DateTime dtm;
						if (!(value is DateTime))
						{
							if (!DateTime.TryParse(value.ToString(), out dtm))
							{
								throw new InvalidCastException("Cannot convert " + value.ToString() + " to DateTime");
							}
						}
						break;
					case "double":
						if (!(value is double))
						{
							double d;
							if (!Double.TryParse(value.ToString(), out d))
							{
								throw new InvalidCastException("Cannot convert " + value.ToString() + " to Double");
							}
						}
						break;
					default:
						throw new InvalidCastException("Unsupported type in ScreenParameter table of " + this.TypeName);
				}

				_value = value;
			}
		}

		public int ToInt()
		{
			if (Value is Int32)
			{
				return (int)Value;
			}
			return Int32.Parse(Value.ToString());
		}

		public double ToDouble()
		{
			if (Value is Double)
			{
				return (double)Value;
			}
			return Double.Parse(Value.ToString());
		}

		public DateTime ToDateTime()
		{
			if (Value is DateTime)
			{
				return (DateTime)Value;
			}
			return DateTime.Parse(Value.ToString());
		}

		public Date ToDate()
		{
			if (Value is Date)
			{
				return (Date)Value;
			}
			return Date.Parse(Value.ToString());
		}

		public override string ToString()
		{
			if (null == Value)
			{
				return "";
			}
			return Value.ToString();
		}

		public override bool Equals(object obj)
		{
			if (null == obj && null == Value)
			{
				return true;
			}
			return null != Value && Value.Equals(obj);
		}

		public override int GetHashCode()
		{
			return null == Value ? 0 : Value.GetHashCode();
		}

		#region IDataErrorInfo Members

		public string Error
		{
			get
			{
				string msg;
				if ((msg = this["ScreenCode"]).Length > 0)
				{
					return msg;
				}
				if ((msg = this["Name"]).Length > 0)
				{
					return msg;
				}
				if ((msg = this["TypeName"]).Length > 0)
				{
					return msg;
				}
				if ((msg = this["Required"]).Length > 0)
				{
					return msg;
				}
				if ((msg = this["DefaultValue"]).Length > 0)
				{
					return msg;
				}
				if ((msg = this["IsDefault"]).Length > 0)
				{
					return msg;
				}

				return String.Empty;
			}
		}

		public string this[string columnName]
		{
			get
			{
				List<ValidationMessage> msgs = new List<ValidationMessage>();

				switch (columnName)
				{
					case "Name":
						ValidationHelper.Required(msgs, "Name", Name);
						ValidationHelper.Length(msgs, "Name", Name, 0, 8096);
						break;

					case "TypeName":
						ValidationHelper.Required(msgs, "Type name", TypeName);
						ValidationHelper.Length(msgs, "Type name", TypeName, 0, 8096);
						break;

					case "Required":
						ValidationHelper.Required(msgs, "Required", Required);
						break;

					case "DefaultValue":
						ValidationHelper.Required(msgs, "Default value", DefaultValue);
						ValidationHelper.Length(msgs, "Default value", DefaultValue, 0, 8096);
						break;

					case "IsDefault":
						ValidationHelper.Required(msgs, "Is default", IsDefault);
						break;
				}

				if (msgs.Count > 0)
				{
					return msgs[0].Message;
				}
				return "";
			}
		}

		#endregion
	}
}
