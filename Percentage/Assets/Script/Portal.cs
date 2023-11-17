using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Portal : MonoBehaviour
{
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(UsePortal());
        }
    }

    void Update()
    {
        
    }

    IEnumerator UsePortal()
    {
        // 포탈 타면 플레이어의 움직임, bgm을 멈추고 이동하는 효과음 재생
        // 그 후 뭔가 서서히 뿌옇게 되면서 이동하는 효과를 주면 좋을 것 같음
        GameManager.instance.player.stopMove = true;
        AudioManager.instance.BGMStop();
        AudioManager.instance.EffectPlay(AudioManager.Effect.Portal);
        StartCoroutine(GameManager.instance.Blur());

        yield return new WaitForSeconds(5);

        // 다음 스테이지로 가거나 게임 클리어거나
        // 현재 스테이지는 +2, 다음 스테이지는 +3
        // 아하.. GetSceneAt(n)은 씬 매니저에 로딩된(한번 이상 로딩 된) 씬에 한해서 n번째 씬을 가져오는 함수였다...
        // GetSceneByBuildIndex(n): 빌드된 씬의 n번째 신을 가져오는 함수. 그래서 한번로 로딩되지 않은 씬도 있으면 가져올 수 있다.
        // 근데 인덱스에 벗어난 씬을 가져오려 하면 에러를 뱉는다. 쓰면 안되겠지...?

        // 다음 씬 인덱스가 마지막 스테이지 씬 인덱스보다 크면 게임 클리어
        // 아니면 다음 스테이지 입장
        int nextSceneIndex = GameManager.instance.stage + 3;
        if (nextSceneIndex > GameManager.instance.lastStageIndex + 2)
        {
            StartCoroutine(GameManager.instance.GameResult());
        }
        else
        {
            GameManager.instance.ui.ClearMapBoard();
            StartCoroutine(LoadScene());
            StartCoroutine(UnloadScene());
            GameManager.instance.GameInit();

            gameObject.SetActive(false);
        }
    }



    IEnumerator LoadScene()
    {
        yield return SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
    }

    IEnumerator UnloadScene()
    {
        if(SceneManager.GetSceneByBuildIndex(GameManager.instance.stage + 2).IsValid())
        {
            yield return SceneManager.UnloadSceneAsync(GameManager.instance.stage + 2);
        }
    }
}
