using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MultipleLevel : MonoBehaviour
{
    public int currentX, currentY;
    public int maxX, maxY;
    public int firstX;
    public int lastX;
    public int firstY;
    public int lastY;
    public TileBase clear;

    public struct Level
    {
        int x, y;
        public List<int> missingUp;
        public List<int> missingDown;
        public List<int> missingRight;
        public List<int> missingLeft;

        public void AddMissingUp(int x)
        {
            if (x == 0) return;
            if (!missingUp.Contains(x)) missingUp.Add(x);
        }
        public void RemoveMissingUp(int x)
        {
            if (missingUp.Contains(x)) missingUp.Remove(x);
        }

        public void AddMissingDown(int x)
        {
            if (x == FindFirstObjectByType<MultipleLevel>().maxX) return;
            if (!missingDown.Contains(x)) missingDown.Add(x);
        }
        public void RemoveMissingDown(int x)
        {
            if (missingDown.Contains(x)) missingDown.Remove(x);
        }

        public void AddMissingLeft(int y)
        {
            if (y == 0) return;
            if (!missingLeft.Contains(x)) missingLeft.Add(x);
        }
        public void RemoveMissingLeft(int y)
        {
            if (missingLeft.Contains(x)) missingLeft.Remove(x);
        }

        public void AddMissingRight(int y)
        {
            if (y == FindFirstObjectByType<MultipleLevel>().maxY) return;
            if (!missingRight.Contains(x)) missingRight.Add(x);
        }
        public void RemoveMissingRight(int y)
        {
            if (missingRight.Contains(x)) missingRight.Remove(x);
        }

        public void Loaded()
        {
            WallManager wm = FindFirstObjectByType<WallManager>();
            MultipleLevel ml = FindFirstObjectByType<MultipleLevel>();
            if (wm == null || ml == null) return;

            for(int i = 0; i < missingUp.Count; i++) {
                wm.outsideWallTilemap.SetTile(new Vector3Int(missingUp[i], ml.firstY, 0), ml.clear);
            }
            for (int i = 0; i < missingDown.Count; i++)
            {
                wm.outsideWallTilemap.SetTile(new Vector3Int(missingDown[i], ml.lastY, 0), ml.clear);
            }
            for (int i = 0; i < missingRight.Count; i++)
            {
                wm.outsideWallTilemap.SetTile(new Vector3Int(ml.firstX, missingLeft[i], 0), ml.clear);
            }
            for (int i = 0; i < missingRight.Count; i++)
            {
                wm.outsideWallTilemap.SetTile(new Vector3Int(ml.firstX, missingRight[i], 0), ml.clear);
            }
        }
    }
}
