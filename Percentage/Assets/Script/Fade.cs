using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    Image fadePanel;
    bool isActive;

    void Awake()
    {
        fadePanel = GetComponent<Image>();
        isActive = fadePanel.gameObject.activeSelf;
    }

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        
    }

    IEnumerator FadeIn()
    {
        Color color = fadePanel.color;
        for(float i= 1f; i >= 0; i -= 0.01f) {
            color.a = i;
            fadePanel.color = color;
            yield return new WaitForSeconds(0.01f);
        }

        fadePanel.gameObject.SetActive(false);
    }

    public IEnumerator FadeOut()
    {
        Color color = fadePanel.color;
        for (float i = 0; i <= 1f; i += 0.01f)
        {
            color.a = i;
            fadePanel.color = color;
            yield return new WaitForSeconds(0.01f);
        }

        fadePanel.gameObject.SetActive(false);
    }

    public void ClickStartButton()
    {
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        // 1초간 점점 어두워지는 애니메이션 넣기
        StartCoroutine(FadeOut());

        yield return new WaitForSeconds(1f);

        StopCoroutine(FadeOut());

        int isGameStart = Random.Range(0, 2);
        if (isGameStart == 0)
        {
            SceneManager.LoadScene("Intro2");
        }
        else
        {
            SceneManager.LoadScene("Permanent Scene", LoadSceneMode.Additive);
            SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
        }
    }
}
