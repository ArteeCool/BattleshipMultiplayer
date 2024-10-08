using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

public class ProfileControl : MonoBehaviour
{
    public static ProfileControl Instance;

    [SerializeField] public Profile _profile;
    
    [SerializeField] public String _playFabId;
    [SerializeField] private String _uniqueId;

    private Boolean _logged;
    private Boolean _profileSynchronized;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        PlayFabSettings.DeveloperSecretKey = "BKKKKIJO1NBPPX8TOFUSWEHDG77M59J3MF8IQUNGFDCG9K6ZYE";
        
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "D90EF";
        }
    }

    private void Start()
    {
        StartCoroutine(StartUp());
    }

    private IEnumerator StartUp()
    {
        CheckForExistence();
       ProfileViewmodel.Instance.Setup();
        _profile = JsonConvert.DeserializeObject<Profile>(ReadFromFile());
        
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Login();

            while (!_logged)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        
        GetProfile();

        while (!_profileSynchronized)
        {
            yield return new WaitForEndOfFrame();    
        }
        
        ProfileViewmodel.Instance.Setup();
    }

    public void SaveData()
    {
        SetProfile();
        WriteToFile(JsonConvert.SerializeObject(_profile));
    }

    private void Login()
    {
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
        {
            CustomId = (Application.isEditor ? "editor_" : "") + SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        }, result =>
        {
            _playFabId = result.PlayFabId;
            _uniqueId = (Application.isEditor ? "editor_" : "") + SystemInfo.deviceUniqueIdentifier;
            _logged = true;
        }, error =>
        {
            _logged = false;
        });
    }


    private void GetProfile()
    {
        if (!_logged || Application.internetReachability == NetworkReachability.NotReachable)
        {
            _profile.LastTimeWasInGame = DateTime.Now;
            SaveData();
            return;
        }
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = _playFabId,
            Keys = null
        }, result =>
        {
            if (result.Data.TryGetValue("Profile", out var record) && !String.IsNullOrEmpty(record.Value))
            {
                Profile profile = JsonConvert.DeserializeObject<Profile>(record.Value);
                
                if (_profile.LastTimeWasInGame > profile.LastTimeWasInGame)
                {
                    _profile.LastTimeWasInGame = DateTime.Now;
                    SaveData();
                }
                else
                {
                    profile.LastTimeWasInGame = DateTime.Now;
                    _profile = profile;
                    SaveData();
                }
            }
            else
            {
                SetProfile();
            }

            _profileSynchronized = true;
        }, error =>
        {
            
        });
    }
    
    private void SetProfile()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<String, String>
            {
                {"Profile", JsonConvert.SerializeObject(_profile)}
            }
        },result =>
        {

        }, error =>
        {
            
        });
    }
    
    private void WriteToFile(String str)
    {
        File.WriteAllText(Application.persistentDataPath + $"\\{(Application.isEditor?"editor_":"")}profile.txt", str);
    }

    private String ReadFromFile()
    {
        return File.ReadAllText(Application.persistentDataPath + $"\\{(Application.isEditor?"editor_":"")}profile.txt");
    }

    private void CheckForExistence()
    {
        if (!File.Exists(Application.persistentDataPath + $"\\{(Application.isEditor?"editor_":"")}profile.txt"))
        {
            _profile.Name = GeneratePlayerNickname();
            WriteToFile(JsonConvert.SerializeObject(_profile));
        }
    }
    
    private String GeneratePlayerNickname()
    {
        Int32 randomInt32 = Random.Range(100000000, 999999999);
        return "Player#" + randomInt32;
    }
}

[Serializable]
public class Profile
{
    public String Name;
    public Int32 Wins;
    public Int32 AvatarId;
    public DateTime LastTimeWasInGame;
}