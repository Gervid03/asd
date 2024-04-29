using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MultipleLevel : MonoBehaviour
{
    public int currentX, currentY;
    public int leftX;
    public int rightX;
    public int upY;
    public int downY;
    public TileBase clear;
    public TileBase wall;
    public List<Level> levels;

    [System.Serializable]
    public struct Level
    {
        public int x, y;
        public string levelName;
        public List<int> missingUp;
        public List<int> missingDown;
        public List<int> missingRight;
        public List<int> missingLeft;
        MultipleLevel ml;

        public void Set(int x1, int y1, string name = "")
        {
            x = x1;
            y = y1;
            missingUp = new List<int>();
            missingDown = new List<int>();
            missingRight = new List<int>();
            missingLeft = new List<int>();
            levelName = name;
            ml = FindFirstObjectByType<MultipleLevel>();
        }

        public void AddMissingUp(int x1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x + 1, y)) return;
            if (!missingUp.Contains(x1)) missingUp.Add(x1);
            Level l = ml.FindLevel(x + 1, y);
            if(!l.missingDown.Contains(x1)) l.missingDown.Add(x1);
        }
        public void RemoveMissingUp(int x1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x + 1, y)) return;
            if (missingUp.Contains(x1)) missingUp.Remove(x1);
            Level l = ml.FindLevel(x + 1, y);
            if (l.missingDown.Contains(x1)) l.missingDown.Remove(x1);
        }

        public void AddMissingDown(int x1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x - 1, y)) return;
            if (!missingDown.Contains(x1)) missingDown.Add(x1);
            Level l = ml.FindLevel(x - 1, y);
            if (!l.missingUp.Contains(x1)) l.missingUp.Add(x1);
        }
        public void RemoveMissingDown(int x1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x - 1, y)) return;
            if (missingDown.Contains(x1)) missingDown.Remove(x1);
            Level l = ml.FindLevel(x - 1, y);
            if (l.missingUp.Contains(x1)) l.missingUp.Remove(x1);
        }

        public void AddMissingLeft(int y1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x, y - 1)) return;
            if (!missingLeft.Contains(y1)) missingLeft.Add(y1);
            Level l = ml.FindLevel(x, y - 1);
            if (!l.missingRight.Contains(y1)) l.missingRight.Add(y1);
        }
        public void RemoveMissingLeft(int y1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x, y - 1)) return;
            if (missingLeft.Contains(y1)) missingLeft.Remove(y1);
            Level l = ml.FindLevel(x, y - 1);
            if (l.missingRight.Contains(y1)) l.missingRight.Remove(y1);
        }

        public void AddMissingRight(int y1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x, y + 1)) return;
            if (!missingRight.Contains(y1)) missingRight.Add(y1);
            Level l = ml.FindLevel(x, y + 1);
            if (!l.missingLeft.Contains(y1)) l.missingLeft.Add(y1);
        }
        public void RemoveMissingRight(int y1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x, y + 1)) return;
            if (missingRight.Contains(y1)) missingRight.Remove(y1);
            Level l = ml.FindLevel(x, y + 1);
            if (l.missingLeft.Contains(y1)) l.missingLeft.Remove(y1);
        }

        public void Unloaded()
        {
            WallManager wm = FindFirstObjectByType<WallManager>();
            ml = FindFirstObjectByType<MultipleLevel>();
            if (wm == null || ml == null) return;
            for (int i = 0; i < missingUp.Count; i++)
            {
                wm.outsideWallTilemap.SetTile(new Vector3Int(missingUp[i], ml.upY, 0), ml.wall);
            }
            for (int i = 0; i < missingDown.Count; i++)
            {
                wm.outsideWallTilemap.SetTile(new Vector3Int(missingDown[i], ml.downY, 0), ml.wall);
            }
            for (int i = 0; i < missingLeft.Count; i++)
            {
                wm.outsideWallTilemap.SetTile(new Vector3Int(ml.leftX, missingLeft[i], 0), ml.wall);
            }
            for (int i = 0; i < missingRight.Count; i++)
            {
                wm.outsideWallTilemap.SetTile(new Vector3Int(ml.rightX, missingRight[i], 0), ml.wall);
            }
        }

        public void Loaded()
        {
            WallManager wm = FindFirstObjectByType<WallManager>();
            ml = FindFirstObjectByType<MultipleLevel>();
            if (wm == null || ml == null) return;
            for(int i = 0; i < missingUp.Count; i++) {
                wm.outsideWallTilemap.SetTile(new Vector3Int(missingUp[i], ml.upY, 0), ml.clear);
            }
            for (int i = 0; i < missingDown.Count; i++)
            {
                wm.outsideWallTilemap.SetTile(new Vector3Int(missingDown[i], ml.downY, 0), ml.clear);
            }
            for (int i = 0; i < missingLeft.Count; i++)
            {
                wm.outsideWallTilemap.SetTile(new Vector3Int(ml.leftX, missingLeft[i], 0), ml.clear);
            }
            for (int i = 0; i < missingRight.Count; i++)
            {
                wm.outsideWallTilemap.SetTile(new Vector3Int(ml.rightX, missingRight[i], 0), ml.clear);
            }
        }
    }

    private void Start()
    {
        Level l = new Level();
        l.Set(currentX, currentY, "0");
        levels.Add(l);
        l = new Level();
        l.Set(currentX + 1, currentY, "0");
        levels.Add(l);
        l = new Level();
        l.Set(currentX - 1, currentY, "0");
        levels.Add(l);
        l = new Level();
        l.Set(currentX, currentY + 1, "0");
        levels.Add(l);
        l = new Level();
        l.Set(currentX, currentY - 1, "0");
        levels.Add(l);

        levels[0].AddMissingUp(1);
        levels[0].AddMissingUp(2);
        levels[0].AddMissingUp(3);

        levels[0].AddMissingDown(1);
        levels[0].AddMissingDown(2);
        levels[0].AddMissingDown(3);

        levels[0].AddMissingLeft(1);
        levels[0].AddMissingLeft(2);
        levels[0].AddMissingLeft(3);

        levels[0].AddMissingRight(1);
        levels[0].AddMissingRight(2);
        levels[0].AddMissingRight(3);
        levels[0].Loaded();
    }

    public void SwitchUp()
    {
        //
        Debug.Log("^^");
        CurrentLevel().Unloaded();
        FindFirstObjectByType<Map>().index = FindLevel(currentX + 1, currentY).levelName;
        currentX++;
        FindFirstObjectByType<Map>().LoadMap(false);
        FindLevel(currentX, currentY).Loaded();
        UnityEngine.Transform tr = FindFirstObjectByType<movement>().transform;
        tr.position = new Vector3(tr.position.x, -tr.position.y + 0.5f, tr.position.z);
    }
    public void SwitchDown()
    {
        //
        Debug.Log("vv");
        CurrentLevel().Unloaded();
        FindFirstObjectByType<Map>().index = FindLevel(currentX - 1, currentY).levelName;
        currentX--;
        FindFirstObjectByType<Map>().LoadMap(false);
        FindLevel(currentX, currentY).Loaded();
        UnityEngine.Transform tr = FindFirstObjectByType<movement>().transform;
        tr.position = new Vector3(tr.position.x, -tr.position.y - 0.5f, tr.position.z);
    }
    public void SwitchLeft()
    {
        //
        Debug.Log("<<");
        CurrentLevel().Unloaded();
        FindFirstObjectByType<Map>().index = FindLevel(currentX, currentY - 1).levelName;
        currentY--;
        FindFirstObjectByType<Map>().LoadMap(false);
        FindLevel(currentX, currentY).Loaded();
        UnityEngine.Transform tr = FindFirstObjectByType<movement>().transform;
        tr.position = new Vector3(-tr.position.x - 0.5f, tr.position.y, tr.position.z);
    }
    public void SwitchRight()
    {
        //
        Debug.Log(">>");
        CurrentLevel().Unloaded();
        FindFirstObjectByType<Map>().index = FindLevel(currentX, currentY + 1).levelName;
        currentY++;
        FindFirstObjectByType<Map>().LoadMap(false);
        FindLevel(currentX, currentY).Loaded();
        UnityEngine.Transform tr = FindFirstObjectByType<movement>().transform;
        tr.position = new Vector3(-tr.position.x + 0.5f, tr.position.y, tr.position.z);

    }

    public Level FindLevel(int x, int y)
    {
        for(int i = 0; i < levels.Count; i++)
        {
            if (levels[i].x == x && levels[i].y == y)
            {
                return levels[i];
            }
        }
        return new Level();
    }

    public bool IsLevel(int x, int y)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].x == x && levels[i].y == y)
            {
                return true;
            }
        }
        return false;
    }

    public Level CurrentLevel()
    {
        return FindLevel(currentX, currentY);
    }
}
