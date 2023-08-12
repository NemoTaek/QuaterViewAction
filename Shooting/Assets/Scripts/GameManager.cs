using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // SceneManager ����ϱ� ����
using System.IO;    // ���� ������� ����

public class GameManager : MonoBehaviour
{
    public string[] enemyObjects;
    public Transform[] spawnEnemyPosition;
    public float nextSpawnDelay;        // ���� �ֱ�
    public float currentSpawnDelay;     // ���� �ǰ� ���� ���� �ð�

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
        
        // string.Format(����, ��): ���� ���Ŀ� ���� ���ڿ��� ��ȯ
        // {0:n0}: ���ڸ����� ��ǥ�� �����ִ� ���� ���
        gameScore.text = string.Format("{0:n0}", playerComponent.score);
    }

    void ReadTextFile()
    {
        // ���� �ʱ�ȭ
        spawnList.Clear();
        spawnIndex = 0;
        endSpawn = false;

        // ���� �б�
        TextAsset textFile = Resources.Load("Stage" + stage) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        // ������ ���� ���Ҷ����� �ݺ�
        while(stringReader != null)
        {
            string line = stringReader.ReadLine();  // �ؽ�Ʈ �����͸� ���پ� ��ȯ
            if (line == null) break;

            // ���� �����͸� ����ü�� ����
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        // ������ �о����� �ݾƾ� ��
        stringReader.Close();

        nextSpawnDelay = spawnList[0].delay;
    }

    public void StageStart()
    {
        // �÷��̾� ��ġ ����ġ
        player.transform.position = playerPosition.position;

        // �������� ���� �ؽ�Ʈ UI
        startAnimator.SetTrigger("OnStage");
        startAnimator.GetComponent<Text>().text = "Stage " + stage + "\nStart!!!";

        // �������� �ε�
        ReadTextFile();

        // ���̵� ��
        fadeAnimator.SetTrigger("FadeIn");
    }
    public void StageEnd()
    {
        // �������� Ŭ���� �ؽ�Ʈ UI
        clearAnimator.SetTrigger("OnStage");
        clearAnimator.GetComponent<Text>().text = "Stage " + stage + "\nClear!!!";

        // ���̵� �ƿ�
        fadeAnimator.SetTrigger("FadeOut");

        // �������� ���� �� 3�� �Ĵ��� �������� ���� ����
        // ������ ���������� ���ӿ��� ����
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
        // ������ Instantiate�� �����ϴ� ������Ʈ�� ObjectManager�� ������Ʈ Ǯ���� ����Ͽ� �������� ����
        GameObject enemy = objectManager.GetGameObject(enemyObjects[enemyIndex]);
        enemy.transform.position = spawnEnemyPosition[spawnPosition].position;

        // �������� �̹� ���� �ö�� ������Ʈ�� ������ �Ұ����ϴ�
        // �׷��� ���� �Ŵ������� ���� ������ �Ŀ� �̸� �Ѱ��ִ� ������� ����
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
            //enemy.transform.Rotate(Vector3.forward * 90);  // Z������ 90�� �����ڴ�
            rigid.velocity = new Vector2((enemySpeed / 2), -1);
        }
        else if (spawnPosition == 10)
        {
            //enemy.transform.Rotate(Vector3.back * 90);  // Z������ 90�� �����ڴ�
            rigid.velocity = new Vector2((enemySpeed / -2), -1);
        }

        // ���� ��ȯ ������ �ε����� �÷� ���� ���� ���� �غ�
        // ���� ������ ���̸� ����
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

    // 2�� �Ŀ� �������ǰ� 2�ʰ� ��������
    public void InvokeRespawnPlayer()
    {
        Invoke("RespawnPlayer", 2f);
        Invoke("Invincible", 4f);
    }

    public void UpdateLifeIcon(int life)
    {
        // ���� �Ⱥ��̴� ���·� ��� ������ŭ ����
        for(int i = 0; i < 3; i++)
        {
            lifeImages[i].color = new Color(1, 1, 1, 0);
        }

        // �� �� �����ִ� ��� ������ŭ ���İ� ����
        for (int i = 0; i < life; i++)
        {
            lifeImages[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        // ���� �Ⱥ��̴� ���·� �ʻ�� ������ŭ ����
        for (int i = 0; i < 5; i++)
        {
            boomImages[i].color = new Color(1, 1, 1, 0);
        }

        // �� �� �����ִ� �ʻ�� ������ŭ ���İ� ����
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
        // ������ ��ź ��ư ��Ȱ��ȭ
        boomButton.interactable = false;

        // �� ������ ����
        endSpawn = true;
        
        // ���ӿ��� UI Ȱ��ȭ
        gameOverSet.SetActive(true);
    }

    public void Retry()
    {
        // ���� �� ���ε�
        // LoadScene �ȿ� �ִ� �Ű������� File - Build Settings - Scenes In Build�� �ִ� �� ��ȣ�� �ۼ�
        SceneManager.LoadScene(0);
    }
}
