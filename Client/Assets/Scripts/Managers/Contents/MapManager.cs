using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public struct Pos // left bottom 기준
{
    public Pos(int y, int x) { Y = y; X = x; }
    public int Y;
    public int X;
}

public struct PQNode : IComparable<PQNode>
{   // F = G + H
	public int F;
    public int G;
    public int Y;
    public int X;

    public int CompareTo(PQNode other)
    {
        if (F == other.F)
            return 0;
        return F < other.F ? 1 : -1;
    }
}

// map load 삭제 역할, load가 되었으면 collision file 추출
public class MapManager
{
	// map을 구성하는 component
    public Grid CurrentGrid { get; private set; }

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

	public int SizeX { get { return MaxX - MinX + 1; } }
	public int SizeY { get { return MaxY - MinY + 1; } }

	bool[,] _collision;
    // GameObject[,] _obj; // obj좌표가 변할 떄마다 실시간으로 갱신

    // destpos가 넘어옴
    public bool CanGo(Vector3Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x >= MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y >= MaxY)
            return false;

        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y; // 위쪽으로 갈수록 y가 높아짐
        return !_collision[y, x];
    }

    public void LoadMap(int mapId)
    {
        DestroyMap(); // 기존에 있었던것 삭제

        string mapName = "Map_" + mapId.ToString("000"); // formatting, 1일경우에 001이됨
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}");
        go.name = "Map";

        // 꺼져 있는 object를 찾는건 까다롭기때문에 Tilemap_Collision은 active true인 상태에서 시작한다.
        GameObject collision = Util.FindChild(go, "Tilemap_Collision", true);
        if (collision != null)
        {
            collision.SetActive(false);
        }

        CurrentGrid = go.GetComponent<Grid>();

        // Collision 관련 파일 .txt하면 안됨
        TextAsset txt = Managers.Resource.Load<TextAsset>($"Map/{mapName}");
        // parsing 문장단위로 읽을 때 편리
        StringReader reader = new StringReader(txt.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());
        
        int xCount = MaxX - MinX + 1;
        int yCount = MaxY - MinY + 1;
        _collision = new bool[yCount, xCount];

        for(int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for(int x = 0; x < xCount; x++)
            {
                _collision[y, x] = (line[x] == '1' ? true : false);
            }
        }
    }

    public void DestroyMap()
    {
        GameObject map = GameObject.Find("Map");
        if(map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }

	#region A* PathFinding

	// U D L R
	int[] _deltaY = new int[] { 1, -1, 0, 0 };
	int[] _deltaX = new int[] { 0, 0, -1, 1 };
	int[] _cost = new int[] { 10, 10, 10, 10 };

	public List<Vector3Int> FindPath(Vector3Int startCellPos, Vector3Int destCellPos, bool ignoreDestCollision = false)
	{
		List<Pos> path = new List<Pos>();

		// 점수 매기기
		// F = G + H
		// F = 최종 점수 (작을 수록 좋음, 경로에 따라 달라짐)
		// G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을 수록 좋음, 경로에 따라 달라짐)
		// H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)

		// (y, x) 이미 방문했는지 여부 (방문 = closed 상태)
		bool[,] closed = new bool[SizeY, SizeX]; // CloseList

		// (y, x) 가는 길을 한 번이라도 발견했는지
		// 발견X => MaxValue
		// 발견O => F = G + H
		int[,] open = new int[SizeY, SizeX]; // OpenList
		for (int y = 0; y < SizeY; y++)
			for (int x = 0; x < SizeX; x++)
				open[y, x] = Int32.MaxValue; // 초기화

		Pos[,] parent = new Pos[SizeY, SizeX];

		// 오픈리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
		PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

		// CellPos -> ArrayPos
		Pos pos = Cell2Pos(startCellPos);
		Pos dest = Cell2Pos(destCellPos);

		// 시작점 발견 (예약 진행)
		open[pos.Y, pos.X] = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X));
		pq.Push(new PQNode() { F = 10 * (Math.Abs(dest.Y - pos.Y) + Math.Abs(dest.X - pos.X)), G = 0, Y = pos.Y, X = pos.X });
		parent[pos.Y, pos.X] = new Pos(pos.Y, pos.X); // 시작점은 자기 자신을 부모로 둔다.

		while (pq.Count > 0)
		{
			// 제일 좋은 후보를 찾는다
			PQNode node = pq.Pop();
			// 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
			if (closed[node.Y, node.X])
				continue;

			// 방문한다
			closed[node.Y, node.X] = true;
			// 목적지 도착했으면 바로 종료
			if (node.Y == dest.Y && node.X == dest.X)
				break;

			// 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다
			for (int i = 0; i < _deltaY.Length; i++)
			{
				Pos next = new Pos(node.Y + _deltaY[i], node.X + _deltaX[i]);

				// 유효 범위를 벗어났으면 스킵
				// 벽으로 막혀서 갈 수 없으면 스킵
				if (!ignoreDestCollision || next.Y != dest.Y || next.X != dest.X)
				{
					if (CanGo(Pos2Cell(next)) == false) // CellPos
						continue;
				}

				// 이미 방문한 곳이면 스킵
				if (closed[next.Y, next.X])
					continue;

				// 비용 계산
				int g = 0; // node.G + _cost[i];
				int h = 10 * ((dest.Y - next.Y) * (dest.Y - next.Y) + (dest.X - next.X) * (dest.X - next.X));
				
				// 다른 경로에서 더 빠른 길 이미 찾았으면 스킵
				if (open[next.Y, next.X] < g + h)
					continue;

				// 예약 진행
				open[next.Y, next.X] = g + h;
				pq.Push(new PQNode() { F = g + h, G = g, Y = next.Y, X = next.X });
				parent[next.Y, next.X] = new Pos(node.Y, node.X);
			}
		}

		return CalcCellPathFromParent(parent, dest);
	}

	List<Vector3Int> CalcCellPathFromParent(Pos[,] parent, Pos dest)
	{
		List<Vector3Int> cells = new List<Vector3Int>();

		int y = dest.Y;
		int x = dest.X;
		while (parent[y, x].Y != y || parent[y, x].X != x)
		{
			cells.Add(Pos2Cell(new Pos(y, x)));
			Pos pos = parent[y, x];
			y = pos.Y;
			x = pos.X;
		}
		cells.Add(Pos2Cell(new Pos(y, x)));
		cells.Reverse();

		return cells;
	}

	Pos Cell2Pos(Vector3Int cell)
	{
		// CellPos -> ArrayPos
		return new Pos(MaxY - cell.y, cell.x - MinX);
	}

	Vector3Int Pos2Cell(Pos pos)
	{
		// ArrayPos -> CellPos
		return new Vector3Int(pos.X + MinX, MaxY - pos.Y, 0);
	}

	#endregion
}
