using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("--------[Object Pooling]--------")]    // 인스펙터에서 사용하는 변수를 헤더로 분리해서 정리
    public List<Dongle> donglePool;
    public List<ParticleSystem> effectPool;
    [Range(1, 30)]  // 변수에 사용할 범위를 지정
    public int poolSize;
    public int poolCursor;

    [Header("--------[Game Objects]--------")]
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;
    public GameObject effectPrefab;
    public Transform effectGroup;
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;
    public enum Sfx { LevelUp, Next, Attach, Button, GameOver };
    int sfxCursor;

    [Header("--------[Core]--------")]
    public int maxLevel;
    public int score;
    public bool isGameOver;

    [Header("--------[UI]--------")]
    public Text scoreText;
    public Text highestScoreText;
    public GameObject gameOverGroup;
    public Text endScoreText;
    public GameObject gameStartGroup;
    public GameObject line;
    public GameObject bottom;
    public GameObject leftWall;
    public GameObject rightWall;

    void Awake()
    {
        if(!PlayerPrefs.HasKey("HighestScore"))
        {
            PlayerPrefs.SetInt("HighestScore", 0);
        }
        highestScoreText.text = PlayerPrefs.GetInt("HighestScore").ToString();


        // Application.targetFrameRate: 프레임(FPS) 설정 속성
        Application.targetFrameRate = 60;

        donglePool = new List<Dongle>();
        effectPool = new List<ParticleSystem>();
        for(int i=0; i<poolSize; i++)
        {
            MakeDonglePool();
        }
    }

    void Start()
    {

    }

    void Update()
    {
        // 모바일에서 뒤로가기 버튼을 누르면 어플리케이션 종료
        if(Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    void LateUpdate()
    {
        scoreText.text = score.ToString();
    }

    public void GameStart()
    {
        // 시작 시 필요한 오브젝트 활성화
        gameStartGroup.SetActive(false);
        line.SetActive(true);
        bottom.SetActive(true);
        leftWall.SetActive(true);
        rightWall.SetActive(true);
        scoreText.gameObject.SetActive(true);
        highestScoreText.gameObject.SetActive(true);

        // 게임 시작
        SfxPlay(Sfx.Button);
        bgmPlayer.Play();
        NextDongle();
        //StartCoroutine(BgmPlay());
    }

    //IEnumerator BgmPlay()
    //{
    //    yield return new WaitForSeconds(1);
    //    bgmPlayer.Play();
    //    NextDongle();
    //}

    IEnumerator WaitNext()
    {
        while(lastDongle != null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2.5f);
        NextDongle();
    }

    Dongle MakeDonglePool()
    {
        // 이펙트 및 이펙트 오브젝트 풀 생성
        GameObject instantEffect = Instantiate(effectPrefab, effectGroup);
        instantEffect.name = "Effect " + effectPool.Count;
        ParticleSystem effect = instantEffect.GetComponent<ParticleSystem>();
        effectPool.Add(effect);

        // 동글 및 동글 오브젝트 풀 생성
        GameObject instantDongle = Instantiate(donglePrefab, dongleGroup);
        instantDongle.name = "Dongle " + donglePool.Count;
        Dongle dongle = instantDongle.GetComponent<Dongle>();
        dongle.manager = this;
        dongle.effect = effect;
        donglePool.Add(dongle);

        return dongle;
    }

    Dongle GetDongle()
    {
        // 동글 풀을 탐색하면서 비활성화 된 동글이 있으면 이를 반환
        for (int i = 0; i < donglePool.Count; i++)
        {
            poolCursor = (poolCursor + 1) % donglePool.Count;
            if (!donglePool[poolCursor].gameObject.activeSelf)
            {
                return donglePool[poolCursor];
            }
        }

        return MakeDonglePool();
    }

    void NextDongle()
    {
        if(!isGameOver)
        {
            lastDongle = GetDongle();
            lastDongle.level = Random.Range(0, maxLevel);
            lastDongle.gameObject.SetActive(true);

            SfxPlay(Sfx.Next);
            StartCoroutine(WaitNext());
        }
    }

    public void TouchDown()
    {
        if(lastDongle != null)
        {
            lastDongle.Drag();
        }
        
    }

    public void TouchUp()
    {
        if (lastDongle != null)
        {
            lastDongle.Drop();
            lastDongle = null;
        }
    }

    public void GameOver()
    {
        if(isGameOver)
        {
            return;
        }

        // 게임오버가 되면 bgm 멈추고, 로직 시작
        isGameOver = true;
        bgmPlayer.Stop();
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        // 씬 안에 활성화 되어있는 모든 동글 가져오기
        Dongle[] dongles = FindObjectsOfType<Dongle>(); // 타입이 동글인 오브젝트 찾기

        // 숨기기 전에 모든 동글의 물리효과 비활성화
        foreach (Dongle dongle in dongles)
        {
            dongle.rigid.simulated = false;
        }

        // 게임 플레이 중에는 나올 수 없는 큰 값을 전달하여 숨기기
        foreach (Dongle dongle in dongles)
        {
            dongle.Hide(Vector3.up * 100);
            dongle.EffectPlay();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1);

        // 최고점수 갱신
        int highestScore = Mathf.Max(score, PlayerPrefs.GetInt("HighestScore"));
        PlayerPrefs.SetInt("HighestScore", highestScore);

        // 게임오버 UI 출력
        endScoreText.text = "점수: " + scoreText.text;
        gameOverGroup.SetActive(true);

        // 게임오버 사운드
        SfxPlay(Sfx.GameOver);
    }

    public void SfxPlay(Sfx type)
    {
        switch (type)
        {
            // 레벨업 할 때, 효과음을 LevelUp A, B, C 중 아무거나 틀겠다
            case Sfx.LevelUp:
                sfxPlayer[sfxCursor].clip = sfxClip[Random.Range(0, 3)];
                break;
            case Sfx.Next:
                sfxPlayer[sfxCursor].clip = sfxClip[3];
                break;
            case Sfx.Attach:
                sfxPlayer[sfxCursor].clip = sfxClip[4];
                break;
            case Sfx.Button:
                sfxPlayer[sfxCursor].clip = sfxClip[5];
                break;
            case Sfx.GameOver:
                sfxPlayer[sfxCursor].clip = sfxClip[6];
                break;
        }

        sfxPlayer[sfxCursor].Play();
        //sfxCursor = sfxCursor == sfxPlayer.Length - 1 ? sfxCursor = 0 : sfxCursor++;
        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }

    public void Restart()
    {
        SfxPlay(Sfx.Button);
        StartCoroutine(RestartRoutine());
    }

    IEnumerator RestartRoutine()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Main");
    }
}
