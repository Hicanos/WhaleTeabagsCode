using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceControl : MonoBehaviour
{
    //ResourceControl은 ResourceObject를 관리하는 스크립트
    //ResourceObject의 자원이 0이 되었을 때, 일정 시간이 지나면 다시 활성화 시켜야 함

    public static ResourceControl Instance { get; private set; } // 싱글톤 인스턴스
    private void Awake()
    {
        Instance = this;
    }

    public void RequestRespawn(ResourceObject resource, float delay)
    {
        // 자원 오브젝트를 비활성화하고 일정 시간 후에 다시 활성화
        StartCoroutine(RespawnCoroutine(resource, delay));
    }

    IEnumerator RespawnCoroutine(ResourceObject resource, float delay)
    {
        //시간 측정 디버그

        yield return new WaitForSeconds(delay);

        // 리스폰 Area는 ResourceObject의 부모 빈 오브젝트에 box콜라이더로 설정되어있음
        ResourceArea resourceArea = resource.GetComponentInParent<ResourceArea>();
        while (resourceArea != null && resourceArea.IsBlocked)
        {
            //지역이 차단되어 있다면 시간 지연을 함 (테스트용으로 1초씩 추가 체크, 벗어나면 원래 지연 시간만큼 기다릴 예정.)
            yield return new WaitForSeconds(1f);
            if (resourceArea.IsBlocked == false)
            {
                yield return new WaitForSeconds(delay); // 지역이 차단되지 않았다면 원래 지연 시간만큼 기다림. 중간에 또 차단되면 다시 10초씩 기다림
            }

        }

        resource.Initialize(); // 자원 초기화
        resource.gameObject.SetActive(true); // 자원 오브젝트 활성화

    }

    // 여기서 만약 IsTrigger 콜라이더에 Tag= Player나 Enemy가 있다면, resource.gameObject.SetActive를 true로 하지 않고 추가 지연을 시킴
    // => 별도의 스크립트로 빼기

}
