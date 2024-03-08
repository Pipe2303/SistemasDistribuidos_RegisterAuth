using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Unity.VisualScripting;

public class AuthController : MonoBehaviour
{
    private string url = "https://sid-restapi.onrender.com";

    [SerializeField] private GameObject StartMenu;
    [SerializeField] private GameObject UserMenu;
    

    // Start is called before the first frame update
    void Start()
    {
        Token = PlayerPrefs.GetString("token");
        if (string.IsNullOrEmpty(Token))
        {
            Debug.Log("No hay token");
        }
        else
        {
            
            Username = PlayerPrefs.GetString("username");
            StartCoroutine("GetProfile");
            
        }
    }

    public void sendRegister()
    {
        AuthenticationData data = new AuthenticationData();

        data.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        data.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;

        StartCoroutine("Register", JsonUtility.ToJson(data));
    }

    public void sendLogin()
    {
        AuthenticationData data = new AuthenticationData();

        data.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        data.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;

        StartCoroutine("Login", JsonUtility.ToJson(data));
    }

    IEnumerator Register(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(url + "/api/usuarios", json);
        request.method = "POST";
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                Debug.Log("Registro Exitoso");
                StartCoroutine("Login", json);
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    IEnumerator Login(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(url + "/api/auth/login", json);
        request.method = "POST";
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                AuthenticationData data = JsonUtility.FromJson<AuthenticationData>(request.downloadHandler.text);
                Token = data.token;
                Username = data.usuario.username;
                PlayerPrefs.SetString("username", Username);
                PlayerPrefs.SetString("token", Token);
                Highscore.SetAmount(PlayerPrefs.GetInt("HighscoreAmount"));
                Debug.Log(data.token);
                StartMenu.SetActive(true);
                UserMenu.SetActive(false);
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    public void LogOut()
    {
        StartMenu.SetActive(false);
        UserMenu.SetActive(true);

        Token = null;
        PlayerPrefs.SetString("token",Token);
        Highscore.SetAmount(0);
    }
    public string Username { get; set; }

    public string Token { get; set; }

    IEnumerator GetProfile()
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "/api/usuarios/" + Username);
        request.SetRequestHeader("x-token", Token);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                AuthenticationData data = JsonUtility.FromJson<AuthenticationData>(request.downloadHandler.text);
                Debug.Log("El usuario " + data.usuario.username + " se encuentra autenticado y su puntaje es " +
                          data.usuario.data.score);
                StartMenu.SetActive(true);
                UserMenu.SetActive(false);
                if (PlayerPrefs.GetInt("HighscoreAmount") < data.usuario.data.score)
                {
                    PlayerPrefs.SetInt("HighscoreAmount", data.usuario.data.score);
                    Highscore.SetAmount(PlayerPrefs.GetInt("HighscoreAmount"));
                }

                UsuarioJson user = new UsuarioJson();
                user.data = new DataUser();
                user.username = Username;
                int amount = Highscore.GetAmount();
                if ( amount > user.data.score)
                {
                    user.data.score = Highscore.GetAmount();
                    Debug.Log(JsonUtility.ToJson(user));
                    StartCoroutine("SetScoreToUser", JsonUtility.ToJson(user));
                }
                /*UsuarioJson[] usuarios = new UsuarioJson[10];
                UsuarioJson[] usuariosOrganizados =
                    usuarios.OrderByDescending(user => user.data.score).Take(7).ToArray();*/
            }
            else
            {
                //Debug.Log(request.responseCode + "|" + request.error);
                Debug.Log("Usuario no autenticado");
            }
        }
    }

    IEnumerator SetScoreToUser(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(url + "/api/usuarios", json);
        request.method = "PATCH";
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-token", Token);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                AuthenticationData data = JsonUtility.FromJson<AuthenticationData>(request.downloadHandler.text);
                Debug.Log("El usuario " + data.usuario.username + " actualizó su puntaje y es " +
                          data.usuario.data.score);
            }
            else
            {
                //Debug.Log(request.responseCode + "|" + request.error);
                Debug.Log("Usuario no autenticado");
            }
        }
    }

/*public void SetScoreUser(PlayerPrefs score)
{

}*/
    [System.Serializable]
    public class AuthenticationData
    {
        public string username;
        public string password;
        public UsuarioJson usuario;
        public string token;
    }

    [System.Serializable]
    public class UsuarioJson
    {
        public string _id;
        public string username;
        public DataUser data;
    }

    [System.Serializable]
    public class DataUser
    {
        public int score;
    }
}