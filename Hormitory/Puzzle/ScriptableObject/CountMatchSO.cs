using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 숫자 맞추기 퍼즐로 통하는 오브젝트들이 달고 있는 스크립트.
/// RequireNumber를 가지고 있음(정답 매칭용)
/// </summary>

public interface IRequireNumber
{
    int RequiredNum { get; }
}

[CreateAssetMenu(fileName ="MatchObj",menuName ="PuzzleObj/CountMatch")]
public class CountMatchSO : ScriptableObject, IRequireNumber
{
    //해당 오브젝트로 호출한 퍼즐에서 요구하는 정답 코드 (4자리)
    [SerializeField] private int requiredNum;
    public int RequiredNum => requiredNum;
}
