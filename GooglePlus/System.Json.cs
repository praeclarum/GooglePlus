using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Json
{
	public enum JsonType
	{
		Object,
		Array,
		Number,
		String,
	}
	
	public abstract class JsonValue
	{
		public readonly JsonType JsonType;
		
		protected JsonValue (JsonType type)
		{
			JsonType = type;
		}
		
		public static JsonValue Parse (string json)
		{
			var ser = new System.Web.Script.Serialization.JavaScriptSerializer ();
			var obj = ser.DeserializeObject (json);
			return FromObject (obj);
		}
		
		protected static JsonValue FromObject (object obj)
		{
			if (obj == null) {
				return null;
			}
			else if (obj is JsonValue) {
				return (JsonValue)obj;
			}
			else if (obj is Dictionary<string, object>) {
				return new JsonObject ((Dictionary<string,object>)obj);
			}
			else if (obj is object[]) {
				return new JsonArray ((object[])obj);
			}
			else if (obj is string) {
				return new JsonPrimitive ((string)obj);
			}
			else if (obj is int) {
				return new JsonPrimitive ((int)obj);
			}
			else if (obj is long) {
				return new JsonPrimitive ((long)obj);
			}
			else if (obj is decimal) {
				return new JsonPrimitive ((decimal)obj);
			}
			else if (obj is double) {
				return new JsonPrimitive ((double)obj);
			}
			else {
				throw new NotSupportedException (obj.GetType().FullName);
			}
		}
		
		public static explicit operator int (JsonValue val)
		{
			var p = val as JsonPrimitive;
			if (p == null) throw new InvalidCastException ("Cannot convert " + val.JsonType + " to System.Int32");
			return Convert.ToInt32 (p.Value);
		}
	}
	
	public class JsonObject : JsonValue, IDictionary<string, JsonValue>
	{
		readonly Dictionary<string, JsonValue> dictionary;
		public JsonObject (Dictionary<string, object> dict)
			: base (JsonType.Object)
		{
			var d = new Dictionary<string, JsonValue>();
			foreach (var k in dict) {
				d [k.Key] = JsonValue.FromObject (k.Value);
			}
			dictionary = d;
		}

		#region IDictionary[System.String,JsonValue] implementation
		public void Add (string key, JsonValue value)
		{
			dictionary.Add (key, value);
		}

		public bool ContainsKey (string key)
		{
			return dictionary.ContainsKey (key);
		}

		public bool Remove (string key)
		{
			return dictionary.Remove (key);
		}

		public bool TryGetValue (string key, out JsonValue val)
		{
			return dictionary.TryGetValue (key, out val);
		}

		public JsonValue this[string key] {
			get {
				return dictionary [key];
			}
			set {
				dictionary [key] = value;
			}
		}

		public ICollection<string> Keys {
			get {
				return dictionary.Keys;
			}
		}

		public ICollection<JsonValue> Values {
			get {
				return dictionary.Values;
			}
		}
		#endregion

		#region IEnumerable[System.Collections.Generic.KeyValuePair[System.String,JsonValue]] implementation
		public IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator ()
		{
			return dictionary.GetEnumerator ();
		}
		#endregion

		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return ((System.Collections.IEnumerable)dictionary).GetEnumerator ();
		}
		#endregion

		#region ICollection[System.Collections.Generic.KeyValuePair[System.String,JsonValue]] implementation
		void ICollection<KeyValuePair<string, JsonValue>>.Add (KeyValuePair<string, JsonValue> item)
		{
			((ICollection<KeyValuePair<string, JsonValue>>)dictionary).Add (item);
		}

		public void Clear ()
		{
			dictionary.Clear ();
		}

		public bool Contains (KeyValuePair<string, JsonValue> item)
		{
			return ((ICollection<KeyValuePair<string, JsonValue>>)dictionary).Contains (item);
		}

		public void CopyTo (KeyValuePair<string, JsonValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, JsonValue>>)dictionary).CopyTo (array, arrayIndex);
		}

		bool ICollection<KeyValuePair<string, JsonValue>>.Remove (KeyValuePair<string, JsonValue> item)
		{
			return ((ICollection<KeyValuePair<string, JsonValue>>)dictionary).Remove (item);
		}

		public int Count {
			get {
				return dictionary.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}
		#endregion
	}
	
	public class JsonArray : JsonValue, IList<JsonValue>
	{
		readonly List<JsonValue> list;
		public JsonArray (object[] arr)
			: base (JsonType.Array)
		{
			list = (from o in arr select JsonValue.FromObject (o)).ToList ();
		}

		#region IList[JsonValue] implementation
		public int IndexOf (JsonValue item)
		{
			return list.IndexOf (item);
		}

		public void Insert (int index, JsonValue item)
		{
			list.Insert (index, item);
		}

		public void RemoveAt (int index)
		{
			list.RemoveAt (index);
		}

		public JsonValue this[int index] {
			get {
				return list [index];
			}
			set {
				list [index] = value;
			}
		}
		#endregion

		#region IEnumerable[JsonValue] implementation
		public IEnumerator<JsonValue> GetEnumerator ()
		{
			return list.GetEnumerator ();
		}
		#endregion

		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return ((System.Collections.IEnumerable)list).GetEnumerator ();
		}
		#endregion

		#region ICollection[JsonValue] implementation
		public void Add (JsonValue item)
		{
			list.Add (item);
		}

		public void Clear ()
		{
			list.Clear ();
		}

		public bool Contains (JsonValue item)
		{
			return list.Contains (item);
		}

		public void CopyTo (JsonValue[] array, int arrayIndex)
		{
			list.CopyTo (array, arrayIndex);
		}

		public bool Remove (JsonValue item)
		{
			return list.Remove (item);
		}

		public int Count {
			get {
				return list.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return false;
			}
		}
		#endregion
	}
	
	public class JsonPrimitive : JsonValue
	{
		internal object Value;
		public JsonPrimitive (int val)
			: base (JsonType.Number)
		{
			Value = val;
		}
		public JsonPrimitive (long val)
			: base (JsonType.Number)
		{
			Value = val;
		}
		public JsonPrimitive (decimal val)
			: base (JsonType.Number)
		{
			Value = val;
		}
		public JsonPrimitive (double val)
			: base (JsonType.Number)
		{
			Value = val;
		}
		public JsonPrimitive (string val)
			: base (JsonType.String)
		{
			Value = val;
		}
		public override string ToString ()
		{
			return Value != null ? Value.ToString () : "(null)";
		}
	}
}

