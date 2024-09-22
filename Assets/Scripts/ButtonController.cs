using System;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : NetworkBehaviour
{
    [SerializeField] public Boolean _isPlayerField;

    private Boolean _wasClicked;

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
            GetComponent<Button>().onClick.AddListener(() => RPC_OnClick(index, true));
        else
            GetComponent<Button>().onClick.AddListener(() => OnClick(_isPlayerField, true));

    }
    
    [Rpc (RpcSources.All, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_OnClick(Int32 index, Boolean needToChangeState, RpcInfo info = default)
    {
        if (info.Source.PlayerId == NetworkController.Instance._runner.LocalPlayer.PlayerId)
        {
            FieldController.Instance._enemyButtons[index].GetComponent<ButtonController>().OnClick(false, needToChangeState);
        }
        else
        {
            foreach (var button in FieldController.Instance._enemyButtons)
            {
                button.GetComponent<Button>().interactable = true;
            }
            
            FieldController.Instance._playerButtons[index].GetComponent<ButtonController>().OnClick(true, false);
        }
    }

    public void OnClick(Boolean isPlayerField, Boolean needsToChangeTurn)
    {
        {
            if (_wasClicked) return;


            Int32 index = isPlayerField
                ? FieldController.Instance._playerButtons.IndexOf(gameObject)
                : FieldController.Instance._enemyButtons.IndexOf(gameObject);

            if (FieldController.Instance.GetPointState(index, isPlayerField) == 0)
            {
                _button.interactable = false;
                GetComponentsInChildren<Image>()[1].sprite = FieldController.Instance._missSprite;
                if (needsToChangeTurn && !GameProcess.Instance._isMultiplayer)
                {
                    GameProcess.Instance.ChangeTurn();
                    
                }
                else if (needsToChangeTurn && GameProcess.Instance._isMultiplayer)
                {
                    foreach (var button in FieldController.Instance._enemyButtons)
                    {
                        button.GetComponent<Button>().interactable = false;
                    }
                }
            }
            else if(FieldController.Instance.GetPointState(index, isPlayerField) == 1)
            {
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
}