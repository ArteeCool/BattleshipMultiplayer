using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ProfileViewmodel : MonoBehaviour
{
    public static ProfileViewmodel Instance;
    
    [SerializeField] private Image _miniProfileAvatar;
    [SerializeField] private Image _profileAvatar;
    [SerializeField] private TextMeshProUGUI _miniProfileNickname;
    [SerializeField] private TMP_InputField _profileNickname;
    [SerializeField] private TextMeshProUGUI _winCount;

    [SerializeField] public List<Sprite> _avatarList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Setup();
        _profileNickname.onEndEdit.AddListener(ChangeNickname);

    }

    public void ChangeAvatar(Boolean next)
    {
        ProfileControl.Instance._profile.AvatarId += next ? 1 : -1;
        
        if (ProfileControl.Instance._profile.AvatarId < 0)
        {
            ProfileControl.Instance._profile.AvatarId = _avatarList.Count - 1;
        }
        else if (ProfileControl.Instance._profile.AvatarId > _avatarList.Count - 1)
        {
            ProfileControl.Instance._profile.AvatarId = 0;
        }
        
        if(_profileAvatar != null) _profileAvatar.sprite = _avatarList[ProfileControl.Instance._profile.AvatarId];
        
        ProfileControl.Instance.SaveData();
        Setup();
    }

    public void ChangeNickname(String smth)
    {
        if (_profileNickname != null)
        {
            if (_profileNickname.text != ProfileControl.Instance._profile.Name)
            {
                if (_profileNickname.text.Length >= 3)
                {
                    ProfileControl.Instance._profile.Name = _profileNickname.text;    
                }
                if (_profileNickname.text.Length >= 16)
                {
                    ProfileControl.Instance._profile.Name = _profileNickname.text.Substring(0, 12);    
                }
            }           
            
            ProfileControl.Instance.SaveData();
        }
        Setup();
    }
    
    public void Setup()
    {
        if (_miniProfileAvatar != null) _miniProfileAvatar.sprite = _avatarList[ProfileControl.Instance._profile.AvatarId];
        if (_profileAvatar != null) _profileAvatar.sprite = _avatarList[ProfileControl.Instance._profile.AvatarId];
        if (_miniProfileNickname != null) _miniProfileNickname.text = ProfileControl.Instance._profile.Name;
        if (_profileNickname != null) _profileNickname.text = ProfileControl.Instance._profile.Name;
        if (_winCount != null) _winCount.text = $"Wins:\n{ProfileControl.Instance._profile.Wins.ToString()}";
    }
}
