using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    // start update 없어도 creature에 있는 start update가 자동 호출
    protected override void Init()
    {
        base.Init();
        // state & dir를 바꿔주면 animation 재생
        State = CreatureState.Idle;
        Dir = MoveDir.None;
    }

    // CreatureController의 Update에서 호출됨
    protected override void UpdateController()
    {
        base.UpdateController();
    }

    public override void OnDamage()
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);

        Managers.Object.Remove(gameObject);
        Managers.Resource.Destroy(gameObject);
    }
}
