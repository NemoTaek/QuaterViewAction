using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
{
    // 게임 정보창에 적을거
    // 먹은 아이템, 걸린 시간, 현재 스테이지, 직업
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
        // 게임 결과 출력
        PrintResult();

        // 플레이 결과에 따라서 강화게임 돈 지급하기
        CalculateResultScore();

        // 획득한 아이템 하나씩 보여주는 애니메이션
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
            result.text = "플레이어 사망";
            resultImage.sprite = bannerImage[0];
            AudioManager.instance.EffectPlay(AudioManager.Effect.Dead);
        }
        else
        {
            result.text = "행운의 플레이어";
            resultImage.sprite = bannerImage[1];
            AudioManager.instance.EffectPlay(AudioManager.Effect.Victory);
        }

        role.text = GameManager.instance.player.roleName;
        stage.text = GameManager.instance.stage.ToString();
        time.text = TimeSpan.FromSeconds(GameManager.instance.elapsedTime).ToString("mm\\:ss\\:ff");
    }

    void CalculateResultScore()
    {
        // 플탐, 킬 수, 무기/스킬 강화 결과, 코인, 획득 아이템 수, 스테이지 등
        // 아이작 데일리런 스코어: 스테이지 보너스 + 탐험보너스 + 스웩(?)보너스 + 데미지 패널티 + 시간 패널티 + 아이템 패널티
        // 스테이지 보너스: 층을 시작할 때 주어지는 고정 보너스. 500 / 1000 / 1500 과 같이 늘어난다. 이 점수는 맵 세팅 후 추가된다.
        int stageScore = 0;
        for (int i = 0; i < GameManager.instance.stage; i++)
        {
            stageScore += ((i + 1) * 500);
        }
        // 탐험 보너스: 일반방 = 10점, 보스방 = 10점 클리어시 + 100점, 상점방 = 30점, 아이템방 = 30점
        // 또한 floor(ceil(킬수^0.2 * 5)) 점수가 더해진다.
        explorationScore += Mathf.CeilToInt(Mathf.Pow(GameManager.instance.player.killEnemyCount, 0.2f) * 5);
        // 스웩 보너스: 한 게임에서 픽업을 수집한 보너스 = 여기서는 코인과 하트밖에 없다. 하트: 반칸당 1, 코인: 1원당 1. 이 점수는 습득 즉시 더해진다.
        // 또한 게임 종료 시 현재 체력 반칸당 1점이 더해진다.
        pickUpScore += (int)(GameManager.instance.player.currentHealth * 2);
        // 데미지 패널티: ceil(1 - e^(맞은 횟수 * log(0.8) / 12) * 탐험 보너스 * 0.8).
        int damagePenalty = Mathf.CeilToInt((1 - Mathf.Exp(GameManager.instance.player.damagedCount * Mathf.Log10(0.8f) / 12)) * explorationScore * 0.8f);
        // 시간 패널티: floor(ceil((스테이지 보너스 * 0.8) * (1 - e^(경과 시간 * (-0.22) / 스테이지 패널티))). 여기서 스테이지 패널티는 60 / 120 / 180 과 같이 늘어난다.
        int timePenalty = Mathf.CeilToInt((stageScore / GameManager.instance.stage) * 0.8f * (1 - Mathf.Exp(((int)GameManager.instance.elapsedTime) * -0.22f / 120)));
        // 아이템 패널티: floor(ceil((스웩 보너스 * 0.8) * (1 - e^(획득 아이템 수 * (-0.22) / (엔딩 값 * 2.5))))). 여기서 엔딩 값은 각 스테이지 클리어 기준으로 0 / 1 / 2 와 같이 늘어난다.
        int itemPenalty = Mathf.CeilToInt(pickUpScore * 0.8f * (1 - Mathf.Exp(GameManager.instance.getItemList.Count * -0.22f / (GameManager.instance.stage * 2.5f))));

        // 최종 점수
        resultScore += (stageScore + explorationScore + pickUpScore - damagePenalty - timePenalty - itemPenalty);

        // 최종 점수에 따라 강화게임 돈 추가
        GameManager.instance.GameDataManage("돈", resultScore * 10000);
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
        resultScoreText.text = $"최종 점수 : {resultScore}점\n\n검 강화하기 게임 {scoreMoney}원 획득!";
    }
}
