using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager 
{
    public MyPlayerController MyPlayer { get; set; }
    // Server List���� Dictionary�� �ٲ�
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    // id���� type ����
    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    // ���ο��� ������ִ� factory pattern
    // ���� player�� ������ִ°��� �ƴ϶� ȭ�� monster ��� ������� ���̴�.
    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        GameObjectType objectType = GetObjectTypeById(info.ObjectId);

        if(objectType == GameObjectType.Player)
        {
            if (myPlayer)
            {
                GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                // Player�� ������ ���� setting�ϰ� �ִµ� Init�� �ؾ� animator�� sprite�� �ִ�.
                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = info.PosInfo;
                // cellpos�� ���� transform�� ���߾� �ִ°��� �ʿ�
                MyPlayer.SyncPos();
            }
            else
            {
                GameObject go = Managers.Resource.Instantiate("Creature/Player");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                PlayerController pc = go.GetComponent<PlayerController>();
                pc.Id = info.ObjectId;
                pc.PosInfo = info.PosInfo;
                // cellpos�� ���� transform�� ���߾� �ִ°��� �ʿ�
                pc.SyncPos();
            }
        }
        else if (objectType == GameObjectType.Monster)
        {

        }
        // ������ projectile ȭ�� �ϳ��ۿ� ����.
        else if(objectType == GameObjectType.Projectile)
        {
            // ���� �� ���� ����
            GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
            go.name = "Arrow";
            _objects.Add(info.ObjectId, go);

            ArrowController ac = go.GetComponent<ArrowController>();
            ac.Dir = info.PosInfo.MoveDir;
            ac.CellPos = new Vector3Int(info.PosInfo.PosX, info.PosInfo.PosY, 0);
            ac.SyncPos();
        }
    }

    public void Remove(int id) 
    {
        GameObject go = FindById(id);
        if (go == null) return;

        _objects.Remove(id); 
        Managers.Resource.Destroy(go); 
    }

    public void RemoveMyPlayer()
    {
        if (MyPlayer == null) return;
        Remove(MyPlayer.Id);
        MyPlayer = null;
    }

    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    // �־��� ��ǥ�� object�� �ִ��� Ȯ��
    public GameObject Find(Vector3Int cellPos)
    {
        foreach(GameObject obj in _objects.Values)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null) continue;

            if (cc.CellPos == cellPos) return obj;
        }

        return null;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach(GameObject obj in _objects.Values)
        {   
            if (condition.Invoke(obj))
                return obj;
        }

        return null;
    }

    public void Clear() 
    {
        foreach (GameObject obj in _objects.Values)
        {
            Managers.Resource.Destroy(obj);
        }
        _objects.Clear(); 
    }
}
