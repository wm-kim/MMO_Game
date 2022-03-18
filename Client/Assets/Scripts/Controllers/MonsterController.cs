using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    // start update ��� creature�� �ִ� start update�� �ڵ� ȣ��
    protected override void Init()
    {
        base.Init();
        // state & dir�� �ٲ��ָ� animation ���
        State = CreatureState.Idle;
        Dir = MoveDir.None;
    }

    // CreatureController�� Update���� ȣ���
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
