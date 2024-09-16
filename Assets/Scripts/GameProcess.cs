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
            ship.GetComponent<Ship>()._isMovable = false;
        }
    }

    public Boolean GameCanBeStarted(List<GameObject> ships)
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
                ship.GetComponent<Ship>()._isMovable = false;
                var color = ship.GetComponent<Image>().color;
                color.a = 0f;
                ship.GetComponent<Image>().color = color;
            }
            
            foreach (var ship in FieldController.Instance._enemyShips)
            {
                ship.GetComponent<Ship>()._isMovable = true;
            }
        }
        else if (!_isEnemyShipsPlaced && GameCanBeStarted(FieldController.Instance._enemyShips))
        {
            _isEnemyShipsPlaced = true;
            
            foreach (var ship in FieldController.Instance._enemyShips)
            {
                ship.GetComponent<Ship>()._isMovable = false;
                var color = ship.GetComponent<Image>().color;
                color.a = 0f;
                ship.GetComponent<Image>().color = color;
            }
        }
    }
}
