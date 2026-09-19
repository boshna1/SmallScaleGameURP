using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public  class AudioManager : MonoBehaviour {

    public static AudioManager Instance { get; private set; }
     AudioSource audioSource;
     bool isAudioPaused;
    [SerializeField] List<AudioClip> sfxList;
    [SerializeField] List<AudioClip> musicList;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();  
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    public void PlaySoundAtPos(Vector3 pos, float vol, string clipName)
    {
        AudioClip tempClip = sfxList.Find(sfx => sfx.name == clipName);
        AudioSource.PlayClipAtPoint(tempClip, pos, vol);
    }

    public  void PlaySoundAmbient(string clipName, float vol)
    {
        Debug.Log(sfxList.Find(sfx => sfx.name == clipName));
        AudioClip tempClip = sfxList.Find(sfx => sfx.name == clipName);
        audioSource.PlayOneShot(tempClip, vol);
    }


    public  void StopSounds()
    {
        audioSource.Stop();
    }

    public  void PauseAudio()
    {
        if (isAudioPaused)
        {
            audioSource.UnPause();
        }
        else
        {
            audioSource.Pause();
        }
    }

}
