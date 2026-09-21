using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

internal static class vp_TargetEventHandler
{
	private static List<Dictionary<object, Dictionary<string, Delegate>>> m_TargetDict;

	private static List<Dictionary<object, Dictionary<string, Delegate>>> TargetDict
	{
		get
		{
			if (m_TargetDict == null)
			{
				m_TargetDict = new List<Dictionary<object, Dictionary<string, Delegate>>>(100);
				for (int i = 0; i < 8; i++)
				{
					m_TargetDict.Add(new Dictionary<object, Dictionary<string, Delegate>>(100));
				}
			}
			return m_TargetDict;
		}
	}

	public static void Register(object target, string eventName, Delegate callback, int dictionary)
	{
		if (target == null)
		{
			UnityEngine.Debug.LogWarning("Warning: (" + vp_Utility.GetErrorLocation(2) + " -> vp_TargetEvent.Register) Target object is null.");
			return;
		}
		if (string.IsNullOrEmpty(eventName))
		{
			UnityEngine.Debug.LogWarning("Warning: (" + vp_Utility.GetErrorLocation(2) + " -> vp_TargetEvent.Register) Event name is null or empty.");
			return;
		}
		if ((object)callback == null)
		{
			UnityEngine.Debug.LogWarning("Warning: (" + vp_Utility.GetErrorLocation(2) + " -> vp_TargetEvent.Register) Callback is null.");
			return;
		}
		if (callback.Method.Name.StartsWith("<"))
		{
			UnityEngine.Debug.LogWarning("Warning: (" + vp_Utility.GetErrorLocation(2) + " -> vp_TargetEvent.Register) Target events can only be registered to declared methods.");
			return;
		}
		if (!TargetDict[dictionary].ContainsKey(target))
		{
			TargetDict[dictionary].Add(target, new Dictionary<string, Delegate>(100));
		}
		TargetDict[dictionary].TryGetValue(target, out Dictionary<string, Delegate> value);
		Delegate value2;
		while (true)
		{
			value.TryGetValue(eventName, out value2);
			if ((object)value2 != null)
			{
				if (value2.GetType() == callback.GetType())
				{
					break;
				}
				eventName += "_";
				continue;
			}
			value.Add(eventName, callback);
			return;
		}
		callback = Delegate.Combine(value2, callback);
		if ((object)callback != null)
		{
			value.Remove(eventName);
			value.Add(eventName, callback);
		}
	}

	public static void Unregister(object target, string eventName = null, Delegate callback = null)
	{
		if ((eventName != null || (object)callback == null) && ((object)callback != null || eventName == null))
		{
			for (int i = 0; i < 8; i++)
			{
				Unregister(target, i, eventName, callback);
			}
		}
	}

	public static void Unregister(Component component)
	{
		if (!(component == null))
		{
			for (int i = 0; i < 8; i++)
			{
				Unregister(i, component);
			}
		}
	}

	private static void Unregister(int dictionary, Component component)
	{
		if (!(component == null))
		{
			if (TargetDict[dictionary].TryGetValue(component, out Dictionary<string, Delegate> value))
			{
				TargetDict[dictionary].Remove(component);
			}
			object transform = component.transform;
			if (transform != null && TargetDict[dictionary].TryGetValue(transform, out value))
			{
				List<string> list = new List<string>(value.Keys);
				foreach (string item in list)
				{
					Delegate value2;
					if (item != null && value.TryGetValue(item, out value2) && (object)value2 != null)
					{
						Delegate[] invocationList = value2.GetInvocationList();
						if (invocationList != null && invocationList.Length >= 1)
						{
							for (int num = invocationList.Length - 1; num > -1; num--)
							{
								if (invocationList[num].Target == component)
								{
									value.Remove(item);
									Delegate @delegate = Delegate.Remove(value2, invocationList[num]);
									if (@delegate.GetInvocationList().Length > 0)
									{
										value.Add(item, @delegate);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	private static void Unregister(object target, int dictionary, string eventName, Delegate callback)
	{
		Dictionary<string, Delegate> value;
		if (target == null || !TargetDict[dictionary].TryGetValue(target, out value) || value == null || value.Count == 0)
		{
			return;
		}
		if (eventName == null && (object)callback == null)
		{
			TargetDict[dictionary].Remove(value);
		}
		else
		{
			if (!value.TryGetValue(eventName, out Delegate value2))
			{
				return;
			}
			if ((object)value2 != null)
			{
				value.Remove(eventName);
				value2 = Delegate.Remove(value2, callback);
				if ((object)value2 != null && value2.GetInvocationList() != null)
				{
					value.Add(eventName, value2);
				}
			}
			else
			{
				value.Remove(eventName);
			}
			if (value.Count <= 0)
			{
				TargetDict[dictionary].Remove(target);
			}
		}
	}

	public static void UnregisterAll()
	{
		m_TargetDict = null;
	}

	public static Delegate GetCallback(object target, string eventName, bool upwards, int d, vp_TargetEventOptions options)
	{
		if (target == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(eventName))
		{
			return null;
		}
		while (true)
		{
			Delegate value = null;
			if ((options & vp_TargetEventOptions.IncludeInactive) != vp_TargetEventOptions.IncludeInactive)
			{
				GameObject gameObject = target as GameObject;
				if (gameObject != null)
				{
					if (!vp_Utility.IsActive(gameObject))
					{
						if (upwards)
						{
							goto IL_009c;
						}
						return null;
					}
				}
				else
				{
					Behaviour behaviour = target as Behaviour;
					if (behaviour != null && (!behaviour.enabled || !vp_Utility.IsActive(behaviour.gameObject)))
					{
						if (!upwards)
						{
							return null;
						}
						goto IL_009c;
					}
				}
			}
			Dictionary<string, Delegate> value2 = null;
			if (!TargetDict[d].TryGetValue(target, out value2))
			{
				if (!upwards)
				{
					return null;
				}
			}
			else if (!value2.TryGetValue(eventName, out value) && !upwards)
			{
				break;
			}
			goto IL_009c;
			IL_009c:
			if ((object)value != null || !upwards)
			{
				return value;
			}
			target = vp_Utility.GetParent(target as Component);
			if (target == null)
			{
				return value;
			}
		}
		return null;
	}

	public static void OnNoReceiver(string eventName, vp_TargetEventOptions options)
	{
		if ((options & vp_TargetEventOptions.RequireReceiver) == vp_TargetEventOptions.RequireReceiver)
		{
			UnityEngine.Debug.LogError("Error: (" + vp_Utility.GetErrorLocation(2) + ") vp_TargetEvent '" + eventName + "' has no receiver!");
		}
	}

	public static string Dump()
	{
		Dictionary<object, string> dictionary = new Dictionary<object, string>();
		foreach (Dictionary<object, Dictionary<string, Delegate>> item in TargetDict)
		{
			foreach (object key in item.Keys)
			{
				string text = string.Empty;
				if (key != null)
				{
					if (item.TryGetValue(key, out Dictionary<string, Delegate> value))
					{
						foreach (string key2 in value.Keys)
						{
							text = text + "        \"" + key2 + "\" -> ";
							bool flag = false;
							if (!string.IsNullOrEmpty(key2) && value.TryGetValue(key2, out Delegate value2))
							{
								if (value2.GetInvocationList().Length > 1)
								{
									flag = true;
									text += "\n";
								}
								Delegate[] invocationList = value2.GetInvocationList();
								foreach (Delegate @delegate in invocationList)
								{
									string text2 = text;
									text = text2 + (flag ? "                        " : string.Empty) + @delegate.Method.ReflectedType + ".cs -> ";
									string text3 = string.Empty;
									ParameterInfo[] parameters = @delegate.Method.GetParameters();
									foreach (ParameterInfo parameterInfo in parameters)
									{
										text2 = text3;
										text3 = text2 + vp_Utility.GetTypeAlias(parameterInfo.ParameterType) + " " + parameterInfo.Name + ", ";
									}
									if (text3.Length > 0)
									{
										text3 = text3.Remove(text3.LastIndexOf(", "));
									}
									text = text + vp_Utility.GetTypeAlias(@delegate.Method.ReturnType) + " ";
									if (@delegate.Method.Name.Contains("m_"))
									{
										string text4 = @delegate.Method.Name.TrimStart('<');
										text4 = text4.Remove(text4.IndexOf('>'));
										text = text + text4 + " -> delegate";
									}
									else
									{
										text += @delegate.Method.Name;
									}
									text = text + "(" + text3 + ")\n";
								}
							}
						}
					}
					if (!dictionary.TryGetValue(key, out string value3))
					{
						dictionary.Add(key, text);
					}
					else
					{
						dictionary.Remove(key);
						dictionary.Add(key, value3 + text);
					}
				}
			}
		}
		string text5 = "--- TARGET EVENT DUMP ---\n\n";
		foreach (object key3 in dictionary.Keys)
		{
			if (key3 != null)
			{
				text5 = text5 + key3.ToString() + ":\n";
				if (dictionary.TryGetValue(key3, out string value4))
				{
					text5 += value4;
				}
			}
		}
		return text5;
	}
}
