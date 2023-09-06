using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
