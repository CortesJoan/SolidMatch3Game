using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width;
    public int height;
    public float cellSize;
    public GameObject cellPrefab;

    private GameObject[,] cells;

    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        cells = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cellSize, y * cellSize, 0);
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity);
                cells[x, y] = cell;
            }
        }
    }
    
}
