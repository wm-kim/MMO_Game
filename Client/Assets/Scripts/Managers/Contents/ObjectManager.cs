using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager 
{
    // Server
    // Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();  
    List<GameObject> _objects = new List<GameObject>();

    // id를 주면 내부에서 만들어주는 factory pattern이 있으면 좋겠다.
    public void Add(GameObject go) { _objects.Add(go); }
    public void Remove(GameObject go) { _objects.Remove(go); }

    // 느린 방법, 주어진 좌표에 object가 있는지 확인, O(n) 객체가 적다면 괜찮음
    public GameObject Find(Vector3Int cellPos)
    {
        foreach(GameObject obj in _objects)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null) continue;

            if (cc.CellPos == cellPos) return obj;
        }

        return null;
    }



    public void Clear() { _objects.Clear(); }
}
