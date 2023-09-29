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

        // 무기강화, 스킬강화 중 랜덤 선택
        int weaponOrSkill = Random.Range(0, 2);

        // 확률로 데이터 설정
        // 근데 낮은 티어는 확률이 높고 높은 티어는 확률이 낮아야 되는거 아닐까?
        // [0, 30): 전사 5티어, [30, 55): 전사 4티어, [55, 75): 전사 3티어, [75, 90): 전사 2티어, [90, 100): 전사 1티어
        int random = Random.Range(0, 400);
        int roleRandom = random / 100;  // 0: 검사, 1: 법사, 2: 도적, 3: 거너
        int dataIndex = random % 100;
        int dataRandom = 0;
        if (dataIndex < 30) dataRandom = 0;
        else if (dataIndex < 55) dataRandom = 1;
        else if (dataIndex < 75) dataRandom = 2;
        else if (dataIndex < 90) dataRandom = 3;
        else dataRandom = 4;

        // 무기이면
        if (weaponOrSkill == 0)
        {
            WeaponData randomWeaponData = GameManager.instance.weaponData[roleRandom * 5 + dataRandom];
            roomReward.AcquireOrUpgrade(roleRandom * 5 + dataRandom, randomWeaponData);
        }

        // 스킬이면
        else if (weaponOrSkill == 1)
        {
            SkillData randomSkillData = GameManager.instance.skillData[roleRandom * 5 + dataRandom];
            roomReward.AcquireOrUpgrade(roleRandom * 5 + dataRandom, randomSkillData);
        }

        gameObject.SetActive(false);
    }
}
