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

    // dirty flag : dir, state, cellpos �����ϳ��� ������� ����
    // my player�� check�Ұ��̹Ƿ� creature controller�� �ִ°��� �� �ָ��ϱ��ϴ�.
    protected bool _updated = false;

    PositionInfo _positionInfo = new PositionInfo();

    // Init���� �ʱⰪ�� �����ȴ�.
    // UpdateIdle���� dir�� ���� moving State�� �ٲ�ų�
    // MoveToNextPos���� �Է��� ���� ���� ��� Dir�� non�̵Ǹ鼭 Idle state�� �ٲ��.
    public PositionInfo PosInfo 
    { 
        get { return _positionInfo; } 
        set
        {
            // ���� �ʿ����
            if (_positionInfo.Equals(value)) return;

            // _positionInfo = value;
            // _Lastdir�� ���� ������Ʈ �ޱ� ���ؼ� ���������� ���� �־����� �ʰ�
            // �ϳ��� ������Ʈ
            CellPos = new Vector3Int(value.PosX, value.PosY, 0);
            State = value.State;
            Dir = value.MoveDir;
            // UpdateAnimation();
        }
    }

    // cellpos�� transform �����ִ� �Լ�
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

    // �������� ���� �� ��ų ���� �����ϰ� �Ұ���
    // boolean�� �ø��� �ʰ� state�� ����
    // protected bool _isMoving = false;

    // idle�����̰� dir�� non�� �ƴ� ���¿��� UpdateIdle�� ȣ��Ǹ� �ٷ� moving state�� �ٷιٲ�
    // protected CreatureState _state = CreatureState.Idle;

    public virtual CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
            // State & dir ���� ������ ��� ���� �ϳ��� �ٲ�� ȣ��
            UpdateAnimation(); 
            _updated = true;
        }
    }

    // ���������� �ٶ󺸰� �ִ� ����
    // �ļ� none�̸� �� �ǳʶ� idle animation�� �ȵȴ�.
    // �ʹ� �������� �� �ٶ󺸴� ������ �ȴ�. ������ _lastdir ������
    // protected MoveDir _lastDir = MoveDir.Down;

    // ������ ó�������� �� �����̴� ���� ���� ���� �ʱⰪ�� �ƹ��ų� ����
    // UpdateAnimation�� ȣ���� �ȵǰ� �ʹ� �������� ������ �޸��⸦ ����Ѵ�.
    // protected MoveDir _dir = MoveDir.Down;
    
    public MoveDir Dir
    {
        get { return PosInfo.MoveDir; }
        set
        {
            if (PosInfo.MoveDir == value) return;
            // �� �������� �̵��ߴ� �������� idle animation
            // dir�� ������ �� �ٲٴ� ���� �´��� �ƴϸ� State���� animation�� �����ϴ°� �´��� 
            // �ָ��Ҷ��� ���� �κп� ���� ���� ���� �����ϴ� �� ����. -> UpdateAnimation
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

    // �ٷ� ��ĭ�� cell�� ��� �ʹ�.
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

    // state�� ���� check
    // (idle�ϋ��� skill�� �����Ҷ��� lastdir Ȯ��, moving�϶��� dirȮ��)
    protected virtual void UpdateAnimation()
    {
        // ���� switch�� �������� �������� if-else
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
            // ���������� �ٶ� �������� skill�� �������ϹǷ� _dir��� 
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
        // � cell ��ġ���� world ��ǥ, ������ grid size�� unit size�� �����Ƿ� ���� �����ص��Ǳ���
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        // �⺻���̶� ���� �������� �ƹ��� ���� ������ �ʾ��� ��� �ٽ� �ʱⰪ ����
        State = CreatureState.Idle;
        Dir = MoveDir.Down;
        // (0,0,0)�� �ƴ϶� Server �ʿ��� ��û�� Player��ġ�� ������ִ°� ������
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

    // �̵� ������ ����(Idle)�϶� ���� ��ǥ �̵�
    // -> �ٸ� class���� �������� ����ϱ� ���� ����
    // PlayerController���� dir�� ���� moving state�� �ٲ���
    protected virtual void UpdateIdle()
    {
        // ������ �̵� animation�� ������ �������� �̵��� �� ���� ����
        // if (State == CreatureState.Idle && _dir != MoveDir.None)
        // ȣ���ϴ� ���� �б�� ��
        // 
        // MoveToNextPos�� �̵�
        // UpdateIdle���� UpdateMoving���� �Ѿ�� �ڵ尡 �ʿ��ϰԵ� 
    }

    // ���⿡ ���� client�󿡼� �ε巴�� �̵��ϱ� ���� �뵵
    protected virtual void UpdateMoving()
    {
        // ������ �߰��ϴ� �ͺ��� Update������ State���� �б⸦ �δ°� �ڵ� ������ ����
        // if (State != CreatureState.Moving) return;

        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 movDir = destPos - transform.position;

        // ���� ���� check
        float dist = movDir.magnitude;
        // �����ߴٸ�
        if (dist < _speed * Time.deltaTime)
        {
            transform.position = destPos;
            // ���������� ��ǥ �̵��ϴ� �Լ�
            // �ƹ� �Է��� ���� ���� ��� _moveKeyPressed�� false�� Idle state�� �ٲ��� �� C_Move�� ���ش�.
            MoveToNextPos();

            // ���������� animation�� ���� control
            // State�� ���� �����ϸ� animation update�� �߻��ؼ� ���ٰ� ���߰� �ݺ���.  
            // _state = CreatureState.Idle;
            // �ʹ� ���¸� ����ֱ� ���ؼ�
            // if (_dir == MoveDir.None)
               // UpdateAnimation(); // Idle animation���� �ٲ�
        }
        else
        {
            // �ʹ� speed�� ������ ������ �� �� ����
            transform.position += movDir.normalized * _speed * Time.deltaTime;
            State = CreatureState.Moving; // �����ص� �Ǳ���
        }
    }

    // �ٸ� ������� ���� �ϰ� ������ �̰͸� override 
    // �Է����� dir�� �ް� �׿� ���� ��ǥ�� �̵��� ������
    // ���� ������ �ٶ󺸴� �������� ��� �̵�
    protected virtual void MoveToNextPos()
    {
        // MyPlayer�� �̵� - ���� Player�� ���������� �������� �ʴ´�.
        // ������ broadcasting �� ���� �ٸ� player�� �����ִ� ���·� ���� 
    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }

    // �״� effect & ȭ�� �ǰ� object �ı� 
    public virtual void OnDamage()
    {

    }
}
