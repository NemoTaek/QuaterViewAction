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
            int weaponOrSkill = Random.Range(0, 2);

            // ���⳪ ��ų �߿��� �������� ����
            //int random = Random.Range(0, 20);
            int random = Random.Range(GameManager.instance.player.role * 5, GameManager.instance.player.role * 5 + 5);

            // �����̸�
            if (weaponOrSkill == 0)
            {
                WeaponData randomWeaponData = GameManager.instance.weaponData[random];
                roomReward.AcquireOrUpgrade(random, GameManager.instance.player.acquireWeapons, randomWeaponData);
            }

            // ��ų�̸�
            else if (weaponOrSkill == 1)
            {
                SkillData randomSkillData = GameManager.instance.skillData[random];
                roomReward.AcquireOrUpgrade(random, GameManager.instance.player.acquireSkills, randomSkillData);
            }

            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }
}
