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
    }

    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(price < GameManager.instance.coin)
            {
                // 아이템 가격만큼 코인 차감
                GameManager.instance.coin -= price;

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
        }
    }

    void Update()
    {
        
    }

    public IEnumerator UseItem(int id)
    {
        switch (id)
        {
            // 밴드
            case 1:
                player.health++;
                player.currentHealth++;
                break;
            // 달러
            case 2:
                GameManager.instance.coin += 20;
                break;
            // 버섯
            case 3:
                player.powerUp += 3;
                player.speed -= 0.5f;
                player.attackSpeedUp += 1;
                player.health += 1;
                player.currentHealth += 1;
                player.transform.localScale += Vector3.one * 0.25f;
                break;
            // 미니버섯
            case 4:
                player.speed += 0.5f;
                player.attackSpeedUp += 1;
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
        }

        yield return null;
    }
}
