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
            // ���� ���������� ���ų� ���� Ŭ����ų�
            // ���� ���������� +3, ���� ���������� +4
            // ����.. GetSceneAt(n)�� �� �Ŵ����� �ε���(�ѹ� �̻� �ε� ��) ���� ���ؼ� n��° ���� �������� �Լ�����...
            // GetSceneByBuildIndex(n): ����� ���� n��° ���� �������� �Լ�. �׷��� �ѹ��� �ε����� ���� ���� ������ ������ �� �ִ�.
            // �ٵ� �ε����� ��� ���� �������� �ϸ� ������ ��´�. ���� �ȵǰ���...?

            // ���� �� �ε����� ������ �������� �� �ε������� ũ�� ���� Ŭ����
            // �ƴϸ� ���� �������� ����
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
