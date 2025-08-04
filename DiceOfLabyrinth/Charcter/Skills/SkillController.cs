using UnityEngine;
using System.Collections.Generic;
using static DesignEnums;

/// <summary>
/// 스킬을 실제로 사용하는 컨트롤러
/// </summary>
public class SkillController
{
    // 스킬 사용 메인 진입점

    static GameObject characterPrefab;

    public static void SkillUse(BattleCharacter user, SkillSO skill, List<BattleCharacter> allAllies, BattleEnemy enemy)
    {
        List<IDamagable> targets = GetTargets(user, skill.SkillTarget, allAllies, enemy);
        

        switch (skill.SkillRule)
        {
            case SkillRule.Cost:
                UseCostSkill(user, skill, targets);
                break;
            case SkillRule.SumOver:
                UseSumOverSkill(user, skill, targets);
                break;
            case SkillRule.UniqueSigniture:
                UseUniqueSignitureSkill(user, skill, targets);
                break;
            case SkillRule.DeckMaid:
                UseDeckMaidSkill(user, skill, targets);
                break;
            case SkillRule.TeamSignitureDeckMaid:
                UseTeamSignitureDeckMaidSkill(user, skill, targets);
                break;
            default:
                Debug.LogWarning($"SkillRule 미구현: {skill.SkillRule}");
                break;
        }
    }

    // 타겟 결정
    private static List<IDamagable> GetTargets(BattleCharacter user, SkillTarget targetType, List<BattleCharacter> allAllies, BattleEnemy enemy)
    {
        switch (targetType)
        {
            case SkillTarget.Self:
                return new List<IDamagable> { user };
            case SkillTarget.Enemy:
                return enemy != null ? new List<IDamagable> { enemy } : new List<IDamagable>();
            case SkillTarget.Team:
                return new List<IDamagable>(allAllies);
            default:
                return new List<IDamagable> { user };
        }
    }

    // 각 SkillRule별 처리 (예시)
    private static void UseCostSkill(BattleCharacter user, SkillSO skill, List<IDamagable> targets)
    {
        // 코스트 소모
        // 여기 왔다는 것 = ActiveSkill이므로 코스트가 존재함
        // skillSO가 ActiveSO 경우에만 코스트를 소모

        if (skill is ActiveSO activeSO)
        {
            BattleManager.Instance.SpendCost(activeSO.SkillCost);
        }        

        if (skill.IsAttacking)
        {
            foreach (var target in targets)
            {
                int damage = Mathf.RoundToInt(user.CurrentATK * skill.SkillValue);
                target.TakeDamage(damage);
                Debug.Log($"{user.CharNameKr}이(가) {target}에게 {damage}의 피해를 주었습니다. 차후 추가타의 형식으로 구현(boolean)");
            }
        }
        else
        {
            // 버프 적용 로직 등
            ApplyBuff(user, skill, targets);
        }
    }

    private static void UseSumOverSkill(BattleCharacter user, SkillSO skill, List<IDamagable> targets)
    {
        if (skill.IsAttacking)
        {
            foreach (var target in targets)
            {
                int damage = Mathf.RoundToInt(user.CurrentATK * skill.SkillValue);
                target.TakeDamage(damage);
                Debug.Log($"{user.CharNameKr}이(가) {target}에게 {damage}의 피해를 주었습니다. 차후 추가타의 형식으로 구현(boolean)");
            }
        }
        else
        {
            // 버프 적용 로직 등
            ApplyBuff(user, skill, targets);
        }
    }

    private static void UseUniqueSignitureSkill(BattleCharacter user, SkillSO skill, List<IDamagable> targets)
    {
        if (skill.IsAttacking)
        {
            foreach (var target in targets)
            {
                int damage = Mathf.RoundToInt(user.CurrentATK * skill.SkillValue);
                target.TakeDamage(damage);
                Debug.Log($"{user.CharNameKr}이(가) {target}에게 {damage}의 피해를 주었습니다. 차후 추가타의 형식으로 구현(boolean)");
            }
        }
        else
        {
            // 버프 적용 로직 등
            ApplyBuff(user, skill, targets);
        }
    }

    private static void UseDeckMaidSkill(BattleCharacter user, SkillSO skill, List<IDamagable> targets)
    {
        if (skill.IsAttacking)
        {
            foreach (var target in targets)
            {
                int damage = Mathf.RoundToInt(user.CurrentATK * skill.SkillValue);
                target.TakeDamage(damage);
                Debug.Log($"{user.CharNameKr}이(가) {target}에게 {damage}의 피해를 주었습니다. 차후 추가타의 형식으로 구현(boolean)");
            }
        }
        else
        {
            // 버프 적용 로직 등
            ApplyBuff(user, skill, targets);
        }
    }

    private static void UseTeamSignitureDeckMaidSkill(BattleCharacter user, SkillSO skill, List<IDamagable> targets)
    {
        if (skill.IsAttacking)
        {
            foreach (var target in targets)
            {
                int damage = Mathf.RoundToInt(user.CurrentATK * skill.SkillValue);
                target.TakeDamage(damage);
                Debug.Log($"{user.CharNameKr}이(가) {target}에게 {damage}의 피해를 주었습니다. 차후 추가타의 형식으로 구현(boolean)");
            }
        }
        else
        {
            // 버프 적용 로직 등
            ApplyBuff(user, skill, targets);
        }
    }

    // Skill의 BuffID에 따른 버프 적용
    private static void ApplyBuff(BattleCharacter user, SkillSO skill, List<IDamagable> target)
    {
        characterPrefab = BattleManager.Instance.PartyData.Characters[BattleManager.Instance.PartyData.DefaultCharacters.IndexOf(user)].Prefab;
        characterPrefab.GetComponent<SpawnedCharacter>().SkillAttack();
        Debug.Log($"{user.CharNameKr}이(가) {skill.SkillNameKr} 스킬을 사용하여 버프/디버프를 적용했습니다.");
    }
}
