using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameProcess : MonoBehaviour
{
    public static GameProcess Instance;
    
    [SerializeField] private GameObject _interactionButton;
    [SerializeField] private GameObject _randomButtonPlayer;
    [SerializeField] private GameObject _randomButtonEnemy;
    
    [SerializeField] public Boolean _isMultiplayer;
    
    [SerializeField] private TextMeshProUGUI _infoText;
    
    private Boolean _isPlayerShipsPlaced;
    
    public Boolean _isEnemyShipsPlaced;
    
    private Boolean _turn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

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
        if (_isMultiplayer)
        {
            if (!_isPlayerShipsPlaced && GameCanBeStarted(FieldController.Instance._playerShips))
            {
                _isPlayerShipsPlaced = true;
                NetworkController.Instance._playersReady++;

                List<byte> rotationByteList = new List<byte>();                
                List<byte> indices = new List<byte>();                


                foreach (var ship in FieldController.Instance._playerShips)
                {
                    rotationByteList.Add((byte)Convert.ToInt32(ship.GetComponent<Ship>()._rotated));
                    indices.Add((byte)Convert.ToInt32(FieldController.Instance.GetIndex(ship)));
                }
                
                List<byte> byteArrayField = FieldController.Instance._playerField.Select(i => (byte)i).ToList();
                foreach (var playerRef in NetworkController.Instance._runner.ActivePlayers)
                {
                    if (playerRef.PlayerId == NetworkController.Instance._runner.LocalPlayer.PlayerId) continue;
                    
                    NetworkController.Instance._runner.SendReliableDataToPlayer(playerRef, ReliableKey.FromInts(0) , byteArrayField .ToArray());
                    NetworkController.Instance._runner.SendReliableDataToPlayer(playerRef, ReliableKey.FromInts(1) , rotationByteList .ToArray());
                    NetworkController.Instance._runner.SendReliableDataToPlayer(playerRef, ReliableKey.FromInts(2) , indices .ToArray());
                }
                
                NetworkController.Instance.CheckPlayersReadiness();
                return;
            }

            if (_isEnemyShipsPlaced && _isPlayerShipsPlaced)
            {
                foreach (var button in FieldController.Instance._playerButtons)
                {
                    button.GetComponent<Button>().interactable = false;
                }
                
                foreach (var button in FieldController.Instance._enemyButtons)
                {
                    button.GetComponent<Button>().interactable = true;
                }

                foreach (var ship in FieldController.Instance._playerShips)
                {
                    ship.GetComponent<Image>().raycastTarget = false;
                    var color = ship.GetComponent<Image>().color;
                    color.a = 0f;
                    ship.GetComponent<Image>().color = color;
                }
                
                foreach (var ship in FieldController.Instance._enemyShips)
                {
                    ship.GetComponent<Image>().raycastTarget = false;
                    var color = ship.GetComponent<Image>().color;
                    color.a = 0f;
                    ship.GetComponent<Image>().color = color;
                }
                
                _interactionButton.SetActive(false);
                _randomButtonPlayer.SetActive(false);
                _randomButtonEnemy.SetActive(false);
            }
        }
        else
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
                _randomButtonPlayer.SetActive(false);
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
                _randomButtonEnemy.SetActive(false);
            }

            if (_isEnemyShipsPlaced && _isPlayerShipsPlaced)
            {
                foreach (var button in FieldController.Instance._playerButtons)
                {
                    button.GetComponent<Button>().interactable = true;
                }
            
                foreach (var button in FieldController.Instance._enemyButtons)
                {
                    button.GetComponent<Button>().interactable = false;
                }
            
                _interactionButton.SetActive(false);
            }
        }
        
    }

    public void ChangeTurn()
    {
        ChangeFieldState(_turn);
        _turn = !_turn;
    }

    private void ChangeFieldState(Boolean turnToChange)
    {

            foreach (var button in FieldController.Instance._playerButtons)
            {
                button.GetComponent<Button>().interactable = turnToChange;
            }
            
            foreach (var button in FieldController.Instance._enemyButtons)
            {
                button.GetComponent<Button>().interactable = !turnToChange;
            }
    }

    public void Win(Boolean whoWon)
    {
        if (whoWon)
        {
            _infoText.text = "Left player won";
        }
        else
        {
            _infoText.text = "Right player won";
        }
    }

    private void Restart()
    {
        
    }
}
