using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TPWeaponShooter : TimerBehaviour
{
	public GameObject Weapon;

	public bool isMuzzle = true;

	public Transform Muzzle;

	public MeshAtlas[] WeaponAtlas;

	public bool Audio = true;

	public AudioSource m_AudioSource;

	private bool isSound = true;

	private void Start()
	{
		UpdateOptions();
		EventManager.AddListener("UpdateOptions", UpdateOptions);
	}

	private void UpdateOptions()
	{
		isSound = SaveLoadManager.GetSound();
	}

	public void Active()
	{
		Weapon.SetActive(value: true);
	}

	public void Deactive()
	{
		Weapon.SetActive(value: false);
	}

	public void Fire(bool isVisible = true)
	{
		if (isVisible && isMuzzle)
		{
			Muzzle.gameObject.SetActive(value: true);
			vp_Timer.In(0.05f, delegate
			{
				Muzzle.gameObject.SetActive(value: false);
			}, GetTimer());
		}
		if (isSound && Audio)
		{
			m_AudioSource.Play();
		}
	}
}
