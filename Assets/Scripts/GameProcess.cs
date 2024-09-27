using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameProcess : MonoBehaviour 
{
    public static GameProcess Instance;
    
    [SerializeField] public GameObject _interactionButton;
    [SerializeField] public GameObject _randomButtonPlayer;
    [SerializeField] public  GameObject _randomButtonEnemy;
    [SerializeField] public GameObject _restartButton;
    
    [SerializeField] public GameObject _rpcGameObject;
    
    [SerializeField] public Boolean _isMultiplayer;
    
    [SerializeField] public TextMeshProUGUI _infoText;
    [SerializeField] public TextMeshProUGUI _restartText;

    public Boolean _isPlayerShipsPlaced;
    
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

        if (_isMultiplayer)
        {
            foreach (var ship in FieldController.Instance._enemyShips)
            {
                ship.GetComponent<Image>().raycastTarget = false;
                _randomButtonEnemy.GetComponent<Button>().interactable = false;
            }    
            
            _restartButton.GetComponent<Button>().onClick.AddListener(() => _rpcGameObject.GetComponent<RPC>().RPC_RestartReady());
        }
        else
        {
            _restartButton.GetComponent<Button>().onClick.AddListener(() => Restart());
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
                
                foreach (var ship in FieldController.Instance._playerShips)
                {
                    ship.GetComponent<Image>().raycastTarget = false;
                    var color = ship.GetComponent<Image>().color;
                    color.a = 0f;
                    ship.GetComponent<Image>().color = color;
                }
                
                _randomButtonPlayer.SetActive(false);

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
        _restartButton.SetActive(true);
        if (_isMultiplayer)
        {
            if (whoWon)
            {
                _infoText.text = "You lost";
            }
            else
            {
                _infoText.text = "You won";
            }
        }
        else
        {
            if (whoWon)
            {
                _infoText.text = "Right player won";
            }
            else
            {
                _infoText.text = "Left player won";
            }
        }

        foreach (var button in FieldController.Instance._enemyButtons)
        {
            button.GetComponent<Button>().interactable = false;
        }
        
        foreach (var button in FieldController.Instance._playerButtons)
        {
            button.GetComponent<Button>().interactable = false;
        }
    }
    
    public void Restart()
    {
        FieldController.Instance._playerField = new List<Int32>();

        for (int i = 0; i < FieldController.Instance._playerField.Count; i++)
        {
            FieldController.Instance._playerField.Add(0);
        }
        
        FieldController.Instance._enemyField = new List<Int32>();

        for (int i = 0; i < FieldController.Instance._enemyField.Count; i++)
        {
            FieldController.Instance._enemyField.Add(0);
        }

        foreach (var button in FieldController.Instance._playerButtons)
        {
            button.GetComponent<ButtonController>()._wasClicked = false;
            button.GetComponentsInChildren<Image>()[1].sprite = null;
            button.GetComponentsInChildren<Image>()[1].color = Color.white;
        }
        
        foreach (var button in FieldController.Instance._enemyButtons)
        {
            button.GetComponent<ButtonController>()._wasClicked = false;
            button.GetComponentsInChildren<Image>()[1].sprite = null;
            button.GetComponentsInChildren<Image>()[1].color = Color.white;
        }

        Instance._isEnemyShipsPlaced = false;
        Instance._isPlayerShipsPlaced = false;
        _turn = false;
        
        Instance._randomButtonEnemy.SetActive(true);
        Instance._randomButtonPlayer.SetActive(true);
        Instance._interactionButton.SetActive(true);

        foreach (var ship in FieldController.Instance._playerShips)
        {
            ship.GetComponent<RectTransform>().anchoredPosition = ship.GetComponent<Ship>()._startPosition;
            ship.GetComponent<Image>().raycastTarget = true;
            var color = ship.GetComponent<Image>().color;
            color.a = 1f;
            ship.GetComponent<Image>().color = color;
            ship.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
            ship.GetComponent<Ship>()._rotated = false;
        }        
        
        foreach (var ship in FieldController.Instance._enemyShips)
        {
            ship.GetComponent<RectTransform>().anchoredPosition = ship.GetComponent<Ship>()._startPosition;
            ship.GetComponent<Image>().raycastTarget = true;
            var color = ship.GetComponent<Image>().color;
            color.a = 1f;
            ship.GetComponent<Image>().color = color;
            ship.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
            ship.GetComponent<Ship>()._rotated = false;
        }
        
        Instance._infoText.text = String.Empty;
        if(_isMultiplayer) NetworkController.Instance._playersReady = 0;
        if(!_isMultiplayer) _restartButton.SetActive(false);
    }
}