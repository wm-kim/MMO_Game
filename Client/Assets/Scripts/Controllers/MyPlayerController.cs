using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MyPlayerController : PlayerController
{
    // None ��ſ� bool�� Ȯ��  input�� ���õȰ��̹Ƿ� myplayer�� �д�
    bool _moveKeyPressed = false;

    protected override void Init()
    {
        // Sprite, Animator component ��������
        base.Init();
    }
    protected override void UpdateController()
    {
        switch (State)
        {
            // idle�϶� Ȥ�� Moving�� �� key �Է��� �޴´�.
            case CreatureState.Idle:
                // �ƹ��Է��� ���� ������ dir�� none���εǸ鼭 idle animation���� �ٲ�
                GetDirInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }

        base.UpdateController();
    }

    // UpdateController���� CreatureState.Idle�϶�
    protected override void UpdateIdle()
    {
        // �̵����� Ȯ�� UpdateIdle���� UpdateMoving���� �Ѿ�� �ڵ�
        if (_moveKeyPressed)
        {
            State = CreatureState.Moving;
            return;
        }

        // skill ���·� ���� Ȯ��
        // ��Ÿ�� ��� packet�� ������ �ʰ� cooldown�� �ִ°� ���� 
        // ó���ϴ� ��� - skill ��û �ð��� ��� ���, Coroutine�� �̿��ϴ� ���
        if (_coSkillCooltime == null && Input.GetKey(KeyCode.Space))
        {
            Debug.Log("SKill !");

            C_Skill skill = new C_Skill { Info = new SkillInfo() }; 
            // 1 punch 2 arrow 
            skill.Info.SkillId = 2; 
            // Server���� ���� �� S_SkillHandler���� UseSkill ȣ��
            Managers.Network.Send(skill);

            // State = CreatureState.Skill;
            // �������� ����� ���� �� skill ���
            // State�� UseSkill���� CoStartPunch �ڷ�ƾ�� �����Ͽ� Skill�� �ٲ��ش�.

            // _coSkill = StartCoroutine("CoStartShootArrow");
            _coSkillCooltime = StartCoroutine("CoInputCooltime", 0.2f);
        }
    }

    Coroutine _coSkillCooltime;
    IEnumerable CoInputCooltime(float time)
    {
        yield return new WaitForSeconds(time);
        _coSkillCooltime = null;
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
        _moveKeyPressed = true;

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
            _moveKeyPressed = false;
        }
    }

    // Server�� ���� ���� client���� �������� ���� ó���ϰ� �ִ�.
    protected override void MoveToNextPos()
    {
        // ���� updateflag�� ���ߴ��� Ȯ���� ���̹Ƿ� �ʿ����.
        CreatureState prevState = State;
        Vector3Int prevCellPos = CellPos;

        // ���� �ڵ����� �ذ�, ��� ������ ������ 
        // �߰��� idle animation�� ������ �ʴ´�.
        if (_moveKeyPressed == false) // �ƹ� �Էµ� ������ ������
        {
            // UpdateAnimation
            State = CreatureState.Idle;
            // idle state�� �ٲ�����Ƿ� packet�� ���� (C_Move)
            CheckUpdatedFlag();
            return;
        }

        Vector3Int destPos = CellPos;
        switch (Dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }

        // ���� ���� �ʿ� ���� dir�� Non�� �ƴϸ� ��� �����̴� animation Ʋ����
        // none���� �ٲ�°� �Է� �޴� �Լ��� �ƹ��� �Է��� ������ none���� �ٲ۴�.
        // State = CreatureState.Moving;

        // �̺κ� �������� ����ϱ� ������ ȭ������ player������ ���� ��������
        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;
                // ���� ����, �̵��� �� ��� ��� �����̴� animation�� Ʋ���ֵ���
                // ���� �̵���
                // State = CreatureState.Moving;
            }
        }

        // idle ���·� �ٲ�� movePacket�� ������.
        //if (prevState != State || CellPos != prevCellPos)
        //{
        //    C_Move movePacket = new C_Move();
        //    movePacket.PosInfo = PosInfo;
        //    Managers.Network.Send(movePacket);
        //}

        // �ϴ� cellPos�� �ٲ�����Ƿ� C_Move packet�� ���ش�.
        // dir�� �ٲ� ������ �Է¹޴� �κп��� packet�� ���ָ� ��Ÿ�� ������ ������ �δ��� ����.
        // dirty flag�� check���ؼ� �δ��� ����.
        CheckUpdatedFlag();
    }

    protected override void CheckUpdatedFlag()
    {
        if(_updated)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
            _updated = false;
        }
    }
}
