using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    protected Coroutine _coSkill;
    // skill이 많아지면 어떤 skill의 어떤 animation을 틀지는
    // skill관련된 datasheet를 빼서 관리 PlayerController로부터 분리
    protected bool _rangeSkill = false;

    // start update 없어도 creature에 있는 start update가 자동 호출
    protected override void Init()
    {
        // Sprite, Animator component 가져오기
        base.Init();
    }

    // Player 전용 animation
    protected override void UpdateAnimation()
    {
        // 이중 switch는 가독성이 떨어져서 if-else
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
            // 마지막으로 바라본 기준으로 skill이 나가야하므로 _dir대신 
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
       // 나중에 network을 이용하여 조종, keyboard 입력 안받음
        base.UpdateController();
    }

    protected override void UpdateIdle()
    {
        // 이동상태 확인 UpdateIdle에서 UpdateMoving으로 넘어가는 코드
        if(Dir != MoveDir.None) 
        {
            State = CreatureState.Moving;
            return;
        }
    }

    IEnumerator CoStartPunch()
    {
        // 피격 판정
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if(go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null) cc.OnDamage();
        }

        // 대기 시간
        _rangeSkill = false;
        yield return new WaitForSeconds(0.5f);
        // 0.5초마다 idle 상태로 돌아감
        State = CreatureState.Idle;
        _coSkill = null;
    }

    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        // ArrowController Init에서 moving state로 설정해줌
        ArrowController ac = go.GetComponent<ArrowController>();
        // 키보드를 안누른 상태라고 해도 이전의 마지막으로 바라보고 있던 상태로 진행
        ac.Dir = _lastDir;
        ac.CellPos = CellPos;

        // 대기 시간
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
