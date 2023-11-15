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
        // 사운드 추가
        AudioManager.instance.BGMPlay(AudioManager.BGM.bgm1);

        StartCoroutine(FadeIn());
    }

    void Update()
    {
        
    }

    IEnumerator FadeIn()
    {
        // 처음은 메인 이미지, 그 후에는 다시 누르라는 이미지
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
        // 클릭 카운트 추가, 효과음 재생
        clickCount++;
        AudioManager.instance.ButtonClickEffectPlay();

        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        // 1초간 점점 어두워지는 애니메이션 넣기
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
