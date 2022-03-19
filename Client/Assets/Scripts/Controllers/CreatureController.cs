using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    [SerializeField]
    public float _speed = 5.0f;

    // protected Vector3Int _cellPos = Vector3Int.zero;
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;  

    protected Animator _animator;
    protected SpriteRenderer _sprite;

    // 움직이지 못할 때 스킬 시전 가능하게 할거임
    // boolean을 늘리지 않고 state로 관리
    // protected bool _isMoving = false;
    [SerializeField]
    protected CreatureState _state = CreatureState.Idle;
    public virtual CreatureState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;
            
            _state = value;
            UpdateAnimation(); // State & dir 양쪽 상태중 어느 것이 하나가 바뀌면 호출
        }
    }

    // 마지막으로 바라보고 있던 방향
    // 꼼수 none이면 다 건너띄어서 idle animation을 안틀어줌.
    // _dir이 none이 아니라면 UpdateIsMoving(UpdateIdle)에서 자동으로 Moving State로 만들어줌
    protected MoveDir _lastDir = MoveDir.Down;
    // 게임을 처음실행할 때 움직이는 것을 막기 위해 초기값을 아무거나 설정
    // UpdateAnimation이 호출이 안되고 초반 설정값인 오른쪽 달리기를 재생한다.
    [SerializeField]
    protected MoveDir _dir = MoveDir.Down;
    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value) return;
            // 맨 마지막에 이동했던 방향으로 idle animation
            // dir를 선택할 때 바꾸는 것이 맞는지 아니면 State에서 animation을 관리하는게 맞는지 
            // 애매할때는 공용 부분에 놓고 따로 빼서 관리하는 게 좋다. -> UpdateAnimation
            _dir = value;

            if (value != MoveDir.None)
                _lastDir = value; // idle animation의 방향 선택

            UpdateAnimation();
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
        else if (dir.y < 0)
            return MoveDir.Down;
        else
            return MoveDir.None;
    }

    // 바로 앞칸의 cell을 얻고 싶다.
    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;
        switch(_lastDir)
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
    protected virtual void UpdateAnimation()
    {
        // 이중 switch는 가독성이 떨어져서 if-else
        if(_state == CreatureState.Idle)
        {
            switch(_lastDir)
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
        else if(_state == CreatureState.Moving)
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
                case MoveDir.None:
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
    protected virtual void UpdateIdle()
    {
        // 완전히 이동 animation이 끝나기 전까지는 이동할 수 없게 막음
        // if (State == CreatureState.Idle && _dir != MoveDir.None)
        // 호출하는 쪽의 분기로 들어감 
        // MoveToNextPos로 이동
        // UpdateIdle에서 UpdateMoving으로 넘어가는 코드가 필요하게됨 
    }

    // client상에서 부드럽게 이동하기 위한 용도
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
            // 그 다음 움직임
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
    protected virtual void MoveToNextPos()
    {
        // 버그 자연 치유, 계속 누르고 있으면 
        // 중간에 idle animation이 나오지 않는다.
        if(_dir == MoveDir.None)
        {
            // UpdateAnimation
            State = CreatureState.Idle;
            return;
        }

        Vector3Int destPos = CellPos;
        switch (_dir)
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

        // 굳이 필요 없음 dir이 Non이 아니면 계속 움직이고 싶은 상태
        // State = CreatureState.Moving;

        // 공용으로 사용하기 어려운게 화살인지 player인지에 때라 나뉘었음
        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;
                // 버그 수정, 이동할 수 없어도 계속 움직이는 animation을 틀어주도록
                // State = CreatureState.Moving;
            }
        }
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
