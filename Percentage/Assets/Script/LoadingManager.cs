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
    public Text tip;
    public Image progressBar;
    public float timer;
    int stage;

    void Start()
    {
        timer = 0;
        randomCharacter = Random.Range(0, 2);
        StartCoroutine(LoadingScene());
    }

    void Update()
    {
        // 로딩 중 캐릭터 애니메이션 효과
        character.sprite = Mathf.CeilToInt(timer) % 2 == 0 ? loadingImage[randomCharacter * 2] : loadingImage[randomCharacter * 2 + 1];
    }

    IEnumerator LoadingScene()
    {
        // 로딩 중에는 UI가 보이면 안되므로 크기를 0으로 임시조정
        RectTransform ui = GameManager.instance.uiCanvas.GetComponentsInChildren<RectTransform>()[1];
        ui.localScale = Vector3.zero;

        // 0: 인트로1, 1: 인트로2, 2: 로딩 씬, 3: 파괴되면 안되는 오브젝트 모음 씬
        // 4부터가 스테이지 1, 2, 3
        stage = GameManager.instance.stage;
        Debug.Log(stage);
        AsyncOperation operation = SceneManager.LoadSceneAsync(stage + 3, LoadSceneMode.Additive);

        // 씬 로딩이 완료되어도 장면이 전환되지 않도록 설정
        operation.allowSceneActivation = false;

        // 스테이지 씬 로딩이 완료되었으면 
        while (!operation.isDone)
        {
            // 반복문이 돌 때마다 유니티에게 제어권을 한번 넘겨준다.
            // 반복문이 끝나기 전에는 화면이 갱신되지 않기 때문에 한번 반복할때마다 갱신시켜주어야 한다.
            yield return null;

            timer += Time.deltaTime;

            // progress 값이 최대가 0.9인것 같다. 아무리 로딩이 빨라도 로그에서 1이 아니라 0.9가 찍힌다.
            if (operation.progress < 0.9f)
            {
                // 게이지가 차는 느낌을 들게 하기 위해 Lerp 사용
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, operation.progress, timer);
                if (progressBar.fillAmount >= operation.progress)
                {
                    timer = 0f;
                }
            }
            // 그래서 0.9 이상은 적절히 로딩의 느낌을 주기 위해 따로 Lerp 설정을 해주고
            // 로딩이 다 되어도 2초간은 팁(?)을 보라고 2초 후에 전환하도록 설정
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (progressBar.fillAmount == 1.0f)
                {
                    // 다 로딩이 되더라도 2초 후에 씬 전환
                    yield return new WaitForSeconds(2f);

                    // 전환된 씬을 보여주면서 UI도 같이 보이도록
                    operation.allowSceneActivation = true;
                    if (ui != null) ui.localScale = Vector3.one;

                    // 전에 있던 씬 unload
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
