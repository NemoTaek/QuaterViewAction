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
            BoxOpen();
        }
    }

    void Update()
    {
        
    }

    public void BoxOpen()
    {
        GameManager.instance.isOpenBox = true;
        GameManager.instance.rewardBoxPanel.gameObject.SetActive(true);
        roomReward = GameManager.instance.rewardBoxPanel.GetComponent<RoomReward>();

        // ���Ⱝȭ, ��ų��ȭ �� ���� ����
        int weaponOrSkill = Random.Range(0, 2);

        // Ȯ���� ������ ����
        // �ٵ� ���� Ƽ��� Ȯ���� ���� ���� Ƽ��� Ȯ���� ���ƾ� �Ǵ°� �ƴұ�?
        // [0, 30): ���� 5Ƽ��, [30, 55): ���� 4Ƽ��, [55, 75): ���� 3Ƽ��, [75, 90): ���� 2Ƽ��, [90, 100): ���� 1Ƽ��
        int random = Random.Range(0, 400);
        int roleRandom = random / 100;  // 0: �˻�, 1: ����, 2: ����, 3: �ų�
        int dataIndex = random % 100;
        int dataRandom = 0;
        if (dataIndex < 30) dataRandom = 0;
        else if (dataIndex < 55) dataRandom = 1;
        else if (dataIndex < 75) dataRandom = 2;
        else if (dataIndex < 90) dataRandom = 3;
        else dataRandom = 4;

        // �����̸�
        if (weaponOrSkill == 0)
        {
            WeaponData randomWeaponData = GameManager.instance.weaponData[roleRandom * 5 + dataRandom];
            roomReward.AcquireOrUpgrade(roleRandom * 5 + dataRandom, randomWeaponData);
        }

        // ��ų�̸�
        else if (weaponOrSkill == 1)
        {
            SkillData randomSkillData = GameManager.instance.skillData[roleRandom * 5 + dataRandom];
            roomReward.AcquireOrUpgrade(roleRandom * 5 + dataRandom, randomSkillData);
        }

        gameObject.SetActive(false);
    }
}
