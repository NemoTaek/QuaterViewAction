using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject gameCamera;
    public Player player;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject warp;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int remainEnemyACount;
    public int remainEnemyBCount;
    public int remainEnemyCCount;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject gameoverPanel;
    public Text maxScoreText;
    public Text scoreText;
    public Text stageText;
    public Text playTimeText;
    public Text playerHealthText;
    public Text playerAmmoText;
    public Text playerCoinText;
    public Image weapon1Image;
    public Image weapon2Image;
    public Image weapon3Image;
    public Image weaponRImage;
    public Text remainEnemyACountText;
    public Text remainEnemyBCountText;
    public Text remainEnemyCCountText;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    public Text gameoverScoreText;
    public Text bestText;

    void Awake()
    {
        // PlayerPrefs: ����Ƽ���� �����ϴ� ������ ���� ���
        if(!PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        enemyList = new List<int>();
    }

    public void GameStart()
    {
        mainCamera.SetActive(false);
        gameCamera.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        gameoverPanel.SetActive(true);
        gameoverScoreText.text = scoreText.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void StageStart()
    {
        // ����, ���� ��Ȱ��ȭ �� ���� �� Ȱ��ȭ
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        warp.SetActive(false);
        foreach(Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(true);
        }

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        // ����, ���� Ȱ��ȭ �� ���� �� ��Ȱ��ȭ
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        warp.SetActive(true);
        foreach (Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(false);
        }

        isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
        if(stage % 5 == 0)
        {
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[4].position, enemyZones[4].rotation);

            // �������� ����Ƽ���� ���� �ö�� ������Ʈ�� ������ �� ���� ������ ��ũ��Ʈ ������ ����
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            boss = instantEnemy.GetComponent<Boss>();

            while (boss.health > 0)
            {
                yield return null;
            }
        }
        else
        {
            for (int i = 0; i < stage; i++)
            {
                int random = Random.Range(0, 3);
                enemyList.Add(random);

                switch(random)
                {
                    case 0:
                        remainEnemyACount++;
                        break;
                    case 1:
                        remainEnemyBCount++;
                        break;
                    case 2:
                        remainEnemyCCount++;
                        break;
                }
            }

            // �غ��� ���� 5�ʸ��� ����
            while (enemyList.Count > 0)
            {
                int randomZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[randomZone].position, enemyZones[randomZone].rotation);

                // �������� ����Ƽ���� ���� �ö�� ������Ʈ�� ������ �� ���� ������ ��ũ��Ʈ ������ ����
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.manager = this;
                enemyList.RemoveAt(0);

                yield return new WaitForSeconds(5);
            }

            while (remainEnemyACount > 0 || remainEnemyBCount > 0 || remainEnemyCCount > 0)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(3);
        boss = null;
        StageEnd();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(isBattle)
        {
            playTime += Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        // ���� ����
        scoreText.text = string.Format("{0:n0}", player.score);
        stageText.text = "STAGE " + stage;
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);
        playTimeText.text = string.Format("{0:00}", hour) + " : " + string.Format("{0:00}", min) + " : " + string.Format("{0:00}", sec);

        // �÷��̾� ����
        playerHealthText.text = player.health + " / " + player.maxHealth;
        if(player.currentEquipWeaponIndex < 1)  playerAmmoText.text = "- / " + player.ammo;
        else    playerAmmoText.text = player.weaponList[player.currentEquipWeaponIndex].currentAmmo + " / " + player.ammo;
        playerCoinText.text = string.Format("{0:n0}", player.coin);

        // �÷��̾� ���� ���� ��Ȳ
        weapon1Image.color = new Color(1, 1, 1, player.hasWeapon[0] ? 1 : 0);
        weapon2Image.color = new Color(1, 1, 1, player.hasWeapon[1] ? 1 : 0);
        weapon3Image.color = new Color(1, 1, 1, player.hasWeapon[2] ? 1 : 0);
        weaponRImage.color = new Color(1, 1, 1, player.bomb > 0 ? 1 : 0);

        // �ʿ� �ִ� �� ��
        remainEnemyACountText.text = remainEnemyACount.ToString();
        remainEnemyBCountText.text = remainEnemyBCount.ToString();
        remainEnemyCCountText.text = remainEnemyCCount.ToString();

        // ���� ü��
        if(boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            float currentBossHealth = (float)boss.health / boss.maxHealth;
            bossHealthBar.localScale = new Vector3(currentBossHealth > 0 ? currentBossHealth : 0, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
    }
}
