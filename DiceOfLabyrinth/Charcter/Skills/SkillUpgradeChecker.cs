using UnityEngine;
using System.Collections.Generic;

public class SkillUpgradeChecker
{
    // 스킬 레벨업에 필요한 스킬북 개수 정의
    private static readonly Dictionary<int, Dictionary<SkillBookType, int>> SkillBookRequirements = new()
    {
        { 1, new Dictionary<SkillBookType, int> { { SkillBookType.Low, 5 } } },
        { 2, new Dictionary<SkillBookType, int> { { SkillBookType.Low, 10 }, { SkillBookType.Middle, 5 } } },
        { 3, new Dictionary<SkillBookType, int> { { SkillBookType.Middle, 10 }, { SkillBookType.High, 5 } } },
        { 4, new Dictionary<SkillBookType, int> { { SkillBookType.High, 10 } } },
    };

    // 최대 스킬 레벨
    public const int MaxSkillLevel = 5;

    // 스킬 레벨업 가능 여부 체크
    public static bool CanUpgradeSkill(int skillLevel)
    {
        if (skillLevel >= MaxSkillLevel) return false;
        int nextLevel = skillLevel + 1;
        if (!SkillBookRequirements.ContainsKey(skillLevel)) return false;
        var requirements = SkillBookRequirements[skillLevel];
        var ownedBooks = ItemManager.Instance.GetSkillBooks();
        foreach (var req in requirements)
        {
            int owned = 0;
            foreach (var kv in ownedBooks)
            {
                var itemSO = ItemManager.Instance.GetItemSO(kv.Key) as SkillBook;
                if (itemSO != null && itemSO.BookType == req.Key)
                    owned += kv.Value;
            }
            if (owned < req.Value)
                return false;
        }
        return true;
    }
}
