using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _lobby;
    [SerializeField] private GameObject _quitMenu;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _profileMenu;
    [SerializeField] private GameObject _miniProfile;
    
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _hostButton;

    private void Start()
    {
        if (_mainMenu == null)
        {
            if (GameProcess.Instance._isMultiplayer)
            {
                _backButton.onClick.AddListener(() =>
                {
                    NetworkController.Instance.Disconnect();
                    _backButton.onClick.AddListener(() => ChangeScene(0));
                });
            }
            else
            {
                _backButton.onClick.AddListener(() => ChangeScene(0));
            }
        }
        else
        {
            _hostButton.onClick.AddListener(NetworkController.Instance.HostServer);
        }
    }

    public void ActivateMainMenu()
    {
        TurnOffAllMenus();
        AudioController.Instance.PlaySfx(AudioController.Instance._click);
        _mainMenu.SetActive(true);
        _miniProfile.SetActive(true);
    }

    public void ActivateLobby()
    {
        TurnOffAllMenus();
        AudioController.Instance.PlaySfx(AudioController.Instance._click);
        _lobby.SetActive(true);
    }
    
    public void ActivateQuitMenu()
    {
        TurnOffAllMenus();
        AudioController.Instance.PlaySfx(AudioController.Instance._click);
        _quitMenu.SetActive(true);
    }
    
    public void ActivateProfileMenu()
    {
        TurnOffAllMenus();
        AudioController.Instance.PlaySfx(AudioController.Instance._click);
        _profileMenu.SetActive(true);
    }
    
    public void ActivateSettingsMenu()
    {
        TurnOffAllMenus();
        AudioController.Instance.PlaySfx(AudioController.Instance._click);
        _settingsMenu.SetActive(true);
    }
    
    private void TurnOffAllMenus()
    {
        _mainMenu.SetActive(false);
        _lobby.SetActive(false);
        _quitMenu.SetActive(false);
        _settingsMenu.SetActive(false);
        _profileMenu.SetActive(false);
        _miniProfile.SetActive(false);
    }

    public void Quit()
    {
        AudioController.Instance.PlaySfx(AudioController.Instance._click);
        Application.Quit();
    }

    public void ChangeScene(Int32 sceneIndex)
    {
        if (!NetworkController.Instance._runner.LobbyInfo.IsValid) return;
        AudioController.Instance.PlaySfx(AudioController.Instance._click);
        SceneManager.LoadScene(sceneIndex);
    }
    
    public void Return()
    {
        AudioController.Instance.PlaySfx(AudioController.Instance._click);
        SceneManager.LoadScene(0);
    }
}
