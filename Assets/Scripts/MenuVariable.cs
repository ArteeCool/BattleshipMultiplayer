using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuVariable : MonoBehaviour
{
    public static MenuVariable Instance;
    
    [SerializeField] public GameObject _viewport;
    [SerializeField] public TMP_InputField _inputField;
    [SerializeField] public GameObject _errorText;
    
    [SerializeField] public Slider _sfx;
    [SerializeField] public Slider _music;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _sfx.value = PlayerPrefs.GetFloat("SFXValue", 1.0f);
        _music.value = PlayerPrefs.GetFloat("MusicValue", 1.0f);
        
        AudioController.Instance._sfxSource.volume = _sfx.value;
        AudioController.Instance._musicSource.volume = _music.value;  
        
        _sfx.onValueChanged.AddListener(value =>
        {
            AudioController.Instance._sfxSource.volume = value;
            PlayerPrefs.SetFloat("SFXValue", _sfx.value);
        });
        _music.onValueChanged.AddListener(value =>
        {
            AudioController.Instance._musicSource.volume = value;
            PlayerPrefs.SetFloat("MusicValue", _music.value);
        });
        
        if(_inputField != null) _inputField.onSelect.AddListener(Call);
    }

    private void Call(string arg0)
    {
        _errorText.SetActive(false);
    }
}
