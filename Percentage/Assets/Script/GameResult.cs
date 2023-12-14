using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
{
    // ���� ����â�� ������
    // ���� ������, �ɸ� �ð�, ���� ��������, ����
    public Text result;
    public Image resultImage;
    public Text role;
    public Text stage;
    public Text time;
    public GameObject itemImageContainer;
    public Image itemImageCase;
    Image itemImage;
    public Sprite[] bannerImage;
    public int explorationScore;
    public int pickUpScore;
    public int resultScore;
    public Text resultScoreText;

    void OnEnable()
    {
        // ���� ��� ���
        PrintResult();

        // �÷��� ����� ���� ��ȭ���� �� �����ϱ�
        CalculateResultScore();

        // ȹ���� ������ �ϳ��� �����ִ� �ִϸ��̼�
        StartCoroutine(PrintGetItems());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Intro");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Permanent Scene");
            SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
        }
    }

    void PrintResult()
    {
        if (GameManager.instance.player.isDead)
        {
            result.text = "�÷��̾� ���";
            resultImage.sprite = bannerImage[0];
            AudioManager.instance.EffectPlay(AudioManager.Effect.Dead);
        }
        else
        {
            result.text = "����� �÷��̾�";
            resultImage.sprite = bannerImage[1];
            AudioManager.instance.EffectPlay(AudioManager.Effect.Victory);
        }

        role.text = GameManager.instance.player.roleName;
        stage.text = GameManager.instance.stage.ToString();
        time.text = TimeSpan.FromSeconds(GameManager.instance.elapsedTime).ToString("mm\\:ss\\:ff");
    }

    void CalculateResultScore()
    {
        // ��Ž, ų ��, ����/��ų ��ȭ ���, ����, ȹ�� ������ ��, �������� ��
        // ������ ���ϸ��� ���ھ�: �������� ���ʽ� + Ž�躸�ʽ� + ����(?)���ʽ� + ������ �г�Ƽ + �ð� �г�Ƽ + ������ �г�Ƽ
        // �������� ���ʽ�: ���� ������ �� �־����� ���� ���ʽ�. 500 / 1000 / 1500 �� ���� �þ��. �� ������ �� ���� �� �߰��ȴ�.
        int stageScore = 0;
        for (int i = 0; i < GameManager.instance.stage; i++)
        {
            stageScore += ((i + 1) * 500);
        }
        // Ž�� ���ʽ�: �Ϲݹ� = 10��, ������ = 10�� Ŭ����� + 100��, ������ = 30��, �����۹� = 30��
        // ���� floor(ceil(ų��^0.2 * 5)) ������ ��������.
        explorationScore += Mathf.CeilToInt(Mathf.Pow(GameManager.instance.player.killEnemyCount, 0.2f) * 5);
        // ���� ���ʽ�: �� ���ӿ��� �Ⱦ��� ������ ���ʽ� = ���⼭�� ���ΰ� ��Ʈ�ۿ� ����. ��Ʈ: ��ĭ�� 1, ����: 1���� 1. �� ������ ���� ��� ��������.
        // ���� ���� ���� �� ���� ü�� ��ĭ�� 1���� ��������.
        pickUpScore += (int)(GameManager.instance.player.currentHealth * 2);
        // ������ �г�Ƽ: ceil(1 - e^(���� Ƚ�� * log(0.8) / 12) * Ž�� ���ʽ� * 0.8).
        int damagePenalty = Mathf.CeilToInt((1 - Mathf.Exp(GameManager.instance.player.damagedCount * Mathf.Log10(0.8f) / 12)) * explorationScore * 0.8f);
        // �ð� �г�Ƽ: floor(ceil((�������� ���ʽ� * 0.8) * (1 - e^(��� �ð� * (-0.22) / �������� �г�Ƽ))). ���⼭ �������� �г�Ƽ�� 60 / 120 / 180 �� ���� �þ��.
        int timePenalty = Mathf.CeilToInt((stageScore / GameManager.instance.stage) * 0.8f * (1 - Mathf.Exp(((int)GameManager.instance.elapsedTime) * -0.22f / 120)));
        // ������ �г�Ƽ: floor(ceil((���� ���ʽ� * 0.8) * (1 - e^(ȹ�� ������ �� * (-0.22) / (���� �� * 2.5))))). ���⼭ ���� ���� �� �������� Ŭ���� �������� 0 / 1 / 2 �� ���� �þ��.
        int itemPenalty = Mathf.CeilToInt(pickUpScore * 0.8f * (1 - Mathf.Exp(GameManager.instance.getItemList.Count * -0.22f / (GameManager.instance.stage * 2.5f))));

        // ���� ����
        resultScore += (stageScore + explorationScore + pickUpScore - damagePenalty - timePenalty - itemPenalty);

        // ���� ������ ���� ��ȭ���� �� �߰�
        GameManager.instance.GameDataManage("��", resultScore * 10000);
    }

    IEnumerator PrintGetItems()
    {
        foreach(Item item in GameManager.instance.getItemList)
        {
            itemImage = Instantiate(itemImageCase, itemImageContainer.transform);
            itemImage.sprite = item.image;
            yield return new WaitForSeconds(0.2f);
        }

        resultScoreText.gameObject.SetActive(true);
        string scoreMoney = string.Format("{0:#,###}", resultScore * 10000);
        resultScoreText.text = $"���� ���� : {resultScore}��\n\n�� ��ȭ�ϱ� ���� {scoreMoney}�� ȹ��!";
    }
}
