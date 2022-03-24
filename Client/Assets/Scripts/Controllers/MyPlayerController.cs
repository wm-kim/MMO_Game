using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MyPlayerController : PlayerController
{
    // None 대신에 bool로 확인  input과 관련된것이므로 myplayer에 둔다
    bool _moveKeyPressed = false;

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

    // UpdateController에서 CreatureState.Idle일때
    protected override void UpdateIdle()
    {
        // 이동상태 확인 UpdateIdle에서 UpdateMoving으로 넘어가는 코드
        if (_moveKeyPressed)
        {
            State = CreatureState.Moving;
            return;
        }

        // skill 상태로 갈지 확인
        // 연타시 계속 packet을 보내지 않고 cooldown을 주는게 좋음 
        // 처리하는 방식 - skill 요청 시간을 재는 방법, Coroutine을 이용하는 방법
        if (_coSkillCooltime == null && Input.GetKey(KeyCode.Space))
        {
            Debug.Log("SKill !");

            C_Skill skill = new C_Skill { Info = new SkillInfo() }; 
            // 1 punch 2 arrow 
            skill.Info.SkillId = 2; 
            // Server에서 검증 후 S_SkillHandler에서 UseSkill 호출
            Managers.Network.Send(skill);

            // State = CreatureState.Skill;
            // 서버쪽의 허락을 받은 후 skill 사용
            // State는 UseSkill에서 CoStartPunch 코루틴을 실행하여 Skill로 바꿔준다.

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
        //2d z값은 기본적으로 0 0 -10, z값을 바꾸면 화면이 안보이는 문제가 생길 수 있다
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // 키보드 입력을 받아서 방향 설정
    // skill이 나가는 동안에는 입력을 받을 수 없게 할것
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

    // Server의 승인 없이 client에서 움직임을 먼저 처리하고 있다.
    protected override void MoveToNextPos()
    {
        // 이제 updateflag로 변했는지 확인할 것이므로 필요없다.
        CreatureState prevState = State;
        Vector3Int prevCellPos = CellPos;

        // 버그 자동으로 해결, 계속 누르고 있으면 
        // 중간에 idle animation이 나오지 않는다.
        if (_moveKeyPressed == false) // 아무 입력도 누르지 않으면
        {
            // UpdateAnimation
            State = CreatureState.Idle;
            // idle state로 바뀌었으므로 packet을 쏴줌 (C_Move)
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

        // 이제 굳이 필요 없음 dir이 Non이 아니면 계속 움직이는 animation 틀어줌
        // none으로 바뀌는건 입력 받는 함수가 아무런 입력이 없을때 none으로 바꾼다.
        // State = CreatureState.Moving;

        // 이부분 공용으로 사용하기 어려운게 화살인지 player인지에 따라 나뉘었음
        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;
                // 버그 수정, 이동할 수 없어도 계속 움직이는 animation을 틀어주도록
                // 위로 이동함
                // State = CreatureState.Moving;
            }
        }

        // idle 상태로 바뀌여도 movePacket을 보낸다.
        //if (prevState != State || CellPos != prevCellPos)
        //{
        //    C_Move movePacket = new C_Move();
        //    movePacket.PosInfo = PosInfo;
        //    Managers.Network.Send(movePacket);
        //}

        // 일단 cellPos가 바뀌었으므로 C_Move packet을 쏴준다.
        // dir이 바뀔 때마다 입력받는 부분에서 packet을 쏴주면 연타를 날릴때 서버에 부담이 간다.
        // dirty flag로 check를해서 부담을 던다.
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
