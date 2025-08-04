using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Print : MonoBehaviour, IInteractable
{
    bool isMention = true; // 두 가지의 대사를 번갈아 사용하기 위한 변수
    public bool CanInteract = false; // 상호작용 가능 여부
    public void Interact()
    {
        SoundManager.PlaySfx(SoundCategory.Interaction, "Rustle");
        if(!CanInteract)
        {
            UiManager.Instance.Get<TalkUi>().Popup("여인이 소를 끌어당기는 그림이다.");
            return; // 상호작용이 불가능하면 아무 것도 하지 않음
        }
        if (isMention)
        {
            UiManager.Instance.Get<TalkUi>().Popup("액자 구석에 뭔가 적혀있군...");
            isMention = false; // 다음에는 다른 대사를 사용하도록 설정
        }
        else
        {
            UiManager.Instance.Get<TalkUi>().Popup("3785? 무슨 의미지?");
            isMention = true; // 다음에는 다른 대사를 사용하도록 설정
        }
    }
}
