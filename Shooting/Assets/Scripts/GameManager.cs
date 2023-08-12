using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // SceneManager 사용하기 위함
using System.IO;    // 파일 입출력을 위함

public class GameManager : MonoBehaviour
{
    public string[] enemyObjects;
    public Transform[] spawnEnemyPosition;
    public float nextSpawnDelay;        // 스폰 주기
    public float currentSpawnDelay;     // 스폰 되고 나서 지난 시간

    public GameObject player;
    Player playerComponent;
    public Transform playerPosition;

    public Text gameScore;
    public Image[] lifeImages;
    public Image[] boomImages;
    public GameObject gameOverSet;
    public Button boomButton;

    public ObjectManager objectManager;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool endSpawn;
    public int stage;
    public Animator startAnimator;
    public Animator clearAnimator;
    public Animator fadeAnimator;

    void Awake()
    {
        spawnList = new List<Spawn>();
        playerComponent = player.GetComponent<Player>();
        enemyObjects = new string[] { "enemySmall", "enemyMedium", "enemyLarge", "boss" };
        StageStart();
    }

    void Update()
    {
        currentSpawnDelay += Time.deltaTime;
        if(currentSpawnDelay > nextSpawnDelay && !endSpawn)
        {
            SpawnEnemy();
            currentSpawnDelay = 0;
        }
        
        // string.Format(형식, 값): 값을 형식에 맞춰 문자열로 반환
        // {0:n0}: 세자리마다 쉼표로 나눠주는 숫자 양식
        gameScore.text = string.Format("{0:n0}", playerComponent.score);
    }

    void ReadTextFile()
    {
        // 변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        endSpawn = false;

        // 파일 읽기
        TextAsset textFile = Resources.Load("Stage" + stage) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        // 파일을 읽지 못할때까지 반복
        while(stringReader != null)
        {
            string line = stringReader.ReadLine();  // 텍스트 데이터를 한줄씩 반환
            if (line == null) break;

            // 읽은 데이터를 구조체에 저장
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        // 파일을 읽었으면 닫아야 함
        stringReader.Close();

        nextSpawnDelay = spawnList[0].delay;
    }

    public void StageStart()
    {
        // 플레이어 위치 원위치
        player.transform.position = playerPosition.position;

        // 스테이지 시작 텍스트 UI
        startAnimator.SetTrigger("OnStage");
        startAnimator.GetComponent<Text>().text = "Stage " + stage + "\nStart!!!";

        // 스테이지 로드
        ReadTextFile();

        // 페이드 인
        fadeAnimator.SetTrigger("FadeIn");
    }
    public void StageEnd()
    {
        // 스테이지 클리어 텍스트 UI
        clearAnimator.SetTrigger("OnStage");
        clearAnimator.GetComponent<Text>().text = "Stage " + stage + "\nClear!!!";

        // 페이드 아웃
        fadeAnimator.SetTrigger("FadeOut");

        // 스테이지 증가 후 3초 후다음 스테이지 시작 로직
        // 마지막 스테이지면 게임오버 실행
        stage++;
        if(stage > 2)
        {
            GameOver();
        }
        else
        {
            Invoke("StageStart", 3);
        }
    }

    void SpawnEnemy()
    {
        int enemySpeed = Random.Range(2, 6);
        int spawnPosition = spawnList[spawnIndex].point;

        int enemyIndex = 0;
        switch(spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                enemySpeed = 2;
                break;
        }

        //GameObject enemy = Instantiate(enemyObjects[enemyType], spawnEnemyPosition[spawnPosition].position, spawnEnemyPosition[spawnPosition].rotation);
        // 기존에 Instantiate로 생성하던 오브젝트를 ObjectManager의 오브젝트 풀링을 사용하여 만들어내도록 수정
        GameObject enemy = objectManager.GetGameObject(enemyObjects[enemyIndex]);
        enemy.transform.position = spawnEnemyPosition[spawnPosition].position;

        // 프리팹은 이미 씬에 올라온 오브젝트에 접근이 불가능하다
        // 그래서 게임 매니저에서 적을 생성한 후에 이를 넘겨주는 방법으로 구현
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        enemyComponent.player = player;
        enemyComponent.gameManager = this;
        enemyComponent.objectManager = objectManager;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        if(spawnPosition < 9)
        {
            rigid.velocity = Vector2.down * enemySpeed;
        }
        else if(spawnPosition == 9)
        {
            //enemy.transform.Rotate(Vector3.forward * 90);  // Z축으로 90도 돌리겠다
            rigid.velocity = new Vector2((enemySpeed / 2), -1);
        }
        else if (spawnPosition == 10)
        {
            //enemy.transform.Rotate(Vector3.back * 90);  // Z축으로 90도 돌리겠다
            rigid.velocity = new Vector2((enemySpeed / -2), -1);
        }

        // 적을 소환 했으면 인덱스를 올려 다음 적을 만들 준비
        // 만약 마지막 적이면 종료
        spawnIndex++;
        if (spawnIndex == spawnList.Count)
        {
            endSpawn = true;
            return;
        }
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    void RespawnPlayer()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);
        playerComponent.spriteRenderer.color = new Color(1, 1, 1, 0.5f);
    }

    void Invincible()
    {
        playerComponent.spriteRenderer.color = new Color(1, 1, 1, 1);
        playerComponent.isDamaged = false;
    }

    // 2초 후에 리스폰되고 2초간 무적상태
    public void InvokeRespawnPlayer()
    {
        Invoke("RespawnPlayer", 2f);
        Invoke("Invincible", 4f);
    }

    public void UpdateLifeIcon(int life)
    {
        // 먼저 안보이는 형태로 목숨 개수만큼 세팅
        for(int i = 0; i < 3; i++)
        {
            lifeImages[i].color = new Color(1, 1, 1, 0);
        }

        // 그 후 남아있는 목숨 개수만큼 알파값 조절
        for (int i = 0; i < life; i++)
        {
            lifeImages[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        // 먼저 안보이는 형태로 필살기 개수만큼 세팅
        for (int i = 0; i < 5; i++)
        {
            boomImages[i].color = new Color(1, 1, 1, 0);
        }

        // 그 후 남아있는 필살기 개수만큼 알파값 조절
        for (int i = 0; i < boom; i++)
        {
            boomImages[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void CallExplosion(Vector3 position, string type)
    {
        GameObject explosion = objectManager.GetGameObject("explosion");
        Explosion explosionComponent = explosion.GetComponent<Explosion>();

        explosion.transform.position = position;
        explosionComponent.StartExplosion(type);
    }

    public void GameOver()
    {
        // 죽으면 폭탄 버튼 비활성화
        boomButton.interactable = false;

        // 적 리스폰 멈춤
        endSpawn = true;
        
        // 게임오버 UI 활성화
        gameOverSet.SetActive(true);
    }

    public void Retry()
    {
        // 현재 씬 리로드
        // LoadScene 안에 있는 매개변수는 File - Build Settings - Scenes In Build에 있는 씬 번호를 작성
        SceneManager.LoadScene(0);
    }
}
