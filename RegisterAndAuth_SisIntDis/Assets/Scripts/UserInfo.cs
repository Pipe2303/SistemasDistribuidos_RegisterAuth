using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UserInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    public AuthController AuthController { get; private set; }
    public void SetUserInfo(AuthController userInfo)
    {
        AuthController = userInfo;
    }
}
