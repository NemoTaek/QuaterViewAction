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
    public Sprite icon;

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
        icon = data.itemIcon;
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
                // ������ ���ݸ�ŭ ���� ����
                GameManager.instance.coin -= price;

                // ������ ȹ�� UI Ȱ��ȭ

                // ������ ������ ����
                StartCoroutine(UseItem(id));

                // �Ǹ� �Ϸ� ó��
                isPurchased = true;

                // �������� ������ ����
                GameManager.instance.currentRoom.isItemSet = false;
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
            // ���
            case 1:
                player.maxHealth++;
                player.health++;
                break;
            // �޷�
            case 2:
                GameManager.instance.coin += 20;
                break;
            // ����
            case 3:
                player.powerUp += 3;
                player.speed -= 0.5f;
                player.attackSpeedUp += 1;
                player.health += 1;
                player.maxHealth += 1;
                player.transform.localScale += Vector3.one * 0.25f;
                break;
            // �̴Ϲ���
            case 4:
                player.speed += 0.5f;
                player.attackSpeedUp += 1;
                player.transform.localScale -= Vector3.one * 0.25f;
                break;
            // ���׷��̵�
            case 5:
                float[] status = { player.health, player.speed, player.powerUp, player.attackSpeedUp };
                float[] delta = { 1, 0.2f, 1, 0.5f };
                for(int i = 0; i < status.Length; i++)
                {
                    int randomStat = Random.Range(0, 2);
                    if (randomStat == 0) status[i] += delta[i];
                    else status[i] -= delta[i];

                    if (i == 0) player.health = status[i];
                    else if (i == 1) player.speed = status[i];
                    else if (i == 2) player.powerUp = status[i];
                    else if (i == 3) player.attackSpeedUp = status[i];
                }
                break;
        }

        yield return null;
    }
}
