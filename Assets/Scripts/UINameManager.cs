using UnityEngine;

public class UINameManager : MonoBehaviour
{
	public Camera m_Camera;

	public bool AutoFire;

	public bool OnlyHeadshot;

	public Vector3 AimVector = new Vector3(0.5f, 0.48f, 0f);

	private UILabel Label;

	private float Timer = 0.1f;

	private string LastName;

	private string PlayerName;

	private void Start()
	{
		PlayerName = PhotonNetwork.playerName;
		Label = UIGameManager.instance.NameLabel;
	}

	private void OnDisable()
	{
		Label.text = string.Empty;
	}

	private void Update()
	{
		Timer -= Time.deltaTime;
		if (!(Timer <= 0f))
		{
			return;
		}
		Timer = 0.1f;
		Ray ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
		{
			if (hitInfo.collider.CompareTag("PlayerSkin"))
			{
				string name = hitInfo.transform.root.name;
				if (name != LastName)
				{
					if (name != PlayerName)
					{
						ControllerManager component = hitInfo.transform.root.GetComponent<ControllerManager>();
						if (component.PlayerSkin.PlayerTeam == Team.Blue)
						{
							Label.effectColor = Color.blue;
						}
						else
						{
							Label.effectColor = Color.red;
						}
						LastName = name;
						Label.text = LastName;
					}
				}
				else
				{
					Label.text = LastName;
				}
			}
			else if (!string.IsNullOrEmpty(Label.text))
			{
				Label.text = string.Empty;
			}
		}
		else if (!string.IsNullOrEmpty(Label.text))
		{
			Label.text = string.Empty;
		}
	}
}
