using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("--------[Object Pooling]--------")]    // �ν����Ϳ��� ����ϴ� ������ ����� �и��ؼ� ����
    public List<Dongle> donglePool;
    public List<ParticleSystem> effectPool;
    [Range(1, 30)]  // ������ ����� ������ ����
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


        // Application.targetFrameRate: ������(FPS) ���� �Ӽ�
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
        // ����Ͽ��� �ڷΰ��� ��ư�� ������ ���ø����̼� ����
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
        // ���� �� �ʿ��� ������Ʈ Ȱ��ȭ
        gameStartGroup.SetActive(false);
        line.SetActive(true);
        bottom.SetActive(true);
        leftWall.SetActive(true);
        rightWall.SetActive(true);
        scoreText.gameObject.SetActive(true);
        highestScoreText.gameObject.SetActive(true);

        // ���� ����
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
        // ����Ʈ �� ����Ʈ ������Ʈ Ǯ ����
        GameObject instantEffect = Instantiate(effectPrefab, effectGroup);
        instantEffect.name = "Effect " + effectPool.Count;
        ParticleSystem effect = instantEffect.GetComponent<ParticleSystem>();
        effectPool.Add(effect);

        // ���� �� ���� ������Ʈ Ǯ ����
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
        // ���� Ǯ�� Ž���ϸ鼭 ��Ȱ��ȭ �� ������ ������ �̸� ��ȯ
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

        // ���ӿ����� �Ǹ� bgm ���߰�, ���� ����
        isGameOver = true;
        bgmPlayer.Stop();
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        // �� �ȿ� Ȱ��ȭ �Ǿ��ִ� ��� ���� ��������
        Dongle[] dongles = FindObjectsOfType<Dongle>(); // Ÿ���� ������ ������Ʈ ã��

        // ����� ���� ��� ������ ����ȿ�� ��Ȱ��ȭ
        foreach (Dongle dongle in dongles)
        {
            dongle.rigid.simulated = false;
        }

        // ���� �÷��� �߿��� ���� �� ���� ū ���� �����Ͽ� �����
        foreach (Dongle dongle in dongles)
        {
            dongle.Hide(Vector3.up * 100);
            dongle.EffectPlay();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1);

        // �ְ����� ����
        int highestScore = Mathf.Max(score, PlayerPrefs.GetInt("HighestScore"));
        PlayerPrefs.SetInt("HighestScore", highestScore);

        // ���ӿ��� UI ���
        endScoreText.text = "����: " + scoreText.text;
        gameOverGroup.SetActive(true);

        // ���ӿ��� ����
        SfxPlay(Sfx.GameOver);
    }

    public void SfxPlay(Sfx type)
    {
        switch (type)
        {
            // ������ �� ��, ȿ������ LevelUp A, B, C �� �ƹ��ų� Ʋ�ڴ�
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
