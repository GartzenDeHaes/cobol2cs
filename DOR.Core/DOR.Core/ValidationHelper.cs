using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DOR.Core
{
	public static class ValidationHelper
	{
		public static bool Required
		(
			List<ValidationMessage> msgs,
			string propertyName,
			object val,
			string message = "{0} is required."
		)
		{
			if (null == val || (val is string && ((string)val).Length == 0))
			{
				msgs.Add(new ValidationMessage(false, propertyName, -1, String.Format(message, propertyName), ""));
				return false;
			}
			return true;
		}

		public static bool Range<T>
		(
			List<ValidationMessage> msgs,
			string propertyName,
			T value,
			T min,
			T max,
			string message = "{0} must be between {1} and {2}."
		) where T : IComparable<T>
		{
			if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
			{
				msgs.Add(new ValidationMessage(false, propertyName, -1, String.Format(message, propertyName, min, max), ""));
				return false;
			}
			return true;
		}

		public static bool Range
		(
			List<ValidationMessage> msgs,
			string propertyName,
			Int32? value,
			int min,
			int max,
			string message = "{0} must be between {1} and {2}."
		)
		{
			if (!Required(msgs, propertyName, value))
			{
				return false;
			}
			return Range<Int32>(msgs, propertyName, value.Value, min, max, message);
		}

		public static bool Range
		(
			List<ValidationMessage> msgs,
			string propertyName,
			Int16? value,
			Int16 min,
			Int16 max,
			string message = "{0} must be between {1} and {2}."
		)
		{
			if (!Required(msgs, propertyName, value))
			{
				return false;
			}
			return Range<Int16>(msgs, propertyName, value.Value, min, max, message);
		}

		public static bool Range
		(
			List<ValidationMessage> msgs,
			string propertyName,
			Int64? value,
			long min,
			long max,
			string message = "{0} must be between {1} and {2}."
		)
		{
			if (!Required(msgs, propertyName, value))
			{
				return false;
			}
			return Range<Int64>(msgs, propertyName, value.Value, min, max, message);
		}

		public static bool Range
		(
			List<ValidationMessage> msgs,
			string propertyName,
			Byte? value,
			Byte min,
			Byte max,
			string message = "{0} must be between {1} and {2}."
		)
		{
			if (!Required(msgs, propertyName, value))
			{
				return false;
			}
			return Range<Byte>(msgs, propertyName, value.Value, min, max, message);
		}

		public static bool Range
		(
			List<ValidationMessage> msgs,
			string propertyName,
			Double? value,
			double min,
			double max,
			string message = "{0} must be between {1} and {2}."
		)
		{
			if (!Required(msgs, propertyName, value))
			{
				return false;
			}
			return Range<Double>(msgs, propertyName, value.Value, min, max, message);
		}

		public static bool Length
		(
			List<ValidationMessage> msgs,
			string propertyName,
			string val,
			int minLen,
			int maxLen,
			string message = "{0} must be between {1} and {2} characters."
		)
		{
			if (null == val)
			{
				msgs.Add(new ValidationMessage(false, propertyName, -1, String.Format("{0} is required", propertyName), ""));
				return false;
			}
			if (val.Length > maxLen || val.Length < minLen)
			{
				msgs.Add(new ValidationMessage(false, propertyName, -1, String.Format(message, propertyName, minLen, maxLen), ""));
				return false;
			}
			return true;
		}

		///TODO: Convert to user friendly error messages.
		public static ValidationMessage FormatException(Exception ex)
		{
			if (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			return new ValidationMessage(true, "", -1, ex.Message, ex.StackTrace);
		}

		public static string ErrorCheckProperties
		(
			IDataErrorInfo obj
		)
		{
			foreach (PropertyInfo pi in obj.GetType().GetProperties())
			{
				if (! pi.PropertyType.IsPrimitive)
				{
					continue;
				}

				string msg = obj[pi.Name];
				if (!String.IsNullOrEmpty(msg))
				{
					return msg;
				}
			}

			return "";
		}

		public static string FlattenMessages(List<ValidationMessage> msgs)
		{
			StringBuilder buf = new StringBuilder();

			foreach (var m in msgs)
			{
				buf.Append(m.Message);
				buf.Append(".  ");
			}

			return buf.ToString();
		}
	}
}
