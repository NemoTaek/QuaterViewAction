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
            // 상점에 있고, 아이템 가격보다 많은 돈을 가지고 있다면 돈 차감
            if(isInShop && !getItem)
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
        // 패시브면 획득 아이템 리스트에 넣고 바로 적용하고 아이템 삭제
        // 액티브면 액티브 아이템 칸에 추가 (이미 있다면 교체)
        if (type == ItemData.ItemType.Passive)
        {
            // 획득한 아이템 리스트에 추가 후 안보이도록 설정하고 플레이어에 귀속
            GameManager.instance.getItemList.Add(this);
            
            // SetActive(false) 하면 활성화 되어있지 않는 오브젝트는 코루틴을 실행할 수 없기 때문에 크기를 0으로 설정하여 안보이는것 처럼 설정
            gameObject.transform.localScale = Vector3.zero;

            // 이대로 두면 스테이지가 바뀌면 사라지기 때문에 귀속하도록 플레이어에 넣는다.
            // 2번째 매개변수가 true면 이전과 동일한 위치, 각도, 크기를 유지하도록 상대적으로 조절된다.
            gameObject.transform.SetParent(GameManager.instance.player.getItems.transform, false);

            // 그리고 사용
            StartCoroutine(UseItem(id));
        }
        else
        {
            // 일단 2가지 경우의 수가 있다. 지금 가지고있는 액티브 아이템이 있는가 없는가
            // 없으면 그냥 획득 로직을 타고, 있다면 교체해야 한다.
            if (player.activeItem)
            {
                // 먼저 가지고 있는 액티브 아이템을 임시로 저장
                int tempItemId = player.activeItem.id;

                // 아이템 획득
                SetActiveItem();

                // 지금 이 방에 있는 아이템을 임시 저장한 아이템으로 교체
                // 플레이어가 가진 아이템 id의 인덱스에 해당하는 아이템 데이터로 현재 방의 아이템을 다시 세팅
                // 인덱스의 아이템 데이터니까 +1 되어 들어가서 오류가 발생했던 것.
                // 이미지도 바꿔주자
                Init(GameManager.instance.itemData[tempItemId - 1]);
                itemSpriteRenderer.sprite = image;
            }
            else
            {
                // UI의 액티브 칸을 활성화
                GameManager.instance.ui.activeItem.SetActive(true);

                // 아이템 획득
                SetActiveItem();

                // SetActive(false) 하면 활성화 되어있지 않는 오브젝트는 코루틴을 실행할 수 없기 때문에 크기를 0으로 설정하여 안보이는것 처럼 설정
                gameObject.transform.localScale = Vector3.zero;
                // 이대로 두면 스테이지가 바뀌면 사라지기 때문에 귀속하도록 플레이어에 넣는다.
                // 2번째 매개변수가 true면 이전과 동일한 위치, 각도, 크기를 유지하도록 상대적으로 조절된다.
                gameObject.transform.SetParent(GameManager.instance.player.getItems.transform, false);
            }
        }

        // 스탯 변화가 있다면 스탯 창 UI 갱신
        GameManager.instance.ui.isChanged = true;

        // 획득 완료 처리
        getItem = true;
    }

    void SetActiveItem()
    {
        // 이미지와 게이지를 세팅
        GameManager.instance.ui.activeItemImage.sprite = image;

        // 플레이어에 귀속되는 아이템 데이터를 저장
        if (player.activeItem)
        {
            player.activeItem.Init(GameManager.instance.itemData[id - 1]);
        }
        else
        {
            player.activeItem = this;
        }

        // 액티브 아이템 UI 갱신
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

        // 스탯 올라가는 것들은 공통 적용 (스탯 변동 없어도 0이니까 적용해도 똑같다)
        player.speed += itemSpeed;
        player.attackSpeedUp += itemAttackSpeed;
        player.powerUp += itemDamage;
        player.staticPower += itemStaticDamage;
        player.health += itemHealth;
        player.currentHealth = Mathf.Min(player.currentHealth + itemCurrentHealth, player.health);

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
                GenerateFamiliar(0);
                break;
            // 큐피드의 화살
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
            // 브라더 바비
            case 15:
                GenerateFamiliar(1);
                break;
            // 맛있는 심장
            case 16:
                player.currentHealth = Mathf.Min(player.currentHealth + 1, player.health);
                break;
            // 모래시계
            case 17:
                // 해당 방의 적 8초간 둔화
                enemies = Map.instance.currentRoom.GetComponentsInChildren<Enemy>();
                foreach (Enemy enemy in enemies)
                {
                    StartCoroutine(enemy.MoveSlow(8f));
                }
                break;
            // 유니콘의 뿔
            case 18:
                // 6초간 이동속도 0.28이 증가하고 무적이 된다.
                player.speed += 0.28f;
                StartCoroutine(player.PlayerInvincibility(6f));
                player.speed -= 0.28f;
                break;
            // 헌혈백
            case 19:
                // 체력 0.5를 소모하여 동전 드랍
                player.currentHealth -= 0.5f;

                pos = GameManager.instance.CheckAround(player.transform.position);
                reward = Instantiate(GameManager.instance.objectPool.prefabs[1], Map.instance.currentRoom.roomReward.transform);
                reward.transform.position = player.transform.position + pos;
                break;
            // 피의 권리
            case 20:
                // 체력 1을 소모하여 해당 방의 적에게 40의 데미지
                player.currentHealth--;
                enemies = Map.instance.currentRoom.GetComponentsInChildren<Enemy>();
                foreach (Enemy enemy in enemies)
                {
                    enemy.EnemyDamaged(40);
                }
                break;
            // 나무 동전
            case 21:
                // 반반의 확률로 동전 드랍
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    pos = GameManager.instance.CheckAround(player.transform.position);
                    reward = Instantiate(GameManager.instance.objectPool.prefabs[1], Map.instance.currentRoom.roomReward.transform);
                    reward.transform.position = player.transform.position + pos;
                }
                break;
            // 파괴 방지
            case 22:
                // 22랑 23이랑 둘중 하나만 먹을 수 있도록 해야하는데...
                yield return new WaitForSeconds(0.1f);
                Map.instance.currentRoom.DeleteItem();
                break;
            // 강화
            case 23:
                // 습득 후 0.5초 간격을 두고 강화
                yield return new WaitForSeconds(0.5f);

                GameManager.instance.isOpenBox = true;
                GameManager.instance.rewardBoxPanel.gameObject.SetActive(true);
                RoomReward roomReward = GameManager.instance.rewardBoxPanel.GetComponent<RoomReward>();

                // 현재 지니고 있는 무기와 선택한 스킬을 강화 (파괴돼도 난 모름)
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
        // 있던 아이템을 그대로 재활용하면 계속 아이템 먹은거 처리되는 오류가 발생하므로
        // 새로 패밀리어 오브젝트를 만들어 사용
        Familiar fam = Instantiate(GameManager.instance.familiarPool[familiarIndex], player.familiar.transform);
        Familiar[] haveFamiliars = player.GetComponentsInChildren<Familiar>();

        // flipX가 true면 (오른쪽 기준)반대방향을 보고 있단 말이니까 왼쪽을 보고있다. 그러므로 패밀리어는 오른쪽으로 늘어나야 한다.
        fam.transform.localPosition = (player.spriteRenderer.flipX ? Vector3.right : Vector3.left) * haveFamiliars.Length;
    }
}
