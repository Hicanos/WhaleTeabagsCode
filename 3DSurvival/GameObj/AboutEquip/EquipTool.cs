using UnityEngine;

public class EquipTool : Equip
{
    // 캐릭터가 도구를 사용할 때 액션 등의 처리
    // 도구에는 Collider를 이용, IsTrigger Enter가 되면 공격 혹은 자원 채집이 됨 (Layer = Resorce면 자원이, Layer = Enemy면 공격이 됨)
    // 도구에 대한 내용은 EquipItem에서 설정하고 이곳에 받아옴

    [SerializeField] private EquipItem equipData; // 도구 아이템 데이터
    private PlayerController controller; // 플레이어 컨트롤러 참조, 컨트롤러의 애니메이션이 진행되는 것= 공격임
    // transform을 참조할 Player 불러오기
    private Player player;
    private Ray ray; // 도구가 장착된 오브젝트의 위치에서 시작하는 Ray

    private void Awake()
    {
        controller = GetComponentInParent<PlayerController>();
        player = GetComponentInParent<Player>(); // 플레이어 컴포넌트 참조
    }

    private void Start()
    {
        //어차피 상호작용 중인 EquipTool = 해당 스크립트가 붙은 오브젝트임
        // 도구 아이템 데이터가 반드시 설정되어 있으므로, 해당 아이템 Data에서 Distance에 따른 Raycast를 사용하여 자원/적을 탐지
        if (equipData == null)
        {
            Debug.LogError("EquipTool: equipData is not set!");
            return;
        }

    }
    public override void OnAttackInput()
    {
        CheckTarget();
        
    }

    //공격 시 자원/적을 탐지하는 메서드
    //콜라이더랑 부딪히면 - 자원 채집 혹은 공격
    //Layer = Resource면 자원 채집, Layer = Enemy면 공격
    //공격 키를 눌렀을 때 캐릭터의 전방으로 Raycast 혹은 BoxCast를 쏴서 전방에 자원/적이 있는지 확인 ->있다면 자원을 얻거나 적을 공격
    //Raycast를 구하는 메서드를 생성

    private void CheckTarget()
    {
        //Raycast를 통해 탐지한 자원/ 적에 대한 처리
        //Ray는 해당 Tool이 장착된 오브젝트의 위치에서 시작. 해당 오브젝트의 전방 방향으로 Ray를 쏘기
        // ray의 origin을 Player의 위치로 설정
        ray = new Ray(player.transform.position + Vector3.up * 1.5f, player.transform.forward); // 플레이어의 위치에서 시작, 위치를 약간 위로
        RaycastHit hit;

        // 도구의 공격 거리 내에 자원이나 적이 있는지 확인
        if (Physics.Raycast(ray, out hit, equipData.attackDistance))
        {
            // 자원 채집
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Resource") && equipData.canGathering)
            {
                Debug.Log("자원 채집: " + hit.collider.name);
                ResourceObject resource = hit.collider.GetComponent<ResourceObject>();
                if (resource != null)
                {
                    // 자원 획득
                    resource.Gather(hit.point, hit.normal); // 자원 채집 메서드 호출
                }
            }
            // 적 공격
            else if (hit.collider.CompareTag("Enemy") && equipData.attackPower > 0)
            {
                Debug.Log("공격: " + hit.collider.name);
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // 적에게 피해를 입힘
                    enemy.TakeDamage(equipData.attackPower);
                }
            }
        }
    }

}
