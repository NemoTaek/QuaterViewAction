using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int id;
    public string itemName;
    public int price;
    public string desc;
    public float coolTime;
    public float duringTime;
    public Sprite image;

    float itemSpeed;
    float itemAttackSpeed;
    float itemDamage;
    float itemStaticDamage;
    float itemHealth;
    float itemCurrentHealth;

    public bool isInShop;
    public bool isPurchased;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    public void Init(ItemData data)
    {
        id = data.itemId;
        itemName = data.itemName;
        price = data.itemPrice;
        desc = data.itemDesc;
        coolTime = data.itemCoolTime;
        duringTime = data.itemDuringTime;
        image = data.itemImage;

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
            if(isInShop)
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
        StartCoroutine(UseItem(id));
        GameManager.instance.getItemList.Add(image);

        // ���� ��ȭ�� �ִٸ� ���� â UI ����
        GameManager.instance.ui.isChanged = true;

        // �Ǹ� �Ϸ� ó��
        isPurchased = true;

        // �������� ������ ����
        Map.instance.currentRoom.isItemSet = false;
        gameObject.SetActive(false);
    }

    public IEnumerator UseItem(int id)
    {
        Bullet[] bullets = GameManager.instance.bulletPool.playerBullets;
        Bullet[] existedBullet = GameManager.instance.bulletPool.GetComponentsInChildren<Bullet>(true);
        Weapon[] weapons = GameManager.instance.weapon;

        // ���� �ö󰡴� �͵��� ���� ���� (���� ���� ��� 0�̴ϱ� �����ص� �Ȱ���)
        player.speed += itemSpeed;
        player.attackSpeedUp += itemAttackSpeed;
        player.powerUp += itemDamage;
        player.staticPower += itemStaticDamage;
        player.health += itemHealth;
        player.currentHealth += itemCurrentHealth;

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
                // ���� �ƹ� �������� ���������� ����ϱ� ��� ������ ������ ó����
                //Item coinSack = GameManager.instance.itemPool.items[6];
                //Instantiate(coinSack, player.familiar.transform);
                break;
            // ť�ǵ��� ȭ��
            case 7:
                for(int i=0; i<bullets.Length; i++)
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
        }

        yield return null;
    }
}
