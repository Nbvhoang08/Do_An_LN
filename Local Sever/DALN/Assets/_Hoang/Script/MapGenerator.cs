using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject gridPrefab; // Prefab của ô lưới
    public int rows; // Số hàng của bàn cờ
    public int cols; // Số cột của bàn cờ
    public float tileSize = 0.8f; // Kích thước của mỗi ô

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        // Tính toán offset để ô trung tâm nằm ở vị trí (0, 0)
        float xOffset = (cols - 1) * tileSize / 2;
        float yOffset = (rows - 1) * tileSize / 2;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // Tính toán vị trí của ô lưới với offset
                float xPos = col * tileSize - xOffset;
                float yPos = row * tileSize - yOffset;

                // Tạo ra một ô lưới mới
                GameObject tile = Instantiate(gridPrefab, new Vector2(xPos, yPos), Quaternion.identity);

                // Đặt tile dưới GameObject chính để giữ mọi thứ gọn gàng
                tile.transform.SetParent(this.transform);

                // Đổi màu ô lưới theo vị trí chẵn/lẻ
                bool isEven = (row + col) % 2 == 0;
                SpriteRenderer tileRenderer = tile.GetComponent<SpriteRenderer>();
                tileRenderer.color = isEven ? Color.white : Color.black;
            }
        }

        // Đặt vị trí của GameObject chính
        transform.position = Vector2.zero;
    }
}
