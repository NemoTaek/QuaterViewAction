using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("------- Component -------")]
    public static GameManager instance;
    public AudioSource bgmPlayer;
    public AudioSource[] effectPlayer;
    public AudioClip[] bgmClip;
    public AudioClip[] effectClip;
    public Block blockPrefab;
    public Board board;
    public Map map;

    [Header("------- Panel UI -------")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject introUI;
    public GameUI gameUI;
    public GameObject selectMapUI;
    public GameObject stageClearPanel;

    public GameObject levelStage;
    public GameObject shutter;
    SpriteRenderer[] shutterLines;

    [Header("------- Sound Info -------")]
    public bool isGamePlaying = false;
    public bool isLoading = false;
    int effectCursor;
    public enum Bgm { OPENING, PLAYING };
    public enum Effect { MOVE, DROP, ROTATE, LINECLEAR, ADDBLOCK, ADDLINE, SELECT, GAMEOVER, LEVELUP, STAGECLEAR };

    [Header("------- Map Info -------")]
    public Dictionary<int, int[]> mapInfo;
    public Dictionary<int, int[]> stageMapInfo;

    [Header("------- Game Info -------")]
    public int score;
    public int line;
    public int level;
    public int combo;
    public int highestCombo;
    public int[] levelStandard;
    public int[] stageStandard;

    public float fallingInterval;   // �ѹ� �����ϴ� �ֱ�
    public float landingInterval;   // �ٴڿ� ����� �� ������ �������� �ֱ�

    public enum ModeList { Normal, Hard, Stage };
    public int mode;
    public int blockCount;  // ���� ����� �� ����(�ʿ� ������� ���̳� ���� �߰��� �� ���)

    // �������� ��� ���� ����
    public int remainLines;
    public int stage;

    [Header("------- Game UI -------")]
    public Text comboText;


    void Awake()
    {
        instance = this;
        levelStandard = new int[10] { 10, 20, 30, 40, 50, 75, 100, 130, 160, 200 };
        stageStandard = new int[18] { 5, 10, 12, 10, 13, 16, 12, 15, 18, 12, 15, 18, 12, 15, 18, 12, 15, 18 };
        shutterLines = shutter.GetComponentsInChildren<SpriteRenderer>(true);
    }

    void Start()
    {
        // ���� ��� �ε�
        if (!PlayerPrefs.HasKey("FirstScore"))
        {
            PlayerPrefs.SetInt("FirstScore", 0);
        }
        if (!PlayerPrefs.HasKey("SecondScore"))
        {
            PlayerPrefs.SetInt("SecondScore", 0);
        }
        if (!PlayerPrefs.HasKey("ThirdScore"))
        {
            PlayerPrefs.SetInt("ThirdScore", 0);
        }
        if (!PlayerPrefs.HasKey("FirstCombo"))
        {
            PlayerPrefs.SetInt("FirstCombo", 0);
        }
        if (!PlayerPrefs.HasKey("SecondCombo"))
        {
            PlayerPrefs.SetInt("SecondCombo", 0);
        }
        if (!PlayerPrefs.HasKey("ThirdCombo"))
        {
            PlayerPrefs.SetInt("ThirdCombo", 0);
        }

        // �� ���� �ε�
        mapInfo = new Dictionary<int, int[]>()
        {
            {0, map.MapEmpty() }, {1, map.MapCliff() }, {2, map.MapHoledCliff()}, {3, map.MapFish()}, {4, map.MapSkyLand()}, {5, map.MapDot()}
        };

        stageMapInfo = new Dictionary<int, int[]>()
        {
            {1, map.MapEmpty() }, {2, map.MapEmpty() }, {3, map.MapEmpty()}, {4, map.MapMission1()}, {5, map.MapMission2()}, {6, map.MapMission3()},
            {7, map.MapEmpty() }, {8, map.MapEmpty() }, {9, map.MapEmpty()}, {10, map.MapEmpty()}, {11, map.MapEmpty()}, {12, map.MapEmpty()},
            {13, map.MapMission4() }, {14, map.MapMission5() }, {15, map.MapMission6()}, {16, map.MapMission7()}, {17, map.MapMission8()}, {18, map.MapMission7()}
        };

        // ������ ���
        StartCoroutine(BgmPlay(Bgm.OPENING));
    }

    void Update()
    {
        // �޺� ����
        comboText.text = combo + " Combo!!";
        if (combo > 2) comboText.gameObject.transform.localScale = Vector3.one;
        else comboText.gameObject.transform.localScale = Vector3.zero;

        // �� ���� �ӵ� ����
        if(mode != 2)
        {
            fallingInterval = Mathf.Max((3.0f / level), 0.3f);
        }
        else
        {
            fallingInterval = Mathf.Max((3.0f / stage), 0.3f);
        }
    }

    void SetHighestScore()
    {
        int firstScore = PlayerPrefs.GetInt("FirstScore");
        int secondScore = PlayerPrefs.GetInt("SecondScore");
        int thirdScore = PlayerPrefs.GetInt("ThirdScore");

        if (firstScore < score)
        {
            PlayerPrefs.SetInt("FirstScore", score);
            PlayerPrefs.SetInt("SecondScore", firstScore);
            PlayerPrefs.SetInt("ThirdScore", secondScore);
        }
        else if(secondScore < score)
        {
            PlayerPrefs.SetInt("SecondScore", score);
            PlayerPrefs.SetInt("ThirdScore", secondScore);
        }
        else if(thirdScore < score) {
            PlayerPrefs.SetInt("ThirdScore", score);
        }
    }

    void SetHighestCombo()
    {
        int firstCombo = PlayerPrefs.GetInt("FirstCombo");
        int secondCombo = PlayerPrefs.GetInt("SecondCombo");
        int thirdCombo = PlayerPrefs.GetInt("ThirdCombo");

        if (firstCombo < highestCombo)
        {
            PlayerPrefs.SetInt("FirstCombo", highestCombo);
            PlayerPrefs.SetInt("SecondCombo", firstCombo);
            PlayerPrefs.SetInt("ThirdCombo", secondCombo);
        }
        else if (secondCombo < highestCombo)
        {
            PlayerPrefs.SetInt("SecondCombo", highestCombo);
            PlayerPrefs.SetInt("ThirdCombo", secondCombo);
        }
        else if (thirdCombo < highestCombo)
        {
            PlayerPrefs.SetInt("ThirdCombo", highestCombo);
        }
    }

    public void EnterSelectMapMenu()
    {
        // �� ���� UI
        selectMapUI.transform.localScale = Vector3.one;

        // Ŭ�� ���� ���
        EffectPlay(Effect.SELECT);

        // ���� ���õ� �� ����
        map.mapTitle.text = map.mapTitleArray[map.mapIndex];
        board.CreateMap(mapInfo[map.mapIndex]);
    }

    public void SelectMap()
    {
        // �� ���� UI
        selectMapUI.transform.localScale = Vector3.zero;

        // Ŭ�� ���� ���
        EffectPlay(Effect.SELECT);
    }

    public void SetNoramlMode()
    {
        // �븻��� ����
        mode = (int)ModeList.Normal;
        GameStart();
    }

    public void SetHardMode()
    {
        // �ϵ��� ����
        mode = (int)ModeList.Hard;
        GameStart();
    }

    public void SetStageMode()
    {
        // ����������� ����
        mode = (int)ModeList.Stage;
        remainLines = stageStandard[stage - 1];

        // �ٸ� ���� ���õǾ��ִٸ� �⺻������ �ٽ� ����
        board.CreateMap(map.MapEmpty());
        GameStart();
    }

    public void GameStart()
    {
        // ���� ��ŸƮ
        isGamePlaying = true;
        StartCoroutine(BgmPlay(Bgm.PLAYING));
        SetGameUI();

        // Ŭ�� ���� ���
        EffectPlay(Effect.SELECT);

        // ���� ���� �Ŀ� ù ��Ʈ�ι̳� ����
        board.InitTetromino();
    }

    public IEnumerator SetNextStage()
    {
        // bgm ����
        bgmPlayer.Stop();
        isLoading = true;
        isGamePlaying = false;
        board.tetromino.gameObject.SetActive(false);
        stageClearPanel.SetActive(true);

        // �������� Ŭ���� ȿ���� ��� (�� 5��)
        yield return new WaitForSeconds(0.5f);
        EffectPlay(Effect.STAGECLEAR);

        // ȿ���� ��� �Ϸ� �� ���� �������� ����
        yield return new WaitForSeconds(5f);
        StartCoroutine(SetLoading());
        stage++;
        remainLines = stageStandard[stage - 1];
        blockCount = 0;

        // �� �����ְ� 1�� �� ����
        yield return new WaitForSeconds(Time.deltaTime * 40);
        yield return new WaitForSecondsRealtime(3f);
        board.InitTetromino();
        isGamePlaying = true;
        isLoading = false;
        board.tetromino.gameObject.SetActive(true);

        // bgm ���
        SetBgm(Bgm.PLAYING);
        bgmPlayer.Play();
    }

    public IEnumerator SetLoading()
    {
        int count = 0;
        while(count < 200)
        {
            if(count % 10 == 0) yield return new WaitForSeconds(Time.deltaTime);
            shutterLines[count].sortingOrder = 10;
            count++;
        }

        // �� ����
        board.CreateMap(stageMapInfo[stage]);

        yield return new WaitForSecondsRealtime(1f);
        stageClearPanel.SetActive(false);
        while (count > 0)
        {
            if (count % 10 == 0) yield return new WaitForSeconds(Time.deltaTime);
            count--;
            shutterLines[count].sortingOrder = -10;
        }
    }

    public void GamePause()
    {
        // �Ͻ�����
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        isGamePlaying = false;

        // Ŭ�� ���� ���
        EffectPlay(Effect.SELECT);
    }

    public void GameResume()
    {
        // �̾ ����
        isGamePlaying = true;
        Time.timeScale = 1;
        pausePanel.SetActive(false);

        // Ŭ�� ���� ���
        EffectPlay(Effect.SELECT);
    }

    public void NewGameStart()
    {
        // ���� ȭ������
        Time.timeScale = 1;
        SetIntroUI();
        board.ResetBoard();

        // �������� ����� ����
        StopCoroutine(BgmPlay(Bgm.PLAYING));
        bgmPlayer.Stop();

        // Ŭ�� ���� ���
        EffectPlay(Effect.SELECT);
    }

    public void GameOver()
    {
        // ���ӿ��� ó��
        isGamePlaying = false;
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);

        // �������� ����� ����
        StopCoroutine(BgmPlay(Bgm.PLAYING));
        bgmPlayer.Stop();

        // ���ӿ��� ���� ���
        EffectPlay(Effect.GAMEOVER);

        // �Ͻ����� ��ư ��Ȱ��ȭ
        gameUI.pauseButton.interactable = false;

        // ���� �� �ִ��޺� ����
        SetHighestScore();
        SetHighestCombo();
    }

    void SetGameUI()
    {
        introUI.SetActive(false);
        gameUI.gameObject.SetActive(true);
    }

    void SetIntroUI()
    {
        score = 0;
        line = 0;
        level = 1;
        combo = 0;
        highestCombo = 0;

        introUI.SetActive(true);
        gameUI.gameObject.SetActive(false);
        if (gameOverPanel.activeSelf) gameOverPanel.SetActive(false);
        if (pausePanel.activeSelf) pausePanel.SetActive(false);
    }

    void SetBgm(Bgm bgm)
    {
        switch (bgm)
        {
            case Bgm.OPENING:
                bgmPlayer.clip = bgmClip[0];
                break;
            case Bgm.PLAYING:
                bgmPlayer.clip = bgmClip[Random.Range(1, 6)];
                break;
        }
    }

    public IEnumerator BgmPlay(Bgm bgm)
    {
        // BGM Ÿ�� ���� �� ���
        SetBgm(bgm);
        bgmPlayer.Play();

        // �÷��� BGM�̸� 1�ʸ��� �ѹ��� üũ�Ͽ� ����� ������ �ٽ� ���� �� ���
        while (true)
        {
            // �ڷ�ƾ 1�ʴ� �ѹ��� ���ѹݺ�
            yield return new WaitForSeconds(1f);

            if (!isLoading && isGamePlaying && !bgmPlayer.isPlaying)
            {
                // ������� BGM�� ������ �ٸ� BGM���� ���� ���
                SetBgm(Bgm.PLAYING);
                bgmPlayer.Play();
            }
        }

    }

    public void EffectPlay(Effect effect)
    {
        switch (effect)
        {
            case Effect.MOVE:
                effectPlayer[effectCursor].clip = effectClip[0];
                break;
            case Effect.DROP:
                effectPlayer[effectCursor].clip = effectClip[1];
                break;
            case Effect.ROTATE:
                effectPlayer[effectCursor].clip = effectClip[2];
                break;
            case Effect.LINECLEAR:
                effectPlayer[effectCursor].clip = effectClip[3];
                break;
            case Effect.ADDBLOCK:
                effectPlayer[effectCursor].clip = effectClip[4];
                break;
            case Effect.ADDLINE:
                effectPlayer[effectCursor].clip = effectClip[5];
                break;
            case Effect.SELECT:
                effectPlayer[effectCursor].clip = effectClip[6];
                break;
            case Effect.GAMEOVER:
                effectPlayer[effectCursor].clip = effectClip[7];
                break;
            case Effect.LEVELUP:
                effectPlayer[effectCursor].clip = effectClip[8];
                break;
            case Effect.STAGECLEAR:
                effectPlayer[effectCursor].clip = effectClip[9];
                break;
        }

        effectPlayer[effectCursor].Play();
        effectCursor = (effectCursor + 1) % effectPlayer.Length;
    }

    public void PlaySelectEffect()
    {
        effectPlayer[effectCursor].clip = effectClip[4];
        effectPlayer[effectCursor].Play();
        effectCursor = (effectCursor + 1) % effectPlayer.Length;
    }
}
