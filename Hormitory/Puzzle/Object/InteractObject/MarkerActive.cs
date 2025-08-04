using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerActive : MonoBehaviour
{
    // SolidUi를 가진 오브젝트 활성화
    [SerializeField] private SolidUi solidUi;

    private void Start()
    {
        // SolidUi는 항상 자녀 오브젝트로 설정
        if (solidUi == null)
        {
            solidUi = GetComponentInChildren<SolidUi>();
            solidUi.gameObject.SetActive(false); // 초기에는 비활성화
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Collider가 Player 태그를 가진 경우 SolidUi 활성화
        if (other.CompareTag("Player"))
        {
            solidUi.gameObject.SetActive(true);
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            solidUi.gameObject.SetActive(false);
        }
    }
}
