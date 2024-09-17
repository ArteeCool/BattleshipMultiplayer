using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameProcess : MonoBehaviour
{
    private Boolean _isPlayerShipsPlaced;
    private Boolean _isEnemyShipsPlaced;
    
    private void Start()
    {
        foreach (var ship in FieldController.Instance._enemyShips)
        {
            ship.GetComponent<Image>().raycastTarget = false;
        }
    }

    private Boolean GameCanBeStarted(List<GameObject> ships)
    {
        foreach (var ship in ships)
        {
            if(ship.GetComponent<Ship>()._haveInvalidPlacement) return false;
            if (!ship.GetComponent<Ship>()._wasPlaced) return false;
        }

        return true;
    }

    public void PlaceShips()
    {
        if (!_isPlayerShipsPlaced && GameCanBeStarted(FieldController.Instance._playerShips))
        {
            _isPlayerShipsPlaced = true;
            foreach (var ship in FieldController.Instance._playerShips)
            {
                ship.GetComponent<Image>().raycastTarget = false;
                var color = ship.GetComponent<Image>().color;
                color.a = 0f;
                ship.GetComponent<Image>().color = color;
            }
            
            foreach (var ship in FieldController.Instance._enemyShips)
            {
                ship.GetComponent<Image>().raycastTarget = true;
            }
        }
        else if (!_isEnemyShipsPlaced && GameCanBeStarted(FieldController.Instance._enemyShips))
        {
            _isEnemyShipsPlaced = true;
            
            foreach (var ship in FieldController.Instance._enemyShips)
            {
                ship.GetComponent<Image>().raycastTarget = false;
                var color = ship.GetComponent<Image>().color;
                color.a = 0f;
                ship.GetComponent<Image>().color = color;
            }
        }

        if (_isEnemyShipsPlaced && _isPlayerShipsPlaced)
        {
            foreach (var button in FieldController.Instance._playerButtons)
            {
                button.GetComponent<Button>().interactable = true;
            }
            
            foreach (var button in FieldController.Instance._enemyButtons)
            {
                button.GetComponent<Button>().interactable = true;
            }
        }
    }
}
