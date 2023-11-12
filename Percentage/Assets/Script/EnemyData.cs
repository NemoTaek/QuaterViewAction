using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Object/EnemyData")]
public class EnemyData : ScriptableObject
{
    public enum EnemyType { Trace, Random, Stand, Boss };

    [Header("----- Main Info -----")]
    public EnemyType enemyType;
    public int enemyId;
    public float enemySpeed;
    public float enemyMaxHealth;
    public Sprite enemyImage;
    public RuntimeAnimatorController enemyAnimator;

    [Header("----- Property Info -----")]
    public bool isFly;
}
