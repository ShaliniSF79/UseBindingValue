namespace SfDataGridSample
{
	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Dynamic;
	using System.Reflection;

	/// <summary>
	/// Provides a dynamic model based on a properties of 
	/// key-value pairs.
	/// </summary>
	public class DictionaryModel : DynamicObject,
		ICollection<KeyValuePair<string, object>>, IDictionary<string, object>,
		INotifyCollectionChanged, INotifyPropertyChanged,
		IReflectableType
	{
		private readonly ConcurrentDictionary<string, PropertyInfo> infos = new ConcurrentDictionary<string, PropertyInfo>();
		private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

		/// <summary>Event raised when the collection changes.</summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };

		/// <summary>Event raised when a property on the collection changes.</summary>
		public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

		/// <summary>
		/// Initializes an instance of the class.
		/// </summary>
		public DictionaryModel()
			: this(new Dictionary<string, object>())
		{
		}

		/// <summary>
		/// Initializes the model with the properties to be used 
		/// as properties.
		/// </summary>
		public DictionaryModel(IDictionary<string, object> properties)
		{
			foreach (var item in properties)
			{
				this.Add(item.Key, item.Value);
			}
		}

		public Type GetType(string key)
		{
			if (!this.properties.ContainsKey(key) || this.properties[key] == null)
			{
				return typeof(object);
			}

			return this.properties[key].GetType();
		}

		public object GetValue(IDictionary<string, object> properties, string key)
		{
			object value;
			properties.TryGetValue(key, out value);
			return value;
		}

		public void SetValue(IDictionary<string, object> properties, string key, object value)
		{
			properties[key] = value;
		}

		private void AddWithNotification(string key, object value)
		{
			while(this.properties.ContainsKey(key))
			{
				key += "_";
			}

			value = this.WrapNestedDictionary(key, value);
			this.properties.Add(key, value);

			CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
				new KeyValuePair<string, object>(key, value)));
			PropertyChanged(this, new PropertyChangedEventArgs("Count"));
			PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
			PropertyChanged(this, new PropertyChangedEventArgs("Values"));
			PropertyChanged(this, new PropertyChangedEventArgs(key));
		}

		private bool RemoveWithNotification(string key)
		{
			object value;
			if (this.properties.TryGetValue(key, out value) && this.properties.Remove(key))
			{
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
					new KeyValuePair<string, object>(key, value)));
				PropertyChanged(this, new PropertyChangedEventArgs("Count"));
				PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
				PropertyChanged(this, new PropertyChangedEventArgs("Values"));
				PropertyChanged(this, new PropertyChangedEventArgs(key));

				return true;
			}

			return false;
		}

		private void UpdateWithNotification(string key, object value)
		{
			object existing;
			if (this.properties.TryGetValue(key, out existing))
			{
				value = this.WrapNestedDictionary(key, value);
				this.properties[key] = value;

				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
					new KeyValuePair<string, object>(key, value),
					new KeyValuePair<string, object>(key, existing)));
				PropertyChanged(this, new PropertyChangedEventArgs("Values"));
				PropertyChanged(this, new PropertyChangedEventArgs(key));
			}
			else
			{
				this.AddWithNotification(key, value);
			}
		}

		// Nested dictionaries are wrapped in our own model 
		// again to propagate property changes upwards.
		private object WrapNestedDictionary(string key, object value)
		{
			var childProps = value as IDictionary<string, object>;
			if (childProps != null)
			{
				var innerModel = new DictionaryModel(childProps);
				innerModel.CollectionChanged += (sender, args) => PropertyChanged(this, new PropertyChangedEventArgs(key));
				return innerModel;
			}

			return value;
		}

		#region DynamicObject

		/// <summary>
		/// Tries to retrieve the value of a property using dynamic syntax.
		/// </summary>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return this.properties.TryGetValue(binder.Name, out result);
		}

		/// <summary>
		/// Sets the value of a dictionary key using dynamic syntax.
		/// </summary>
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			this[binder.Name] = value;
			return true;
		}

		#endregion

		#region IReflectableType

		/// <summary>
		/// Retrieves an object that represents the type of this instance.
		/// </summary>
		public TypeInfo GetTypeInfo()
		{
			return new DynamicTypeInfo(name => this.infos.GetOrAdd(name, key => new DynamicPropertyInfo(
				  typeof(DictionaryModel),
				  key,
				  this.GetType(key),
				  obj => this.GetValue((DictionaryModel)obj, key),
				  (obj, value) => this.SetValue((DictionaryModel)obj, key, value))));
		}

		#endregion

		#region IDictionary<string,object> Members

		/// <summary>
		/// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <param name="key">The object to use as the key of the element to add.</param>
		/// <param name="value">The object to use as the value of the element to add.</param>
		public void Add(string key, object value)
		{
			this.AddWithNotification(key, value);
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
		/// <returns>
		/// true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.
		/// </returns>
		public bool ContainsKey(string key)
		{
			return this.properties.ContainsKey(key);
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
		public ICollection<string> Keys
		{
			get { return this.properties.Keys; }
		}

		/// <summary>
		/// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <returns>
		/// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </returns>
		public bool Remove(string key)
		{
			return this.RemoveWithNotification(key);
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
		/// <returns>
		/// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.
		/// </returns>
		public bool TryGetValue(string key, out object value)
		{
			return this.properties.TryGetValue(key, out value);
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
		public ICollection<object> Values
		{
			get { return this.properties.Values; }
		}

		/// <summary>
		/// Gets or sets the element with the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public object this[string key]
		{
			get { return this.properties[key]; }
			set { this.UpdateWithNotification(key, value); }
		}

		#endregion

		#region ICollection<KeyValuePair<string,object>> Members

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			this.AddWithNotification(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<string, object>>.Clear()
		{
			((ICollection<KeyValuePair<string, object>>)this.properties).Clear();

			CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			PropertyChanged(this, new PropertyChangedEventArgs("Count"));
			PropertyChanged(this, new PropertyChangedEventArgs("Keys"));
			PropertyChanged(this, new PropertyChangedEventArgs("Values"));
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			return ((ICollection<KeyValuePair<string, object>>)this.properties).Contains(item);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, object>>)this.properties).CopyTo(array, arrayIndex);
		}

		int ICollection<KeyValuePair<string, object>>.Count
		{
			get { return ((ICollection<KeyValuePair<string, object>>)this.properties).Count; }
		}

		bool ICollection<KeyValuePair<string, object>>.IsReadOnly
		{
			get { return ((ICollection<KeyValuePair<string, object>>)this.properties).IsReadOnly; }
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			return this.RemoveWithNotification(item.Key);
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,object>> Members

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return ((ICollection<KeyValuePair<string, object>>)this.properties).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((ICollection<KeyValuePair<string, object>>)this.properties).GetEnumerator();
		}

		#endregion
	}
}