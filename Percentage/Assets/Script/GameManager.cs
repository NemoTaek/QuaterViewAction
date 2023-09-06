using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Fade fadeAnimation;

    void Start()
    {
        if(!fadeAnimation.gameObject.activeSelf) fadeAnimation.gameObject.SetActive(true);
    }

    void Update()
    {

    }

    public void ClickStartButton()
    {
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        // 1�ʰ� ���� ��ο����� �ִϸ��̼� �ֱ�
        if (!fadeAnimation.gameObject.activeSelf) fadeAnimation.gameObject.SetActive(true);
        StartCoroutine(fadeAnimation.FadeOut());

        yield return new WaitForSeconds(1f);

        StopCoroutine(fadeAnimation.FadeOut());

        int isGameStart = Random.Range(0, 2);
        if(isGameStart == 0)
        {
            SceneManager.LoadScene("Intro2");
        }
        else
        {
            SceneManager.LoadScene("Stage1");
        }
    }
}
