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
        // �ε� �� ĳ���� �ִϸ��̼� ȿ��
        character.sprite = Mathf.CeilToInt(timer) % 2 == 0 ? loadingImage[randomCharacter * 2] : loadingImage[randomCharacter * 2 + 1];
    }

    IEnumerator LoadingScene()
    {
        // �ε� �߿��� UI�� ���̸� �ȵǹǷ� ũ�⸦ 0���� �ӽ�����
        RectTransform ui = GameManager.instance.uiCanvas.GetComponentsInChildren<RectTransform>()[1];
        ui.localScale = Vector3.zero;

        // 0: ��Ʈ��1, 1: ��Ʈ��2, 2: �ε� ��, 3: �ı��Ǹ� �ȵǴ� ������Ʈ ���� ��
        // 4���Ͱ� �������� 1, 2, 3
        stage = GameManager.instance.stage;
        Debug.Log(stage);
        AsyncOperation operation = SceneManager.LoadSceneAsync(stage + 3, LoadSceneMode.Additive);

        // �� �ε��� �Ϸ�Ǿ ����� ��ȯ���� �ʵ��� ����
        operation.allowSceneActivation = false;

        // �������� �� �ε��� �Ϸ�Ǿ����� 
        while (!operation.isDone)
        {
            // �ݺ����� �� ������ ����Ƽ���� ������� �ѹ� �Ѱ��ش�.
            // �ݺ����� ������ ������ ȭ���� ���ŵ��� �ʱ� ������ �ѹ� �ݺ��Ҷ����� ���Ž����־�� �Ѵ�.
            yield return null;

            timer += Time.deltaTime;

            // progress ���� �ִ밡 0.9�ΰ� ����. �ƹ��� �ε��� ���� �α׿��� 1�� �ƴ϶� 0.9�� ������.
            if (operation.progress < 0.9f)
            {
                // �������� ���� ������ ��� �ϱ� ���� Lerp ���
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, operation.progress, timer);
                if (progressBar.fillAmount >= operation.progress)
                {
                    timer = 0f;
                }
            }
            // �׷��� 0.9 �̻��� ������ �ε��� ������ �ֱ� ���� ���� Lerp ������ ���ְ�
            // �ε��� �� �Ǿ 2�ʰ��� ��(?)�� ����� 2�� �Ŀ� ��ȯ�ϵ��� ����
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (progressBar.fillAmount == 1.0f)
                {
                    // �� �ε��� �Ǵ��� 2�� �Ŀ� �� ��ȯ
                    yield return new WaitForSeconds(2f);

                    // ��ȯ�� ���� �����ָ鼭 UI�� ���� ���̵���
                    operation.allowSceneActivation = true;
                    if (ui != null) ui.localScale = Vector3.one;

                    // ���� �ִ� �� unload
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
