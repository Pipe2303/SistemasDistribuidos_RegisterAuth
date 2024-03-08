using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Listing : MonoBehaviour
{
    public TextMeshProUGUI TextUsername;
    public TextMeshProUGUI TextScore;

    public void SetItem(UsuarioJson usuario, int pos) 
    {
        TextUsername.text = usuario.username;
        TextScore.text = "" + usuario.data.score;
    }
}
