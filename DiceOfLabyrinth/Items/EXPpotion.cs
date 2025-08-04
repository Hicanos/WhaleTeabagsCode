using UnityEngine;


/// <summary>
/// 경험치 포션 SO 클래스 
/// </summary>

[CreateAssetMenu(fileName = "NewEXPpotion", menuName = "Items/EXPpotion")]
public class EXPpotion : ItemSO
{
    //ItemSO에서 정의된 기본적인 속성을 제외한 고유 속성 정의
    [Header("경험치 포션 속성")]
    public int ExpAmount; // 포션 사용 시 획득하는 경험치 양
}
