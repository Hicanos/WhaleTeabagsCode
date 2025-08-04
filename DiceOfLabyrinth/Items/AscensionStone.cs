using UnityEngine;

/// <summary>
/// 돌파석 SO 클래스
/// </summary>

[CreateAssetMenu(fileName = "NewAscensionStone", menuName = "Items/AscensionStone")]
public class AscensionStone : ItemSO
{
    // 돌파석: 캐릭터를 중복 획득 했을 때 해당 아이템을 획득함
    // 돌파석을 사용하면 캐릭터의 돌파레벨을 상승시키고, 능력치를 일정 비율 상승시킴
    // 현재는 급하게 구현할 필요 없음
    [Header("돌파석 속성")]
    public string CharID; // 돌파석이 속한 캐릭터의 ID (해당 캐릭터의 돌파석임을 나타내며, 
                          // 돌파석을 사용하면 해당 캐릭터의 돌파 레벨이 상승함)
}
