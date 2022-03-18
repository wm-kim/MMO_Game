using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    Coroutine _coSkill;
    // skill�� �������� � skill�� � animation�� Ʋ����
    // skill���õ� datasheet�� ���� ���� PlayerController�κ��� �и�
    bool _rangeSkill = false;

    // start update ��� creature�� �ִ� start update�� �ڵ� ȣ��
    protected override void Init()
    {
        base.Init();
    }

    // Player ���� animation
    protected override void UpdateAnimation()
    {
        // ���� switch�� �������� �������� if-else
        if (_state == CreatureState.Idle)
        {
            switch (_lastDir)
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
        else if (_state == CreatureState.Moving)
        {
            switch (_dir)
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
                case MoveDir.None:
                    break;
            }
        }
        else if (_state == CreatureState.Skill)
        {
            // ���������� �ٶ� �������� skill�� �������ϹǷ� _dir��� 
            switch (_lastDir)
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
                case MoveDir.None:
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
        switch (State)
        {
            // idle�϶� Ȥ�� Moving�� �� key �Է��� �޴´�.
            case CreatureState.Idle:
                GetDirInput();
                GetIdleInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;

        }

        base.UpdateController();
    }

    void LateUpdate()
    {
        //2d z���� �⺻������ 0 0 -10, z���� �ٲٸ� ȭ���� �Ⱥ��̴� ������ ���� �� �ִ�
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // Ű���� �Է��� �޾Ƽ� ���� ����
    // skill�� ������ ���ȿ��� �Է��� ���� �� ���� �Ұ�
    void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.Up;

        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }

    // ���߿� skill�� �������� skill class�� ���� �ļ� ���������� ����
    // ������ playController�� ����ϴ� ����

    // anim �ٲ��ش�.
    private void GetIdleInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            // _coSkill = StartCoroutine("CoStartPunch");
            _coSkill = StartCoroutine("CoStartShootArrow");
        }
    }

    IEnumerator CoStartPunch()
    {
        // �ǰ� ����
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if(go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null) cc.OnDamage();
        }

        // ��� �ð�
        _rangeSkill = false;
        yield return new WaitForSeconds(0.5f);
        // 0.5�ʸ��� idle ���·� ���ư�
        State = CreatureState.Idle;
        _coSkill = null;
    }

    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        // Ű���带 �ȴ��� ���¶�� �ص� ������ ���������� �ٶ󺸰� �ִ� ���·� ����
        ac.Dir = _lastDir;
        ac.CellPos = CellPos;

        // ��� �ð�
        _rangeSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _coSkill = null;
    }
}
