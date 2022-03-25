using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    public int Id { get; set; }

    [SerializeField]
    public float _speed = 5.0f;

    // dirty flag : dir, state, cellpos 셋중하나에 변경사항 추적
    // my player만 check할것이므로 creature controller에 넣는것은 좀 애매하긴하다.
    protected bool _updated = false;

    PositionInfo _positionInfo = new PositionInfo();

    // Init에서 초기값이 설정된다.
    // UpdateIdle에서 dir에 따라 moving State로 바뀌거나
    // MoveToNextPos에서 입력을 받지 않을 경우 Dir이 non이되면서 Idle state로 바뀐다.
    public PositionInfo PosInfo 
    { 
        get { return _positionInfo; } 
        set
        {
            // 굳이 필요없음
            if (_positionInfo.Equals(value)) return;

            // _positionInfo = value;
            // _Lastdir도 같이 업데이트 받기 위해서 직접적으로 값을 넣어주지 않고
            // 하나씩 업데이트
            CellPos = new Vector3Int(value.PosX, value.PosY, 0);
            State = value.State;
            Dir = value.MoveDir;
            // UpdateAnimation();
        }
    }

    // cellpos와 transform 맞춰주는 함수
    public void SyncPos()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = destPos;
    }

    public Vector3Int CellPos 
    { 
        get
        {
            return new Vector3Int(PosInfo.PosX, PosInfo.PosY, 0);
        }
        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y) return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            _updated = true;
        }
    }  

    protected Animator _animator;
    protected SpriteRenderer _sprite;

    // 움직이지 못할 때 스킬 시전 가능하게 할거임
    // boolean을 늘리지 않고 state로 관리
    // protected bool _isMoving = false;

    // idle상태이고 dir이 non이 아닌 상태에서 UpdateIdle이 호출되면 바로 moving state로 바로바뀜
    // protected CreatureState _state = CreatureState.Idle;

    public virtual CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
            // State & dir 양쪽 상태중 어느 것이 하나가 바뀌면 호출
            UpdateAnimation(); 
            _updated = true;
        }
    }

    // 마지막으로 바라보고 있던 방향
    // 꼼수 none이면 다 건너띄어서 idle animation이 안된다.
    // 초반 접속했을 때 바라보는 방향이 된다. 이제는 _lastdir 사용안함
    // protected MoveDir _lastDir = MoveDir.Down;

    // 게임을 처음실행할 때 움직이는 것을 막기 위해 초기값을 아무거나 설정
    // UpdateAnimation이 호출이 안되고 초반 설정값인 오른쪽 달리기를 재생한다.
    // protected MoveDir _dir = MoveDir.Down;
    
    public MoveDir Dir
    {
        get { return PosInfo.MoveDir; }
        set
        {
            if (PosInfo.MoveDir == value) return;
            // 맨 마지막에 이동했던 방향으로 idle animation
            // dir를 선택할 때 바꾸는 것이 맞는지 아니면 State에서 animation을 관리하는게 맞는지 
            // 애매할때는 공용 부분에 놓고 따로 빼서 관리하는 게 좋다. -> UpdateAnimation
            PosInfo.MoveDir = value;

            UpdateAnimation();
            _updated = true;
        }
    }

    public MoveDir GetDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else if (dir.y > 0)
            return MoveDir.Up;
        else 
            return MoveDir.Down;
    }

    // 바로 앞칸의 cell을 얻고 싶다.
    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;
        switch(Dir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up; // (0, 1, 0)
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }

    // state를 먼저 check
    // (idle일떄나 skill을 시전할때는 lastdir 확인, moving일때는 dir확인)
    protected virtual void UpdateAnimation()
    {
        // 이중 switch는 가독성이 떨어져서 if-else
        if(State == CreatureState.Idle)
        {
            switch(Dir)
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
        else if(State == CreatureState.Moving)
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
            // 마지막으로 바라본 기준으로 skill이 나가야하므로 _dir대신 
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = true;

                    break;
                case MoveDir.Right:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else
        {
            // DEAD?
        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateController();
    }

    protected virtual void Init()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        // 어떤 cell 위치에서 world 좌표, 지금은 grid size가 unit size와 같으므로 굳이 사용안해도되긴함
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        // 기본값이라 값이 서버에서 아무런 값이 들어오지 않았을 경우 다시 초기값 대입
        State = CreatureState.Idle;
        Dir = MoveDir.Down;
        // (0,0,0)이 아니라 Server 쪽에서 요청한 Player위치에 만들어주는게 정상적
        // CellPos = new Vector3Int(0, 0, 0);
        UpdateAnimation();
    }

    protected virtual void UpdateController()
    {
        switch(State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    // 이동 가능한 상태(Idle)일때 실제 좌표 이동
    // -> 다른 class에서 공동으로 사용하기 위해 수정
    // PlayerController에서 dir에 따라 moving state로 바꿔줌
    protected virtual void UpdateIdle()
    {
        // 완전히 이동 animation이 끝나기 전까지는 이동할 수 없게 막음
        // if (State == CreatureState.Idle && _dir != MoveDir.None)
        // 호출하는 쪽의 분기로 들어감
        // 
        // MoveToNextPos로 이동
        // UpdateIdle에서 UpdateMoving으로 넘어가는 코드가 필요하게됨 
    }

    // 방향에 따라 client상에서 부드럽게 이동하기 위한 용도
    protected virtual void UpdateMoving()
    {
        // 조건을 추가하는 것보다 Update문에서 State따라 분기를 두는게 코드 구성이 좋음
        // if (State != CreatureState.Moving) return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 movDir = destPos - transform.position;

        // 도착 여부 check
        float dist = movDir.magnitude;
        // 도착했다면
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            // 실질적으로 좌표 이동하는 함수
            // 아무 입력을 받지 않을 경우 _moveKeyPressed는 false로 Idle state로 바꿔준 후 C_Move를 쏴준다.
            MoveToNextPos();

            // 예외적으로 animation을 직접 control
            // State에 직접 대입하면 animation update가 발생해서 가다가 멈추고 반복함.  
            // _state = CreatureState.Idle;
            // 초반 상태를 잡아주기 위해서
            // if (_dir == MoveDir.None)
               // UpdateAnimation(); // Idle animation으로 바꿈
        }
        else
        {
            // 너무 speed가 빠르면 문제가 될 수 있음
            transform.position += movDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving; // 생략해도 되긴함
        }
    }

    // 다른 방식으로 진행 하고 싶으면 이것만 override 
    // 입력으로 dir을 받고 그에 따라서 좌표의 이동이 결정됨
    // 받지 않으면 바라보는 방향으로 계속 이동
    protected virtual void MoveToNextPos()
    {
        // MyPlayer만 이동 - 남의 Player은 직접적으로 움직이지 않는다.
        // 서버가 broadcasting 한 토대로 다른 player를 맞춰주는 형태로 동작 
    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }

    // 죽는 effect & 화살 피격 object 파괴 
    public virtual void OnDamage()
    {

    }
}
