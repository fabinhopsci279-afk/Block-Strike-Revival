using UnityEngine;

public class VoiceManager : MonoBehaviour
{
	public static bool MicOn;

	public void ToggleMic()
	{
		MicOn = !MicOn;
		UIToast.Show(MicOn ? "Microfone ON" : "Microfone OFF");
	}
}
