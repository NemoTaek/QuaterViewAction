using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
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
    public bool isPurchased;
    public bool isSpecialItem;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
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
        // 패시브면 획득 아이템 리스트에 넣고 바로 적용하고
        // 액티브면 액티브 아이템 칸에 추가 (이미 있다면 교체)
        if (type == ItemData.ItemType.Passive)
        {
            GameManager.instance.getItemList.Add(image);
            StartCoroutine(UseItem(id));
        }
        else
        {
            GameManager.instance.ui.activeItem.SetActive(true);
            GameManager.instance.ui.activeItemImage.sprite = image;
            Image[] guage = GameManager.instance.ui.activeItemGuage.GetComponentsInChildren<Image>();
            if (activeGuage == 0) guage[1].gameObject.SetActive(false);
            else guage[1].fillAmount = (float)currentGuage / activeGuage;
            player.activeItem = this;
        }

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
                // 있던 아이템을 그대로 재활용하면 계속 아이템 먹은거 처리되는 오류가 발생하므로
                // 새로 패밀리어 오브젝트를 만들어 사용
                Familiar fam = Instantiate(GameManager.instance.familiarPool[0], player.familiar.transform);
                Familiar[] haveFamiliars = player.GetComponentsInChildren<Familiar>();
                // flipX가 true면 (오른쪽 기준)반대방향을 보고 있단 말이니까 왼쪽을 보고있다. 그러므로 패밀리어는 오른쪽으로 늘어나야 한다.
                fam.transform.localPosition = (player.spriteRenderer.flipX ? Vector3.right : Vector3.left) * haveFamiliars.Length;
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
            // 브라더 바비
            case 15:
                fam = Instantiate(GameManager.instance.familiarPool[1], player.familiar.transform);
                haveFamiliars = player.GetComponentsInChildren<Familiar>();
                fam.transform.localPosition = (player.spriteRenderer.flipX ? Vector3.right : Vector3.left) * haveFamiliars.Length;
                break;
            // 맛있는 심장
            case 16:
                player.currentHealth++;
                break;
        }

        yield return null;
    }
}
