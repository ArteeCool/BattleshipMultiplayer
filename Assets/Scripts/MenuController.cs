using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _lobby;
    [SerializeField] private GameObject _quitMenu;
    [SerializeField] private GameObject _settingsMenu;

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
        SceneManager.LoadScene(sceneIndex);
    }
}
