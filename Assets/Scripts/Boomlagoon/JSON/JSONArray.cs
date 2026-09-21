using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Boomlagoon.JSON
{
	public class JSONArray : IEnumerable, IEnumerable<JSONValue>
	{
		private readonly List<JSONValue> values = new List<JSONValue>();

		public JSONValue this[int index]
		{
			get
			{
				return values[index];
			}
			set
			{
				values[index] = value;
			}
		}

		public int Length => values.Count;

		public JSONArray()
		{
		}

		public JSONArray(JSONArray array)
		{
			values = new List<JSONValue>();
			foreach (JSONValue value in array.values)
			{
				values.Add(new JSONValue(value));
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return values.GetEnumerator();
		}

		public void Add(JSONValue value)
		{
			values.Add(value);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			foreach (JSONValue value in values)
			{
				stringBuilder.Append(value.ToString());
				stringBuilder.Append(',');
			}
			if (values.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		public IEnumerator<JSONValue> GetEnumerator()
		{
			return values.GetEnumerator();
		}

		public static JSONArray Parse(string jsonString)
		{
			return JSONObject.Parse("{ \"array\" :" + jsonString + '}')?.GetValue("array").Array;
		}

		public void Clear()
		{
			values.Clear();
		}

		public void Remove(int index)
		{
			if (index >= 0 && index < values.Count)
			{
				values.RemoveAt(index);
			}
			else
			{
				JSONLogger.Error("index out of range: " + index + " (Expected 0 <= index < " + values.Count + ")");
			}
		}

		public static JSONArray operator +(JSONArray lhs, JSONArray rhs)
		{
			JSONArray jSONArray = new JSONArray(lhs);
			foreach (JSONValue value in rhs.values)
			{
				jSONArray.Add(value);
			}
			return jSONArray;
		}
	}
}
