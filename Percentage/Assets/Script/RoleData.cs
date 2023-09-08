using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleData : ScriptableObject
{
    public enum RoleType { Knight, Wizard, Thief, Gunner }

    [Header("----- Main Info -----")]
    public RoleType roleType;
    public int roleId;
    public string roleName;
    public int roleBasicWeapon;
    public int roleBasicSkill;


}
