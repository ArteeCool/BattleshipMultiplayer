using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameProfileViewmodel : MonoBehaviour
{
    public static GameProfileViewmodel Instance;
    
    [SerializeField] private TextMeshProUGUI _nicknameText;
    [SerializeField] private Image _avatarImage;
    [SerializeField] public TextMeshProUGUI _winCountText;
    
    [SerializeField] private TextMeshProUGUI _enemyNicknameText;
    [SerializeField] private Image _enemyAvatarImage;
    [SerializeField] public TextMeshProUGUI _enemyWinCountText;
    
    [SerializeField] public List<Sprite> _avatarList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void Setup(Boolean isPlayer, String nickname, Int32 imageId, Int32 winCount)
    {
        if (isPlayer)
        {
            if(nickname != null) _nicknameText.text = nickname;
            if(imageId != -1) _avatarImage.sprite = _avatarList[imageId];
            if(winCount != -1) _winCountText.text = $"Wins:{winCount.ToString()}";
        }
        else
        {
            if(nickname != null) _enemyNicknameText.text = nickname;
            if(imageId != -1) _enemyAvatarImage.sprite = _avatarList[imageId];
            if(winCount != -1) _enemyWinCountText.text = $"Wins:{winCount.ToString()}";
        }
    }

    public void Reset()
    {
        _enemyNicknameText.text = "Player#000000000";
        _enemyAvatarImage.sprite = null;
        _enemyWinCountText.text = "Wins:0";
    }
}