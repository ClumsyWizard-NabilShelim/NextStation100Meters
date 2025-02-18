using System;
using System.Collections;
using System.Collections.Generic;
using ClumsyWizard.Utilities;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;

namespace ClumsyWizard.Gameplay
{
    [Serializable]
    public class GridData
    {
        public int CellX;
        public int CellY;
        public int CellZ;
        public float CellRadius;
        public GridType GridType;
    }

    public enum GridType
    {
        TwoD,
        ThreeD
    }

    [Serializable]
    public class Cell
    {
        public Vector3 Position;
        public Vector3Int Index;

        public Cell(Vector3 position, Vector3Int index)
        {
            Position = position;
            Index = index;
        }
    }

    public class CW_GridSystem<TCell> where TCell : Cell, new()
    {
        public delegate TCell CreateCellFunction(Vector3 position, Vector3Int index);

        private TCell[,,] grid;

        private GridData gridData;
        private float cellDiameter;
        private Vector3 worldOrigin;

        public CW_GridSystem(Vector3 center, GridData gridData, CreateCellFunction createCellCallback)
        {
            this.gridData = gridData;
            cellDiameter = gridData.CellRadius * 2.0f;

            worldOrigin = center - (Vector3.right * gridData.CellX * gridData.CellRadius) - (gridData.GridType == GridType.ThreeD ? 
                (Vector3.back * gridData.CellZ * gridData.CellRadius) 
                :
                (Vector3.up * gridData.CellY * gridData.CellRadius)
            );

            grid = new TCell[gridData.CellX, gridData.CellY, gridData.CellZ];

            for (int x = 0; x < gridData.CellX; x++)
            {
                for (int z = 0; z < gridData.CellZ; z++)
                {
                    for (int y = 0; y < gridData.CellY; y++)
                    {
                        Vector3 cellPosition = worldOrigin + new Vector3(x * cellDiameter + gridData.CellRadius, y * cellDiameter + gridData.CellRadius, z * cellDiameter + gridData.CellRadius);
                        grid[x, y, z] = createCellCallback(cellPosition, new Vector3Int(x, y, z));
                    }
                }
            }
        }

        public TCell GetCell(Vector3 worldPosition, bool clamp)
        {
            int x = Mathf.FloorToInt((worldPosition - worldOrigin).x / cellDiameter);
            int y = Mathf.FloorToInt((worldPosition - worldOrigin).y / cellDiameter);
            int z = Mathf.FloorToInt((worldPosition - worldOrigin).z / cellDiameter);

            if (clamp)
            {
                if (x < 0)
                    x = 0;
                if (x > gridData.CellX - 1)
                    x = gridData.CellX - 1;

                if (y < 0)
                    y = 0;
                if (y > gridData.CellY - 1)
                    y = gridData.CellY - 1;

                if (z < 0)
                    z = 0;
                if (z > gridData.CellZ - 1)
                    z = gridData.CellZ - 1;
            }

            return GetCell(x, y, z);
        }

        public TCell GetCell(Vector3Int index)
        {
            if (IsValid(index.x, index.y, index.z))
                return grid[index.x, index.y, index.z];
            else
                return null;
        }

        public TCell GetCell(int x, int y, int z)
        {
            if (IsValid(x, y, z))
                return grid[x, y, z];
            else
                return null;
        }

        public void ForEach(Action<TCell> callback)
        {
            for (int x = 0; x < gridData.CellX; x++)
            {
                for (int z = 0; z < gridData.CellZ; z++)
                {
                    for (int y = 0; y < gridData.CellY; y++)
                    {
                        callback(grid[x, y, z]);
                    }
                }
            }
        }

        //Helper Functions
        private bool IsValid(int x, int y, int z)
        {
            if (x >= 0 && x < gridData.CellX && y >= 0 && y < gridData.CellY && z >= 0 && z < gridData.CellZ)
                return true;

            return false;
        }
        public int GetDistanceInCells(Cell fromCell, Cell toCell)
        {
            Vector3 dir = fromCell.Position - toCell.Position;
            int distance = Mathf.FloorToInt(dir.magnitude / cellDiameter);

            return distance;
        }
        //Debug
        public void DebugGrid(Color color)
        {
            Gizmos.color = color;
            foreach (TCell cell in grid)
            {
                Gizmos.DrawWireCube(cell.Position, new Vector3(cellDiameter, cellDiameter, cellDiameter));
            }
        }
    }
}