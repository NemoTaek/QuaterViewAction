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
        // ��Ż Ÿ�� �÷��̾��� ������, bgm�� ���߰� �̵��ϴ� ȿ���� ���
        // �� �� ���� ������ �ѿ��� �Ǹ鼭 �̵��ϴ� ȿ���� �ָ� ���� �� ����
        GameManager.instance.player.stopMove = true;
        AudioManager.instance.BGMStop();
        AudioManager.instance.EffectPlay(AudioManager.Effect.Portal);
        StartCoroutine(GameManager.instance.Blur());

        yield return new WaitForSeconds(5);

        // ���� ���������� ���ų� ���� Ŭ����ų�
        // ���� ���������� +2, ���� ���������� +3
        // ����.. GetSceneAt(n)�� �� �Ŵ����� �ε���(�ѹ� �̻� �ε� ��) ���� ���ؼ� n��° ���� �������� �Լ�����...
        // GetSceneByBuildIndex(n): ����� ���� n��° ���� �������� �Լ�. �׷��� �ѹ��� �ε����� ���� ���� ������ ������ �� �ִ�.
        // �ٵ� �ε����� ��� ���� �������� �ϸ� ������ ��´�. ���� �ȵǰ���...?

        // ���� �� �ε����� ������ �������� �� �ε������� ũ�� ���� Ŭ����
        // �ƴϸ� ���� �������� ����
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
