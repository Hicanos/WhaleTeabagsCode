using UnityEngine;

/// <summary>
/// 아이템 스크립터블 오브젝트
/// </summary>

[CreateAssetMenu(fileName = "NewSkillBook", menuName = "Items/SkillBook")]
public class SkillBook : ItemSO
{
    // 스킬북: 3가지 종류로 나뉘어져 있음(하 중 상)
    // 일정 개수를 꽉 채워야 스킬 레벨을 올릴 수 있음
    // 그리고 스킬 레벨을 올리면 그에 따라 스킬이 일정 비율로 계수가 증가함
    // 스킬북은 스킬의 레벨에 따라 필요한 스킬북의 종류와 개수가 다름
    [Header("스킬북 속성")]
    public SkillBookType BookType; // 스킬북의 종류 (하급, 중급, 상급)
}

public enum SkillBookType
{
    // 스킬북의 종류
    Low, // 하급 스킬북
    Middle, // 중급 스킬북
    High // 상급 스킬북
}