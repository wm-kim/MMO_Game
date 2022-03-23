using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : CreatureController
{
    protected override void Init()
    {
        // 화살 그림 방향 수정
        switch(Dir)
        {
            case MoveDir.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case MoveDir.Down:
                transform.rotation = Quaternion.Euler(0, 0, -180);
                break;
            case MoveDir.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case MoveDir.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;

        }

        State = CreatureState.Moving;
        _speed = 15.0f;

        base.Init();
    }

    protected override void UpdateAnimation() { }

    // state가 idle인 상태에서 UpdateController에서 호출 (update)
    // PlayerController의 경우 input을 받지 않으면
    // GetDirInput에서 dir이 자동으로 none이 되지만 여기서는 그렇지 않음
    // UpdateIdle -> MoveToNextPos
    protected override void MoveToNextPos()
    {
        // 여기서 _dir != MoveDir.None 확인해도 달라지는건 없음
        
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

        State = CreatureState.Moving; // 자동으로 UpdateMoving() 호출
        // 이제 이부분도 굳이 안넣어줘도 됨
        // UpdateController base에서 UpdateIdle호출해서 이동상태로 넘어갈 것

        if (Managers.Map.CanGo(destPos))
        {
            GameObject go = Managers.Object.Find(destPos);
            if (go == null)
            {
                CellPos = destPos;
            }
            else 
            {
                // 갈 수 있긴한데 화살의 피격 판정 object(monster)와 충돌
                CreatureController cc = go.GetComponent<CreatureController>();
                if (cc != null) cc.OnDamage();

                Managers.Resource.Destroy(gameObject);
            }
        }
        else
        {
            Managers.Resource.Destroy(gameObject);
        }
    } 
}
