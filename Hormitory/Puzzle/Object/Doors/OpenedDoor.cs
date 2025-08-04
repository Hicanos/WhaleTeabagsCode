using UnityEngine;
/// <summary>
/// 이미 열려있는 문. 즉, 별도의 퍼즐이 필요하지 않은 문.
/// </summary>
public class OpenedDoor : MonoBehaviour, IInteractable
{
    private bool isOpened = false; // 문이 열려있는지 여부
    private Animator animator;// 애니메이터 컴포넌트

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Service.Log("애니메이터 컴포넌트가 없습니다");
        }
    }
    public void Interact()
    {
        if (!isOpened)
        {
            isOpened = true; // 문이 열렸음을 표시
            animator.SetBool("isOpen", true); // 애니메이터에 문이 열렸음을 전달
            SoundManager.PlaySfx(SoundCategory.Interaction, "OpenDoor"); // 문 열리는 소리 재생
        }
        else
        {
            isOpened = false; // 문이 닫혔음을 표시
            animator.SetBool("isOpen", false); // 애니메이터에 문이 닫혔음을 전달
            SoundManager.PlaySfx(SoundCategory.Interaction, "CloseDoor"); // 문 닫히는 소리 재생
        }
    }
}
