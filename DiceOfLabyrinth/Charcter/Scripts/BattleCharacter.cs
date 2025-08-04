using System;
using System.Collections.Generic;
using UnityEngine;
using static DataSaver;

public enum StatType
{
    HP,
    ATK,
    DEF,
    CritChance,
    CritDamage,
    Penetration
}

/// <summary>
/// 배틀(전투)에서 사용되는 캐릭터 정보 및 기능
/// (실시간 변동 데이터, 버프 등)
/// </summary>
[System.Serializable]
public class BattleCharacter : IDamagable
{
    // 캐릭터의 고유 데이터
    public string CharID { get; private set; }
    public CharacterSO CharacterData { get; private set; }

    // Regular: 현재 최대값(버프 등 포함)
    public int RegularHP { get; private set; }
    public int RegularATK { get; private set; }
    public int RegularDEF { get; private set; }
    public float RegularCritChance { get; private set; }
    public float RegularCritDamage { get; private set; }
    public float Penetration { get; private set; }
    public int Level { get; private set; }

    // Current: 실시간 값(버프 등 적용)
    public string CharNameKr;
    public string CharNameEn;
    public int CurrentHP;
    public int CurrentATK;
    public int CurrentDEF;
    public float CurrentCritChance;
    public float CurrentCritDamage;
    public float CurrentPenetration;

    // initial: 초기값(Lobby 기준) 1층>2층으로 넘어갈 때 한 번 리셋하고, 진입시 보유한 아티팩트를 다시 계산
    private int initialHP;
    private int initialATK;
    private int initialDEF;
    private float initialCritChance;
    private float initialCritDamage;
    private float initialPenetration;
    private int initialLevel;

    public bool IsDied { get; set; }
    public bool UsingSkill { get; set; } = false;

    public event Action<int> OnHPChanged;
    public event Action OnDied;
    public event Action OnRevived;

    [Header("액티브 스킬 데이터")]
    // LobbyCharacter를 확인하고, 해당 데이터와 연동
    public int SkillLevelA = 1;
    public float SkillValueA;
    public float BuffProbabilityA;
    public float BuffValueA;
    public ActiveSO ActiveSkill;


    [Header("패시브 스킬 데이터")]
    // LobbyCharacter를 확인하고, 해당 데이터와 연동
    public int SkillLevelB = 1;
    public float SkillValueB;
    public float BuffProbabilityB;
    public float BuffValueB;
    public PassiveSO PassiveSkill;



    public BattleCharacter(CharacterSO so)
    {
        SetCharacterSO(so);
    }

    public BattleCharacter(string charID)
    {
        SetCharID(charID);
    }

    public void SetCharacterSO(CharacterSO so)
    {
        CharacterData = so;
        CharID = so.charID;
        InitializeFromLobbyAuto();
    }

    public void SetCharID(string charID)
    {
        CharID = charID;
        CharacterData = CharacterManager.Instance.AllCharacters.TryGetValue(charID, out var so) ? so : null;
        InitializeFromLobbyAuto();
    }

    /// <summary>
    /// CharacterSO/charID를 기반으로 자동으로 LobbyCharacter를 찾아 초기화 (시작시 1회 한정)
    /// </summary>
    private void InitializeFromLobbyAuto()
    {
        LobbyCharacter lobbyChar = null;
        if (CharacterData != null)
            lobbyChar = CharacterManager.Instance.GetLobbyCharacterBySO(CharacterData);
        else if (!string.IsNullOrEmpty(CharID))
            lobbyChar = CharacterManager.Instance.GetLobbyCharacterByID(CharID);

        if (lobbyChar == null)
            throw new InvalidOperationException($"BattleCharacter: 해당하는 LobbyCharacter를 찾을 수 없습니다. (charID: {CharID})");

        IsDied = false;
        // 초기값 저장 (Lobby 기준)
        initialLevel = lobbyChar.Level;
        initialATK = lobbyChar.RegularATK;
        initialDEF = lobbyChar.RegularDEF;
        initialHP = lobbyChar.RegularHP;
        initialCritChance = lobbyChar.CritChance;
        initialCritDamage = lobbyChar.CritDamage;
        initialPenetration = lobbyChar.CharacterData.penetration;

        // Regular: 현재 최대값(버프 등 포함, 초기엔 Lobby 기준)
        CharNameKr = CharacterData.nameKr; // 캐릭터 이름 설정
        CharNameEn = CharacterData.nameEn; // 캐릭터 영어 이름 설정
        Level = initialLevel;
        RegularATK = initialATK;
        RegularDEF = initialDEF;
        RegularHP = initialHP;
        RegularCritChance = initialCritChance;
        RegularCritDamage = initialCritDamage;
        Penetration = initialPenetration;

        // 스킬 동기화
        ActiveSkill = lobbyChar.ActiveSkill;
        SkillLevelA = lobbyChar.SkillLevelA;
        SkillValueA = lobbyChar.SkillValueA;
        BuffProbabilityA = lobbyChar.BuffProbabilityA;
        BuffValueA = lobbyChar.BuffValueA;

        PassiveSkill = lobbyChar.PassiveSkill;
        SkillLevelB = lobbyChar.SkillLevelB;
        SkillValueB = lobbyChar.SkillValueB;
        BuffProbabilityB = lobbyChar.BuffProbabilityB;
        BuffValueB = lobbyChar.BuffValueB;

        // Current: 실시간 값(버프 등 적용)
        ResetBattleData();
        IsDied = false;
    }

    /// <summary>
    /// 배틀 데이터 초기화 (스테이지 시작 시)
    /// </summary>
    public void ResetBattleData()
    {
        IsDied = false;
        CurrentHP = RegularHP;
        CurrentATK = RegularATK;
        CurrentDEF = RegularDEF;
        CurrentCritChance = RegularCritChance;
        CurrentCritDamage = RegularCritDamage;
        CurrentPenetration = Penetration;
    }

    // 공통 계산 메서드
    private int Result(int baseValue, float buffPercent)
    {
        return Mathf.RoundToInt(baseValue * (1f + buffPercent));
    }

    private float Result(float baseValue, float buffPercent)
    {
        return baseValue * (1f + buffPercent);
    }

    /// <summary>
    /// Regular(최대값) 갱신: Artifact/Engraving 등 외부 요인에 의해 단일 항목만 처리
    /// StatType과 buffPercent만 넘기면 됨
    /// </summary>
    public void ApplyRegularBuff(StatType statType, float buffPercent)
    {
        switch (statType)
        {
            case StatType.HP:
                RegularHP = Result(initialHP, buffPercent);
                if (CurrentHP > RegularHP)
                    CurrentHP = RegularHP;
                OnHPChanged?.Invoke(CurrentHP);
                break;
            case StatType.ATK:
                RegularATK = Result(initialATK, buffPercent);
                break;
            case StatType.DEF:
                RegularDEF = Result(initialDEF, buffPercent);
                break;
            case StatType.CritChance:
                RegularCritChance = Result(initialCritChance, buffPercent);
                break;
            case StatType.CritDamage:
                RegularCritDamage = Result(initialCritDamage, buffPercent);
                break;
            case StatType.Penetration:
                Penetration = Result(initialPenetration, buffPercent);
                break;
        }
    }

    public void DataSetting(BattleCharacterData data)
    {
        //배틀 캐릭터를 DataSaver에서 Regular 값을 세팅할 수 있도록 해주는 메서드
        // 전달받은 데이터로 Regular 값 세팅
        IsDied = false;
        RegularHP = data.regularHP;
        RegularATK = data.regularATK;
        RegularDEF = data.regularDEF;
        RegularCritChance = data.regularCritChance;
        RegularCritDamage = data.regularCritDamage;
        Penetration = data.regularPenetration;
        Level = data.level;

        CurrentHP = data.currentHP;
        CurrentATK = data.currentATK;
        CurrentDEF = data.currentDEF;
        CurrentCritChance = data.currentCritChance;
        CurrentCritDamage = data.currentCritDamage;
        CurrentPenetration = data.currentPenetration;

        OnHPChanged?.Invoke(CurrentHP);
    }



    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0)
        {
            CurrentHP = 0;
            IsDied = true;
            OnDied?.Invoke();
        }
        OnHPChanged?.Invoke(CurrentHP);
    }

    public void Heal(int amount)
    {
        CurrentHP += amount;
        if (CurrentHP > RegularHP)
        {
            CurrentHP = RegularHP;
        }
        OnHPChanged?.Invoke(CurrentHP);
    }

    public int AttackDamage() => CurrentATK;

    public void Revive()
    {
        if (IsDied)
        {
            IsDied = false;
            CurrentHP = RegularHP;
            ResetBattleData();
            OnRevived?.Invoke();
            OnHPChanged?.Invoke(CurrentHP);
        }
    }

    // 스킬 사용 메서드
    // 에너미 데이터는 BattleCharacter가 아님
    public void UseActiveSkill(List<BattleCharacter> allAllies, BattleEnemy enemy)
    {
        if (ActiveSkill != null)
        {
            SkillController.SkillUse(this, ActiveSkill, allAllies, enemy);
        }
        else
        {
            Debug.Log($"스킬이 안보인다 카더라");
        }            
    }

    public void UsePassiveSkill(List<BattleCharacter> allAllies, BattleEnemy enemy)
    {
        if (PassiveSkill != null)
        {
            SkillController.SkillUse(this, PassiveSkill, allAllies, enemy);
        }
    }
}
