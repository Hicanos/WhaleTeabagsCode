using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : ItemReceiver, IInteractable
{
    // ExitDoor는 Battle씬과 연결됨
    // 열기 전 아이템 매치 해야함

    public override void InteractAction()
    {
        // ExitDoor 상호작용 액션
        // 아이템이 일치하면 Exit 씬으로 전환
        if (isCleard)
        {
            if (UiManager.Instance.Get<TalkUi>().onTalk) return;
            UiManager.Instance.Get<TalkUi>().Popup("좋았어, 열렸다!");
            SoundManager.PlaySfx(SoundCategory.Interaction, "UnLockDoor");
            UiManager.Instance.Get<MissionUi>().Popup("목표 : 밖으로 탈출하기");
        }
        else
        {
            if (UiManager.Instance.Get<TalkUi>().onTalk) return;
            UiManager.Instance.Get<TalkUi>().Popup("문을 열 수 있는 물건이 필요해.");
            SoundManager.PlaySfx(SoundCategory.Interaction, "LockDoor");
        }
    }

    public override void AfterInteract()
    {
        // 여기서 Battle씬으로 전환
        // 여긴 IsCleard가 true일 때만 호출됨
        SoundManager.PlaySfx(SoundCategory.Interaction, "OpenDoor");
        UiManager.Instance.Get<FadeUi>().OnFade(FadeEvent);
    }

    private void FadeEvent()
    {
        SceneManager.LoadScene("Battle");
        UiManager.Instance.Get<FadeUi>().OnFade();
    }
}
