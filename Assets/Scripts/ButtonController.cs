using System;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : NetworkBehaviour
{
    [SerializeField] public Boolean _isPlayerField;

    [SerializeField] public Boolean _wasClicked;

    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();

        var index = 0;

        if (_isPlayerField)
            index = FieldController.Instance._playerButtons.IndexOf(gameObject);  
        else
            index = FieldController.Instance._enemyButtons.IndexOf(gameObject);

        if (GameProcess.Instance._isMultiplayer)
            GetComponent<Button>().onClick.AddListener(() => RPC_OnClick(index, true, true));
        else
            GetComponent<Button>().onClick.AddListener(() => OnClick(_isPlayerField, 0, true, true));

    }
    
    [Rpc (RpcSources.All, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_OnClick(Int32 index, Boolean needToChangeState, Boolean playSound, RpcInfo info = default)
    {
        if (info.Source.PlayerId == NetworkController.Instance._runner.LocalPlayer.PlayerId)
        {
            if (needToChangeState)
            {
                foreach (var button in FieldController.Instance._enemyButtons)
                {
                    if (button.GetComponent<ButtonController>()._wasClicked) continue;
                    button.GetComponent<Button>().interactable = false;
                }
                GameProcess.Instance._infoText.text = "Waiting for enemy choice";
            }
            
            FieldController.Instance._enemyButtons[index].GetComponent<ButtonController>().OnClick(false, info.Source.PlayerId, needToChangeState, playSound);
        }
        else
        {
            if (needToChangeState)
            {
                foreach (var button in FieldController.Instance._enemyButtons)
                {
                    if (button.GetComponent<ButtonController>()._wasClicked) continue;
                    button.GetComponent<Button>().interactable = true;
                }
                GameProcess.Instance._infoText.text = "Your turn";
            }

            FieldController.Instance._playerButtons[index].GetComponent<ButtonController>().OnClick(true, info.Source.PlayerId,false, playSound);
        }
    }

    public void OnClick(Boolean isPlayerField, Int32 playerId, Boolean needsToChangeTurn, Boolean playSound)
    {
        if (_wasClicked) return;

            Int32 index = isPlayerField
                ? FieldController.Instance._playerButtons.IndexOf(gameObject)
                : FieldController.Instance._enemyButtons.IndexOf(gameObject);

            if (FieldController.Instance.GetPointState(index, isPlayerField) == 0)
            {
                GetComponentsInChildren<Image>()[1].sprite = FieldController.Instance._missSprite;
                
                if (playSound)
                {
                    AudioController.Instance.PlaySfx(AudioController.Instance._miss);
                }
                
                if (needsToChangeTurn && !GameProcess.Instance._isMultiplayer)
                {
                    GameProcess.Instance.ChangeTurn();
                }
                else if (needsToChangeTurn && GameProcess.Instance._isMultiplayer)
                {
                    Boolean change = playerId != NetworkController.Instance._runner.LocalPlayer.PlayerId;

                    foreach (var button in FieldController.Instance._enemyButtons)
                    {
                        if (button.GetComponent<ButtonController>()._wasClicked) continue;
                        button.GetComponent<Button>().interactable = change;
                    }
                    
                    GameProcess.Instance._infoText.text = change ? "Your turn" : "Waiting for enemy choice";
                    
                }

                _button.interactable = false;
            }
            else if(FieldController.Instance.GetPointState(index, isPlayerField) == 1)
            {
                Boolean change = playerId == NetworkController.Instance._runner.LocalPlayer.PlayerId;

                if (GameProcess.Instance._isMultiplayer)
                {
                    foreach (var button in FieldController.Instance._enemyButtons)
                    {
                        if (button.GetComponent<ButtonController>()._wasClicked) continue;
                        button.GetComponent<Button>().interactable = change;
                    }
                    GameProcess.Instance._infoText.text = change ? "Your turn" : "Waiting for enemy choice";
                }
                
                _button.interactable = false;
                
                GetComponentsInChildren<Image>()[1].sprite = FieldController.Instance._hitSprite;

                if (isPlayerField)
                    FieldController.Instance._playerField[index] = 2;
                else
                    FieldController.Instance._enemyField[index] = 2;

                FieldController.Instance.CheckIsKilled(index, isPlayerField);
            }

            _wasClicked = true;
    }
}