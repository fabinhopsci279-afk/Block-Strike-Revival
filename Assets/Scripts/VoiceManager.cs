using UnityEngine;
using UnityEngine.Events;

public class VoiceManager : MonoBehaviour
{
	public static bool MicOn;

	public void ToggleMic()
	{
		MicOn = !MicOn;
		UIToast.Show(MicOn ? "Microfone ON" : "Microfone OFF");
	}

	public static void SetupVoiceButton()
	{
		if (GameObject.Find("Voice") != null)
		{
			return;
		}
		GameObject chat = null;
		GameObject[] all = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		for (int i = 0; i < all.Length; i++)
		{
			if (all[i].name == "Chat" && all[i].GetComponent<UISprite>() != null && all[i].GetComponent<BoxCollider>() != null)
			{
				chat = all[i];
				break;
			}
		}
		if (chat == null)
		{
			return;
		}
		GameObject voice = Object.Instantiate(chat);
		voice.name = "Voice";
		voice.transform.SetParent(chat.transform.parent, worldPositionStays: false);
		voice.transform.localPosition = chat.transform.localPosition + new Vector3(0f, -60f, 0f);
		voice.transform.localScale = Vector3.one;
		MonoBehaviour[] monos = voice.GetComponents<MonoBehaviour>();
		for (int i = 0; i < monos.Length; i++)
		{
			if (!(monos[i] is UIAnchor))
			{
				Object.Destroy(monos[i]);
			}
		}
		UITexture tex = voice.AddComponent<UITexture>();
		tex.mainTexture = Resources.Load<Texture2D>("voice/Mic");
		tex.width = 50;
		tex.height = 50;
		VoiceManager vm = voice.AddComponent<VoiceManager>();
		UIEventClick click = voice.AddComponent<UIEventClick>();
		UnityEvent ev = new UnityEvent();
		ev.AddListener(vm.ToggleMic);
		click.onClick = ev;
		voice.SetActive(value: true);
	}
}
