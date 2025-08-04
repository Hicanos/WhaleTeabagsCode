using UnityEngine;

public class Drawer : MonoBehaviour, IInteractable
{
    [SerializeField] private Print printing; // 상호작용 허용 대상 아이템
    private bool isInteracted = false; // 상호작용 여부

    public void Interact()
    {
        if (!isInteracted)
        {
            UiManager.Instance.Get<TalkUi>().Popup("서랍이 열렸다. 메모가 보인다.");
            SoundManager.PlaySfx(SoundCategory.Interaction, "OpenDoor");
            isInteracted = true; // 상호작용 상태 변경
        }
        else
        {
            UiManager.Instance.Get<TalkUi>().Popup("복도.실험실.문.큰 그림");
            SoundManager.PlaySfx(SoundCategory.Interaction, "Rustle");
            if (printing != null)
            {
                printing.CanInteract = true; // Print 클래스의 상호작용 가능 상태를 true로 변경
                UiManager.Instance.Get<MissionUi>().Popup("목표: 알맞은 그림을 찾아라"); // 미션 UI 팝업
            }
            else
            {
                return;
            }
        }
    }

}
