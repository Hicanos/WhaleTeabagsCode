using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    // 자원(재료), 제작템, 장비(장비-도구, 무기), 소모품 기타
    Resource,
    Crafting,
    Equipable,
    Consumable,
    Etc
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    // 고유 ID를 추가하면 아이템 관리 및 저장/불러오기에 유용 (확장성 고려)
    public string itemID; // 아이템의 고유 식별자
    public string itemName; // 아이템 이름
    public Sprite itemIcon; // 아이템 아이콘
    [TextArea(3, 5)] // 인스펙터에서 여러 줄로 입력 가능
    public string itemDescription; // 아이템 설명
    public List<ItemType> itemType; // 아이템 타입 다중 설정 가능

    [Header("인벤토리 설정")]
    public bool isStackable = true; // 겹치기 가능 여부 (기본값 true), 도구나 무기에서는 false로 변경
    public int maxStackSize = 20; // 최대 겹침 개수 (기본값 20)
    public GameObject dropPrefab; // 월드에 떨어뜨렸을 때 생성될 프리팹 (선택 사항)
}

