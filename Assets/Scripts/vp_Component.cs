using System;
using System.Collections.Generic;
using UnityEngine;

public class vp_Component : MonoBehaviour
{
	public bool Persist;

	protected vp_StateManager m_StateManager;

	public vp_EventHandler EventHandler;

	[NonSerialized]
	protected vp_State m_DefaultState;

	protected bool m_Initialized;

	protected Transform m_Transform;

	protected Transform m_Parent;

	protected Transform m_Root;

	protected AudioSource m_Audio;

	protected Collider m_Collider;

	public List<vp_State> States = new List<vp_State>();

	public List<vp_Component> Children = new List<vp_Component>();

	public List<vp_Component> Siblings = new List<vp_Component>();

	public List<vp_Component> Family = new List<vp_Component>();

	public List<Renderer> Renderers = new List<Renderer>();

	public List<AudioSource> AudioSources = new List<AudioSource>();

	protected vp_Timer.Handle m_DeactivationTimer = new vp_Timer.Handle();

	public vp_StateManager StateManager => m_StateManager;

	public vp_State DefaultState => m_DefaultState;

	public float Delta => Time.deltaTime * 60f;

	public float SDelta => Time.smoothDeltaTime * 60f;

	public Transform Transform
	{
		get
		{
			if (m_Transform == null)
			{
				m_Transform = base.transform;
			}
			return m_Transform;
		}
	}

	public Transform Parent
	{
		get
		{
			if (m_Parent == null)
			{
				m_Parent = base.transform.parent;
			}
			return m_Parent;
		}
	}

	public Transform Root
	{
		get
		{
			if (m_Root == null)
			{
				m_Root = base.transform.root;
			}
			return m_Root;
		}
	}

	public AudioSource Audio
	{
		get
		{
			if (m_Audio == null)
			{
				m_Audio = GetComponent<AudioSource>();
			}
			return m_Audio;
		}
	}

	public Collider Collider
	{
		get
		{
			if (m_Collider == null)
			{
				m_Collider = GetComponent<Collider>();
			}
			return m_Collider;
		}
	}

	public bool Rendering
	{
		get
		{
			return Renderers.Count > 0 && Renderers[0].enabled;
		}
		set
		{
			foreach (Renderer renderer in Renderers)
			{
				if (!(renderer == null))
				{
					renderer.enabled = value;
				}
			}
		}
	}

	protected virtual void Awake()
	{
		EventHandler = (vp_EventHandler)Transform.root.GetComponentInChildren(typeof(vp_EventHandler));
		CacheChildren();
		CacheSiblings();
		CacheFamily();
		CacheRenderers();
		CacheAudioSources();
		m_StateManager = new vp_StateManager(this, States);
		StateManager.SetState("Default", base.enabled);
	}

	protected virtual void Start()
	{
		ResetState();
	}

	protected virtual void Init()
	{
	}

	protected virtual void OnEnable()
	{
		if (EventHandler != null)
		{
			EventHandler.Register(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (EventHandler != null)
		{
			EventHandler.Unregister(this);
		}
	}

	protected virtual void Update()
	{
		if (!m_Initialized)
		{
			Init();
			m_Initialized = true;
		}
	}

	protected virtual void FixedUpdate()
	{
	}

	protected virtual void LateUpdate()
	{
	}

	public void SetState(string state, bool enabled = true, bool recursive = false, bool includeDisabled = false)
	{
		m_StateManager.SetState(state, enabled);
		if (recursive)
		{
			foreach (vp_Component child in Children)
			{
				if (includeDisabled || (vp_Utility.IsActive(child.gameObject) && child.enabled))
				{
					child.SetState(state, enabled, recursive: true, includeDisabled);
				}
			}
		}
	}

	public void ActivateGameObject(bool setActive = true)
	{
		if (setActive)
		{
			Activate();
			foreach (vp_Component sibling in Siblings)
			{
				sibling.Activate();
			}
		}
		else
		{
			DeactivateWhenSilent();
			foreach (vp_Component sibling2 in Siblings)
			{
				sibling2.DeactivateWhenSilent();
			}
		}
	}

	public void ResetState()
	{
		m_StateManager.Reset();
		Refresh();
	}

	public bool StateEnabled(string stateName)
	{
		return m_StateManager.IsEnabled(stateName);
	}

	public void RefreshDefaultState()
	{
		vp_State vp_State = null;
		if (States.Count == 0)
		{
			vp_State = new vp_State(GetType().Name, "Default");
			States.Add(vp_State);
		}
		else
		{
			for (int num = States.Count - 1; num > -1; num--)
			{
				if (States[num].Name == "Default")
				{
					vp_State = States[num];
					States.Remove(vp_State);
					States.Add(vp_State);
				}
			}
			if (vp_State == null)
			{
				vp_State = new vp_State(GetType().Name, "Default");
				States.Add(vp_State);
			}
		}
		if (vp_State.Preset == null || vp_State.Preset.ComponentType == null)
		{
			vp_State.Preset = new vp_ComponentPreset();
		}
		if (vp_State.TextAsset == null)
		{
			vp_State.Preset.InitFromComponent(this);
		}
		vp_State.Enabled = true;
		m_DefaultState = vp_State;
	}

	public void ApplyPreset(vp_ComponentPreset preset)
	{
		vp_ComponentPreset.Apply(this, preset);
		RefreshDefaultState();
		Refresh();
	}

	public vp_ComponentPreset Load(string path)
	{
		vp_ComponentPreset result = vp_ComponentPreset.LoadFromResources(this, path);
		RefreshDefaultState();
		Refresh();
		return result;
	}

	public vp_ComponentPreset Load(TextAsset asset)
	{
		vp_ComponentPreset result = vp_ComponentPreset.LoadFromTextAsset(this, asset);
		RefreshDefaultState();
		Refresh();
		return result;
	}

	public void CacheChildren()
	{
		Children.Clear();
		vp_Component[] componentsInChildren = GetComponentsInChildren<vp_Component>(includeInactive: true);
		foreach (vp_Component vp_Component in componentsInChildren)
		{
			if (vp_Component.transform.parent == base.transform)
			{
				Children.Add(vp_Component);
			}
		}
	}

	public void CacheSiblings()
	{
		Siblings.Clear();
		vp_Component[] components = GetComponents<vp_Component>();
		foreach (vp_Component vp_Component in components)
		{
			if (vp_Component != this)
			{
				Siblings.Add(vp_Component);
			}
		}
	}

	public void CacheFamily()
	{
		Family.Clear();
		vp_Component[] componentsInChildren = base.transform.root.GetComponentsInChildren<vp_Component>(includeInactive: true);
		foreach (vp_Component vp_Component in componentsInChildren)
		{
			if (vp_Component != this)
			{
				Family.Add(vp_Component);
			}
		}
	}

	public void CacheRenderers()
	{
		Renderers.Clear();
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer item in componentsInChildren)
		{
			Renderers.Add(item);
		}
	}

	public void CacheAudioSources()
	{
		AudioSources.Clear();
		AudioSource[] componentsInChildren = GetComponentsInChildren<AudioSource>(includeInactive: true);
		foreach (AudioSource item in componentsInChildren)
		{
			AudioSources.Add(item);
		}
	}

	public virtual void Activate()
	{
		m_DeactivationTimer.Cancel();
		vp_Utility.Activate(base.gameObject);
	}

	public virtual void Deactivate()
	{
		vp_Utility.Activate(base.gameObject, activate: false);
	}

	public void DeactivateWhenSilent()
	{
		if (!(this == null))
		{
			if (vp_Utility.IsActive(base.gameObject))
			{
				foreach (AudioSource audioSource in AudioSources)
				{
					if (audioSource.isPlaying && !audioSource.loop)
					{
						Rendering = false;
						vp_Timer.In(0.1f, delegate
						{
							DeactivateWhenSilent();
						}, m_DeactivationTimer);
						return;
					}
				}
			}
			Deactivate();
		}
	}

	public virtual void Refresh()
	{
	}
}
