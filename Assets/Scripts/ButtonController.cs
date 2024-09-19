using System;
using System.Collections.Generic;
using Fusion;
using IngameDebugConsole;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] public Boolean _isPlayerField;
    
    private Boolean _wasClicked;

    private Button _button;
    
    private void Start()
    {
        _button = GetComponent<Button>();

        if(GameProcess.Instance._isMultiplayer) GetComponent<Button>().onClick.AddListener(() => RPC_OnClick());
        else GetComponent<Button>().onClick.AddListener(() => OnClick());

    }
    
    [Rpc]
    public void RPC_OnClick()
    {
        if(_wasClicked) return;
        Int32 index = _isPlayerField
            ? FieldController.Instance._playerButtons.IndexOf(gameObject)
            : FieldController.Instance._enemyButtons.IndexOf(gameObject);  
        
        if (FieldController.Instance.GetPointState(index, _isPlayerField) == 0)
        {
            _button.interactable = false;
            GetComponentsInChildren<Image>()[1].sprite = FieldController.Instance._missSprite;
        }
        
        
        if (FieldController.Instance.GetPointState(index, _isPlayerField) == 1)
        {
            _button.interactable = false;
            GetComponentsInChildren<Image>()[1].sprite = FieldController.Instance._hitSprite;

            
            if (_isPlayerField)
                FieldController.Instance._playerField[index] = 2;
            else
                FieldController.Instance._enemyField[index] = 2;

            _wasClicked = true;
            FieldController.Instance.CheckIsKilled(index, _isPlayerField);
        }
    }
    
    public void OnClick()
    {
        if(_wasClicked) return;
        
        Int32 index = _isPlayerField
            ? FieldController.Instance._playerButtons.IndexOf(gameObject)
            : FieldController.Instance._enemyButtons.IndexOf(gameObject);  
        
        if (FieldController.Instance.GetPointState(index, _isPlayerField) == 0)
        {
            _button.interactable = false;
            GetComponentsInChildren<Image>()[1].sprite = FieldController.Instance._missSprite;
            GameProcess.Instance.ChangeTurn();
        }
        
        
        if (FieldController.Instance.GetPointState(index, _isPlayerField) == 1)
        {
            _button.interactable = false;
            GetComponentsInChildren<Image>()[1].sprite = FieldController.Instance._hitSprite;

            
            if (_isPlayerField)
                FieldController.Instance._playerField[index] = 2;
            else
                FieldController.Instance._enemyField[index] = 2;

            FieldController.Instance.CheckIsKilled(index, _isPlayerField);
        }
        
        _wasClicked = true;
    }
}
