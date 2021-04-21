using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region Singleton
    static UIManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    [Header("Loading")]
    public GameObject loadingScreen;
    public TextMeshProUGUI loadScreenText;
    public Animator animator;

    public static void UpdateLoadScreenText(string text)
    {
        instance.loadScreenText.SetText(text);
    }

    public static void StopLoading()
    {
        instance.animator.SetTrigger("FinishLoad"); 
    }

    public void SetUnactive()
    {
        instance.loadingScreen.SetActive(false);
    }
}
