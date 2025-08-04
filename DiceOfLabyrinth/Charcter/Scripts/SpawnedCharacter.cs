using UnityEngine;
/// <summary>
/// 스폰된 캐릭터의 애니메이션 등을 관리
/// </summary>
public class SpawnedCharacter : MonoBehaviour
{
    public Animator Animator { get; private set; } // 캐릭터의 애니메이터 컴포넌트
    [SerializeField] string CharacterID; // 스폰된 캐릭터의 고유 ID
    [SerializeField] AudioClip AttackSFX; // 캐릭터의 오디오 클립
    [SerializeField] AudioClip SkillSFX; // 캐릭터의 스킬 오디오 클립

    private void Awake()
    {
        // 애니메이터 컴포턴트 초기화, 자녀오브젝트에 애니메이터가 있음
        Animator = GetComponentInChildren<Animator>();
        if (Animator == null)
        {
            Debug.LogError("스폰 캐릭터 오브젝트에 애니메이터가 없습니다.");
        }
    }

    public void SetCharacterID(string characterID)
    {
        CharacterID = characterID; // 캐릭터 ID 설정
    }

    public void IsIdle()
    {
        // 모든 애니메이션 boolean을 false로 설정
        Animator.SetBool("AttackIdle", false);
        Animator.SetBool("IsDied", false);
        Animator.SetBool("Victory", false);
        Animator.SetBool("Defeat", false);
        Animator.SetBool("IsRun", false);
    }

    public void Run()
    {
        Animator.SetBool("IsRun", true); // 달리기 애니메이션 설정
    }

    public void PrepareAttack()
    {        
        Animator.SetBool("AttackIdle", true); // 공격 준비 애니메이션 설정
    }

    public void Attack()
    {
        Animator.SetBool("AttackIdle", false); // 공격 준비 애니메이션 해제
        Animator.SetTrigger("Attack"); // 공격 애니메이션 트리거 설정
        SoundManager.Instance.PlaySFX(AttackSFX);
    }

    public void SkillAttack()
    {
        // 스킬 사용 시 애니메이션 조정
        Attack(); // 기본 공격 애니메이션
        SoundManager.Instance.PlaySFX(SkillSFX); // 스킬 사운드 재생
    }

    public void GetDamage()
    {        
        Animator.SetTrigger("HasAttacked");
    }

    public void Die()
    {
        Animator.SetBool("IsDied", true); // 죽음 애니메이션 설정
    }
    public void Revive()
    {
        Animator.SetBool("IsDied", false); // 죽음 애니메이션 해제
        Animator.SetBool("IsIdle", true); // 대기 애니메이션 설정
    }

    public void Victory()
    {
        Animator.SetBool("Victory", true); // 승리 애니메이션 설정
    }
    public void Defeat()
    {
        Animator.SetBool("Defeat", true); // 패배 애니메이션 설정
    }
}
