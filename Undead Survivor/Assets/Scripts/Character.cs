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
        icon = GetComponentsInChildren<Image>(true)[1]; // 0�� �ε����� �ڱ� �ڽ��̱� ������, 1�� �ε����� �����;� ��
        icon.sprite = characterData.characterIcon;

        // GetComponents�� ������ Hierarchy ������ ��ġ
        Text[] texts = GetComponentsInChildren<Text>(true);
        textName = texts[0];
        textDesc = texts[1];
    }

    void OnEnable()
    {
        textName.text = characterData.characterName;
        textDesc.text = string.Format(characterData.characterDesc);
    }


    // �Ӽ�
    //public static float Speed
    //{
    //    get { return GameManager.instance.playerId == 0 ? 1.1f : 1f; }
    //}

    //// �������� �� ���ư��� �ӵ�
    //public static float WeaponSpeed
    //{
    //    get { return GameManager.instance.playerId == 1 ? 1.1f : 1f; }
    //}

    //// �߻繫�� ������ ����
    //public static float WeaponRate
    //{
    //    get { return GameManager.instance.playerId == 1 ? 0.9f : 1f; }
    //}

    //public static float Damage
    //{
    //    get { return GameManager.instance.playerId == 2 ? 1.1f : 1f; }
    //}

    //// ���̸� �� ���� �߰�, �������̸� ����� Ƚ�� �߰�
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
