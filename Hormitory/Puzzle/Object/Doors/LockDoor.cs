using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 더미용 잠긴 문
/// </summary>
public class LockDoor : MonoBehaviour, IInteractable
{
    //문은 열리지 않고, 잠긴 소리만 들리며, 대사만 출력.
    public void Interact()
    {
        UiManager.Instance.Get<TalkUi>().Popup("젠장, 잠겼잖아.");
        SoundManager.PlaySfx(SoundCategory.Movement, "Player1");
        SoundManager.PlaySfx(SoundCategory.Interaction, "LockDoor");
    }
}
