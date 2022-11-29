using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public List<AudioClip> clips;
    private AudioSource _audioSource;
    private int _currentTrack;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        Initialize();
    }


    private void Initialize()
    {
        _audioSource.clip = clips[0];
        _audioSource.Play();
    }
    public void PlayNextTrack()
    {
        _currentTrack += 1;
        _currentTrack %= clips.Count;
        _audioSource.clip = clips[_currentTrack];
        _audioSource.Play();
        
    }
}
