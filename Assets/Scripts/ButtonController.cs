using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] public Boolean _isPlayerField;

    private Button _button;
    
    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        Int32 index = _isPlayerField
            ? FieldController.Instance._playerButtons.IndexOf(gameObject)
            : FieldController.Instance._enemyButtons.IndexOf(gameObject);  
        
        if (FieldController.Instance.GetPointState(index, _isPlayerField) == 0)
        {
            _button.interactable = false;
            GetComponent<Image>().color = Color.cyan;
        }
        
        
        if (FieldController.Instance.GetPointState(index, _isPlayerField) == 1)
        {
            _button.interactable = false;
            GetComponent<Image>().color = Color.magenta;
            
            if (_isPlayerField)
                FieldController.Instance._playerField[index] = 2;
            else
                FieldController.Instance._enemyField[index] = 2;
            
            FieldController.Instance.CheckIsKilled(index, _isPlayerField);
        }
    }
}
