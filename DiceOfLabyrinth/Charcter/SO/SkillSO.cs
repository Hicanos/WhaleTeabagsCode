using UnityEngine;
using static DesignEnums;

/// <summary>
/// 스킬을 만들기 위한 Scriptable Object
/// 이후 json을 이용하여 Skill을 사용하도록 변경
/// </summary>

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/Create New Skill")]
public class SkillSO : ScriptableObject
{
    // 공통 필드
    public string SkillID;
    public string SkillNameKr;
    public string SkillNameEn;
    public SkillType SkillType;
    public SkillRule SkillRule;
    public SkillTarget SkillTarget;
    public string SkillDescription;
    public Sprite SkillIcon;

    // 수치 필드
    public float SkillValue;
    public float PlusSkillValue;
    public float BuffValue;
    public float PlusBuffValue;
    public int BuffProbability;
    public int PlusBuffProbability;
    public int BuffTurn;
    public int CoolTime;
    public bool IsAttacking; // 해당 스킬이 공격(대미지 부여)스킬인지 여부
}
