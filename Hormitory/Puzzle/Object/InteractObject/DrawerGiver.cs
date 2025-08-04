using UnityEngine;

public class DrawerGiver : ItemReceiver,IInteractable
{
    [SerializeField] private ItemSO giveItem; // 상호작용 시 주어질 아이템
    private bool isGetItem = false; // 아이템 획득 여부

    public override void InteractAction()
    {
        if(isCleard && isGetItem == false)
        {
            ItemManager.instance.RegisterItem(giveItem, giveItem.ItemID); // 아이템 획득
            SoundManager.PlaySfx(SoundCategory.Interaction, "OpenDoor");
            UiManager.Instance.Get<TalkUi>().Popup("서랍에서 작은 열쇠를 발견했다.");
            UiManager.Instance.Get<MissionUi>().Popup("목표 : 열쇠의 사용처 찾기");
            isGetItem = true; // 아이템 획득 상태 변경
        }
        else
        {
            UiManager.Instance.Get<TalkUi>().Popup("쪽지가 보인다. [열쇠는 실험실에 보관할 것]");
            SoundManager.PlaySfx(SoundCategory.Interaction, "LockDoor");
        }

    }
    public override void AfterInteract()
    {
        SoundManager.PlaySfx(SoundCategory.Interaction, "Rustle");
        UiManager.Instance.Get<TalkUi>().Popup("여기에는 더이상 아무것도 없다.");
    }
}
