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

    // �������� ���� �� ��ų ���� �����ϰ� �Ұ���
    // boolean�� �ø��� �ʰ� state�� ����
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
            UpdateAnimation(); // State & dir ���� ������ ��� ���� �ϳ��� �ٲ�� ȣ��
        }
    }

    // ���������� �ٶ󺸰� �ִ� ����
    // �ļ� none�̸� �� �ǳʶ� idle animation�� ��Ʋ����.
    // _dir�� none�� �ƴ϶�� UpdateIsMoving(UpdateIdle)���� �ڵ����� Moving State�� �������
    protected MoveDir _lastDir = MoveDir.Down;
    // ������ ó�������� �� �����̴� ���� ���� ���� �ʱⰪ�� �ƹ��ų� ����
    // UpdateAnimation�� ȣ���� �ȵǰ� �ʹ� �������� ������ �޸��⸦ ����Ѵ�.
    [SerializeField]
    protected MoveDir _dir = MoveDir.Down;
    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value) return;
            // �� �������� �̵��ߴ� �������� idle animation
            // dir�� ������ �� �ٲٴ� ���� �´��� �ƴϸ� State���� animation�� �����ϴ°� �´��� 
            // �ָ��Ҷ��� ���� �κп� ���� ���� ���� �����ϴ� �� ����. -> UpdateAnimation
            _dir = value;

            if (value != MoveDir.None)
                _lastDir = value; // idle animation�� ���� ����

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

    // �ٷ� ��ĭ�� cell�� ��� �ʹ�.
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

    // state�� ���� check
    protected virtual void UpdateAnimation()
    {
        // ���� switch�� �������� �������� if-else
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
            // ���������� �ٶ� �������� skill�� �������ϹǷ� _dir��� 
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
        // � cell ��ġ���� world ��ǥ, ������ grid size�� unit size�� �����Ƿ� ���� �����ص��Ǳ���
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

    // �̵� ������ ����(Idle)�϶� ���� ��ǥ �̵�
    // -> �ٸ� class���� �������� ����ϱ� ���� ����
    protected virtual void UpdateIdle()
    {
        // ������ �̵� animation�� ������ �������� �̵��� �� ���� ����
        // if (State == CreatureState.Idle && _dir != MoveDir.None)
        // ȣ���ϴ� ���� �б�� �� 
        // MoveToNextPos�� �̵�
        // UpdateIdle���� UpdateMoving���� �Ѿ�� �ڵ尡 �ʿ��ϰԵ� 
    }

    // client�󿡼� �ε巴�� �̵��ϱ� ���� �뵵
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
            // �� ���� ������
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
    protected virtual void MoveToNextPos()
    {
        // ���� �ڿ� ġ��, ��� ������ ������ 
        // �߰��� idle animation�� ������ �ʴ´�.
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

        // ���� �ʿ� ���� dir�� Non�� �ƴϸ� ��� �����̰� ���� ����
        // State = CreatureState.Moving;

        // �������� ����ϱ� ������ ȭ������ player������ ���� ��������
        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;
                // ���� ����, �̵��� �� ��� ��� �����̴� animation�� Ʋ���ֵ���
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

    // �״� effect & ȭ�� �ǰ� object �ı� 
    public virtual void OnDamage()
    {

    }
}
