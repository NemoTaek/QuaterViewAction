using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int HP;
    public MovePlayer player;
    public GameObject[] stages;
    public Text UIScore;
    public Text UIStage;
    public GameObject restartButton;
    public Image remainHealth;
    public Text remainHealthText;

    void Update()
    {
        UIScore.text = (totalPoint + stagePoint).ToString();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            HealthDown(300);

            // 체력이 깎이고도 남은 체력이 있으면 리스폰
            if (HP > 0)
            {
                PlayerRespawn();
            }
        }
    }

    void PlayerRespawn()
    {
        // 리스폰될 때 속도 0, + 방향, 초기 위치로 세팅
        player.VelocityZero();
        player.GetComponent<SpriteRenderer>().flipX = false;
        player.transform.position = new Vector3(-5, 2, 0);
    }

    public void HealthDown(int point)
    {
        // 체력바
        HP -= point;
        remainHealthText.text = HP.ToString();
        remainHealth.fillAmount = HP / 1000f;

        if (HP <= 0)
        {
            HP = 0;
            remainHealthText.text = HP.ToString();
            player.OnDie();
        }
    }

    public void NextStage()
    {
        // 스테이지 변경
        if(stageIndex < stages.Length - 1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            UIStage.text = "STAGE " + (stageIndex + 1);

            PlayerRespawn();
        }
        // 게임 클리어
        else
        {
            Time.timeScale = 0; // 멈춰!
            restartButton.SetActive(true);
        }

        // 점수 계산
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
