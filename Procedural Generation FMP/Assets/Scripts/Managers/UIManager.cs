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

    [Header("Pausing")]
    public GameObject gameUI;
    public GameObject pauseUI;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameUI.SetActive(!gameUI.activeInHierarchy);
            pauseUI.SetActive(!pauseUI.activeInHierarchy);

            Time.timeScale = (Time.timeScale == 1) ? 0 : 1;
        }
    }

    public void Unpause()
    {
        gameUI.SetActive(true);
        pauseUI.SetActive(false);
        Time.timeScale = 1;
    }
}
