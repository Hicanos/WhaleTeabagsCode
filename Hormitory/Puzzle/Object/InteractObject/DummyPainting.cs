using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPainting : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        SoundManager.PlaySfx(SoundCategory.Interaction, "Rustle");
        UiManager.Instance.Get<TalkUi>().Popup("이 그림에 특이한 점은 안 보이는데...");
    }
}
