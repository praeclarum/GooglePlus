using System;

namespace System.Json
{
	public enum JsonType
	{
		Number
	}
	
	public class JsonValue
	{
		public readonly JsonType JsonType;
		
		public static JsonValue Parse (string json)
		{
			throw new NotImplementedException ();
		}
		
		public static explicit operator int (JsonValue val)
		{
			throw new NotImplementedException ();
		}
	}
	
	public class JsonObject : JsonValue
	{
	}
}

