using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkController : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkController Instance;
    
    [SerializeField] private GameObject _sessionPrefab;
    [SerializeField] private GameObject _viewport;
    [SerializeField] private TMP_InputField _inputField;

    public Int32 _playersReady;
    public Int32 _restartReady;

    private String _sessionName;
    
    public NetworkRunner _runner;

    private async void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            
            _runner = gameObject.AddComponent<NetworkRunner>();
            
            var result = await _runner.JoinSessionLobby(SessionLobby.ClientServer, "MyCustomLobby");
            if (!result.Ok) 
            {
                Debug.LogError($"Failed to Start: {result.ShutdownReason}");
            }
        }
        else
        {
            Destroy(gameObject);        
        }
    }

    async void StartGame(GameMode mode)
    {
        var scene = SceneRef.FromIndex(2);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid) {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = _sessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void HostServer()
    {
        if (!_runner.LobbyInfo.IsValid) return;
        if (String.IsNullOrWhiteSpace(_inputField.text)) return;
        if (_inputField.text.Length > 10) return;
        _sessionName = _inputField.text;
        StartGame(GameMode.Host);
    }

    public void CheckPlayersReadiness()
    {
        if (_playersReady == 2)
        {
            GameProcess.Instance.PlaceShips();
        }
    }
    
    public void JoinServer(String sessionName)
    {
        if (!_runner.LobbyInfo.IsValid) return;
        _sessionName = sessionName;
        StartGame(GameMode.Client);
    }
    
    public void Disconnect()
    {
        _runner.Shutdown();
        MenuController menuController = new MenuController();
        
        menuController.Return();
    }
    
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        foreach (Transform child in _viewport.transform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var sessionInfo in sessionList)
        {
            var obj = Instantiate(_sessionPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero), _viewport.transform);

            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = sessionInfo.Name;
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{sessionInfo.PlayerCount}/{sessionInfo.MaxPlayers}";
            obj.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
            {
                if (sessionInfo.MaxPlayers != sessionInfo.PlayerCount) JoinServer(sessionInfo.Name);
            });
        }
    }
    
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (_runner.IsClient)
        {
            Disconnect();
        }
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if(player.PlayerId != _runner.LocalPlayer.PlayerId) GameProcess.Instance._interactionButton.GetComponent<Button>().interactable = true;
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_runner.IsServer)
        {
            GameProcess.Instance.Restart();
            GameProcess.Instance._interactionButton.GetComponent<Button>().interactable = false;
        }
    }
    
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        key.GetInts(out var a, out var b, out var c, out var d);
        var dataInt32 = Encoding.UTF8.GetString(data);
        if (a == 0)
        {
            List<int> arrayFromByte = data.Select(i => (Int32)i).ToList();
            FieldController.Instance._enemyField = arrayFromByte;
            GameProcess.Instance._isEnemyShipsPlaced = true;
            _playersReady++;
                        
            foreach (var ship in FieldController.Instance._enemyShips)
            {
                ship.GetComponent<Image>().raycastTarget = false;
                var color = ship.GetComponent<Image>().color;
                color.a = 0f;
                ship.GetComponent<Image>().color = color;
            }
            
            GameProcess.Instance._randomButtonEnemy.SetActive(false);
            
            CheckPlayersReadiness();
        }
        else if (a == 2)
        {
            var ships = FieldController.Instance._enemyShips;
            for (int i = 0; i < ships.Count; i++)
            {
                var rectTransform = ships[i].GetComponent<RectTransform>();

                var rectTransformAnchoredPosition = new Vector2(Convert.ToInt32(data[i]) % 10 * 66,Convert.ToInt32(data[i]) / 10 * -66) + FieldController.Instance._offset;

                rectTransform.anchoredPosition = rectTransformAnchoredPosition;
            }

            FieldController.Instance._enemyShips = ships;
        }
        else if (a == 1)
        {
            var ships = FieldController.Instance._enemyShips;

            for (int i = 0; i < ships.Count; i++)
            {
                    ships[i].GetComponent<Ship>()._rotated = Convert.ToBoolean(Convert.ToInt32(data[i]));

                    Quaternion rotation = (Convert.ToInt32(data[i]) == 1) 
                        ? Quaternion.Euler(0f, 0f, 90f)
                        : Quaternion.Euler(0f, 0f, 0f);

                    ships[i].GetComponent<RectTransform>().rotation = rotation;
            }
        }
        
    }

    #region UnusedCallbacks
    
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
    
    #endregion
}
