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
            // 상점에 있고, 아이템 가격보다 많은 돈을 가지고 있다면 돈 차감
            if(isInShop)
            {
                if(price < GameManager.instance.coin)
                {
                    GameManager.instance.coin -= price;
                    GetItem();
                }
            }
            // 황금방에 있으면 그냥 먹을 수 있음
            else GetItem();
        }
    }

    void Update()
    {
        
    }

    void GetItem()
    {
        // 아이템 획득 UI 활성화
        GameManager.instance.isOpenItemPanel = true;
        GameManager.instance.getItemPanel.gameObject.SetActive(true);
        GameManager.instance.getItemPanel.SetItemPanel(itemName, desc, image);

        // 구매한 아이템 적용
        StartCoroutine(UseItem(id));
        GameManager.instance.getItemList.Add(image);

        // 스탯 변화가 있다면 스탯 창 UI 갱신
        GameManager.instance.ui.isChanged = true;

        // 판매 완료 처리
        isPurchased = true;

        // 상점에서 아이템 갱신
        Map.instance.currentRoom.isItemSet = false;
        gameObject.SetActive(false);
    }

    public IEnumerator UseItem(int id)
    {
        Bullet[] bullets = GameManager.instance.bulletPool.playerBullets;
        Bullet[] existedBullet = GameManager.instance.bulletPool.GetComponentsInChildren<Bullet>(true);
        Weapon[] weapons = GameManager.instance.weapon;

        // 스탯 올라가는 것들은 공통 적용 (스탯 변동 없어도 0이니까 적용해도 똑같다)
        player.speed += itemSpeed;
        player.attackSpeedUp += itemAttackSpeed;
        player.powerUp += itemDamage;
        player.staticPower += itemStaticDamage;
        player.health += itemHealth;
        player.currentHealth += itemCurrentHealth;

        // 스탯 외의 다른 효과가 있으면 추가적으로 작성
        switch (id)
        {
            // 달러
            case 2:
                GameManager.instance.coin += 20;
                break;
            // 버섯
            case 3:
                player.transform.localScale += Vector3.one * 0.25f;
                break;
            // 미니버섯
            case 4:
                player.transform.localScale -= Vector3.one * 0.25f;
                break;
            // 스테로이드
            case 5:
                float[] status = { player.health, player.speed, player.powerUp, player.attackSpeedUp };
                float[] delta = { 1, 0.2f, 1, 0.5f };
                for(int i = 0; i < status.Length; i++)
                {
                    // 0이 나오면 스탯의 변화가 없고, 1이 나오면 스탯에 변화를 준다.
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
            // 동전주머니
            case 6:
                // 플레이어 뒤에 주머니 따라다니도록 추가
                // 일정 수의 적 저치 혹은 일정 수의 방을 클리어 했을 시 1원 드랍
                // 으악 아무 생각없이 아이템으로 만드니까 계속 아이템 먹은거 처리돼
                //Item coinSack = GameManager.instance.itemPool.items[6];
                //Instantiate(coinSack, player.familiar.transform);
                break;
            // 큐피드의 화살
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
            // 비둘기
            case 8:
                GameManager.instance.player.isFly = true;
                break;
            // 성의
            case 11:
                GameManager.instance.player.isScapular = true;
                break;
            // 물린 거미
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
