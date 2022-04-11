﻿// ****************************************************************************
// <copyright file="DispatcherHelper.cs" company="GalaSoft Laurent Bugnion">
// Copyright © GalaSoft Laurent Bugnion 2009-2012
// </copyright>
// ****************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>29.11.2009</date>
// <project>GalaSoft.MvvmLight</project>
// <web>http://www.galasoft.ch</web>
// <license>
// See license.txt in this solution or http://www.galasoft.ch/license_MIT.txt
// </license>
// <LastBaseLevel>BL0004</LastBaseLevel>
// ****************************************************************************

using System;

#if NETFX_CORE
using Windows.UI.Core;
using Windows.UI.Xaml;
#else
using System.Windows.Threading;

#if SILVERLIGHT
using System.Windows;
#endif
#endif

////using GalaSoft.Utilities.Attributes;

namespace GalaSoft.MvvmLight.Threading
{
	/// <summary>
	/// Helper class for dispatcher operations on the UI thread.
	/// </summary>
	//// [ClassInfo(typeof(DispatcherHelper),
	////  VersionString = "4.0.4",
	////  DateString = "201206191330",
	////  Description = "Helper class for dispatcher operations on the UI thread.",
	////  UrlContacts = "http://www.galasoft.ch/contact_en.html",
	////  Email = "laurent@galasoft.ch")]
	public static class DispatcherHelper
	{
		/// <summary>
		/// Gets a reference to the UI thread's dispatcher, after the
		/// <see cref="Initialize" /> method has been called on the UI thread.
		/// </summary>
#if NETFX_CORE
        public static CoreDispatcher UIDispatcher
#else
		public static Dispatcher UIDispatcher
#endif
		{
			get;
			private set;
		}

		/// <summary>
		/// Executes an action on the UI thread. If this method is called
		/// from the UI thread, the action is executed immendiately. If the
		/// method is called from another thread, the action will be enqueued
		/// on the UI thread's dispatcher and executed asynchronously.
		/// <para>For additional operations on the UI thread, you can get a
		/// reference to the UI thread's dispatcher thanks to the property
		/// <see cref="UIDispatcher" /></para>.
		/// </summary>
		/// <param name="action">The action that will be executed on the UI
		/// thread.</param>
		public static void CheckBeginInvokeOnUI(Action action)
		{
#if NETFX_CORE
            if (UIDispatcher.HasThreadAccess)
#else
			if (UIDispatcher.CheckAccess())
#endif
			{
				action();
			}
			else
			{
#if NETFX_CORE
                UIDispatcher.RunAsync(CoreDispatcherPriority.Normal,  () => action());
#else
				UIDispatcher.BeginInvoke(action);
#endif
			}
		}

		public static void CheckInvokeOnUI(Action action)
		{
			if (UIDispatcher.CheckAccess())
			{
				action();
			}
			else
			{
				action.Invoke();
			}
		}

		/// <summary>
		/// Invokes an action asynchronously on the UI thread.
		/// </summary>
		/// <param name="action">The action that must be executed.</param>
		public static void InvokeAsync(Action action)
		{
#if NETFX_CORE
            UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
#else
			UIDispatcher.BeginInvoke(action);
#endif
		}

		/// <summary>
		/// This method should be called once on the UI thread to ensure that
		/// the <see cref="UIDispatcher" /> property is initialized.
		/// <para>In a Silverlight application, call this method in the
		/// Application_Startup event handler, after the MainPage is constructed.</para>
		/// <para>In WPF, call this method on the static App() constructor.</para>
		/// </summary>
		public static void Initialize()
		{
#if SILVERLIGHT
			if (UIDispatcher != null)
#else
#if NETFX_CORE
            if (UIDispatcher != null)
#else
            if (UIDispatcher != null
                && UIDispatcher.Thread.IsAlive)
#endif
#endif
			{
				return;
			}

#if NETFX_CORE
            UIDispatcher = Window.Current.Dispatcher;
#else
#if SILVERLIGHT
			UIDispatcher = Deployment.Current.Dispatcher;
#else
            UIDispatcher = Dispatcher.CurrentDispatcher;
#endif
#endif
		}
	}
}
