using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    // 근접 공격만 사용 - base controller에서는 animation와 관련 없다
    //[SerializeField]
    //bool _rangedSkill = false;

    // start update 없어도 creature에 있는 start update가 자동 호출
    protected override void Init()
    {
        base.Init();

        // state & dir를 바꿔주면 UpdateAnimation 
        // ObjectManger의 Add부분에서 받은 packet 정보에 따라 초기화 해주고 있다.
        // State = CreatureState.Idle;
        // Dir = MoveDir.Down;
        // _rangedSkill = (Random.Range(0, 2) == 0 ? true : false);
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle(); // 빈 코드
    }

    public override void OnDamage()
    {
        // Managers.Object.Remove(Id);
        // Managers.Resource.Destroy(gameObject);
    }

    // 시간 처리 & State 변환을 Server에서 해주고 있다.
    // 스킬을 다 사용하면 State를 바꿔주는 Packet을 S_Move를 통해서 보내줄 것이다.
    public override void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            // animation 업데이트
            State = CreatureState.Skill;
        }
    }
}
