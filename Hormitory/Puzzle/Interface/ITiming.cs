using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITiming
{
   // 이걸 가진 오브젝트는 TimingMatch의 Target으로 사용됨
   bool isSolved { get; set; } // 퍼즐이 해결되었는지 여부
}
