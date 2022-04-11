using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Helpers;

namespace DOR.Core.ComponentModel
{
	[Serializable]
	public class NotifyPropertyChangedBase
	: INotifyPropertyChanged, IRaisePropertyChanged, INotifyPropertyChanging
	{
		#region INotifyPropertyChanging Members

		/// <summary>
		/// Occurs before a property value changes.
		/// </summary>
		public event PropertyChangingEventHandler PropertyChanging;


        /// <summary>
        /// Provides access to the PropertyChanging event handler to derived classes.
        /// </summary>
        protected PropertyChangingEventHandler PropertyChangingHandler
        {
            get
            {
                return PropertyChanging;
            }
        }

		/// <summary>
		/// Raises the PropertyChanging event if needed.
		/// </summary>
		/// <remarks>If the propertyName parameter
		/// does not correspond to an existing property on the current class, an
		/// exception is thrown in DEBUG configuration only.</remarks>
		/// <param name="propertyName">The name of the property that
		/// changed.</param>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
			Justification = "This cannot be an event")]
		protected virtual void RaisePropertyChanging(string propertyName)
		{
#if NETFX_CORE
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new NotSupportedException(
                    "Raising the PropertyChanged event with an empty string or null is not supported in the Windows 8 developer preview");
            }
            else
            {
#endif
			VerifyPropertyName(propertyName);

			var handler = PropertyChanging;
			if (handler != null)
			{
				handler(this, new PropertyChangingEventArgs(propertyName));
			}
#if NETFX_CORE
            }
#endif
		}

		/// <summary>
		/// Raises the PropertyChanging event if needed.
		/// </summary>
		/// <typeparam name="T">The type of the property that
		/// changes.</typeparam>
		/// <param name="propertyExpression">An expression identifying the property
		/// that changes.</param>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
			Justification = "This cannot be an event")]
		[SuppressMessage(
			"Microsoft.Design",
			"CA1006:GenericMethodsShouldProvideTypeParameter",
			Justification = "This syntax is more convenient than other alternatives.")]
		protected virtual void RaisePropertyChanging<T>(Expression<Func<T>> propertyExpression)
		{
			var handler = PropertyChanging;
			if (handler != null)
			{
				var propertyName = GetPropertyName(propertyExpression);
				handler(this, new PropertyChangingEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Extracts the name of a property from an expression.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="propertyExpression">An expression returning the property's name.</param>
		/// <returns>The name of the property returned by the expression.</returns>
		/// <exception cref="ArgumentNullException">If the expression is null.</exception>
		/// <exception cref="ArgumentException">If the expression does not represent a property.</exception>
		protected string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
		{
			if (propertyExpression == null)
			{
				throw new ArgumentNullException("propertyExpression");
			}

			var body = propertyExpression.Body as MemberExpression;

			if (body == null)
			{
				throw new ArgumentException("Invalid argument", "propertyExpression");
			}

			var property = body.Member as PropertyInfo;

			if (property == null)
			{
				throw new ArgumentException("Argument is not a property", "propertyExpression");
			}

			return property.Name;
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		public bool HasPropertyChanged
		{
			get { return PropertyChanged != null; }
		}

		/// <summary>
		/// Raises the PropertyChanged event if needed.
		/// </summary>
		/// <param name="propertyName">The name of the property that
		/// changed.</param>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
			Justification = "This cannot be an event")]
		public virtual void RaisePropertyChanged(string name)
		{
			VerifyPropertyName(name);

			if (null != PropertyChanged)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		public virtual void RaisePropertyChangedAsync(string name)
		{
			VerifyPropertyName(name);

			if (null != PropertyChanged)
			{
				BackgroundWorker worker = new BackgroundWorker();
				worker.DoWork += (ss, ee) =>
				{
					RaisePropertyChanged(name);
				};
				worker.RunWorkerAsync();
			}
		}

		/// <summary>
		/// Raises the PropertyChanged event if needed, and broadcasts a
		/// PropertyChangedMessage using the Messenger instance (or the
		/// static default instance if no Messenger instance is available).
		/// </summary>
		/// <typeparam name="T">The type of the property that
		/// changed.</typeparam>
		/// <param name="propertyName">The name of the property that
		/// changed.</param>
		/// <param name="oldValue">The property's value before the change
		/// occurred.</param>
		/// <param name="newValue">The property's value after the change
		/// occurred.</param>
		/// <param name="broadcast">If true, a PropertyChangedMessage will
		/// be broadcasted. If false, only the event will be raised.</param>
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
			Justification = "This cannot be an event")]
		protected virtual void RaisePropertyChanged<TT>(string propertyName, TT oldValue, TT newValue, bool broadcast)
		{
			RaisePropertyChanged(propertyName);

			if (broadcast)
			{
				Broadcast(oldValue, newValue, propertyName);
			}
		}

		#endregion

		#region Messaging

		/// <summary>
		/// Gets or sets an instance of a <see cref="IMessenger" /> used to
		/// broadcast messages to other objects. If null, this class will
		/// attempt to broadcast using the Messenger's default instance.
		/// </summary>
		protected IMessenger MessengerInstance
		{
			get;
			set;
		}

		/// <summary>
		/// Broadcasts a PropertyChangedMessage using either the instance of
		/// the Messenger that was passed to this class (if available) 
		/// or the Messenger's default instance.
		/// </summary>
		/// <typeparam name="T">The type of the property that
		/// changed.</typeparam>
		/// <param name="oldValue">The value of the property before it
		/// changed.</param>
		/// <param name="newValue">The value of the property after it
		/// changed.</param>
		/// <param name="propertyName">The name of the property that
		/// changed.</param>
		protected virtual void Broadcast<TT>(TT oldValue, TT newValue, string propertyName)
		{
			var message = new PropertyChangedMessage<TT>(this, oldValue, newValue, propertyName);

			if (MessengerInstance != null)
			{
				MessengerInstance.Send(message);
			}
			else
			{
				Messenger.Default.Send(message);
			}
		}

		#endregion

		#region Debugging Aides

		/// <summary>
		/// Warns the developer if this object does not have
		/// a public property with the specified name. This 
		/// method does not exist in a Release build.
		/// </summary>
		[Conditional("DEBUG")]
		[DebuggerStepThrough]
		public void VerifyPropertyName(string propertyName)
		{
			// Verify that the property name matches a real,  
			// public, instance property on this object.
#if ! SILVERLIGHT
			if (TypeDescriptor.GetProperties(this)[propertyName] == null)
			{
				Debug.WriteLine("Invalid property name: " + propertyName);
			}
#endif
		}

		#endregion // Debugging Aides
	}
}
