using System;
using System.Collections.Generic;   
using UnityEngine;
using UnityEngine.UI;

public class FieldController : MonoBehaviour
{
    public static FieldController Instance;

    [SerializeField] private GameObject _buttonPrefab;
    
    [SerializeField] private Transform _playerButtonsParent;
    [SerializeField] private Transform _enemyButtonsParent;
    
    [SerializeField] public List<GameObject> _playerButtons;
    [SerializeField] public List<GameObject> _enemyButtons;

    [SerializeField] public List<GameObject> _playerShips;
    [SerializeField] public List<GameObject> _enemyShips;

    [SerializeField] public List<Int32> _playerField;
    [SerializeField] public List<Int32> _enemyField;

    [SerializeField] private Vector2 _offset;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            var button = Instantiate(_buttonPrefab, Vector3.zero, Quaternion.identity, _playerButtonsParent.transform);
            button.GetComponent<ButtonController>()._isPlayerField = true;
            _playerButtons.Add(button);
        }        
        
        for (int i = 0; i < 100; i++)
        {
            var button = Instantiate(_buttonPrefab, Vector3.zero, Quaternion.identity, _enemyButtonsParent.transform);
            _enemyButtons.Add(button);
        }
            
        foreach (var button in _playerButtons)
        {
            button.GetComponent<Button>().interactable = false;
        }

        foreach (var button in _enemyButtons)
        {
            button.GetComponent<Button>().interactable = false;
        }
    }

    public void FillFields(Boolean isPlayer)
    {
        List<GameObject> buttons = isPlayer ? _playerButtons : _enemyButtons;
        List<GameObject> ships = isPlayer ? _playerShips : _enemyShips;
        
        List<int> field = new List<int>();

        for (int i = 0; i < buttons.Count; i++)
        {
            field.Add(0);
        }

        foreach (var button in buttons)
        {
            foreach (var ship in ships)
            {
                Ship shipClass = ship.GetComponent<Ship>();

                if (Vector2.Distance(button.GetComponent<RectTransform>().anchoredPosition + _offset,
                        ship.GetComponent<RectTransform>().anchoredPosition) == 0)
                {
                    int buttonIndex = buttons.IndexOf(button);
                    int startingRow = buttonIndex / 10;
                    int startingCol = buttonIndex % 10;

                    if (shipClass._rotated)
                    {
                        if (startingRow - shipClass._deckCount + 1 < 0)
                        {
                            ship.GetComponent<Image>().color = Color.red;
                            shipClass._haveInvalidPlacement = true;
                            continue;
                        }
                    }
                    else
                    {
                        if (startingCol + shipClass._deckCount > 10)
                        {
                            ship.GetComponent<Image>().color = Color.red;
                            shipClass._haveInvalidPlacement = true;
                            continue;
                        }
                    }
                    
                    shipClass._haveInvalidPlacement = false;
                    ship.GetComponent<Image>().color = Color.black;
                    
                    for (int i = 0; i < shipClass._deckCount; i++)
                    {
                        if (shipClass._rotated)
                        {
                            field[buttons.IndexOf(button) - i * 10] = 1;
                        }
                        else
                        {
                            field[buttons.IndexOf(button) + i] = 1;
                        }
                    }
                }
            }
        }
        
        if (isPlayer)
        {
            _playerField = field;
        }
        else
        {
            _enemyField = field;
        }
    }

     public void CheckCollisions(Boolean isPlayer)
     {
        List<int> field = isPlayer ? _playerField : _enemyField;
        List<GameObject> buttons = isPlayer ? _playerButtons : _enemyButtons;
        List<GameObject> ships = isPlayer ? _playerShips : _enemyShips;
        
        foreach (var ship in ships)
        {
            Ship shipClass = ship.GetComponent<Ship>();

            foreach (var button in buttons)
            {
                if (ship.GetComponent<RectTransform>().anchoredPosition ==
                    button.GetComponent<RectTransform>().anchoredPosition + _offset)
                {
                    Int32 startIndex = buttons.IndexOf(button);

                    if (field[startIndex] == 1)
                    {
                        List<Int32> shipDeckIndices = new List<Int32>();

                        for (int i = 0; i < shipClass._deckCount; i++)
                        {
                            Int32 newIndex = shipClass._rotated ? startIndex - 10 * i : startIndex + i;
                            shipDeckIndices.Add(newIndex);
                        }

                        bool hasCollision = false;

                        foreach (var deckIndex in shipDeckIndices)
                        {
                            Int32 deckCollisionsCount = 0;

                            for (int j = -1; j <= 1; j++)
                            {
                                for (int k = -1; k <= 1; k++)
                                {
                                    if (j == 0 && k == 0) continue;

                                    Int32 checkIndex = deckIndex - 10 * k + j;

                                    if (checkIndex >= 0 && checkIndex < field.Count &&
                                        checkIndex % 10 == (deckIndex % 10) + j)
                                    {
                                        foreach (var otherShip in ships)
                                        {
                                            if (ship == otherShip) continue;
                                            
                                            Vector2 nextPartOffset = shipClass._rotated ? new Vector2(0f, 66f) : new Vector2(66f, 0f);

                                            Vector2 shipPosition = ship.GetComponent<RectTransform>().anchoredPosition;
                                            Vector2 otherShipPosition = otherShip.GetComponent<RectTransform>().anchoredPosition;
                                            
                                            for (int l = 0; l < shipClass._deckCount; l++)
                                            {
                                                Vector2 currentDeckPosition = shipPosition + nextPartOffset * l;
                                                
                                                if (currentDeckPosition == otherShipPosition)
                                                {
                                                    hasCollision = true;
                                                    break;
                                                }
                                            }
                                        } 
                                        
                                        if (field[checkIndex] == 1 && !shipDeckIndices.Contains(checkIndex))
                                            deckCollisionsCount++;
                                    }
                                }
                            }

                            if (deckCollisionsCount > 0)
                            {
                                hasCollision = true;
                                break;
                            }
                        }

                        if (hasCollision)
                        {
                            ship.GetComponent<Image>().color = Color.red;
                            shipClass._haveInvalidPlacement = true;
                        }
                        else
                        {
                            ship.GetComponent<Image>().color = Color.black;
                            shipClass._haveInvalidPlacement = false;
                        }
                    }
                }
            }
        }

        foreach (var ship in ships)
        {
            if (ship.GetComponent<Ship>()._haveInvalidPlacement)
            {
                break;
            }
        }
    }

    public void CheckIsKilled(Int32 index, Boolean isPlayer)
    {
        List<GameObject> ships = isPlayer ? _playerShips : _enemyShips;
        List<Int32> field = isPlayer ? _playerField : _enemyField;
        List<GameObject> buttons = isPlayer ? _playerButtons : _enemyButtons;

        GameObject targetShip = null;
        Ship shipClass;
        
        foreach (var ship in ships)
        {
            shipClass = ship.GetComponent<Ship>();

            for (int i = 0; i < shipClass._deckCount; i++)
            {
                Vector2 nextPartOffset = shipClass._rotated ? new Vector2(0f, 66f) : new Vector2(66f, 0f);
                if (ship.GetComponent<RectTransform>().anchoredPosition + nextPartOffset * i == buttons[index].GetComponent<RectTransform>().anchoredPosition + _offset)
                {
                    targetShip = ship;
                    break;
                }    
            }
        }
        
        shipClass = targetShip.GetComponent<Ship>();
        Int32 shipStartIndex = 0;
        
        for (int i = 0; i < buttons.Count; i++)
        {
            if (targetShip.GetComponent<RectTransform>().anchoredPosition == buttons[i].GetComponent<RectTransform>().anchoredPosition + _offset)
            {
                shipStartIndex = i;
                break;
            }    
        }
        
        List<Int32> shipDeckIndices = new List<Int32>();
        
        for (int i = 0; i < shipClass._deckCount; i++)
        {
            Int32 newIndex = shipClass._rotated ? shipStartIndex - 10 * i : shipStartIndex + i;
            shipDeckIndices.Add(newIndex);
        }
        
        Int32 hitDeckCount = 0;
        
        foreach (var deckIndex in shipDeckIndices)
        {
            if (field[deckIndex] == 2) hitDeckCount++;
        }
        
        if (hitDeckCount == shipClass._deckCount)
        {
            foreach (var deckIndex in shipDeckIndices)
            {
                buttons[deckIndex].GetComponent<Image>().color = Color.red;
            

            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (j == 0 && k == 0) continue;
                    
                    Int32 checkIndex = deckIndex - 10 * k + j;

                    if (checkIndex >= 0 && checkIndex < field.Count &&
                        checkIndex % 10 == (deckIndex % 10) + j)
                    {
                        if (buttons[checkIndex].GetComponent<Button>().interactable)
                        {
                            buttons[checkIndex].GetComponent<ButtonController>().OnClick();
                        }
                    }
                }
            }
        }
        }
    }

    public Int32 GetPointState(Int32 index, Boolean isPlayer)
    {
        if (isPlayer) return _playerField[index];
        return _enemyField[index];
    }
}