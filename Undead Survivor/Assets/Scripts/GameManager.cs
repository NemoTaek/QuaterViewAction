using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("------- Component -------")]
    public Player player;
    public PoolManager pool;
    public static GameManager instance;
    public LevelUp levelUpUI;
    public TreasureUI treasureUI;
    public Result resultUI;
    public GameObject enemyCleaner;
    public Transform joystickUI;
    public Item[] gameItems;
    public Character[] gameCharacters;

    [Header("------- Game Control -------")]
    public float gameTime;
    public float maxGameTime;
    public bool isTimePassing;

    [Header("------- Player Info -------")]
    public int playerId;
    public int level;
    public int kill;
    public int exp;
    public float[] nextExp;
    public float health;
    public float maxHealth = 100;
    public int currentCoin;
    public int coin;


    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;   // 설정하지 않으면 30프레임까지 떨어질 수 있다

        if (!PlayerPrefs.HasKey("Coin"))
        {
            PlayerPrefs.SetInt("Coin", 0);
        }
        coin = PlayerPrefs.GetInt("Coin");
    }

    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;
        player.gameObject.SetActive(true);

        // 처음 시작 시 기본무기 지급
        if (playerId == 4)
        {
            levelUpUI.Select(0);
            levelUpUI.Select(1);
        }
        else
        {
            levelUpUI.Select(playerId % 2);
        }

        // 게임 시작
        Resume();
        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);

        // 일정 시간마다 상자 소환
        StartCoroutine(SpawnTreasureBox());
    }

    IEnumerator GameOverRoutine()
    {
        isTimePassing = false;

        yield return new WaitForSeconds(0.5f);

        // 동전 먹은 개수 세팅
        coin += currentCoin;
        PlayerPrefs.SetInt("Coin", coin);

        // 결과창 UI 출력
        resultUI.gameObject.SetActive(true);
        resultUI.Lose();

        // 효과음 출력
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
        AudioManager.instance.PlayBgm(false);

        // 멈춰
        Time.timeScale = 0;
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    public void GameRetry()
    {
        currentCoin = 0;
        SceneManager.LoadScene(0);
    }

    public void GameExit()
    {
        Application.Quit();
    }

    IEnumerator GameVictoryRoutine()
    {
        isTimePassing = false;
        enemyCleaner.SetActive(true);   // 화면의 모든 적을 제거

        yield return new WaitForSeconds(0.5f);

        // 동전 먹은 개수 세팅
        coin += currentCoin;
        PlayerPrefs.SetInt("Coin", coin);

        // 결과창 UI 출력
        resultUI.gameObject.SetActive(true);
        resultUI.Win();

        // 효과음 출력
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
        AudioManager.instance.PlayBgm(false);

        // 멈춰
        Time.timeScale = 0;
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    public void GetExp()
    {
        // 승리 후 화면의 모든 적을 제거하는 중에 경험치가 오를 가능성을 제거
        if (!isTimePassing) return;

        exp++;
        if (exp == nextExp[level])
        {
            if (level < nextExp.Length) level++;
            exp = 0;
            levelUpUI.Show();
        }
    }

    public void Pause()
    {
        isTimePassing = false;
        Time.timeScale = 0; // 0~1 사이의 값으로 시간이 흐르는 속도. default는 1
        joystickUI.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isTimePassing = true;
        Time.timeScale = 1;
        joystickUI.localScale = Vector3.one;
    }

    IEnumerator SpawnTreasureBox()
    {
        yield return new WaitForSeconds(120f);
        GameObject box = pool.GetPool(4);
        Vector3 randomVector = player.transform.position + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
        box.transform.position = randomVector;

        // 일정 시간마다 상자 소환
        StartCoroutine(SpawnTreasureBox());
    }

    void Update()
    {
        if (!isTimePassing) return;

        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory();
        }
    }
}
