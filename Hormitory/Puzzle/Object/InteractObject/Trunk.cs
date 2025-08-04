using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trunk : ItemReceiver, IInteractable
{
    [SerializeField] private ItemSO giveItem; // 퍼즐 완료 후 주는 아이템
    private bool isGetItem = false; // 아이템 획득 여부
    public override void InteractAction()
    {
        if (isCleard && isGetItem==false)
        {
            UiManager.Instance.Get<TalkUi>().Popup("안에 무슨 열쇠가 있군...");
            SoundManager.PlaySfx(SoundCategory.Interaction, "OpenDoor");
            ItemManager.instance.RegisterItem(giveItem, giveItem.ItemID);

            UiManager.Instance.Get<MissionUi>().Popup("목표 : 열쇠의 사용처 찾기");
            isGetItem = true; // 아이템 획득 상태 변경
        }
        else
        {            
            UiManager.Instance.Get<TalkUi>().Popup("맞는 열쇠가 없어.");
            UiManager.Instance.Get<MissionUi>().Popup("목표 : 트렁크 열쇠 찾기");
            SoundManager.PlaySfx(SoundCategory.Interaction, "LockDoor");
        }
    }

    public override void AfterInteract()
    {
        UiManager.Instance.Get<TalkUi>().Popup("이 열쇠는 어디서 쓰는 거지?");
    }
}
