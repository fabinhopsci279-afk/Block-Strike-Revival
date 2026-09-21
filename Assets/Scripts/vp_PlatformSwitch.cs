using System.Collections.Generic;
using UnityEngine;

public class vp_PlatformSwitch : vp_Interactable
{
	public float SwitchTimeout;

	public vp_MovingPlatform Platform;

	public AudioSource AudioSource;

	public Vector2 SwitchPitchRange = new Vector2(1f, 1.5f);

	public List<AudioClip> SwitchSounds = new List<AudioClip>();

	protected bool m_IsSwitched;

	protected float m_Timeout;

	protected override void Start()
	{
		base.Start();
		if (AudioSource == null)
		{
			AudioSource = ((GetComponent<AudioSource>() == null) ? base.gameObject.AddComponent<AudioSource>() : GetComponent<AudioSource>());
		}
	}

	public override bool TryInteract(vp_FPPlayerEventHandler player)
	{
		if (Platform == null)
		{
			return false;
		}
		if (m_Player == null)
		{
			m_Player = player;
		}
		if (Time.time < m_Timeout)
		{
			return false;
		}
		PlaySound();
		Platform.SendMessage("GoTo", (Platform.TargetedWaypoint == 0) ? 1 : 0, SendMessageOptions.DontRequireReceiver);
		m_Timeout = Time.time + SwitchTimeout;
		m_IsSwitched = !m_IsSwitched;
		return true;
	}

	public virtual void PlaySound()
	{
		if (AudioSource == null)
		{
			UnityEngine.Debug.LogWarning("Audio Source is not set");
		}
		else if (SwitchSounds.Count != 0)
		{
			AudioClip audioClip = SwitchSounds[Random.Range(0, SwitchSounds.Count)];
			if (!(audioClip == null))
			{
				AudioSource.pitch = Random.Range(SwitchPitchRange.x, SwitchPitchRange.y);
				AudioSource.PlayOneShot(audioClip);
			}
		}
	}

	protected override void OnTriggerEnter(Collider col)
	{
		if (InteractType == vp_InteractType.Trigger)
		{
			using (List<string>.Enumerator enumerator = RecipientTags.GetEnumerator())
			{
				string current;
				do
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					current = enumerator.Current;
				}
				while (!(col.gameObject.tag == current));
			}
			if (m_Player == null)
			{
				m_Player = (Object.FindObjectOfType(typeof(vp_FPPlayerEventHandler)) as vp_FPPlayerEventHandler);
			}
			if (!(m_Player.Interactable.Get() != null) || !(m_Player.Interactable.Get().GetComponent<Collider>() == col))
			{
				TryInteract(m_Player);
			}
		}
	}
}
