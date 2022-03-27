using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    // ���� ���ݸ� ��� - base controller������ animation�� ���� ����
    //[SerializeField]
    //bool _rangedSkill = false;

    // start update ��� creature�� �ִ� start update�� �ڵ� ȣ��
    protected override void Init()
    {
        base.Init();

        // state & dir�� �ٲ��ָ� UpdateAnimation 
        // ObjectManger�� Add�κп��� ���� packet ������ ���� �ʱ�ȭ ���ְ� �ִ�.
        // State = CreatureState.Idle;
        // Dir = MoveDir.Down;
        // _rangedSkill = (Random.Range(0, 2) == 0 ? true : false);
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle(); // �� �ڵ�
    }

    public override void OnDamage()
    {
        // Managers.Object.Remove(Id);
        // Managers.Resource.Destroy(gameObject);
    }

    // �ð� ó�� & State ��ȯ�� Server���� ���ְ� �ִ�.
    // ��ų�� �� ����ϸ� State�� �ٲ��ִ� Packet�� S_Move�� ���ؼ� ������ ���̴�.
    public override void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            // animation ������Ʈ
            State = CreatureState.Skill;
        }
    }
}
