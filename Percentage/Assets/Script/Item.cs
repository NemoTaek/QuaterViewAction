using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    SpriteRenderer itemSpriteRenderer;

    public int id;
    public ItemData.ItemType type;
    public string itemName;
    public int price;
    public string desc;
    public float coolTime;
    public float duringTime;
    public Sprite image;
    public int activeGuage;
    public int currentGuage;

    float itemSpeed;
    float itemAttackSpeed;
    float itemDamage;
    float itemStaticDamage;
    float itemHealth;
    float itemCurrentHealth;

    public bool isInShop;
    public bool getItem;
    public bool isSpecialItem;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
        itemSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(ItemData data)
    {
        id = data.itemId;
        type = data.itemType;
        itemName = data.itemName;
        price = data.itemPrice;
        desc = data.itemDesc;
        coolTime = data.itemCoolTime;
        duringTime = data.itemDuringTime;
        image = data.itemImage;
        activeGuage = data.itemActiveGuage;
        currentGuage = data.itemActiveGuage;

        itemSpeed = data.itemSpeed;
        itemAttackSpeed = data.itemAttackSpeed;
        itemDamage = data.itemDamage;
        itemStaticDamage = data.itemStaticDamage;
        itemHealth = data.itemHealth;
        itemCurrentHealth = data.itemCurrentHealth;
    }

    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            // ������ �ְ�, ������ ���ݺ��� ���� ���� ������ �ִٸ� �� ����
            if(isInShop && !getItem)
            {
                if(price < GameManager.instance.coin)
                {
                    GameManager.instance.coin -= price;
                    GetItem();
                }
            }
            // Ȳ�ݹ濡 ������ �׳� ���� �� ����
            else GetItem();
        }
    }

    void Update()
    {
        
    }

    void GetItem()
    {
        // ������ ȹ�� UI Ȱ��ȭ
        GameManager.instance.isOpenItemPanel = true;
        GameManager.instance.getItemPanel.gameObject.SetActive(true);
        GameManager.instance.getItemPanel.SetItemPanel(itemName, desc, image);

        // ������ ������ ����
        // �нú�� ȹ�� ������ ����Ʈ�� �ְ� �ٷ� �����ϰ� ������ ����
        // ��Ƽ��� ��Ƽ�� ������ ĭ�� �߰� (�̹� �ִٸ� ��ü)
        if (type == ItemData.ItemType.Passive)
        {
            // ȹ���� ������ ����Ʈ�� �߰� �� �Ⱥ��̵��� �����ϰ� �÷��̾ �ͼ�
            GameManager.instance.getItemList.Add(this);
            
            // SetActive(false) �ϸ� Ȱ��ȭ �Ǿ����� �ʴ� ������Ʈ�� �ڷ�ƾ�� ������ �� ���� ������ ũ�⸦ 0���� �����Ͽ� �Ⱥ��̴°� ó�� ����
            gameObject.transform.localScale = Vector3.zero;

            // �̴�� �θ� ���������� �ٲ�� ������� ������ �ͼ��ϵ��� �÷��̾ �ִ´�.
            // 2��° �Ű������� true�� ������ ������ ��ġ, ����, ũ�⸦ �����ϵ��� ��������� �����ȴ�.
            gameObject.transform.SetParent(GameManager.instance.player.getItems.transform, false);

            // �׸��� ���
            StartCoroutine(UseItem(id));
        }
        else
        {
            // �ϴ� 2���� ����� ���� �ִ�. ���� �������ִ� ��Ƽ�� �������� �ִ°� ���°�
            // ������ �׳� ȹ�� ������ Ÿ��, �ִٸ� ��ü�ؾ� �Ѵ�.
            if (player.activeItem)
            {
                // ���� ������ �ִ� ��Ƽ�� �������� �ӽ÷� ����
                int tempItemId = player.activeItem.id;

                // ������ ȹ��
                SetActiveItem();

                // ���� �� �濡 �ִ� �������� �ӽ� ������ ���������� ��ü
                // �÷��̾ ���� ������ id�� �ε����� �ش��ϴ� ������ �����ͷ� ���� ���� �������� �ٽ� ����
                // �ε����� ������ �����ʹϱ� +1 �Ǿ� ���� ������ �߻��ߴ� ��.
                // �̹����� �ٲ�����
                Init(GameManager.instance.itemData[tempItemId - 1]);
                itemSpriteRenderer.sprite = image;
            }
            else
            {
                // UI�� ��Ƽ�� ĭ�� Ȱ��ȭ
                GameManager.instance.ui.activeItem.SetActive(true);

                // ������ ȹ��
                SetActiveItem();

                // SetActive(false) �ϸ� Ȱ��ȭ �Ǿ����� �ʴ� ������Ʈ�� �ڷ�ƾ�� ������ �� ���� ������ ũ�⸦ 0���� �����Ͽ� �Ⱥ��̴°� ó�� ����
                gameObject.transform.localScale = Vector3.zero;
                // �̴�� �θ� ���������� �ٲ�� ������� ������ �ͼ��ϵ��� �÷��̾ �ִ´�.
                // 2��° �Ű������� true�� ������ ������ ��ġ, ����, ũ�⸦ �����ϵ��� ��������� �����ȴ�.
                gameObject.transform.SetParent(GameManager.instance.player.getItems.transform, false);
            }
        }

        // ���� ��ȭ�� �ִٸ� ���� â UI ����
        GameManager.instance.ui.isChanged = true;

        // ȹ�� �Ϸ� ó��
        getItem = true;
    }

    void SetActiveItem()
    {
        // �̹����� �������� ����
        GameManager.instance.ui.activeItemImage.sprite = image;

        // �÷��̾ �ͼӵǴ� ������ �����͸� ����
        if (player.activeItem)
        {
            player.activeItem.Init(GameManager.instance.itemData[id - 1]);
        }
        else
        {
            player.activeItem = this;
        }

        // ��Ƽ�� ������ UI ����
        GameManager.instance.ui.isChanged = true;
    }

    public IEnumerator UseItem(int id)
    {
        Bullet[] bullets;
        Bullet[] existedBullet;
        Weapon[] weapons;
        Enemy[] enemies;
        Vector3 pos;
        GameObject reward;

        // ���� �ö󰡴� �͵��� ���� ���� (���� ���� ��� 0�̴ϱ� �����ص� �Ȱ���)
        player.speed += itemSpeed;
        player.attackSpeedUp += itemAttackSpeed;
        player.powerUp += itemDamage;
        player.staticPower += itemStaticDamage;
        player.health += itemHealth;
        player.currentHealth = Mathf.Min(player.currentHealth + itemCurrentHealth, player.health);

        // ���� ���� �ٸ� ȿ���� ������ �߰������� �ۼ�
        switch (id)
        {
            // �޷�
            case 2:
                GameManager.instance.coin += 20;
                break;
            // ����
            case 3:
                player.transform.localScale += Vector3.one * 0.25f;
                break;
            // �̴Ϲ���
            case 4:
                player.transform.localScale -= Vector3.one * 0.25f;
                break;
            // ���׷��̵�
            case 5:
                float[] status = { player.health, player.speed, player.powerUp, player.attackSpeedUp };
                float[] delta = { 1, 0.2f, 1, 0.5f };
                for(int i = 0; i < status.Length; i++)
                {
                    // 0�� ������ ������ ��ȭ�� ����, 1�� ������ ���ȿ� ��ȭ�� �ش�.
                    int choice = Random.Range(0, 2);
                    if(choice == 1)
                    {
                        int randomStat = Random.Range(0, 2);
                        if (randomStat == 0) status[i] += delta[i];
                        else status[i] -= delta[i];

                        if (i == 0) player.health = status[i];
                        else if (i == 1) player.speed = status[i];
                        else if (i == 2) player.powerUp = status[i];
                        else if (i == 3) player.attackSpeedUp = status[i];
                    }
                }
                break;
            // �����ָӴ�
            case 6:
                // �÷��̾� �ڿ� �ָӴ� ����ٴϵ��� �߰�
                // ���� ���� �� ��ġ Ȥ�� ���� ���� ���� Ŭ���� ���� �� 1�� ���
                GenerateFamiliar(0);
                break;
            // ť�ǵ��� ȭ��
            case 7:
                bullets = GameManager.instance.bulletPool.playerBullets;
                existedBullet = GameManager.instance.bulletPool.GetComponentsInChildren<Bullet>(true);
                weapons = GameManager.instance.weapon;

                for (int i=0; i<bullets.Length; i++)
                {
                    bullets[i].isPenetrate = true;
                }
                foreach(Bullet bullet in existedBullet)
                {
                    if(bullet.type == 0)    bullet.isPenetrate = true;
                }
                for (int i = 0; i < weapons.Length; i++)
                {
                    if(weapons[i])  weapons[i].isPenetrate = true;
                }
                break;
            // ��ѱ�
            case 8:
                GameManager.instance.player.isFly = true;
                break;
            // ����
            case 11:
                GameManager.instance.player.isScapular = true;
                break;
            // ���� �Ź�
            case 12:
                bullets = GameManager.instance.bulletPool.playerBullets;
                existedBullet = GameManager.instance.bulletPool.GetComponentsInChildren<Bullet>(true);
                weapons = GameManager.instance.weapon;

                for (int i = 0; i < bullets.Length; i++)
                {
                    bullets[i].isSlow = true;
                }
                foreach (Bullet bullet in existedBullet)
                {
                    if (bullet.type == 0) bullet.isSlow = true;
                }
                for (int i = 0; i < weapons.Length; i++)
                {
                    if (weapons[i]) weapons[i].isSlow = true;
                }
                break;
            // ���� �ٺ�
            case 15:
                GenerateFamiliar(1);
                break;
            // ���ִ� ����
            case 16:
                player.currentHealth = Mathf.Min(player.currentHealth + 1, player.health);
                break;
            // �𷡽ð�
            case 17:
                // �ش� ���� �� 8�ʰ� ��ȭ
                enemies = Map.instance.currentRoom.GetComponentsInChildren<Enemy>();
                foreach (Enemy enemy in enemies)
                {
                    StartCoroutine(enemy.MoveSlow(8f));
                }
                break;
            // �������� ��
            case 18:
                // 6�ʰ� �̵��ӵ� 0.28�� �����ϰ� ������ �ȴ�.
                player.speed += 0.28f;
                StartCoroutine(player.PlayerInvincibility(6f));
                player.speed -= 0.28f;
                break;
            // ������
            case 19:
                // ü�� 0.5�� �Ҹ��Ͽ� ���� ���
                player.currentHealth -= 0.5f;

                pos = GameManager.instance.CheckAround(player.transform.position);
                reward = Instantiate(GameManager.instance.objectPool.prefabs[1], Map.instance.currentRoom.roomReward.transform);
                reward.transform.position = player.transform.position + pos;
                break;
            // ���� �Ǹ�
            case 20:
                // ü�� 1�� �Ҹ��Ͽ� �ش� ���� ������ 40�� ������
                player.currentHealth--;
                enemies = Map.instance.currentRoom.GetComponentsInChildren<Enemy>();
                foreach (Enemy enemy in enemies)
                {
                    enemy.EnemyDamaged(40);
                }
                break;
            // ���� ����
            case 21:
                // �ݹ��� Ȯ���� ���� ���
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    pos = GameManager.instance.CheckAround(player.transform.position);
                    reward = Instantiate(GameManager.instance.objectPool.prefabs[1], Map.instance.currentRoom.roomReward.transform);
                    reward.transform.position = player.transform.position + pos;
                }
                break;
            // �ı� ����
            case 22:
                // 22�� 23�̶� ���� �ϳ��� ���� �� �ֵ��� �ؾ��ϴµ�...
                yield return new WaitForSeconds(0.1f);
                Map.instance.currentRoom.DeleteItem();
                break;
            // ��ȭ
            case 23:
                // ���� �� 0.5�� ������ �ΰ� ��ȭ
                yield return new WaitForSeconds(0.5f);

                GameManager.instance.isOpenBox = true;
                GameManager.instance.rewardBoxPanel.gameObject.SetActive(true);
                RoomReward roomReward = GameManager.instance.rewardBoxPanel.GetComponent<RoomReward>();

                // ���� ���ϰ� �ִ� ����� ������ ��ų�� ��ȭ (�ı��ŵ� �� ��)
                int currentWeaponId = GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].id;
                roomReward.AcquireOrUpgrade(currentWeaponId, GameManager.instance.weaponData[currentWeaponId]);

                yield return new WaitForSeconds(0.5f);

                GameManager.instance.isOpenBox = true;
                GameManager.instance.rewardBoxPanel.gameObject.SetActive(true);
                roomReward = GameManager.instance.rewardBoxPanel.GetComponent<RoomReward>();

                int currentSkillId = GameManager.instance.skill[GameManager.instance.player.currentSkillIndex].id;
                roomReward.AcquireOrUpgrade(currentSkillId, GameManager.instance.skillData[currentSkillId]);

                yield return new WaitForSeconds(0.1f);
                Map.instance.currentRoom.DeleteItem();
                break;
        }

        yield return null;
    }

    void GenerateFamiliar(int familiarIndex)
    {
        // �ִ� �������� �״�� ��Ȱ���ϸ� ��� ������ ������ ó���Ǵ� ������ �߻��ϹǷ�
        // ���� �йи��� ������Ʈ�� ����� ���
        Familiar fam = Instantiate(GameManager.instance.familiarPool[familiarIndex], player.familiar.transform);
        Familiar[] haveFamiliars = player.GetComponentsInChildren<Familiar>();

        // flipX�� true�� (������ ����)�ݴ������ ���� �ִ� ���̴ϱ� ������ �����ִ�. �׷��Ƿ� �йи���� ���������� �þ�� �Ѵ�.
        fam.transform.localPosition = (player.spriteRenderer.flipX ? Vector3.right : Vector3.left) * haveFamiliars.Length;
    }
}
