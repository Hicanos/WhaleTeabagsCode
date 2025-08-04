using UnityEngine;

/// <summary>
/// 아이템의 기본적인 속성 정의
/// </summary>

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class ItemSO : ScriptableObject
{
    [Header("아이템 기본 정보")]
    public string ItemID; // 아이템의 고유 ID
    public string NameKr; // 아이템의 한국어 이름
    public string NameEn; // 아이템의 영어 이름
    public Sprite Icon; // 아이템의 아이콘 이미지

    [TextArea(3, 5)]
    public string Description; // 아이템의 한국어 설명

    [Header("아이템 속성")]
    public ItemType Type; // 아이템의 종류 (예: Consumables, Equipment 등)
    public ItemRarity Rarity; // 아이템의 희귀도 (예: Common, Rare, Epic 등)
    public ItemLoot LootType; // 아이템의 획득 방식 (예: StageClear, CharacterRecruit 등)
    public bool IsStackable; // 아이템이 인벤토리에서 한 칸에 여러 개 쌓일 수 있는지 여부(true면 스택가능)
    public int MaxStackSize = 1; // 아이템의 최대 스택 크기 (스택 가능 아이템에만 적용)
}

public enum ItemType
{
    // 현재는 소비아이템 뿐이나 추후 추가될 것을 고려해 enum으로 정의
    Consumables
}

public enum ItemRarity
{
    // 아이템의 희귀도(추가될 때마다 enum에 추가)
    None
}

public enum ItemLoot
{
    // 아이템의 획득 방식
    None, // 획득 불가 아이템(튜토리얼 아이템 등)
    StageClear, //스테이지 클리어 보상
    CharacterRecruit // 캐릭터 중복 획득
}
