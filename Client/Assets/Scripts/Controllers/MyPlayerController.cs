using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MyPlayerController : PlayerController
{
    protected override void Init()
    {
        // Sprite, Animator component 가져오기
        base.Init();
    }
    protected override void UpdateController()
    {
        switch (State)
        {
            // idle일때 혹은 Moving일 때 key 입력을 받는다.
            case CreatureState.Idle:
                // 아무입력을 받지 않으면 dir이 none으로되면서 idle animation으로 바뀜
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
        // 이동상태 확인 UpdateIdle에서 UpdateMoving으로 넘어가는 코드
        if (Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }

        // skill 상태로 갈지 확인
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            // _coSkill = StartCoroutine("CoStartPunch");
            _coSkill = StartCoroutine("CoStartShootArrow");
        }
    }

    void LateUpdate()
    {
        //2d z값은 기본적으로 0 0 -10, z값을 바꾸면 화면이 안보이는 문제가 생길 수 있다
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // 키보드 입력을 받아서 방향 설정
    // skill이 나가는 동안에는 입력을 받을 수 없게 할것
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
