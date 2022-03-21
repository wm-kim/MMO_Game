using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine _coSkill;
    Coroutine _coPatrol;
    Coroutine _coSearch;

    [SerializeField]
    Vector3Int _destCellPos;

    [SerializeField]
    GameObject _target; // 내가 추적하고 있는 대상 (player) 1초마다 search

    [SerializeField]
    float _searchRange = 10.0f;

    [SerializeField]
    float _skillRange = 1.0f;

    [SerializeField]
    bool _rangedSkill = false;

    public override CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            base.State = value;

            if (_coPatrol != null)
            {
                StopCoroutine(_coPatrol);
                _coPatrol = null;
            }

            if (_coSearch != null)
            {
                StopCoroutine(_coSearch);
                _coSearch = null;
            }
        }
    }

    // start update 없어도 creature에 있는 start update가 자동 호출
    protected override void Init()
    {
        base.Init();
        // state & dir를 바꿔주면 UpdateAnimation 
        State = CreatureState.Idle;
        Dir = MoveDir.None;
        _speed = 3.0f;
        _rangedSkill = (Random.Range(0, 2) == 0 ? true : false);

        if (_rangedSkill) _skillRange = 10.0f;
        else _skillRange = 1.0f;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle(); // 빈 코드

        // Idle로 상태가 바뀌었으면 매번마다 다시 호출됨
        // patrol
        if(_coPatrol == null)
        {
            // moving State 로 바꾸어줌 
            _coPatrol = StartCoroutine("CoPatrol");
        }

        // 몬스터가 이동하는 도중에 player를 찾으면 player를 향해이동
        if (_coSearch == null)
        {
            _coSearch = StartCoroutine("CoSearch");
        }

    }

    // CreatureController의 Update에서 호출됨
    protected override void UpdateController()
    {
        base.UpdateController();
    }

    protected override void MoveToNextPos()
    {
        Vector3Int destPos = _destCellPos;
        if(_target != null)
        {
            destPos = _target.GetComponent<CreatureController>().CellPos;

            Vector3Int dir = destPos - CellPos;
            if(dir.magnitude <= _skillRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill; // skill animation 재생

                if(_rangedSkill)
                    _coSkill = StartCoroutine("CoStartShootArrow");
                else
                    _coSkill = StartCoroutine("CoStartPunch");
                return;
            }
        }

        // ignoreDestCollision, destPos에 어떤 물체가 있더라도 충돌로 인식하지 않고 길을 만들어줌
        // 가독성 측면에서 ignoreDestCollision: true
        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);

        // 1은 제자리에 있겠다라는 것, 2보다 작은것이면 길을 못찾은 것
        // 혹은 플레이어가 너무 멀리 떨어져 있는 경우
        // 너무 멀리 있을 때는 target이 있을 경우에만 return
        if (path.Count < 2 || (_target != null && path.Count > 20))
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        // path[0]이 현재 위치. 계속 상태가 변하기 때문에 매번 계산을 해야하는건 어쩔 수 없다.
        // optimization 너무 멀어지면 FindPath 계산을 하지 않는다.
        // 작은 배열만 만든다면 FindPath에서 closed를 2차배열로 만드는것보다 list로 관리해도 된다.
        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos;

        // UpdateAnimation
        Dir = GetDirFromVec(moveCellDir);

        // 좌표를 이동하는 부분인데 A star를 이용하여 찾아줌
        //Vector3Int destPos = CellPos;
        //switch (_dir)
        //{
        //    case MoveDir.Up:
        //        destPos += Vector3Int.up;
        //        break;
        //    case MoveDir.Down:
        //        destPos += Vector3Int.down;
        //        break;
        //    case MoveDir.Left:
        //        destPos += Vector3Int.left;
        //        break;
        //    case MoveDir.Right:
        //        destPos += Vector3Int.right;
        //        break;
        //}

        // Object.Find에 자기 자신도 포함
        if (Managers.Map.CanGo(nextPos) && Managers.Object.Find(nextPos) == null)
        {
            CellPos = nextPos;
        }
        else // 막혀 있거나 (다른 몬스터나 플레이어에 의해 막힐 수 있음), 이동완료 했을 때
        {
            // 바로 멈춘다. UpdateAnimation 호출
            // _coPatrol를 null로 밀어준다.
            State = CreatureState.Idle;
        }
    }


    public override void OnDamage()
    {
        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = transform.position;
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);

        Managers.Object.Remove(Id);
        Managers.Resource.Destroy(gameObject);
    }

    IEnumerator CoPatrol()
    {
        int waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        // 랜덤한 위치에 몬스터를 이동 실제 갈 수 있는 위치인지 10번만 try
        for(int i = 0; i < 10; i++)
        {
            int xRange = Random.Range(-5, 6);
            int yRange = Random.Range(-5, 6);
            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            if(Managers.Map.CanGo(randPos) && Managers.Object.Find(randPos) == null)
            {
                _destCellPos = randPos;
                State = CreatureState.Moving;
                yield break; // coroutine을 완전히 빠져나옴 
            }
        }

        State = CreatureState.Idle;
        // state가 바뀌면 자동으로 _coPatrol를 null로 밀어준다.
    }

    // 플레이어에서 실시간으로 이동하기 때문에 1초마다 search 
    IEnumerator CoSearch()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);

            if (_target != null) continue;

            // target 찾기
            _target = Managers.Object.Find((go) =>
            {
                PlayerController pc = go.GetComponent<PlayerController>();
                if (pc == null) return false;

                Vector3Int dir = (pc.CellPos - CellPos);
                if (dir.magnitude > _searchRange)
                    return false;

                return true;
            });
        }
    }

    IEnumerator CoStartPunch()
    {
        // 피격 판정
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null) cc.OnDamage();
        }

        // 대기 시간
        yield return new WaitForSeconds(0.5f);
        // 0.5초마다 idle 상태로 돌아감
        // idle 상태로 돌아가서 다시 patrol 상태로 간다음
        // moving update에서 skill을 쓸때까지 지연시간이 걸린다
        // State = CreatureState.Idle;

        State = CreatureState.Moving; // idle 대기 시간을 없애기 위해서
        _coSkill = null;
    }

    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        // 키보드를 안누른 상태라고 해도 이전의 마지막으로 바라보고 있던 상태로 진행
        ac.Dir = _lastDir;
        ac.CellPos = CellPos;

        // 대기 시간
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Moving; // idle 대기 시간을 없애기 위해서
        _coSkill = null;
    }
}
