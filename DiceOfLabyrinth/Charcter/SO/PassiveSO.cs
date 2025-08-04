using Unity.VisualScripting;
using UnityEngine;
using static DesignEnums;

/// <summary>
/// 패시브 스킬을 만들기 위한 Scriptable Object
/// </summary>
[CreateAssetMenu(fileName = "NewPassiveSkill", menuName = "Skill/Create New Passive Skill")]
public class PassiveSO : SkillSO
{
    public string[] BuffIDs; // 버프 아이디 배열

    // 스킬 룰에 따라 필요한 추가필드
    // SumOver라면 목표치, TeamSignitureDeckMaid 혹은 DeckMaid라면 해당 족보의 값 등
    // UniqueSigniture라면 해당 스킬을 가진 CharacterSO의 DiceData의  public int CignatureNo;를 가져와야함
    // TeamSignitureDeckMaid는 팀 전원의 시그니처 넘버를 확인하고 덱 메이드도봐야함

    public int TargetValue; // 목표치 (SumOver)



}
