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
    public LevelGroup levelGroup;

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
        public bool isSpecial; //later for the special/story maps
        public ComeFrom comeFrom;

        public enum ComeFrom
        {
            none = 0,
            left = 1,
            right = 2,
            up = 3,
            down = 4
        }

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
            if (!ml.IsLevel(x, y + 1)) return;
            if (!missingUp.Contains(x1)) missingUp.Add(x1);
            Level l = ml.FindLevel(x, y + 1);
            if(!l.missingDown.Contains(x1)) l.missingDown.Add(x1);
        }
        public void RemoveMissingUp(int x1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x, y + 1)) return;
            if (missingUp.Contains(x1)) missingUp.Remove(x1);
            Level l = ml.FindLevel(x, y + 1);
            if (l.missingDown.Contains(x1)) l.missingDown.Remove(x1);
        }

        public void AddMissingDown(int x1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x, y - 1)) return;
            if (!missingDown.Contains(x1)) missingDown.Add(x1);
            Level l = ml.FindLevel(x, y - 1);
            if (!l.missingUp.Contains(x1)) l.missingUp.Add(x1);
        }
        public void RemoveMissingDown(int x1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x, y - 1)) return;
            if (missingDown.Contains(x1)) missingDown.Remove(x1);
            Level l = ml.FindLevel(x, y - 1);
            if (l.missingUp.Contains(x1)) l.missingUp.Remove(x1);
        }

        public void AddMissingLeft(int y1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x - 1, y)) return;
            if (!missingLeft.Contains(y1)) missingLeft.Add(y1);
            Level l = ml.FindLevel(x - 1, y);
            if (!l.missingRight.Contains(y1)) l.missingRight.Add(y1);
        }
        public void RemoveMissingLeft(int y1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x - 1, y)) return;
            if (missingLeft.Contains(y1)) missingLeft.Remove(y1);
            Level l = ml.FindLevel(x - 1, y);
            if (l.missingRight.Contains(y1)) l.missingRight.Remove(y1);
        }

        public void AddMissingRight(int y1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x + 1, y)) return;
            if (!missingRight.Contains(y1)) missingRight.Add(y1);
            Level l = ml.FindLevel(x + 1, y);
            if (!l.missingLeft.Contains(y1)) l.missingLeft.Add(y1);
        }
        public void RemoveMissingRight(int y1)
        {
            ml = FindFirstObjectByType<MultipleLevel>();
            if (!ml.IsLevel(x + 1, y)) return;
            if (missingRight.Contains(y1)) missingRight.Remove(y1);
            Level l = ml.FindLevel(x + 1, y);
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

        public void Loaded(ComeFrom cf)
        {
            WallManager wm = FindFirstObjectByType<WallManager>();
            ml = FindFirstObjectByType<MultipleLevel>();
            if (wm == null || ml == null) return;
            if (missingUp != null)
            {
                for(int i = 0; i < missingUp.Count; i++) {
                    wm.outsideWallTilemap.SetTile(new Vector3Int(missingUp[i], ml.upY, 0), ml.clear);
                }
            }
            if (missingDown != null)
            {
                for (int i = 0; i < missingDown.Count; i++)
                {
                    wm.outsideWallTilemap.SetTile(new Vector3Int(missingDown[i], ml.downY, 0), ml.clear);
                }
            }
            if (missingLeft != null)
            {
                for (int i = 0; i < missingLeft.Count; i++)
                {
                    wm.outsideWallTilemap.SetTile(new Vector3Int(ml.leftX, missingLeft[i], 0), ml.clear);
                }
            }
            if (missingRight != null)
            {
                for (int i = 0; i < missingRight.Count; i++)
                {
                    wm.outsideWallTilemap.SetTile(new Vector3Int(ml.rightX, missingRight[i], 0), ml.clear);
                }
            }

            //if it's special, multiple things have to be reset
            if (isSpecial)
            {
                if(comeFrom == ComeFrom.left)
                {
                    if(cf == ComeFrom.left) FindFirstObjectByType<Player>().gameObject.transform.position = new Vector3(FindFirstObjectByType<Player>().gameObject.transform.position.x - 1, FindFirstObjectByType<Player>().gameObject.transform.position.y, 0);
                    for (int i = 0; i < missingLeft.Count; i++)
                    {
                        wm.outsideWallTilemap.SetTile(new Vector3Int(ml.leftX, missingLeft[i], 0), ml.wall);
                    }
                }
                else if (comeFrom == ComeFrom.right)
                {
                    if (cf == ComeFrom.right) FindFirstObjectByType<Player>().gameObject.transform.position = new Vector3(FindFirstObjectByType<Player>().gameObject.transform.position.x + 1, FindFirstObjectByType<Player>().gameObject.transform.position.y, 0);
                    for (int i = 0; i < missingRight.Count; i++)
                    {
                        wm.outsideWallTilemap.SetTile(new Vector3Int(ml.rightX, missingRight[i], 0), ml.wall);
                    }
                }
                else if (comeFrom == ComeFrom.up)
                {
                    if (cf == ComeFrom.up) FindFirstObjectByType<Player>().gameObject.transform.position = new Vector3(FindFirstObjectByType<Player>().gameObject.transform.position.x, FindFirstObjectByType<Player>().gameObject.transform.position.y + 1, 0);
                    for (int i = 0; i < missingUp.Count; i++)
                    {
                        wm.outsideWallTilemap.SetTile(new Vector3Int(missingUp[i], ml.upY, 0), ml.wall);
                    }
                }
                else if (comeFrom == ComeFrom.down)
                {
                    if (cf == ComeFrom.down) FindFirstObjectByType<Player>().gameObject.transform.position = new Vector3(FindFirstObjectByType<Player>().gameObject.transform.position.x, FindFirstObjectByType<Player>().gameObject.transform.position.y - 1, 0);
                    for (int i = 0; i < missingDown.Count; i++)
                    {
                        wm.outsideWallTilemap.SetTile(new Vector3Int(missingDown[i], ml.downY, 0), ml.wall);
                    }
                }

                foreach(int i in FindFirstObjectByType<WallManager>().colors.getIndexes())
                {
                    if(i != 0)
                    {
                        FindFirstObjectByType<WallManager>().colors.remove(i);
                        FindFirstObjectByType<WallManager>().activeAtStart.remove(i);
                    }
                }

                foreach (UnityEngine.Transform child in FindFirstObjectByType<Map>().tilemapParent)
                {
                    if (child.gameObject.GetComponent<DontDestroyThisObject>() != null) continue;
                    GameObject.Destroy(child.gameObject);
                }

                if (FindFirstObjectByType<Cube>() != null) Destroy(FindFirstObjectByType<Cube>().gameObject);
            }
        }
    }

    private void Awake()
    {

    }

    private void Start()
    {
        if (levelGroup != null) levels = levelGroup.levels;
        if (levelGroup == null || FindFirstObjectByType<Map>().loadFromProgress) return;
        currentX = levelGroup.x;
        currentY = levelGroup.y;
        FindFirstObjectByType<Map>().index = CurrentLevel().levelName;
        FindFirstObjectByType<Map>().LoadMap();
        CurrentLevel().Loaded(Level.ComeFrom.none);
    }

    public void LoadAMap()
    {
        if (levelGroup != null) levels = levelGroup.levels;
        FindFirstObjectByType<Map>().index = FindLevel(currentX, currentY).levelName;
        FindFirstObjectByType<Map>().LoadMap(false);
        FindLevel(currentX, currentY).Loaded(Level.ComeFrom.none);
    }

    public void SwitchUp()
    {
        //
        Debug.Log("^^");
        CurrentLevel().Unloaded();
        FindFirstObjectByType<Map>().index = FindLevel(currentX, currentY + 1).levelName;
        currentY++;
        FindFirstObjectByType<Map>().LoadMap(false);
        FindLevel(currentX, currentY).Loaded(Level.ComeFrom.down);
        UnityEngine.Transform tr = FindFirstObjectByType<movement>().transform;
        tr.position = new Vector3(tr.position.x, -tr.position.y + 0.5f, tr.position.z);
    }
    public void SwitchDown()
    {
        //
        Debug.Log("vv");
        CurrentLevel().Unloaded();
        FindFirstObjectByType<Map>().index = FindLevel(currentX, currentY - 1).levelName;
        currentY--;
        FindFirstObjectByType<Map>().LoadMap(false);
        FindLevel(currentX, currentY).Loaded(Level.ComeFrom.up);
        UnityEngine.Transform tr = FindFirstObjectByType<movement>().transform;
        tr.position = new Vector3(tr.position.x, -tr.position.y - 0.5f, tr.position.z);
    }
    public void SwitchLeft()
    {
        //
        Debug.Log("<<");
        CurrentLevel().Unloaded();
        FindFirstObjectByType<Map>().index = FindLevel(currentX - 1, currentY).levelName;
        currentX--;
        FindFirstObjectByType<Map>().LoadMap(false);
        FindLevel(currentX, currentY).Loaded(Level.ComeFrom.right);
        UnityEngine.Transform tr = FindFirstObjectByType<movement>().transform;
        tr.position = new Vector3(-tr.position.x - 0.5f, tr.position.y, tr.position.z);
    }
    public void SwitchRight()
    {
        //
        Debug.Log(">>");
        CurrentLevel().Unloaded();
        FindFirstObjectByType<Map>().index = FindLevel(currentX + 1, currentY).levelName;
        currentX++;
        FindFirstObjectByType<Map>().LoadMap(false);
        FindLevel(currentX, currentY).Loaded(Level.ComeFrom.left);
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
