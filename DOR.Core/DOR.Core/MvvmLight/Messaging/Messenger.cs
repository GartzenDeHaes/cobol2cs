﻿// **************************************************************************
// <copyright file="Messenger.cs" company="GalaSoft Laurent Bugnion">
// Copyright © GalaSoft Laurent Bugnion 2009-2010
// </copyright>
// ****************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>13.4.2009</date>
// <project>GalaSoft.MvvmLight.Messaging</project>
// <web>http://www.galasoft.ch</web>
// <license>
// See license.txt in this project or http://www.galasoft.ch/license_MIT.txt
// </license>
// <LastBaseLevel>BL0010</LastBaseLevel>
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using GalaSoft.MvvmLight.Helpers;

namespace GalaSoft.MvvmLight.Messaging
{
	/// <summary>
	/// The Messenger is a class allowing objects to exchange messages.
	/// </summary>
	////[ClassInfo(typeof(Messenger),
	////    VersionString = "3.0.0.0",
	////    DateString = "201003041420",
	////    Description = "A messenger class allowing a class to send a message to multiple recipients",
	////    UrlContacts = "http://www.galasoft.ch/contact_en.html",
	////    Email = "laurent@galasoft.ch")]
	public class Messenger : IMessenger
	{
		private static Messenger _defaultInstance;

		private Dictionary<Type, List<WeakActionAndToken>> _recipientsOfSubclassesAction;

		private Dictionary<Type, List<WeakActionAndToken>> _recipientsStrictAction;

		private Dictionary<Type, List<Tuple<object, Type, object>>> _unsent = new Dictionary<Type, List<Tuple<object, Type, object>>>();

		/// <summary>
		/// Gets the Messenger's default instance, allowing
		/// to register and send messages in a static manner.
		/// </summary>
		public static Messenger Default
		{
			get
			{
				if (_defaultInstance == null)
				{
					_defaultInstance = new Messenger();
				}

				return _defaultInstance;
			}
		}

		/// <summary>
		/// Provides a way to override the Messenger.Default instance with
		/// a custom instance, for example for unit testing purposes.
		/// </summary>
		/// <param name="newMessenger">The instance that will be used as Messenger.Default.</param>
		public static void OverrideDefault(Messenger newMessenger)
		{
			_defaultInstance = newMessenger;
		}

		/// <summary>
		/// Sets the Messenger's default (static) instance to null.
		/// </summary>
		public static void Reset()
		{
			_defaultInstance = null;
		}

		/// <summary>
		/// Registers a recipient for a type of message TMessage. The action
		/// parameter will be executed when a corresponding message is sent.
		/// <para>Registering a recipient does not create a hard reference to it,
		/// so if this recipient is deleted, no memory leak is caused.</para>
		/// </summary>
		/// <typeparam name="TMessage">The type of message that the recipient registers
		/// for.</typeparam>
		/// <param name="recipient">The recipient that will receive the messages.</param>
		/// <param name="action">The action that will be executed when a message
		/// of type TMessage is sent.</param>
		public virtual void Register<TMessage>(object recipient, Action<TMessage> action)
		{
			Register(recipient, null, false, action);
		}

		/// <summary>
		/// Registers a recipient for a type of message TMessage.
		/// The action parameter will be executed when a corresponding 
		/// message is sent. See the receiveDerivedMessagesToo parameter
		/// for details on how messages deriving from TMessage (or, if TMessage is an interface,
		/// messages implementing TMessage) can be received too.
		/// <para>Registering a recipient does not create a hard reference to it,
		/// so if this recipient is deleted, no memory leak is caused.</para>
		/// </summary>
		/// <typeparam name="TMessage">The type of message that the recipient registers
		/// for.</typeparam>
		/// <param name="recipient">The recipient that will receive the messages.</param>
		/// <param name="receiveDerivedMessagesToo">If true, message types deriving from
		/// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
		/// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
		/// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
		/// and ExecuteOrderMessage to the recipient that registered.
		/// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
		/// transmitted to the recipient. For example, if a SendOrderMessage
		/// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
		/// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
		/// and ExecuteOrderMessage to the recipient that registered.</para>
		/// </param>
		/// <param name="action">The action that will be executed when a message
		/// of type TMessage is sent.</param>
		public virtual void Register<TMessage>(object recipient, bool receiveDerivedMessagesToo, Action<TMessage> action)
		{
			Register(recipient, null, receiveDerivedMessagesToo, action);
		}

		/// <summary>
		/// Registers a recipient for a type of message TMessage.
		/// The action parameter will be executed when a corresponding 
		/// message is sent.
		/// <para>Registering a recipient does not create a hard reference to it,
		/// so if this recipient is deleted, no memory leak is caused.</para>
		/// </summary>
		/// <typeparam name="TMessage">The type of message that the recipient registers
		/// for.</typeparam>
		/// <param name="recipient">The recipient that will receive the messages.</param>
		/// <param name="token">A token for a messaging channel. If a recipient registers
		/// using a token, and a sender sends a message using the same token, then this
		/// message will be delivered to the recipient. Other recipients who did not
		/// use a token when registering (or who used a different token) will not
		/// get the message. Similarly, messages sent without any token, or with a different
		/// token, will not be delivered to that recipient.</param>
		/// <param name="action">The action that will be executed when a message
		/// of type TMessage is sent.</param>
		public virtual void Register<TMessage>(object recipient, object token, Action<TMessage> action)
		{
			Register(recipient, token, false, action);
		}

		/// <summary>
		/// Registers a recipient for a type of message TMessage.
		/// The action parameter will be executed when a corresponding 
		/// message is sent. See the receiveDerivedMessagesToo parameter
		/// for details on how messages deriving from TMessage (or, if TMessage is an interface,
		/// messages implementing TMessage) can be received too.
		/// <para>Registering a recipient does not create a hard reference to it,
		/// so if this recipient is deleted, no memory leak is caused.</para>
		/// </summary>
		/// <typeparam name="TMessage">The type of message that the recipient registers
		/// for.</typeparam>
		/// <param name="recipient">The recipient that will receive the messages.</param>
		/// <param name="token">A token for a messaging channel. If a recipient registers
		/// using a token, and a sender sends a message using the same token, then this
		/// message will be delivered to the recipient. Other recipients who did not
		/// use a token when registering (or who used a different token) will not
		/// get the message. Similarly, messages sent without any token, or with a different
		/// token, will not be delivered to that recipient.</param>
		/// <param name="receiveDerivedMessagesToo">If true, message types deriving from
		/// TMessage will also be transmitted to the recipient. For example, if a SendOrderMessage
		/// and an ExecuteOrderMessage derive from OrderMessage, registering for OrderMessage
		/// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
		/// and ExecuteOrderMessage to the recipient that registered.
		/// <para>Also, if TMessage is an interface, message types implementing TMessage will also be
		/// transmitted to the recipient. For example, if a SendOrderMessage
		/// and an ExecuteOrderMessage implement IOrderMessage, registering for IOrderMessage
		/// and setting receiveDerivedMessagesToo to true will send SendOrderMessage
		/// and ExecuteOrderMessage to the recipient that registered.</para>
		/// </param>
		/// <param name="action">The action that will be executed when a message
		/// of type TMessage is sent.</param>
		public virtual void Register<TMessage>
		(
			 object recipient,
			 object token,
			 bool receiveDerivedMessagesToo,
			 Action<TMessage> action
		)
		{
			var messageType = typeof(TMessage);

			Dictionary<Type, List<WeakActionAndToken>> recipients;

			if (receiveDerivedMessagesToo)
			{
				if (_recipientsOfSubclassesAction == null)
				{
					_recipientsOfSubclassesAction = new Dictionary<Type, List<WeakActionAndToken>>();
				}

				recipients = _recipientsOfSubclassesAction;
			}
			else
			{
				if (_recipientsStrictAction == null)
				{
					_recipientsStrictAction = new Dictionary<Type, List<WeakActionAndToken>>();
				}

				recipients = _recipientsStrictAction;
			}

			List<WeakActionAndToken> list;

			if (!recipients.ContainsKey(messageType))
			{
				list = new List<WeakActionAndToken>();
				recipients.Add(messageType, list);
			}
			else
			{
				list = recipients[messageType];
			}

			var weakAction = new WeakAction<TMessage>(recipient, action);
			var item = new WeakActionAndToken
			{
				Action = weakAction,
				Token = token
			};
			list.Add(item);

			Cleanup();

			if (_unsent.ContainsKey(messageType))
			{
				List<Tuple<object, Type, object>> lst = _unsent[messageType];
				_unsent[messageType] = null;

				var actions = _recipientsStrictAction[messageType];

				foreach (var message in lst)
				{
					SendToList(message.Item1, actions, message.Item2, message.Item3);
				}
			}
		}

		/// <summary>
		/// Sends a message to registered recipients. The message will
		/// reach all recipients that registered for this message type
		/// using one of the Register methods.
		/// </summary>
		/// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
		/// <param name="message">The message to send to registered recipients.</param>
		public virtual void Send<TMessage>(TMessage message)
		{
			SendToTargetOrType(message, null, null);
		}

		/// <summary>
		/// Sends a message to registered recipients. The message will
		/// reach only recipients that registered for this message type
		/// using one of the Register methods, and that are
		/// of the targetType.
		/// </summary>
		/// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
		/// <typeparam name="TTarget">The type of recipients that will receive
		/// the message. The message won't be sent to recipients of another type.</typeparam>
		/// <param name="message">The message to send to registered recipients.</param>
		[SuppressMessage(
			 "Microsoft.Design",
			 "CA1004:GenericMethodsShouldProvideTypeParameter",
			 Justification = "This syntax is more convenient than other alternatives.")]
		public virtual void Send<TMessage, TTarget>(TMessage message)
		{
			SendToTargetOrType(message, typeof(TTarget), null);
		}

		/// <summary>
		/// Sends a message to registered recipients. The message will
		/// reach only recipients that registered for this message type
		/// using one of the Register methods, and that are
		/// of the targetType.
		/// </summary>
		/// <typeparam name="TMessage">The type of message that will be sent.</typeparam>
		/// <param name="message">The message to send to registered recipients.</param>
		/// <param name="token">A token for a messaging channel. If a recipient registers
		/// using a token, and a sender sends a message using the same token, then this
		/// message will be delivered to the recipient. Other recipients who did not
		/// use a token when registering (or who used a different token) will not
		/// get the message. Similarly, messages sent without any token, or with a different
		/// token, will not be delivered to that recipient.</param>
		public virtual void Send<TMessage>(TMessage message, object token)
		{
			SendToTargetOrType(message, null, token);
		}

		/// <summary>
		/// Unregisters a messager recipient completely. After this method
		/// is executed, the recipient will not receive any messages anymore.
		/// </summary>
		/// <param name="recipient">The recipient that must be unregistered.</param>
		public virtual void Unregister(object recipient)
		{
			UnregisterFromLists(recipient, _recipientsOfSubclassesAction);
			UnregisterFromLists(recipient, _recipientsStrictAction);
		}

		/// <summary>
		/// Unregisters a message recipient for a given type of messages only. 
		/// After this method is executed, the recipient will not receive messages
		/// of type TMessage anymore, but will still receive other message types (if it
		/// registered for them previously).
		/// </summary>
		/// <typeparam name="TMessage">The type of messages that the recipient wants
		/// to unregister from.</typeparam>
		/// <param name="recipient">The recipient that must be unregistered.</param>
		[SuppressMessage(
			 "Microsoft.Design",
			 "CA1004:GenericMethodsShouldProvideTypeParameter",
			 Justification =
				  "The type parameter TMessage identifies the message type that the recipient wants to unregister for.")]
		public virtual void Unregister<TMessage>(object recipient)
		{
			Unregister<TMessage>(recipient, null);
		}

		/// <summary>
		/// Unregisters a message recipient for a given type of messages and for
		/// a given action. Other message types will still be transmitted to the
		/// recipient (if it registered for them previously). Other actions that have
		/// been registered for the message type TMessage and for the given recipient (if
		/// available) will also remain available.
		/// </summary>
		/// <typeparam name="TMessage">The type of messages that the recipient wants
		/// to unregister from.</typeparam>
		/// <param name="recipient">The recipient that must be unregistered.</param>
		/// <param name="action">The action that must be unregistered for
		/// the recipient and for the message type TMessage.</param>
		public virtual void Unregister<TMessage>(object recipient, Action<TMessage> action)
		{
			UnregisterFromLists(recipient, action, _recipientsStrictAction);
			UnregisterFromLists(recipient, action, _recipientsOfSubclassesAction);
			Cleanup();
		}

		private static void CleanupList(IDictionary<Type, List<WeakActionAndToken>> lists)
		{
			if (lists == null)
			{
				return;
			}

			var listsToRemove = new List<Type>();
			foreach (var list in lists)
			{
				var recipientsToRemove = new List<WeakActionAndToken>();
				foreach (var item in list.Value)
				{
					if (item.Action == null
						 || !item.Action.IsAlive)
					{
						recipientsToRemove.Add(item);
					}
				}

				foreach (var recipient in recipientsToRemove)
				{
					list.Value.Remove(recipient);
				}

				if (list.Value.Count == 0)
				{
					listsToRemove.Add(list.Key);
				}
			}

			foreach (var key in listsToRemove)
			{
				lists.Remove(key);
			}
		}

		private static bool Implements(Type instanceType, Type interfaceType)
		{
			if (interfaceType == null
				 || instanceType == null)
			{
				return false;
			}

			var interfaces = instanceType.GetInterfaces();
			foreach (var currentInterface in interfaces)
			{
				if (currentInterface == interfaceType)
				{
					return true;
				}
			}

			return false;
		}

		private static void SendToList<TMessage>(
			 TMessage message,
			 IEnumerable<WeakActionAndToken> list,
			 Type messageTargetType,
			 object token)
		{
			if (list != null)
			{
				// Clone to protect from people registering in a "receive message" method
				// Bug correction Messaging BL0004.007
				var listClone = list.Take(list.Count()).ToList();

				foreach (var item in listClone)
				{
					var executeAction = item.Action as IExecuteWithObject;

					if (executeAction != null
						 && item.Action.IsAlive
						 && item.Action.Target != null
						 && (messageTargetType == null
							  || item.Action.Target.GetType() == messageTargetType
							  || Implements(item.Action.Target.GetType(), messageTargetType))
						 && ((item.Token == null && token == null)
							  || item.Token != null && item.Token.Equals(token)))
					{
						executeAction.ExecuteWithObject(message);
					}
				}
			}
		}

		private static void UnregisterFromLists(object recipient, Dictionary<Type, List<WeakActionAndToken>> lists)
		{
			if (recipient == null
				 || lists == null
				 || lists.Count == 0)
			{
				return;
			}

			lock (lists)
			{
				foreach (var messageType in lists.Keys)
				{
					foreach (var item in lists[messageType])
					{
						var weakAction = item.Action;

						if (weakAction != null
							 && recipient == weakAction.Target)
						{
							weakAction.MarkForDeletion();
						}
					}
				}
			}
		}

		private static void UnregisterFromLists<TMessage>(
			 object recipient,
			 Action<TMessage> action,
			 Dictionary<Type, List<WeakActionAndToken>> lists)
		{
			var messageType = typeof(TMessage);

			if (recipient == null
				 || lists == null
				 || lists.Count == 0
				 || !lists.ContainsKey(messageType))
			{
				return;
			}

			lock (lists)
			{
				foreach (var item in lists[messageType])
				{
					var weakActionCasted = item.Action as WeakAction<TMessage>;

					if (weakActionCasted != null
						 && recipient == weakActionCasted.Target
						 && (action == null
							  || action == weakActionCasted.Action))
					{
						item.Action.MarkForDeletion();
					}
				}
			}
		}

		private void Cleanup()
		{
			CleanupList(_recipientsOfSubclassesAction);
			CleanupList(_recipientsStrictAction);
		}

		private void SendToTargetOrType<TMessage>(TMessage message, Type messageTargetType, object token)
		{
			var messageType = typeof(TMessage);
			bool sent = false;

			if (_recipientsOfSubclassesAction != null)
			{
				// Clone to protect from people registering in a "receive message" method
				// Bug correction Messaging BL0008.002
				var listClone = _recipientsOfSubclassesAction.Keys.Take(_recipientsOfSubclassesAction.Count()).ToList();

				foreach (var type in listClone)
				{
					List<WeakActionAndToken> list = null;

					if (messageType == type
						 || messageType.IsSubclassOf(type)
						 || Implements(messageType, type))
					{
						list = _recipientsOfSubclassesAction[type];
					}

					SendToList(message, list, messageTargetType, token);
					sent = true;
				}
			}

			if (_recipientsStrictAction != null)
			{
				if (_recipientsStrictAction.ContainsKey(messageType))
				{
					var list = _recipientsStrictAction[messageType];
					SendToList(message, list, messageTargetType, token);

					sent = true;
				}
			}

			Cleanup();

			if (!sent)
			{
				if (! _unsent.ContainsKey(messageType))
				{
					_unsent.Add(messageType, new List<Tuple<object, Type, object>>());
				}

				_unsent[messageType].Add(new Tuple<object, Type, object>(message, messageTargetType, token));
			}
		}

		private struct WeakActionAndToken
		{
			public WeakAction Action;

			public object Token;
		}
	}
}