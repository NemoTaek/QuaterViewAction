using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public Sprite[] loadingImage;
    public Image character;
    int randomCharacter;
    public Image progressBar;
    float timer;
    int stage;

    void Start()
    {
        timer = 0;
        randomCharacter = Random.Range(0, 2);
        StartCoroutine(LoadingScene());
    }

    void Update()
    {
        
    }

    IEnumerator LoadingScene()
    {
        // 0: 인트로1, 1: 인트로2, 2: 로딩 씬, 3: 파괴되면 안되는 오브젝트 모음 씬
        // 4부터가 스테이지 1, 2, 3
        if (GameManager.instance == null) stage = 1;
        else stage = GameManager.instance.stage;
        Debug.Log(stage);
        AsyncOperation operation = SceneManager.LoadSceneAsync(stage + 3, LoadSceneMode.Additive);

        // 씬 로딩이 완료되어도 장면이 전환되지 않도록 설정
        operation.allowSceneActivation = false;

        // 스테이지 씬 로딩이 완료되었으면 
        while (!operation.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            // 슬라이더 바를 넣든 뭐든 맘대로 넣어봐라
            character.sprite = timer % 2 == 0 ? loadingImage[randomCharacter * 2] : loadingImage[randomCharacter * 2 + 1];

            if (operation.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, operation.progress, timer);
                if (progressBar.fillAmount >= operation.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (progressBar.fillAmount == 1.0f)
                {
                    operation.allowSceneActivation = true;
                    if (SceneManager.GetSceneByName("Intro1").IsValid())
                    {
                        yield return SceneManager.UnloadSceneAsync("Intro1");
                    }
                    if (SceneManager.GetSceneByName("Intro2").IsValid())
                    {
                        yield return SceneManager.UnloadSceneAsync("Intro2");
                    }
                    yield return SceneManager.UnloadSceneAsync("Loading");
                    yield break;
                }
            }
        }
    }
}
