using UnityEngine;

/// <summary>
/// 로비에서 사용되는 캐릭터 정보 및 기능
/// (외부 강화, 경험치, 성장 등)
/// </summary>
public class LobbyCharacter : Character
{
    [Header("로비 정보")]
    public int CurrentExp = 0;

    // Regular 속성을 BattleCharacter에서 가져감
    public int RegularATK;
    public int RegularDEF;
    public int RegularHP;
    public float CritChance;
    public float CritDamage;

    // 스킬 레벨 및 효과 (SkillSO 기반)
    [Header("액티브스킬 데이터")]
    public int SkillLevelA = 1;
    public ActiveSO ActiveSkill => CharacterData?.activeSO;
    public float SkillValueA => ActiveSkill == null ? 0 : ActiveSkill.SkillValue + (SkillLevelA - 1) * ActiveSkill.PlusSkillValue;
    public float BuffProbabilityA => ActiveSkill == null ? 0 : ActiveSkill.BuffProbability + (SkillLevelA - 1) * ActiveSkill.PlusBuffProbability;
    public float BuffValueA => ActiveSkill == null ? 0 : ActiveSkill.BuffValue + (SkillLevelA - 1) * ActiveSkill.PlusBuffValue;

    [Header("패시브스킬 데이터")]
    public int SkillLevelB = 1;
    public PassiveSO PassiveSkill => CharacterData?.passiveSO;
    public float SkillValueB => PassiveSkill == null ? 0 : PassiveSkill.SkillValue + (SkillLevelB - 1) * PassiveSkill.PlusSkillValue;
    public float BuffProbabilityB => PassiveSkill == null ? 0 : PassiveSkill.BuffProbability + (SkillLevelB - 1) * PassiveSkill.PlusBuffProbability;
    public float BuffValueB => PassiveSkill == null ? 0 : PassiveSkill.BuffValue + (SkillLevelB - 1) * PassiveSkill.PlusBuffValue;

    public override void Initialize(CharacterSO so, int level = 1)
    {
        base.Initialize(so, level);
        RegularATK = GetATK();
        RegularDEF = GetDEF();
        RegularHP = GetMaxHP();
        CritChance = CharacterData.critChance;
        CritDamage = CharacterData.critDamage;
    }

    public int GetExpToNextLevel()
    {
        return 250 * (Level + 1);
    }

    public void AddExp(int exp)
    {
        CurrentExp += exp;
        while (CurrentExp >= GetExpToNextLevel())
        {
            CurrentExp -= GetExpToNextLevel();
            LevelUP();
        }
        DataSaver.Instance.SaveCharacter(this);
    }

    private void LevelUP()
    {
        Level++;
        RegularATK = GetATK();
        RegularDEF = GetDEF();
        RegularHP = GetMaxHP();
    }

    public bool TryUpgradeSkillA()
    {
        if (SkillUpgradeChecker.CanUpgradeSkill(SkillLevelA))
        {
            SkillLevelA++;
            DataSaver.Instance.SaveCharacter(this);
            return true;
        }
        return false;
    }

    public bool TryUpgradeSkillB()
    {
        if (SkillUpgradeChecker.CanUpgradeSkill(SkillLevelB))
        {
            SkillLevelB++;
            DataSaver.Instance.SaveCharacter(this);
            return true;
        }
        return false;
    }

    public override int GetMaxHP()
    {
        return base.GetMaxHP();
    }

    public override int GetATK()
    {
        return base.GetATK();
    }

    public override int GetDEF()
    {
        return base.GetDEF();
    }

    public CharData GetCharData(CharDataLoader loader)
    {
        return loader.GetByCharID(CharacterData.charID);
    }
}
