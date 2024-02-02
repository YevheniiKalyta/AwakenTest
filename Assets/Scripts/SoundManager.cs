
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundObjPrefab;
    [SerializeField] List<AudioContainer> containerList;
    [SerializeField] List<AudioSource> audioSources;
    [SerializeField] List<AudioSource> activeSources;
    public static SoundManager Instance;

    [System.Serializable]
    public struct AudioContainer
    {
        public string name;
        public AudioClip clip;
        public float volume;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    public void PlaySFXOneShotVarPitch(string name, bool varPitch = true)
    {
        AudioSource audioSource = GetAS();
        AudioContainer ac = GetSFXByName(name);
        audioSource.volume = ac.volume;
        audioSource.pitch = varPitch ? Random.Range(0.9f, 1.1f) : 1f;
        audioSource.PlayOneShot(ac.clip);
        StartCoroutine(DelayedRelease(audioSource, ac.clip.length));
    }
    public void PlaySFXLoop(string name)
    {
        AudioSource audioSource = GetAS();
        AudioContainer ac = GetSFXByName(name);
        audioSource.loop = true;
        audioSource.volume = ac.volume;
        audioSource.clip = ac.clip;
        audioSource.name = ac.name;
        activeSources.Add(audioSource);
        audioSource.Play();
    }

    public IEnumerator DelayedRelease(AudioSource audioSource, float time)
    {
        yield return new WaitForSeconds(time);
        ReturnAS(audioSource);
    }
    private AudioContainer GetSFXByName(string name)
    {
        for (int i = 0; i < containerList.Count; i++)
        {
            if (containerList[i].name == name)
            {
                return containerList[i];
            }
        }
        Debug.LogError($"Can't find SFX called {name}");
        return new AudioContainer();
    }

    private AudioSource GetAS()
    {
        AudioSource audioSource = audioSources.First(x => x.gameObject.activeInHierarchy == false);
        audioSource.gameObject.SetActive(true);
        return audioSource;
    }
    private void ReturnAS(AudioSource audioSource)
    {
        audioSource.gameObject.SetActive(false);
    }
}