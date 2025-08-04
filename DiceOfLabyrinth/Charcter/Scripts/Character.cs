using UnityEngine;
using UnityEngine.TextCore.Text;
/// <summary>
/// 공통 베이스
/// </summary>
public class Character
{
    // 캐릭터 데이터
    public CharacterSO CharacterData;
    public int Level = 1;

    public virtual void Initialize(CharacterSO so, int level = 1)
    {
        CharacterData = so;
        Level = level;
    }
    // 공통 메서드 (GetMaxHP 등)
    /// <summary>
    /// 최대 HP 계산 (외부 강화 미포함)
    /// </summary>
    public virtual int GetMaxHP()
    {
        return CharacterData.baseHP + CharacterData.plusHP * (Level - 1);
    }

    /// <summary>
    /// 공격력 계산 (외부 강화 미포함)
    /// </summary>
    public virtual int GetATK()
    {
        return CharacterData.baseATK + CharacterData.plusATK * (Level - 1);
    }

    /// <summary>
    /// 방어력 계산 (외부 강화 미포함)
    /// </summary>
    public virtual int GetDEF()
    {
        return CharacterData.baseDEF + CharacterData.plusDEF * (Level - 1);
    }

}
