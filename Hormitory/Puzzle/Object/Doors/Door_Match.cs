using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 숫자 맞추기 퍼즐이 있는 문에 달린 스크립트
/// CountMatchSO에서 데이터를 받아옴.
/// </summary>
public class Door_Match : MonoBehaviour, IInteractable
{
    public CountMatchSO MatchData;
    public int MatchCount;

    private Animator animator;

    private bool isInteracted = false; // 첫 상호작용이 있었는지 여부

    public bool isSolved = false; // 퍼즐이 해결되었는지 여부
    private bool isOpened = false; // 문이 열려있는지 여부

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        MatchCount = MatchData.RequiredNum;
        // 1. 해당 오브젝트의 MatchData를 가져온다 =>할당
        // 2. CountMatch의 StartPuzzle을 시작시킨다.
        if (isSolved == false && isInteracted == false)
        {
            if (UiManager.Instance.Get<TalkUi>().onTalk) return;
            //즉, 첫 상호작용(문이 잠겼음을 확인)
            UiManager.Instance.Get<TalkUi>().Popup("젠장, 좌물쇠가 걸려있잖아!");
            SoundManager.PlaySfx(SoundCategory.Interaction, "LockDoor");
            UiManager.Instance.Get<MissionUi>().Popup("목표: 잠긴 문의 암호를 찾아라"); // 미션 UI 팝업
            isInteracted = true; // 첫 상호작용이 있었음을 표시
        }
        else if (!isSolved)
        {
            if (UiManager.Instance.Get<TalkUi>().onTalk) return;
            UiManager.Instance.Show<LockUi>(true);
            PuzzleManager.instance.GetPuzzle<CountMatchController>().SetTargetDoor(this);
            PuzzleManager.instance.GetPuzzle<CountMatchController>().SetRequiredNum(MatchCount);
            //퍼즐이 시작되고 해결되면, 퍼즐이 해결됨.
        }
        else if (isSolved)
        {
            if (isOpened)
            {
                animator.SetBool("isOpen", false); // 문을 닫음
                isOpened = false; // 문이 닫힌 상태로 변경
                SoundManager.PlaySfx(SoundCategory.Interaction, "CloseDoor");
            }
            else
            {
                animator.SetBool("isOpen", true); // 문을 엶
                isOpened = true; // 문이 열린 상태로 변경
                SoundManager.PlaySfx(SoundCategory.Interaction, "OpenDoor");
            }
        }
        
    }

}
