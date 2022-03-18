using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager 
{
    // Server
    // Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();  
    List<GameObject> _objects = new List<GameObject>();

    // id�� �ָ� ���ο��� ������ִ� factory pattern�� ������ ���ڴ�.
    public void Add(GameObject go) { _objects.Add(go); }
    public void Remove(GameObject go) { _objects.Remove(go); }

    // ���� ���, �־��� ��ǥ�� object�� �ִ��� Ȯ��, O(n) ��ü�� ���ٸ� ������
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
