using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 책장서랍
/// </summary>
public class Shelter : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO giveItem; // 상호작용 시 주어질 아이템
    private bool isInteracted = false; // 상호작용 여부
    private bool getItem = false; // 아이템 획득 여부

    public void Interact()
    {
        if(isInteracted == false)
        {
            isInteracted = true;
            UiManager.Instance.Get<TalkUi>().Popup("책장 서랍 사이 무언가 보인다.");
            SoundManager.PlaySfx(SoundCategory.Interaction, "Rustle");

        }
        else if(getItem == false)
        {
            getItem = true;
            UiManager.Instance.Get<TalkUi>().Popup("책장 서랍 사이에서 작은 열쇠를 발견했다.");
            SoundManager.PlaySfx(SoundCategory.Interaction, "Rustle");
            ItemManager.instance.RegisterItem(giveItem, giveItem.ItemID); // 아이템 획득
            UiManager.Instance.Get<MissionUi>().Popup("목표 : 열쇠의 사용처 찾기");
        }
        else
        {
            UiManager.Instance.Get<TalkUi>().Popup("책장 서랍 사이에 더 이상 아무것도 없다.");

        }
    }
}
