using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace DOR.Core.Collections
{
	public class MemCache : IDisposable
	{
		/// <summary>
		/// Defines an item that's stored in the Cache.
		/// </summary>
		private class CacheItem
		{
			private readonly DateTime _creationDate = DateTime.Now;

			private readonly TimeSpan _validDuration;

			/// <summary>
			/// Constructs a cache item with for the data with a validity of validDuration.
			/// </summary>
			/// <param name="data">The data for the cache.</param>
			/// <param name="validDuration">The duration for the data being valid in the cache.</param>
			public CacheItem(object data, TimeSpan validDuration)
			{
				_validDuration = validDuration;
				Data = data;
			}

			/// <summary>
			/// The data in the Cache.
			/// </summary>
			public object Data { set; private get; }

			/// <summary>
			/// Gets the Data typed.
			/// </summary>
			/// <typeparam name="T">The Type for which the data is set. If the type is wrong null will be returned.</typeparam>
			/// <returns>The data typed.</returns>
			public T GetData<T>() where T : class
			{
				return Data as T;
			}

			/// <summary>
			/// Gets the Data untyped.
			/// </summary>
			/// <returns>The data untyped.</returns>
			public object GetData()
			{
				return Data;
			}

			/// <summary>
			/// Check if the Data is still valid.
			/// </summary>
			/// <returns>Valid if the validDuration hasn't passed.</returns>
			public bool IsValid()
			{
				return _validDuration == TimeSpan.MaxValue || 
					_creationDate.Add(_validDuration) > DateTime.Now;
			}
		}

		public static MemCache Default = new MemCache();

		private readonly IDictionary<string, CacheItem> _cacheItems = new Dictionary<string, CacheItem>();

		private MemCache()
		{
		}

		/// <summary>
		/// All the CacheItems are in a Dictionary
		/// </summary>
		private IDictionary<string, CacheItem> CacheItems
		{
			get { return _cacheItems; }
		}

		/// <summary>
		/// Get full CacheItem based on key.
		/// </summary>
		/// <param name="key">The key for which a CacheItem is stored.</param>
		/// <returns>The CacheItem stored for the given key, if it is still valid. Otherwise null.</returns>
		public object this[string key]
		{
			get
			{
				lock (_cacheItems)
				{
					if (CacheItems.ContainsKey(key))
					{
						CacheItem ci = CacheItems[key];
						if (ci.IsValid())
						{
							return CacheItems[key].GetData();
						}
						else
						{
							CacheItems.Remove(key);
						}
					}
				}
				return null;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		public bool ContainsKey(string key)
		{
			return _cacheItems.ContainsKey(key);
		}

		/// <summary>
		/// Get data typed directly for given key.
		/// </summary>
		/// <typeparam name="T">The Type for which the data is set. If the type is wrong null will be returned.</typeparam>
		/// <param name="key">The key for which a CacheItem is stored.</param>
		/// <returns>The data typed for the given key, if it is still valid. Otherwise null.</returns>
		public T Get<T>(string key) where T : class
		{
			return (T)this[key];
		}

		/// <summary>
		/// Get data untyped directly for given key.
		/// </summary>
		/// <param name="key">The key for which a CacheItem is stored.</param>
		/// <returns>The data untyped for the given key, if it is still valid. Otherwise null.</returns>
		public object Get(string key)
		{
			return this[key];
		}

		/// <summary>
		/// Add a new item to the cache. If the key is already used it will be overwritten.
		/// </summary>
		/// <param name="key">The key for which a CacheItem is stored.</param>
		/// <param name="value"></param>
		private void Add(string key, CacheItem value)
		{
			lock (_cacheItems)
			{
				if (_cacheItems.ContainsKey(key))
				{
					_cacheItems.Remove(key);
				}
				_cacheItems.Add(key, value);
			}
		}

		/// <summary>
		/// Add a new item to the cache. If the key is already used it will be overwritten.
		/// </summary>
		/// <param name="key">The key for which a CacheItem is stored.</param>
		/// <param name="data">The data to cache.</param>
		/// <param name="validDuration">The duration of the caching of the data.</param>
		public void Add(string key, object data, TimeSpan validDuration)
		{
			Add(key, new CacheItem(data, validDuration));
		}

		/// <summary>
		/// Removes the item for the given key from the cache.
		/// </summary>
		/// <param name="key">The key for which a CacheItem is stored.</param>
		/// <returns></returns>
		public bool Remove(string key)
		{
			lock (_cacheItems)
			{
				return _cacheItems.Remove(key);
			}
		}
	}
}
