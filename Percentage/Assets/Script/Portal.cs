using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 다음 스테이지로 가거나 게임 클리어거나
            // 현재 스테이지는 +3, 다음 스테이지는 +4
            // 아하.. GetSceneAt(n)은 씬 매니저에 로딩된(한번 이상 로딩 된) 씬에 한해서 n번째 씬을 가져오는 함수였다...
            // GetSceneByBuildIndex(n): 빌드된 씬의 n번째 신을 가져오는 함수. 그래서 한번로 로딩되지 않은 씬도 있으면 가져올 수 있다.
            // 근데 인덱스에 벗어난 씬을 가져오려 하면 에러를 뱉는다. 쓰면 안되겠지...?

            // 다음 씬 인덱스가 마지막 스테이지 씬 인덱스보다 크면 게임 클리어
            // 아니면 다음 스테이지 입장
            int nextSceneIndex = GameManager.instance.stage + 4;
            if(nextSceneIndex > GameManager.instance.lastStageIndex + 3)
            {
                GameManager.instance.player.isGameClear = true;
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
    }

    void Update()
    {
        
    }

    IEnumerator LoadScene()
    {
        yield return SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
    }

    IEnumerator UnloadScene()
    {
        if(SceneManager.GetSceneByBuildIndex(GameManager.instance.stage + 3).IsValid())
        {
            yield return SceneManager.UnloadSceneAsync(GameManager.instance.stage + 3);
        }
    }
}
