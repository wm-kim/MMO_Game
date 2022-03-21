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
    GameObject _target; // ���� �����ϰ� �ִ� ��� (player) 1�ʸ��� search

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

    // start update ��� creature�� �ִ� start update�� �ڵ� ȣ��
    protected override void Init()
    {
        base.Init();
        // state & dir�� �ٲ��ָ� UpdateAnimation 
        State = CreatureState.Idle;
        Dir = MoveDir.None;
        _speed = 3.0f;
        _rangedSkill = (Random.Range(0, 2) == 0 ? true : false);

        if (_rangedSkill) _skillRange = 10.0f;
        else _skillRange = 1.0f;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle(); // �� �ڵ�

        // Idle�� ���°� �ٲ������ �Ź����� �ٽ� ȣ���
        // patrol
        if(_coPatrol == null)
        {
            // moving State �� �ٲپ��� 
            _coPatrol = StartCoroutine("CoPatrol");
        }

        // ���Ͱ� �̵��ϴ� ���߿� player�� ã���� player�� �����̵�
        if (_coSearch == null)
        {
            _coSearch = StartCoroutine("CoSearch");
        }

    }

    // CreatureController�� Update���� ȣ���
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
                State = CreatureState.Skill; // skill animation ���

                if(_rangedSkill)
                    _coSkill = StartCoroutine("CoStartShootArrow");
                else
                    _coSkill = StartCoroutine("CoStartPunch");
                return;
            }
        }

        // ignoreDestCollision, destPos�� � ��ü�� �ִ��� �浹�� �ν����� �ʰ� ���� �������
        // ������ ���鿡�� ignoreDestCollision: true
        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);

        // 1�� ���ڸ��� �ְڴٶ�� ��, 2���� �������̸� ���� ��ã�� ��
        // Ȥ�� �÷��̾ �ʹ� �ָ� ������ �ִ� ���
        // �ʹ� �ָ� ���� ���� target�� ���� ��쿡�� return
        if (path.Count < 2 || (_target != null && path.Count > 20))
        {
            _target = null;
            State = CreatureState.Idle;
            return;
        }

        // path[0]�� ���� ��ġ. ��� ���°� ���ϱ� ������ �Ź� ����� �ؾ��ϴ°� ��¿ �� ����.
        // optimization �ʹ� �־����� FindPath ����� ���� �ʴ´�.
        // ���� �迭�� ����ٸ� FindPath���� closed�� 2���迭�� ����°ͺ��� list�� �����ص� �ȴ�.
        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos;

        // UpdateAnimation
        Dir = GetDirFromVec(moveCellDir);

        // ��ǥ�� �̵��ϴ� �κ��ε� A star�� �̿��Ͽ� ã����
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

        // Object.Find�� �ڱ� �ڽŵ� ����
        if (Managers.Map.CanGo(nextPos) && Managers.Object.Find(nextPos) == null)
        {
            CellPos = nextPos;
        }
        else // ���� �ְų� (�ٸ� ���ͳ� �÷��̾ ���� ���� �� ����), �̵��Ϸ� ���� ��
        {
            // �ٷ� �����. UpdateAnimation ȣ��
            // _coPatrol�� null�� �о��ش�.
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

        // ������ ��ġ�� ���͸� �̵� ���� �� �� �ִ� ��ġ���� 10���� try
        for(int i = 0; i < 10; i++)
        {
            int xRange = Random.Range(-5, 6);
            int yRange = Random.Range(-5, 6);
            Vector3Int randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            if(Managers.Map.CanGo(randPos) && Managers.Object.Find(randPos) == null)
            {
                _destCellPos = randPos;
                State = CreatureState.Moving;
                yield break; // coroutine�� ������ �������� 
            }
        }

        State = CreatureState.Idle;
        // state�� �ٲ�� �ڵ����� _coPatrol�� null�� �о��ش�.
    }

    // �÷��̾�� �ǽð����� �̵��ϱ� ������ 1�ʸ��� search 
    IEnumerator CoSearch()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);

            if (_target != null) continue;

            // target ã��
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
        // �ǰ� ����
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null) cc.OnDamage();
        }

        // ��� �ð�
        yield return new WaitForSeconds(0.5f);
        // 0.5�ʸ��� idle ���·� ���ư�
        // idle ���·� ���ư��� �ٽ� patrol ���·� ������
        // moving update���� skill�� �������� �����ð��� �ɸ���
        // State = CreatureState.Idle;

        State = CreatureState.Moving; // idle ��� �ð��� ���ֱ� ���ؼ�
        _coSkill = null;
    }

    IEnumerator CoStartShootArrow()
    {
        GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController ac = go.GetComponent<ArrowController>();
        // Ű���带 �ȴ��� ���¶�� �ص� ������ ���������� �ٶ󺸰� �ִ� ���·� ����
        ac.Dir = _lastDir;
        ac.CellPos = CellPos;

        // ��� �ð�
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Moving; // idle ��� �ð��� ���ֱ� ���ؼ�
        _coSkill = null;
    }
}
