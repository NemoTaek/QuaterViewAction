using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public CharacterData characterData;
    Text textName;
    Text textDesc;
    Image icon;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>(true)[1]; // 0번 인덱스는 자기 자신이기 때문에, 1번 인덱스로 가져와야 함
        icon.sprite = characterData.characterIcon;

        // GetComponents의 순서는 Hierarchy 순서와 일치
        Text[] texts = GetComponentsInChildren<Text>(true);
        textName = texts[0];
        textDesc = texts[1];
    }

    void OnEnable()
    {
        textName.text = characterData.characterName;
        textDesc.text = string.Format(characterData.characterDesc);
    }


    // 속성
    //public static float Speed
    //{
    //    get { return GameManager.instance.playerId == 0 ? 1.1f : 1f; }
    //}

    //// 근접무기 삽 돌아가는 속도
    //public static float WeaponSpeed
    //{
    //    get { return GameManager.instance.playerId == 1 ? 1.1f : 1f; }
    //}

    //// 발사무기 라이플 공속
    //public static float WeaponRate
    //{
    //    get { return GameManager.instance.playerId == 1 ? 0.9f : 1f; }
    //}

    //public static float Damage
    //{
    //    get { return GameManager.instance.playerId == 2 ? 1.1f : 1f; }
    //}

    //// 삽이면 삽 개수 추가, 라이플이면 관통력 횟수 추가
    //public static int Count
    //{
    //    get { return GameManager.instance.playerId == 3 ? 1 : 0; }
    //}

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
