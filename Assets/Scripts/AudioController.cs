using System;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    [SerializeField] public AudioClip _miss;
    [SerializeField] public AudioClip _killed;
    [SerializeField] public AudioClip _hit;
    [SerializeField] public AudioClip _click;
    [SerializeField] public AudioClip _victory;
    [SerializeField] public AudioClip _gameOver;

    [SerializeField] private AudioSource _sfxSource;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    public void PlaySfx(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }
}
