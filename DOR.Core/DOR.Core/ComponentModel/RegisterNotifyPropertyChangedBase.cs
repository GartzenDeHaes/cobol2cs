using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOR.Core.ComponentModel
{
	[Serializable]
	public class RegisterNotifyPropertyChangedBase : NotifyPropertyChangedBase, IRegisterNotifyPropertyChanged
	{
		private IRaisePropertyChanged _IPropertyChanged;
		private List<string> _fakeChildern;

		protected string NotifyPropertyChangedName
		{
			get;
			set;
		}

		public virtual void RegisterNotifyPropertyChanged
		(
			IRaisePropertyChanged inpc,
			string propName = null
		)
		{
			_IPropertyChanged = inpc;
			NotifyPropertyChangedName = propName;
		}

		/// <summary>
		/// Fake notifications for child
		/// </summary>
		public virtual void RegisterNotifyPropertyChangedOnChild
		(
			IRaisePropertyChanged inpc,
			string propName
		)
		{
			_IPropertyChanged = inpc;

			if (_fakeChildern == null)
			{
				_fakeChildern = new List<string>();
			}
			_fakeChildern.Add(propName);
		}

		public virtual void RaisePropertyChanged()
		{
			RaisePropertyChanged(NotifyPropertyChangedName);
		}

		public override void RaisePropertyChanged(string propertyName)
		{
			if (String.IsNullOrEmpty(propertyName))
			{
				if (_fakeChildern != null)
				{
					foreach (var c in _fakeChildern)
					{
						_IPropertyChanged.RaisePropertyChanged(c);
					}
				}
			}
			else
			{
				if (null != _IPropertyChanged)
				{
					_IPropertyChanged.RaisePropertyChanged(propertyName);
				}
				if (HasPropertyChanged)
				{
					base.RaisePropertyChanged(propertyName);
				}
			}
		}
	}
}
