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

    public float fallingInterval;   // 한번 낙하하는 주기
    public float landingInterval;   // 바닥에 닿았을 때 착지할 때까지의 주기

    public enum ModeList { Normal, Hard, Stage };
    public int mode;
    public int blockCount;  // 현재 사용한 블럭 개수(맵에 기믹으로 블럭이나 라인 추가할 때 사용)

    // 스테이지 모드 전용 변수
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
        // 게임 기록 로드
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

        // 맵 정보 로드
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

        // 오프닝 재생
        StartCoroutine(BgmPlay(Bgm.OPENING));
    }

    void Update()
    {
        // 콤보 설정
        comboText.text = combo + " Combo!!";
        if (combo > 2) comboText.gameObject.transform.localScale = Vector3.one;
        else comboText.gameObject.transform.localScale = Vector3.zero;

        // 블럭 낙하 속도 설정
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
        // 맵 선택 UI
        selectMapUI.transform.localScale = Vector3.one;

        // 클릭 사운드 출력
        EffectPlay(Effect.SELECT);

        // 현재 선택된 맵 세팅
        map.mapTitle.text = map.mapTitleArray[map.mapIndex];
        board.CreateMap(mapInfo[map.mapIndex]);
    }

    public void SelectMap()
    {
        // 맵 선택 UI
        selectMapUI.transform.localScale = Vector3.zero;

        // 클릭 사운드 출력
        EffectPlay(Effect.SELECT);
    }

    public void SetNoramlMode()
    {
        // 노말모드 세팅
        mode = (int)ModeList.Normal;
        GameStart();
    }

    public void SetHardMode()
    {
        // 하드모드 세팅
        mode = (int)ModeList.Hard;
        GameStart();
    }

    public void SetStageMode()
    {
        // 스테이지모드 세팅
        mode = (int)ModeList.Stage;
        remainLines = stageStandard[stage - 1];

        // 다른 맵이 세팅되어있다면 기본맵으로 다시 세팅
        board.CreateMap(map.MapEmpty());
        GameStart();
    }

    public void GameStart()
    {
        // 게임 스타트
        isGamePlaying = true;
        StartCoroutine(BgmPlay(Bgm.PLAYING));
        SetGameUI();

        // 클릭 사운드 출력
        EffectPlay(Effect.SELECT);

        // 게임 시작 후에 첫 테트로미노 세팅
        board.InitTetromino();
    }

    public IEnumerator SetNextStage()
    {
        // bgm 종료
        bgmPlayer.Stop();
        isLoading = true;
        isGamePlaying = false;
        board.tetromino.gameObject.SetActive(false);
        stageClearPanel.SetActive(true);

        // 스테이지 클리어 효과음 출력 (약 5초)
        yield return new WaitForSeconds(0.5f);
        EffectPlay(Effect.STAGECLEAR);

        // 효과음 출력 완료 후 다음 스테이지 세팅
        yield return new WaitForSeconds(5f);
        StartCoroutine(SetLoading());
        stage++;
        remainLines = stageStandard[stage - 1];
        blockCount = 0;

        // 맵 보여주고 1초 후 시작
        yield return new WaitForSeconds(Time.deltaTime * 40);
        yield return new WaitForSecondsRealtime(3f);
        board.InitTetromino();
        isGamePlaying = true;
        isLoading = false;
        board.tetromino.gameObject.SetActive(true);

        // bgm 출력
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

        // 맵 세팅
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
        // 일시정지
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        isGamePlaying = false;

        // 클릭 사운드 출력
        EffectPlay(Effect.SELECT);
    }

    public void GameResume()
    {
        // 이어서 진행
        isGamePlaying = true;
        Time.timeScale = 1;
        pausePanel.SetActive(false);

        // 클릭 사운드 출력
        EffectPlay(Effect.SELECT);
    }

    public void NewGameStart()
    {
        // 메인 화면으로
        Time.timeScale = 1;
        SetIntroUI();
        board.ResetBoard();

        // 진행중인 배경음 종료
        StopCoroutine(BgmPlay(Bgm.PLAYING));
        bgmPlayer.Stop();

        // 클릭 사운드 출력
        EffectPlay(Effect.SELECT);
    }

    public void GameOver()
    {
        // 게임오버 처리
        isGamePlaying = false;
        Time.timeScale = 0;
        gameOverPanel.SetActive(true);

        // 진행중인 배경음 종료
        StopCoroutine(BgmPlay(Bgm.PLAYING));
        bgmPlayer.Stop();

        // 게임오버 사운드 출력
        EffectPlay(Effect.GAMEOVER);

        // 일시정지 버튼 비활성화
        gameUI.pauseButton.interactable = false;

        // 점수 및 최대콤보 저장
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
        // BGM 타입 설정 후 재생
        SetBgm(bgm);
        bgmPlayer.Play();

        // 플레이 BGM이면 1초마다 한번씩 체크하여 재생이 끝나면 다시 세팅 후 재생
        while (true)
        {
            // 코루틴 1초당 한번씩 무한반복
            yield return new WaitForSeconds(1f);

            if (!isLoading && isGamePlaying && !bgmPlayer.isPlaying)
            {
                // 재생중인 BGM이 끝나면 다른 BGM으로 랜덤 재생
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
