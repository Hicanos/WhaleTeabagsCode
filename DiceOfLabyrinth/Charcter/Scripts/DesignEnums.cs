using System;

public static class DesignEnums
{
    public enum ClassTypes
    {
        Dealer = 0,
        Tanker = 1,
        Supporter = 2,
    }
    public enum ElementTypes
    {
        Fire = 0,
        Grass = 1,
        Rock = 2,
        Electro = 3,
        Water = 4,
    }
    public enum SkillType
    {
        Active = 0,
        Passive = 1,
    }
    public enum SkillTarget
    {
        Self = 0,
        Enemy = 1,
        Team = 2,
    }
    public enum SkillRule
    {
        Cost = 0,
        SumOver = 1,
        UniqueSigniture = 2,
        DeckMaid = 3,
        TeamSignitureDeckMaid = 4,
    }
    public enum BuffType
    {
        Buff = 0,
        Debuff = 1,
        CC = 2,
    }
    public enum BuffCategory
    {
        ATK = 0,
        DEF = 1,
        HP = 2,
        CritC = 3,
        CritD = 4,
        Pene = 5,
        Heal = 6,
        Cost = 7,
        Stun = 8,
        Confusion = 9,
        Paralysis = 10,
        Silence = 11,
        Shield = 12,
        Taunted = 13,
    }
}
