using UnityEngine;
using Photon;
using UnityEngine.Networking;
using System.Collections;

public class MusicSync : Photon.MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip loadedClip;
    private bool isMusicPlaying = false;

    void Awake()
    {
        EnsureAudioSourceExists();
    }

    private void EnsureAudioSourceExists()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }
    }

    // RPC для загрузки аудио
    [PunRPC]
    public void LoadAudioRPC(string url)
    {
        StartCoroutine(LoadAudioCoroutine(url));
    }

    // Локальная загрузка для отладки
    public void LoadAudioLocal(string url)
    {
        StartCoroutine(LoadAudioCoroutine(url));
    }

    private IEnumerator LoadAudioCoroutine(string url)
    {
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            loadedClip = DownloadHandlerAudioClip.GetContent(request);
            EnsureAudioSourceExists();
            audioSource.clip = loadedClip;
            Debug.Log("Audio loaded from URL successfully!");
        }
        else
        {
            Debug.LogError("Failed to load audio: " + request.error);
        }
    }

    // RPC для запуска музыки
    [PunRPC]
    public void StartMusicRPC()
    {
        StartMusic();
    }

    public void StartMusic()
    {
        EnsureAudioSourceExists();
        if (audioSource.clip != null)
        {
            audioSource.Play();
            isMusicPlaying = true;
            Debug.Log("Music started.");
        }
        else
        {
            Debug.LogError("No audio clip loaded!");
        }
    }

    // RPC для паузы музыки
    [PunRPC]
    public void PauseMusicRPC()
    {
        PauseMusic();
    }

    public void PauseMusic()
    {
        EnsureAudioSourceExists();
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isMusicPlaying = false;
            Debug.Log("Music paused.");
        }
    }

    public bool IsMusicPlaying()
    {
        return isMusicPlaying;
    }
}