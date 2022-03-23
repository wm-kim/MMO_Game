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

    // 내부에서 만들어주는 factory pattern
    public void Add(PlayerInfo info, bool myPlayer = false)
    {
        if(myPlayer)
        {
            GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);

            MyPlayer = go.GetComponent<MyPlayerController>();
            MyPlayer.Id = info.PlayerId;
            MyPlayer.PosInfo = info.PosInfo;
            // cellpos에 따라 transform을 맞추어 주는것이 필요
            MyPlayer.SyncPos();
        }
        else
        {
            GameObject go = Managers.Resource.Instantiate("Creature/Player");
            go.name = info.Name;
            _objects.Add(info.PlayerId, go);

            PlayerController pc = go.GetComponent<PlayerController>();
            pc.Id = info.PlayerId;
            pc.PosInfo = info.PosInfo;
            // cellpos에 따라 transform을 맞추어 주는것이 필요
            pc.SyncPos();
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
