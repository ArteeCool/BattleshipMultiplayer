using System;
using TMPro;
using UnityEngine;

public class MenuVariable : MonoBehaviour
{
    public static MenuVariable Instance;
    
    [SerializeField] public GameObject _viewport;
    [SerializeField] public TMP_InputField _inputField;
    [SerializeField] public GameObject _errorText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _inputField.onSelect.AddListener(Call);
    }

    private void Call(string arg0)
    {
        _errorText.SetActive(false);
    }
}
