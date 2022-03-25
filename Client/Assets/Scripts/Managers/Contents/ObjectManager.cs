using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager 
{
    public MyPlayerController MyPlayer { get; set; }
    // Server List에서 Dictionary로 바꿈
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    // id에서 type 추출
    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }

    // 내부에서 만들어주는 factory pattern
    // 이제 player만 만들어주는것이 아니라 화살 monster 등등 만들어줄 것이다.
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

                // Player를 만들자 마자 setting하고 있는데 Init을 해야 animator와 sprite가 있다.
                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.ObjectId;
                MyPlayer.PosInfo = info.PosInfo;
                // cellpos에 따라 transform을 맞추어 주는것이 필요
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
                // cellpos에 따라 transform을 맞추어 주는것이 필요
                pc.SyncPos();
            }
        }
        else if (objectType == GameObjectType.Monster)
        {

        }
        // 지금은 projectile 화살 하나밖에 없다.
        else if(objectType == GameObjectType.Projectile)
        {
            // 생성 및 방향 설정
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

    // 주어진 좌표에 object가 있는지 확인
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
