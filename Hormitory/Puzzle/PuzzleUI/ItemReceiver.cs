using UnityEngine;

/// <summary>
/// 가지고 있는 아이템과 자신의 번호를 비교하여 상호작용할 수 있는 오브젝트
/// </summary>
public class ItemReceiver : MonoBehaviour, IInteractable
{
    [SerializeField] protected int RequiredItemNumber; // 상호작용을 위해 필요한 아이템 번호
    protected ItemSO itemData;
    protected bool isCleard = false; // 퍼즐이 완료되었는지 여부(지정 아이템과 상호작용이 완료되었는지 여부)
    public void Interact()
    {
        //상호작용 시, ItemManager에 RequiredItemNumber와 일치하는 아이템이 있는지 확인

        // ItemManager에서 아이템 번호를 가져와서 비교
        // 아이템 전체 목록에서 특정 아이템 번호를 확인하는 로직
        // Dictionary에서 RequiredItemNumber와 일치하는 아이템을 찾기
        if (RequiredItemNumber > 0)
        {
            itemData = ItemManager.instance.Getitem(RequiredItemNumber);
            if (itemData != null && isCleard == false)
            {
                // 아이템이 있다면 상호작용 액션 실행                
                ItemManager.instance.RemoveItem(itemData); //상호작용한 아이템을 제거
                isCleard = true; // 상호작용 완료 상태로 변경
                InteractAction();
                return; // 아이템이 있으면 상호작용 종료
            }
            else if(itemData == null && isCleard == false)
            {
                InteractAction();
                Service.Log($"아이템 번호 {RequiredItemNumber}을 가진 아이템이 없습니다.");
            }
            else
            {
                //상호 작용 완료 후 interact가 호출되면, 완료된 후의 상호작용 액션 실행
                AfterInteract();
            }
        }
        else
        {
            Service.Log("필요한 아이템 번호가 설정되지 않았습니다.");
        }

    }

    public virtual void InteractAction()
    {
        // 상호작용 성공 시 수행할 액션 기재
        // 상속할 것
        Service.Log("기본 상호작용 액션이 호출되었습니다. 이 메서드는 상속받아 구현해야 합니다.");
    }

    public virtual void AfterInteract()
    {
        // 상호작용 후 수행할 액션 기재
        // 상속할 것
        Service.Log("기본 상호작용 후 액션이 호출되었습니다. 이 메서드는 상속받아 구현해야 합니다.");
    }
}
