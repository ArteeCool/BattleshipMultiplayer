using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    [SerializeField] public AudioClip[] _miss;
    private AudioClip _missPlayed;
    [SerializeField] public AudioClip _killed;
    [SerializeField] public AudioClip _hit;
    [SerializeField] public AudioClip _click;
    [SerializeField] public AudioClip _victory;
    [SerializeField] public AudioClip _gameOver;

    [SerializeField] public AudioSource _sfxSource;
    [SerializeField] public AudioSource _musicSource;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }

    public void PlaySfx(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    public void PlaySfx(AudioClip[] clip)
    {
        AudioClip chosenClip;
        do
        {
            chosenClip = clip[Random.Range(0, clip.Length)];
        } while (chosenClip == _missPlayed);

        _sfxSource.PlayOneShot(chosenClip);
    }

}
