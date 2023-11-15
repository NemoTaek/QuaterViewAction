using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    Image fadePanel;
    public Image background;
    public Sprite[] backgroundImage;
    int clickCount;

    void Awake()
    {
        fadePanel = GetComponent<Image>();
    }

    void Start()
    {
        // ���� �߰�
        AudioManager.instance.BGMPlay(AudioManager.BGM.bgm1);

        StartCoroutine(FadeIn());
    }

    void Update()
    {
        
    }

    IEnumerator FadeIn()
    {
        // ó���� ���� �̹���, �� �Ŀ��� �ٽ� ������� �̹���
        background.sprite = clickCount == 0 ? backgroundImage[0] : backgroundImage[1];

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
    }

    public void ClickStartButton()
    {
        // Ŭ�� ī��Ʈ �߰�, ȿ���� ���
        clickCount++;
        AudioManager.instance.ButtonClickEffectPlay();

        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        // 1�ʰ� ���� ��ο����� �ִϸ��̼� �ֱ�
        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(1f);

        int isGameStart = Random.Range(0, 2);
        if (isGameStart == 0)
        {
            StartCoroutine(FadeIn());
        }
        else
        {
            AudioManager.instance.BGMStop();
            SceneManager.LoadScene("Permanent Scene", LoadSceneMode.Additive);
            SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
        }
    }
}
