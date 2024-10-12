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
        if(_mainMenu != null) _mainMenu.SetActive(true);
        if(_miniProfile != null) _miniProfile.SetActive(true);
    }

    public void ActivateLobby()
    {
        TurnOffAllMenus();
        if(_lobby != null) _lobby.SetActive(true);
    }
    
    public void ActivateQuitMenu()
    {
        TurnOffAllMenus();
        if(_quitMenu != null) _quitMenu.SetActive(true);
    }
    
    public void ActivateProfileMenu()
    {
        TurnOffAllMenus();
        if(_profileMenu != null) _profileMenu.SetActive(true);
    }
    
    public void ActivateSettingsMenu()
    {
        TurnOffAllMenus();
        if(_settingsMenu != null) _settingsMenu.SetActive(true);
    }
    
    public void TurnOffAllMenus()
    {
        AudioController.Instance.PlaySfx(AudioController.Instance._click);
        if(_mainMenu != null) _mainMenu.SetActive(false);
        if(_lobby != null) _lobby.SetActive(false);
        if(_quitMenu != null) _quitMenu.SetActive(false);
        if(_settingsMenu != null) _settingsMenu.SetActive(false);
        if(_profileMenu != null) _profileMenu.SetActive(false);
        if(_miniProfile != null) _miniProfile.SetActive(false);
    }

    public void Quit()
    {
        
        Application.Quit();
    }

    public void ChangeScene(Int32 sceneIndex)
    {
        if(!NetworkController.Instance._runner.LobbyInfo.IsValid || Application.internetReachability == NetworkReachability.NotReachable) return;
        AudioController.Instance.PlaySfx(AudioController.Instance._click);
        SceneManager.LoadScene(sceneIndex);
    }
    
    public void Return()
    {
        AudioController.Instance.PlaySfx(AudioController.Instance._click);
        SceneManager.LoadScene(0);
    }
}
