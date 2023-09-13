using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusInfo : MonoBehaviour
{
    [Header("----- Component -----")]
    public Player player;
    
    //public Image[] statusSkillArea;
    //public Sprite[] skillImage;
    //public Text[] skillName;
    //public Text[] skillDesc;

    public Text statusHeart;
    public Text statusSpeed;
    public Text statusAttackSpeed;
    public Text statusPower;

    public GameObject statusWeaponArea;

    void OnEnable()
    {
        // 스탯창 정보 출력
        statusHeart.text = player.health.ToString();
        statusSpeed.text = player.speed.ToString();
        statusAttackSpeed.text = player.attackSpeed.ToString();
        statusPower.text = player.power.ToString();

        for(int i=0; i<GameManager.instance.player.getWeaponCount; i++)
        {
            // 이미지 0: 배경, 이미지 1: 무기 칸 배경, 텍스트 0: 타이틀
            Image[] weaponImage = statusWeaponArea.GetComponentsInChildren<Image>();
            Text[] weaponText = statusWeaponArea.GetComponentsInChildren<Text>();

            weaponImage[2 * (i + 1)].sprite = GameManager.instance.weapon[i].icon;
            weaponText[1 + (i * 3)].text = GameManager.instance.weapon[i].name;
            weaponText[2 + (i * 3)].text = "레벨: " + GameManager.instance.weapon[i].level.ToString();
            weaponText[3 + (i * 3)].text = "공격력: " + GameManager.instance.weapon[i].damage.ToString();
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
