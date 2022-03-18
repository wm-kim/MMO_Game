using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    // start update 없어도 creature에 있는 start update가 자동 호출
    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {
        GetDirInput();
        base.UpdateController();
    }

    void LateUpdate()
    {
        //2d z값은 기본적으로 0 0 -10, z값을 바꾸면 화면이 안보이는 문제가 생길 수 있다
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // 키보드 입력을 받아서 방향 설정
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

            if(Input.GetKey(KeyCode.Space))
            {
                State = CreatureState.Skill;
            }
        }
    }
}
