using UnityEngine;

public class DoorTiming : MonoBehaviour, IInteractable, ITiming
{
    private bool isInteracted = false; // 첫 상호작용 여부
    public bool isSolved = false; // 퍼즐이 해결되었는지 여부
    private bool isOpened = false; // 문이 열렸는지 여부
    private Animator dooranimator; // 문 애니메이터

    bool ITiming.isSolved
    {
        get => isSolved;
        set => isSolved = value;
    }

    private void Start()
    {
        if (dooranimator == null)
        {
            dooranimator = GetComponent<Animator>();
            if (dooranimator == null)
            {
                // 예외처리
                return;
            }
        }
    }

    public void Interact()
    {
        if (isInteracted == false)
        {
            isInteracted = true;
            SoundManager.PlaySfx(SoundCategory.Interaction, "LockDoor");
            UiManager.Instance.Get<TalkUi>().Popup("고장났나? 잘 돌리면 열릴 것 같은데.");
            UiManager.Instance.Get<MissionUi>().Popup("목표: 잠긴 문을 열어라."); // 미션 UI 팝업
        }
        else if (isSolved == false)
        {
            PuzzleManager.instance.GetPuzzle<TimingMatch>().SetTarget(this); // TimingMatch의 타겟 설정
            PuzzleManager.instance.GetPuzzle<TimingMatch>().StartPuzzle(); // TimingMatch 퍼즐 시작
        }
        else
        {
            if (isOpened)
            {
                dooranimator.SetBool("isOpen", false); // 문을 닫음
                isOpened = false; // 문이 닫힌 상태로 변경
                SoundManager.PlaySfx(SoundCategory.Interaction, "CloseDoor");
            }
            else
            {
                dooranimator.SetBool("isOpen", true); // 문을 엶
                isOpened = true; // 문이 열린 상태로 변경
                SoundManager.PlaySfx(SoundCategory.Interaction, "OpenDoor");
            }
        }
        
    }
}
