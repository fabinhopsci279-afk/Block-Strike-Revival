using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class vp_EventHandler : MonoBehaviour
{
	protected class ScriptMethods
	{
		public List<MethodInfo> Events = new List<MethodInfo>();

		public ScriptMethods(Type type)
		{
			Events = GetMethods(type);
		}

		protected static List<MethodInfo> GetMethods(Type type)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			List<string> list2 = new List<string>();
			while (type != null)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo in methods)
				{
					if (methodInfo.Name.Contains(">m__") || list2.Contains(methodInfo.Name))
					{
						continue;
					}
					string[] supportedPrefixes = m_SupportedPrefixes;
					foreach (string value in supportedPrefixes)
					{
						if (methodInfo.Name.Contains(value))
						{
							list.Add(methodInfo);
							list2.Add(methodInfo.Name);
							break;
						}
					}
				}
				type = type.BaseType;
			}
			return list;
		}
	}

	protected bool m_Initialized;

	protected Dictionary<string, vp_Event> m_HandlerEvents = new Dictionary<string, vp_Event>();

	protected List<object> m_PendingRegistrants = new List<object>();

	protected static Dictionary<Type, ScriptMethods> m_StoredScriptTypes = new Dictionary<Type, ScriptMethods>();

	protected static string[] m_SupportedPrefixes = new string[10]
	{
		"OnMessage_",
		"CanStart_",
		"CanStop_",
		"OnStart_",
		"OnStop_",
		"OnAttempt_",
		"get_OnValue_",
		"set_OnValue_",
		"OnFailStart_",
		"OnFailStop_"
	};

	protected virtual void Awake()
	{
		StoreHandlerEvents();
		m_Initialized = true;
		for (int num = m_PendingRegistrants.Count - 1; num > -1; num--)
		{
			Register(m_PendingRegistrants[num]);
			m_PendingRegistrants.Remove(m_PendingRegistrants[num]);
		}
	}

	protected void StoreHandlerEvents()
	{
		object obj = null;
		List<FieldInfo> fields = GetFields();
		if (fields != null && fields.Count != 0)
		{
			foreach (FieldInfo item in fields)
			{
				try
				{
					obj = Activator.CreateInstance(item.FieldType, item.Name);
				}
				catch
				{
					UnityEngine.Debug.LogError("Error: (" + this + ") does not support the type of '" + item.Name + "' in '" + item.DeclaringType + "'.");
					continue;
				}
				if (obj != null)
				{
					item.SetValue(this, obj);
					foreach (string key in ((vp_Event)obj).Prefixes.Keys)
					{
						m_HandlerEvents.Add(key + item.Name, (vp_Event)obj);
					}
				}
			}
		}
	}

	public List<FieldInfo> GetFields()
	{
		List<FieldInfo> list = new List<FieldInfo>();
		Type type = GetType();
		Type type2 = null;
		do
		{
			if (type2 != null)
			{
				type = type2;
			}
			list.AddRange(type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			if (type.BaseType != typeof(vp_StateEventHandler) && type.BaseType != typeof(vp_EventHandler))
			{
				type2 = type.BaseType;
			}
		}
		while (type.BaseType != typeof(vp_StateEventHandler) && type.BaseType != typeof(vp_EventHandler) && type != null);
		if (list == null || list.Count == 0)
		{
			UnityEngine.Debug.LogWarning("Warning: (" + this + ") Found no fields to store as events.");
		}
		return list;
	}

	public void Register(object target)
	{
		if (target == null)
		{
			UnityEngine.Debug.LogError("Error: (" + this + ") Target object was null.");
			return;
		}
		if (!m_Initialized)
		{
			m_PendingRegistrants.Add(target);
			return;
		}
		ScriptMethods scriptMethods = GetScriptMethods(target);
		if (scriptMethods == null)
		{
			UnityEngine.Debug.LogError("Error: (" + this + ") could not get script methods for '" + target + "'.");
		}
		else
		{
			foreach (MethodInfo @event in scriptMethods.Events)
			{
				if (m_HandlerEvents.TryGetValue(@event.Name, out vp_Event value))
				{
					value.Prefixes.TryGetValue(@event.Name.Substring(0, @event.Name.IndexOf('_', 4) + 1), out int value2);
					if (CompareMethodSignatures(@event, value.GetParameterType(value2), value.GetReturnType(value2)))
					{
						value.Register(target, @event.Name, value2);
					}
				}
			}
		}
	}

	public void Unregister(object target)
	{
		if (target == null)
		{
			UnityEngine.Debug.LogError("Error: (" + this + ") Target object was null.");
		}
		else
		{
			foreach (vp_Event value2 in m_HandlerEvents.Values)
			{
				if (value2 != null)
				{
					string[] invokerFieldNames = value2.InvokerFieldNames;
					foreach (string name in invokerFieldNames)
					{
						Type type = value2.GetType();
						FieldInfo field = type.GetField(name);
						if (field != null)
						{
							object value = field.GetValue(value2);
							if (value != null)
							{
								Delegate @delegate = (Delegate)value;
								if ((object)@delegate != null)
								{
									Delegate[] invocationList = @delegate.GetInvocationList();
									foreach (Delegate delegate2 in invocationList)
									{
										if (delegate2.Target == target)
										{
											value2.Unregister(target);
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	protected bool CompareMethodSignatures(MethodInfo scriptMethod, Type handlerParameterType, Type handlerReturnType)
	{
		if (scriptMethod.ReturnType != handlerReturnType)
		{
			UnityEngine.Debug.LogError("Error: (" + scriptMethod.DeclaringType + ") Return type (" + vp_Utility.GetTypeAlias(scriptMethod.ReturnType) + ") is not valid for '" + scriptMethod.Name + "'. Return type declared in event handler was: (" + vp_Utility.GetTypeAlias(handlerReturnType) + ").");
			return false;
		}
		if (scriptMethod.GetParameters().Length == 1)
		{
			if (((ParameterInfo)scriptMethod.GetParameters().GetValue(0)).ParameterType != handlerParameterType)
			{
				UnityEngine.Debug.LogError("Error: (" + scriptMethod.DeclaringType + ") Parameter type (" + vp_Utility.GetTypeAlias(((ParameterInfo)scriptMethod.GetParameters().GetValue(0)).ParameterType) + ") is not valid for '" + scriptMethod.Name + "'. Parameter type declared in event handler was: (" + vp_Utility.GetTypeAlias(handlerParameterType) + ").");
				return false;
			}
		}
		else if (scriptMethod.GetParameters().Length == 0)
		{
			if (handlerParameterType != typeof(void))
			{
				UnityEngine.Debug.LogError("Error: (" + scriptMethod.DeclaringType + ") Can't register method '" + scriptMethod.Name + "' with 0 parameters. Expected: 1 parameter of type (" + vp_Utility.GetTypeAlias(handlerParameterType) + ").");
				return false;
			}
		}
		else if (scriptMethod.GetParameters().Length > 1)
		{
			UnityEngine.Debug.LogError("Error: (" + scriptMethod.DeclaringType + ") Can't register method '" + scriptMethod.Name + "' with " + scriptMethod.GetParameters().Length + " parameters. Max parameter count: 1 of type (" + vp_Utility.GetTypeAlias(handlerParameterType) + ").");
			return false;
		}
		return true;
	}

	protected ScriptMethods GetScriptMethods(object target)
	{
		if (!m_StoredScriptTypes.TryGetValue(target.GetType(), out ScriptMethods value))
		{
			value = new ScriptMethods(target.GetType());
			m_StoredScriptTypes.Add(target.GetType(), value);
		}
		return value;
	}
}
