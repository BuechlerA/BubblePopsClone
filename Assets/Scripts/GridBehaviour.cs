using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    public int rows = 6;
    public int columns = 2;
    public int unpopulatedRows;

    private int[,] hexGrid;
    private int currentColumns;

    public float cellDistance;

    public HexCellBehaviour cellPrefab;

    [SerializeField]
    private HexCellBehaviour[] cells;

    public GameObject bubbleObject;

    private void Awake()
    {
        hexGrid = new int[rows, columns];
        cells = new HexCellBehaviour[rows * columns];

        for (int y = 0, i = 0; y < columns; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                CreateCell(x, y, i++);
                hexGrid[x, y] = Random.Range(0, 15);
                currentColumns = i;
            }
        }

        int emptyRows = unpopulatedRows * 7 ;

        PopulateCells(emptyRows);
    }

    private void CreateCell(int x, int y, int i)
    {
        Vector3 position;
        position.x = (x + y * cellDistance - y / 2) * (HexMetrics.innerRadius * 2f);
        position.y = y * -cellDistance;
        position.z = 0f;

        HexCellBehaviour cell = cells[i] = Instantiate(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
    }

    [ContextMenu("PopulateCells")]
    private void PopulateCells(int rows)
    {
        for (int i = 0; i < cells.Length - rows; i++)
        {
            GameObject bubble = Instantiate(bubbleObject, cells[i].transform, false) as GameObject;
            bubble.GetComponent<BubbleBehaviour>().Init(11);
        }
    }

    [ContextMenu("AddColumn")]
    private void AddColumn()
    {
        CreateCell(rows, currentColumns, currentColumns);
    }
}
