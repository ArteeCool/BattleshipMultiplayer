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
                    Return();
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
        _mainMenu.SetActive(true);
    }

    public void ActivateLobby()
    {
        TurnOffAllMenus();
        _lobby.SetActive(true);
    }
    
    public void ActivateQuitMenu()
    {
        TurnOffAllMenus();
        _quitMenu.SetActive(true);
    }
    
    public void ActivateSettingsMenu()
    {
        TurnOffAllMenus();
        _settingsMenu.SetActive(true);
    }
    
    private void TurnOffAllMenus()
    {
        _mainMenu.SetActive(false);
        _lobby.SetActive(false);
        _quitMenu.SetActive(false);
        _settingsMenu.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ChangeScene(Int32 sceneIndex)
    {
        if (!NetworkController.Instance._runner.LobbyInfo.IsValid) return;
        SceneManager.LoadScene(sceneIndex);
    }
    
    public void Return()
    {
        SceneManager.LoadScene(0);
    }
}
