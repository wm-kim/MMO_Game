using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    protected Coroutine _coSkill;
    // skill�� �������� � skill�� � animation�� Ʋ����
    // skill���õ� datasheet�� ���� ���� PlayerController�κ��� �и�
    protected bool _rangeSkill = false;

    // start update ��� creature�� �ִ� start update�� �ڵ� ȣ��
    protected override void Init()
    {
        // Sprite, Animator component ��������
        base.Init();
    }

    // Player ���� animation
    protected override void UpdateAnimation()
    {
        if (_animator == null && _sprite == null) return;
        // ��¥�� ���߿� init�ϴ� �ڵ忡�� UpdateAnimation�� �ٽ� �� ����

        // ���� switch�� �������� �������� if-else
        if (State == CreatureState.Idle)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    // transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = true;

                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (State == CreatureState.Skill)
        {
            // ���������� �ٶ� �������� skill�� �������ϹǷ� _dir��� 
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_BACK" : "ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_FRONT" : "ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                    _sprite.flipX = true;

                    break;
                case MoveDir.Right:
                    _animator.Play(_rangeSkill ? "ATTACK_WEAPON_RIGHT" : "ATTACK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else
        {
            // DEAD?
        }
    }

    protected override void UpdateController()
    {
       // ���߿� network�� �̿��Ͽ� ����, keyboard �Է� �ȹ���
        base.UpdateController();
    }

    public void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            _coSkill = StartCoroutine("CoStartPunch");
        }
    }

    protected virtual void CheckUpdatedFlag() { }

    IEnumerator CoStartPunch()
    {
        // �ǰ� ���� - Server ����
        //GameObject go = Managers.Object.Find(GetFrontCellPos());
        //if(go != null)
        //{
        //    CreatureController cc = go.GetComponent<CreatureController>();
        //    if (cc != null) cc.OnDamage();
        //}

        // ��� �ð�
        _rangeSkill = false;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.5f);
        // 0.5�ʸ��� idle ���·� ���ư�
        State = CreatureState.Idle;
        _coSkill = null;
        // C_Move packet ����, Server�ʿ� idle ���·� ���ư���� �˷���
        CheckUpdatedFlag();
    }

    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        // ArrowController Init���� moving state�� ��������
        ArrowController ac = go.GetComponent<ArrowController>();
        // Ű���带 �ȴ��� ���¶�� �ص� ������ ���������� �ٶ󺸰� �ִ� ���·� ����
        ac.Dir = Dir;
        ac.CellPos = CellPos;

        // ��� �ð�
        _rangeSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
    }

    public override void OnDamage()
    {
        Debug.Log("Player HIT!");
    }
}
