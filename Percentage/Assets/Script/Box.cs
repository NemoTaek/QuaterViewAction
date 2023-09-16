using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Box : MonoBehaviour
{
    public RoomReward roomReward;

    void Awake()
    {
        
    }

    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.isOpenBox = true;
            GameManager.instance.rewardBoxPanel.gameObject.SetActive(true);
            roomReward = GameManager.instance.rewardBoxPanel.GetComponent<RoomReward>();

            // ���Ⱝȭ, ��ų��ȭ �� ���� ����
            int weaponOrSkill = Random.Range(1, 2);

            // �����̸�
            if (weaponOrSkill == 0)
            {
                roomReward.AcquireOrUpgrade(GameManager.instance.player.acquireWeapons, "����");
            }

            // �����̸�
            else if (weaponOrSkill == 1)
            {
                roomReward.AcquireOrUpgrade(GameManager.instance.player.acquireSkills, "��ų");
            }

            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }
}
