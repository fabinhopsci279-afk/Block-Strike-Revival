using UnityEngine;
using UnityEditor;
using Photon;

public class MusicControlWindow : EditorWindow
{
    private string audioUrl = "";
    private MusicSync musicSync; // Ссылка на компонент MusicSync
    private GameObject musicObject; // Объект, который создаём в редакторе

    [MenuItem("Window/Music Control with Photon")]
    public static void ShowWindow()
    {
        GetWindow<MusicControlWindow>("Music Control");
    }

    private void OnEnable()
    {
        EnsureMusicObjectExists(); // Создаём или находим объект при открытии окна
    }

    private void OnDisable()
    {
        // Уничтожаем объект при закрытии окна (опционально)
        if (musicObject != null)
        {
            DestroyImmediate(musicObject);
            musicSync = null;
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Music Control with Photon 5.6.5", EditorStyles.boldLabel);
        audioUrl = EditorGUILayout.TextField("Audio URL", audioUrl);

        if (GUILayout.Button("Load Audio for All Players"))
        {
            LoadAudioForAllPlayers();
        }

        if (GUILayout.Button("Start Music for All"))
        {
            StartMusicForAll();
        }

        if (GUILayout.Button("Pause Music for All"))
        {
            PauseMusicForAll();
        }

        if (musicSync != null)
        {
            EditorGUILayout.LabelField("Music Playing: ", musicSync.IsMusicPlaying().ToString());
        }
        else
        {
            EditorGUILayout.LabelField("Music Playing: ", "MusicSync not found");
        }
    }

    // Проверка и создание объекта с MusicSync и PhotonView
    private void EnsureMusicObjectExists()
    {
        if (musicObject == null || !musicObject)
        {
            // Создаём новый объект
            musicObject = new GameObject("MusicPlayer");
            musicSync = musicObject.AddComponent<MusicSync>();
            PhotonView photonView = musicObject.AddComponent<PhotonView>();

            // Настраиваем PhotonView для наблюдения за MusicSync
            photonView.ObservedComponents = new System.Collections.Generic.List<Component> { musicSync };
            photonView.synchronization = ViewSynchronization.UnreliableOnChange; // Опционально

            Debug.Log("MusicPlayer object created with MusicSync and PhotonView.");
        }
        else if (musicSync == null)
        {
            // Если объект есть, но MusicSync отсутствует, добавляем его
            musicSync = musicObject.GetComponent<MusicSync>();
            if (musicSync == null)
            {
                musicSync = musicObject.AddComponent<MusicSync>();
            }
            PhotonView photonView = musicObject.GetComponent<PhotonView>();
            if (photonView == null)
            {
                photonView = musicObject.AddComponent<PhotonView>();
                photonView.ObservedComponents = new System.Collections.Generic.List<Component> { musicSync };
                photonView.synchronization = ViewSynchronization.UnreliableOnChange;
            }
            Debug.Log("MusicSync or PhotonView re-added to existing MusicPlayer.");
        }
    }

    private void LoadAudioForAllPlayers()
    {
        if (!string.IsNullOrEmpty(audioUrl))
        {
            EnsureMusicObjectExists(); // Убеждаемся, что объект существует
            if (PhotonNetwork.connected)
            {
                musicSync.photonView.RPC("LoadAudioRPC", PhotonTargets.All, audioUrl);
            }
            else
            {
                Debug.LogError("Not connected to Photon network! Loading locally only.");
                musicSync.LoadAudioLocal(audioUrl); // Локальная загрузка для отладки
            }
        }
        else
        {
            Debug.LogError("Audio URL is empty!");
        }
    }

    private void StartMusicForAll()
    {
        EnsureMusicObjectExists();
        if (PhotonNetwork.connected)
        {
            musicSync.photonView.RPC("StartMusicRPC", PhotonTargets.All);
        }
        else
        {
            Debug.LogError("Not connected to Photon network! Starting locally only.");
            musicSync.StartMusic();
        }
    }

    private void PauseMusicForAll()
    {
        EnsureMusicObjectExists();
        if (PhotonNetwork.connected)
        {
            musicSync.photonView.RPC("PauseMusicRPC", PhotonTargets.All);
        }
        else
        {
            Debug.LogError("Not connected to Photon network! Pausing locally only.");
            musicSync.PauseMusic();
        }
    }
}