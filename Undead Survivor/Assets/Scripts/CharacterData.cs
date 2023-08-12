using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ScriptableObject: 유니티에서 제공하는 대량의 데이터를 저장하는데 사용할 수 있는 데이터 컨테이너
// MonoBehaviour 스크립트에 변경되지 않는 데이터를 저장하는 프리팹을 사용할 때 유용
// 메모리에 스크립터를 오브젝트의 데이터 사본만 저장하고 이를 참조하는 방식으로 작동
[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Object / CharacterData")]   // 커스텀 메뉴를 생성하는 속성
public class CharacterData : ScriptableObject
{
    [Header("------- Main Info -------")]
    public int characterId;
    public string characterName;
    [TextArea]
    public string characterDesc;
    public Sprite characterIcon;
    public bool isActive;

    [Header("------- Stat Data -------")]
    public float baseDamage;
    public int baseCount;
    public float baseSpeed;
    public float baseWeaponSpeed;
    public float baseWeaponRate;
}
