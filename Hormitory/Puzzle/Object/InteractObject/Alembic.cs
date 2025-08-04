using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alembic : MonoBehaviour, IInteractable, ITiming
{
    private bool isInteracted = false; // 첫 상호작용 여부
    public bool isSolved = false; // 퍼즐이 해결되었는지 여부
    bool ITiming.isSolved
    {
        get => isSolved;
        set => isSolved = value;
    }

    public void Interact()
    {
        //TimingMatch
        if (!isInteracted)
        {            
            SoundManager.PlaySfx(SoundCategory.Movement, "Player2");
            UiManager.Instance.Get<TalkUi>().Popup("이건 또 뭐야?");
            isInteracted = true; // 첫 상호작용이 있었음을 표시
        }
        else if (!isSolved)
        {
            PuzzleManager.instance.GetPuzzle<TimingMatch>().SetTarget(this); // TimingMatch의 타겟 설정
            PuzzleManager.instance.GetPuzzle<TimingMatch>().StartPuzzle(); // TimingMatch 퍼즐 시작
        }
        else
        {
            UiManager.Instance.Get<TalkUi>().Popup("그냥 고물이었군.");
            SoundManager.PlaySfx(SoundCategory.Movement, "Player1");
        }

    }

}
