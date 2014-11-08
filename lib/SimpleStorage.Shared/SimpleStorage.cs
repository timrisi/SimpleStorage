using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Reflection;

namespace PerpetualEngine.Storage {
	public abstract partial class SimpleStorage {
		protected string Group { get; set; }

		/// <summary>
		/// Creates a new Storage. It's ok to use the plattform specifc constructors 
		/// as long as the code depends on a plattorm anyway. But if you have shared
		/// code, consider using the EditGroup(groupName) delegate as described in
		/// the GettingStarted-Component-Description.
		/// </summary>
		/// <param name="groupName">the namespace for this storage object</param>
		public SimpleStorage (string groupName)
		{
			Group = groupName;
		}

		/// <summary>
		/// plattform specific instance to save/load values within a given group (eg. namespace)
		/// </summary>
		/// <value>A delegate expecting a group name and returning a plattform specific variant of SimpleStorage</value>
		public static Func<string, SimpleStorage> EditGroup { get; set; }

		/// <summary>
		/// Persists a value with given key.
		/// </summary>
		/// <param name="value">if value is null, the key will be deleted</param>
		public abstract void Put (string key, string value);

		/// <summary>
		/// Retrieves value with given key.
		/// </summary>
		/// <returns>null, if key can not be found</returns>
		public abstract string Get (string key);

		/// <summary>
		/// Retrives a value with given key. If key can not be found, defaultValue is returned instead of null.
		/// </summary>
		public string Get (string key, string defaultValue)
		{
			var value = Get (key);
			if (value == null)
				return defaultValue;
			else
				return value;
		}

		/// <summary>
		/// Delete the specified key.
		/// </summary>
		public abstract void Delete (string key);

		/// <summary>
		/// Determines whether the storage has the specified key.
		/// </summary>
		/// <returns><c>true</c> if this instance has the specified key; otherwise, <c>false</c>.</returns>
		/// <param name="key">Key.</param>
		public bool HasKey (string key)
		{
			if (Get (key) != null)
				return true;
			return false;
		}

		/// <summary>
		/// Persists a value with given key.
		/// </summary>
		/// <param name="value">if value is null, the key will be deleted</param>
		/// <param name="key">Key.</param>
		public async Task PutAsync (string key, string value)
		{
			await Task.Run (() => Put (key, value))
                .ConfigureAwait (continueOnCapturedContext: false);
		}

		/// <summary>
		/// Retrieves value with given key.
		/// </summary>
		/// <returns>null, if key can not be found</returns>
		public async Task<string> GetAsync (string key)
		{
			return await Task.Run (() => Get (key))
                .ConfigureAwait (continueOnCapturedContext: false);
		}

		/// <summary>
		/// Determines whether the storage has the specified key async.
		/// </summary>
		/// <returns><c>true</c> if this instance has the specified key; otherwise, <c>false</c>.</returns>
		/// <param name="key">Key.</param>
		public async Task<bool> HasKeyAsync (string key)
		{
			return await Task.Run (() => HasKey (key))
                .ConfigureAwait (continueOnCapturedContext: false);
		}

		/// <summary>
		/// Delete the specified key async.
		/// </summary>
		public async Task DeleteAsync (string key)
		{
			await Task.Run (() => Delete (key))
                .ConfigureAwait (continueOnCapturedContext: false);
		}

		/// <summary>
		/// Persists a complex value with given key via binary serialization async.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">must be serializable</param>
		public async Task PutAsync<T> (string key, T value)
		{
			await Task.Run (() => Put<T> (key, value))
                .ConfigureAwait (continueOnCapturedContext: false);
		}

		/// <summary>
		/// Persists a complex value with given key via binary serialization.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">must be serializable</param>
		public void Put<T> (string key, T value)
		{
			var data = SerializeObject (value);
			Put (key, data);
		}

		/// <summary>
		/// Get the specified key or the dafault value of type T if the key does not exist.
		/// </summary>
		/// <remarks>Use HasKey(key) beforehand if T is a Value Type and you do not want the default value.</remarks>
		/// <param name="key">Key.</param>
		/// <returns>deserialized complex type</returns>
		public async Task<T> GetAsync<T> (string key)
		{
			return await Task.Run (() => Get<T> (key))
                .ConfigureAwait (continueOnCapturedContext: false);
		}

		/// <summary>
		/// Get the specified key or the dafault value of type T if the key does not exist.
		/// </summary>
		/// <remarks>Use HasKey(key) beforehand if T is a Value Type and you do not want the default value.</remarks>
		/// <param name="key">Key.</param>
		/// <returns>deserialized complex type</returns>
		public T Get<T> (string key)
		{
			var data = Get (key);
			if (data == null)
				return default(T);
			try {
				return DeserializeObject<T> (data);
			} catch (Exception e) {
				Console.WriteLine (e.Message);
				return default(T);
			}
		}

		/// <summary>
		/// Retrives a value with given key. If key can not be found, defaultValue is returned.
		/// </summary>
		/// <returns>deserialized complex type</returns>
		public T Get<T> (string key, T defaultValue)
		{
			if (!HasKey (key))
				return defaultValue;
			var result = Get<T> (key);
			return result;
		}

		static string SerializeObject (object o)
		{
			return JsonConvert.SerializeObject (o, Formatting.Indented);
		}

		T DeserializeObject<T> (string str)
		{
			return JsonConvert.DeserializeObject<T> (str);
		}
	}
}