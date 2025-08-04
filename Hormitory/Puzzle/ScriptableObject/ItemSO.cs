using UnityEngine;

public interface IItem 
{
    int ItemID { get;} // 아이템 ID - 아이템 구분. ItemManager의 Dictionary의 Value로 사용
    string ItemName { get;} // 아이템 이름
    Sprite Icon { get; } // 아이템 아이콘 이미지 - UI에서 사용

}

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class ItemSO : ScriptableObject, IItem
{
    [SerializeField] private int itemID; // 아이템 ID
    [SerializeField] private string itemName; // 아이템 이름
    [SerializeField] private Sprite icon; // 아이템 아이콘 이미지
    public int ItemID => itemID; // 아이템 ID - ItemManager의 Dictionary의 Key로 사용
    public string ItemName => itemName; // 아이템 이름 - UI에서 사용, 예: 인벤토리, 퀘스트 등에서 표시
    public Sprite Icon => icon; // 아이템 아이콘 이미지 - UI에서 사용
}
