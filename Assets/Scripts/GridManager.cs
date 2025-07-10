using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int gridWidth = 32;
    [SerializeField] private int gridHeight = 32;
    [SerializeField] private float cellSize = 1f;

    private void Start()
    {
        ArrangeGrid();
    }

    private void ArrangeGrid()
    {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            int x = i % gridWidth;
            int y = i / gridWidth;

            if (y >= gridHeight) { break; }

            Vector3 newPos = new(x * cellSize, y * cellSize, 0);

            Transform child = transform.GetChild(i);
            child.localPosition = newPos;
        }
    }
}
