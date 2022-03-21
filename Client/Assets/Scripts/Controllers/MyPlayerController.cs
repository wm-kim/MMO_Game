using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MyPlayerController : PlayerController
{
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

    protected override void UpdateIdle()
    {
        // �̵����� Ȯ�� UpdateIdle���� UpdateMoving���� �Ѿ�� �ڵ�
        if (Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }

        // skill ���·� ���� Ȯ��
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            // _coSkill = StartCoroutine("CoStartPunch");
            _coSkill = StartCoroutine("CoStartShootArrow");
        }
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


}
