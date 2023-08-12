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
        Application.targetFrameRate = 60;   // �������� ������ 30�����ӱ��� ������ �� �ִ�

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

        // ó�� ���� �� �⺻���� ����
        if (playerId == 4)
        {
            levelUpUI.Select(0);
            levelUpUI.Select(1);
        }
        else
        {
            levelUpUI.Select(playerId % 2);
        }

        // ���� ����
        Resume();
        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);

        // ���� �ð����� ���� ��ȯ
        StartCoroutine(SpawnTreasureBox());
    }

    IEnumerator GameOverRoutine()
    {
        isTimePassing = false;

        yield return new WaitForSeconds(0.5f);

        // ���� ���� ���� ����
        coin += currentCoin;
        PlayerPrefs.SetInt("Coin", coin);

        // ���â UI ���
        resultUI.gameObject.SetActive(true);
        resultUI.Lose();

        // ȿ���� ���
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
        AudioManager.instance.PlayBgm(false);

        // ����
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
        enemyCleaner.SetActive(true);   // ȭ���� ��� ���� ����

        yield return new WaitForSeconds(0.5f);

        // ���� ���� ���� ����
        coin += currentCoin;
        PlayerPrefs.SetInt("Coin", coin);

        // ���â UI ���
        resultUI.gameObject.SetActive(true);
        resultUI.Win();

        // ȿ���� ���
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
        AudioManager.instance.PlayBgm(false);

        // ����
        Time.timeScale = 0;
    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }

    public void GetExp()
    {
        // �¸� �� ȭ���� ��� ���� �����ϴ� �߿� ����ġ�� ���� ���ɼ��� ����
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
        Time.timeScale = 0; // 0~1 ������ ������ �ð��� �帣�� �ӵ�. default�� 1
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

        // ���� �ð����� ���� ��ȯ
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
