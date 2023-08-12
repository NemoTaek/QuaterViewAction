using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ScriptableObject: 유니티에서 제공하는 대량의 데이터를 저장하는데 사용할 수 있는 데이터 컨테이너
// MonoBehaviour 스크립트에 변경되지 않는 데이터를 저장하는 프리팹을 사용할 때 유용
// 메모리에 스크립터를 오브젝트의 데이터 사본만 저장하고 이를 참조하는 방식으로 작동
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object / ItemData")]   // 커스텀 메뉴를 생성하는 속성
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Glove, Shoes, Missile, Spear, Boomerang, ShotGun, Heal=99 };

    [Header("------- Main Info -------")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;
    public bool isActive;

    [Header("------- Level Data -------")]
    public float baseDamage;
    public int baseCount;
    public float baseSpeed;
    public float[] damages;
    public int[] counts;

    [Header("------- Weapon -------")]
    public GameObject projectile;   // 발사체 라는 뜻
    public Sprite hand;
}
