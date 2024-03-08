using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class LeaderBoardHandler : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com";

    private UsuarioJson Usuario;
    private string Token;

    public GameObject panelLeaderboard;
    public GameObject itemLeaderboardPrefab;

    private List<GameObject> LeaderboardItems;

    // Start is called before the first frame update
    void Start()
    {
        Token = PlayerPrefs.GetString("token");
    }

    IEnumerator GetLeaderboard()
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "/api/usuarios");
        Debug.Log("Sending Request GetLeaderboard");
        request.SetRequestHeader("x-token", Token);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                UsersList data = JsonUtility.FromJson<UsersList>(request.downloadHandler.text);

                var usuariosOrganizados = data.usuarios.OrderByDescending(user => user.data.score).Take(5).ToArray();
                ShowLeaderboard(usuariosOrganizados);
            }
            else
            {
                //Debug.Log(request.responseCode + "|" + request.error);
                Debug.Log("Usuario no autenticado");
            }
        }
    }

    void ShowLeaderboard(UsuarioJson[] usuarios)
    {
        panelLeaderboard.SetActive(true);
        LeaderboardItems.Clear();
        foreach (UsuarioJson Usuario in usuarios)
        {
            //UsuarioJson usuario = usuarios[];
            GameObject item = GameObject.Instantiate(itemLeaderboardPrefab, panelLeaderboard.transform) as GameObject;
        }
        {
           
        }
    }

    public void HideLeaderboard() 
    {
        panelLeaderboard.SetActive(false);
    }
}
