using System.Collections.Generic;
using System.Linq;

public class ItemManager
{
    public static ItemManager instance;

    static ItemManager()
    {
        instance = new ItemManager();
    }

    // 아이템 데이터와 아이템 번호를 저장함
    public Dictionary<ItemSO, int> ItemDict = new Dictionary<ItemSO, int>();



    /// <summary>
    /// 아이템 등록. ItemSO와 아이템id를 매핑합니다.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="itemNumber"></param>
    public void RegisterItem(ItemSO item, int itemNumber)
    {
        if (item == null)
        {
            Service.Log("아이템이 null입니다. 등록할 수 없습니다.");
            return;
        }
        else if (ItemDict.Count >= 2 && !ItemDict.ContainsKey(item))
        {
            Service.Log("아이템 개수가 2개를 초과했습니다. 새로운 아이템을 등록할 수 없습니다.");
            return;
        }
        else if (ItemDict.ContainsKey(item))
        {
            Service.Log($"{item.name} 아이템이 이미 등록되어 있습니다. 덮어씁니다.");
        }
        // 아이템이 이미 등록되어 있으면 덮어쓰기 (보통, 일어나선 안됨.)
        // 아이템이 2개를 초과하면 등록 불가

        ItemDict[item] = itemNumber;
        // 등록 시 Inventory에 추가함
        // 아이템 슬롯 중 빈 슬롯 찾기(itemNumber가 0이거나 null인 경우)

        if (ItemDict.Count == 1)
        {
            UiManager.Instance.Get<InventoryUi>().SetView(SlotType.FirstSlot, itemNumber); // 첫 번째 슬롯에 아이템 번호를 설정
        }
        else if (ItemDict.Count == 2)
        {
            UiManager.Instance.Get<InventoryUi>().SetView(SlotType.SecondSlot, itemNumber); // 두 번째 슬롯에 아이템 번호를 설정
        }
    }

    // 아이템 번호로 아이템 찾기
    public ItemSO Getitem(int ItemID)
    {
        foreach (var item in ItemDict)
        {
            if (item.Value == ItemID)
            {
                Service.Log($"{item.Key.name} 아이템을 찾았습니다. 아이템 번호: {item.Value}");
                // 아이템을 찾았다면 아이템 정보를 출력
                return item.Key; // 아이템 정보를 반환
            }
        }
        Service.Log($"아이템 번호 {ItemID}에 해당하는 아이템을 찾을 수 없습니다.");
        return null; // 아이템을 찾지 못했을 때 null 반환
    }

    // 아이템 데이터로 아이템 번호 찾기
    public int GetItemNumber(ItemSO item)
    {
        if (item == null)
        {
            Service.Log("아이템이 null입니다.");
            return -1; // 아이템이 없을 때 -1 반환
        }
        if (ItemDict.TryGetValue(item, out int itemNumber))
        {
            return itemNumber;
        }
        Service.Log($"{item.name} 아이템이 등록되어 있지 않습니다.");
        return -1; // 아이템이 등록되어 있지 않을 때 -1 반환
    }



    //아이템 제거 메서드
    public void RemoveItem(ItemSO item)
    {
        if (item == null)
        {
            Service.Log("아이템 데이터가 null입니다. 제거할 수 없습니다.");
            return;
        }
        if (ItemDict.ContainsKey(item))
        {
            // 아이템을 인벤토리에서 제거, 남은 아이템이 있다면 첫번째 슬롯에 남은 아이템을 설정하고, 두번째 슬롯은 비워둠
            ItemDict.Remove(item);
            if (ItemDict.Count == 1)
            {
                // 남은 아이템이 하나라면 첫 번째 슬롯에 남은 아이템 번호를 설정
                UiManager.Instance.Get<InventoryUi>().SetView(SlotType.FirstSlot, ItemDict.Values.First());
                UiManager.Instance.Get<InventoryUi>().SetView(SlotType.SecondSlot, 0); // 두 번째 슬롯 비우기
            }
            else if (ItemDict.Count == 0)
            {
                // 남은 아이템이 없다면 두 슬롯 모두 비우기
                UiManager.Instance.Get<InventoryUi>().SetView(SlotType.FirstSlot, 0);
                UiManager.Instance.Get<InventoryUi>().SetView(SlotType.SecondSlot, 0);
            }
            // 아이템 제거 시 Inventory에서 제거함
        }
        else
        {
            Service.Log($"{item.name} 아이템이 등록되어 있지 않습니다.");
        }
    }


}