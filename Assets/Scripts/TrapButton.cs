using UnityEngine;

public class TrapButton : MonoBehaviour
{
	public Team PlayerTeam = Team.Red;

	[Range(1f, 20f)]
	public int Key;

	public KeyCode Keycode;

	public MeshRenderer ButtonRenderer;

	[Header("Weapon Settings")]
	public bool Weapon;

	[SelectedWeapon(WeaponTypeList.Rifle)]
	public int SelectWeapon;

	private bool isTrigger;

	private bool isClickButton;

	private void Start()
	{
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
		EventManager.AddListener("Button" + Key, DeactiveButton);
	}

	private void Update()
	{
		if (isTrigger && !isClickButton && InputManager.GetButtonDown("Fire") && ButtonRenderer.isVisible)
		{
			ClickButton();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!isClickButton)
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && component.PlayerTeam == PlayerTeam)
			{
				isTrigger = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!isClickButton)
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && component.PlayerTeam == PlayerTeam)
			{
				isTrigger = false;
			}
		}
	}

	private void StartRound()
	{
		isClickButton = false;
		isTrigger = false;
	}

	private void ClickButton()
	{
		GameManager.OnEventManager("Button" + Key);
		isClickButton = true;
		isTrigger = false;
		if (Weapon)
		{
			GameManager.GetController().PlayerInput.PlayerWeapon.CanFire = false;
			vp_Timer.In(0.03f, delegate
			{
				WeaponManager.SetRifleType(SelectWeapon);
				GameManager.GetController().PlayerInput.PlayerWeapon.UpdateWeaponAll(WeaponTypeList.Rifle);
				GameManager.GetController().PlayerInput.PlayerWeapon.CanFire = true;
			});
		}
	}

	private void DeactiveButton()
	{
		isClickButton = true;
		isTrigger = false;
	}

	[ContextMenu("Click Button")]
	private void GetClickButton()
	{
		ClickButton();
	}
}
