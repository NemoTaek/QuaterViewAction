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
        GameManager.instance.player.stopMove = true;
        AudioManager.instance.BGMStop();
        AudioManager.instance.EffectPlay(AudioManager.Effect.Portal);


        // ���� ���������� ���ų� ���� Ŭ����ų�
        // ���� ���������� +2, ���� ���������� +3
        // ����.. GetSceneAt(n)�� �� �Ŵ����� �ε���(�ѹ� �̻� �ε� ��) ���� ���ؼ� n��° ���� �������� �Լ�����...
        // GetSceneByBuildIndex(n): ����� ���� n��° ���� �������� �Լ�. �׷��� �ѹ��� �ε����� ���� ���� ������ ������ �� �ִ�.
        // �ٵ� �ε����� ��� ���� �������� �ϸ� ������ ��´�. ���� �ȵǰ���...?

        // SceneManager.sceneCountInBuildSettings : Build Settings�� ��ϵǾ��ִ� ���� ����
        // ���� �������� + 3�� ���� ���ÿ� ��ϵǾ��ִ� ���� ���� �̻��̸� ���� ����
        // �ƴϸ� ���� ���������� �̵�
        int nextSceneIndex = GameManager.instance.stage + 3;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            // ������ ���������� ���â ���
            yield return new WaitForSeconds(2);
            StartCoroutine(GameManager.instance.GameResult());
        }
        else
        {
            // ���� ���������� ������ �� �� ���̵� ȿ�� ���� �� �ε�
            StartCoroutine(GameManager.instance.Blur());
            yield return new WaitForSeconds(5);

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
