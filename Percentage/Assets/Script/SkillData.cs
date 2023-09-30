using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Object/SkillData")]
public class SkillData : ScriptableObject
{
    public enum UsableRoleType { Knight, Wizard, Thief, Gunner }
    public enum SkillType { Basic, Utility, Buff, Special, Ultimate }

    [Header("----- Main Info -----")]
    public UsableRoleType usableRoleType;
    public SkillType skillType;
    public int skillId;
    public string skillName;
    public string skillDesc;
    public float skillCoolTime;
    public float skillDuringTime;
    public Sprite skillIcon;

    [Header("----- Level Data -----")]
    public float baseDamage;
    public float[] upgradeDamage;
    public int level;
    public int maxLevel;
}
