using System.Net;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class loadText : MonoBehaviour
{
    
    public GameObject txt;
    [SerializeField]


    public string weblink = "https://textmeshpro.nigelsmorris.repl.co/help.html";
    
    // Start is called before the first frame update
    void Start()
    {
        

        string html = new WebClient().DownloadString(weblink);


        txt.GetComponent<TMPro.TextMeshProUGUI>().text = html;


    }

    
   

}



