using System;
using System.IO.IsolatedStorage;

namespace PerpetualEngine.Storage {
	/// <summary>
	/// iOS specific implementation to let EditGroup(string) return a n iOSSimpleStorage object
	/// </summary>
	public partial class SimpleStorage {
		static SimpleStorage ()
		{
			SimpleStorage.EditGroup = (name) => {
				return new WPSimpleStorage (name);
			};
		}
	}

	public class WPSimpleStorage : SimpleStorage {
		public WPSimpleStorage (string groupName)
			: base (groupName)
		{
		}

		/// <summary>
		/// Persists a value with given key.
		/// </summary>
		/// <param name="value">if value is null, the key will be deleted</param>
		override public void Put (string key, string value)
		{
			if (value == null)
				Delete (key);
			else if (!IsolatedStorageSettings.ApplicationSettings.Contains (Group + "_" + key))
				IsolatedStorageSettings.ApplicationSettings.Add (Group + "_" + key, value);
			else
				IsolatedStorageSettings.ApplicationSettings [Group + "_" + key] = value;
			
			IsolatedStorageSettings.ApplicationSettings.Save ();
			//if (value == null)
			//	Delete (key);
			//else
			//	NSUserDefaults.StandardUserDefaults.SetValueForKey (new NSString (value), new NSString (Group + "_" + key));
			//NSUserDefaults.StandardUserDefaults.Synchronize ();
		}

		/// <summary>
		/// Retrieves value with given key.
		/// </summary>
		/// <returns>null, if key can not be found</returns>
		override public string Get (string key)
		{
			if (IsolatedStorageSettings.ApplicationSettings.Contains (Group + "_" + key))
				return IsolatedStorageSettings.ApplicationSettings [Group + "_" + key].ToString ();

			return null;
			//return NSUserDefaults.StandardUserDefaults.StringForKey (Group + "_" + key);
		}

		/// <summary>
		/// Delete the specified key.
		/// </summary>
		override public void Delete (string key)
		{
			IsolatedStorageSettings.ApplicationSettings.Remove (Group + "_" + key);
			IsolatedStorageSettings.ApplicationSettings.Save ();

			//NSUserDefaults.StandardUserDefaults.RemoveObject (new NSString (Group + "_" + key));
			//NSUserDefaults.StandardUserDefaults.Synchronize ();
		}
	}
}