using UnityEngine;
using static DesignEnums;

/// <summary>
/// 액티브 스킬을 만들기 위한 Scriptable Object
/// </summary>
[CreateAssetMenu(fileName = "NewActiveSkill", menuName = "Skill/Create New Active Skill")]
public class ActiveSO : SkillSO
{
    public int SkillCost; // 소모되는 스킬 코스트
    public string[] BuffIDs; // 버프 아이디 배열
}
