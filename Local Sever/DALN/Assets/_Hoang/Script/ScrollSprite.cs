using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;  // Kéo SpriteRenderer của cửa vào đây
    public float openSpeed = 50f;  // Tốc độ mở cửa (theo pixel)

    private Sprite originalSprite;
    private Rect rect;

    void Start()
    {
        originalSprite = spriteRenderer.sprite;
        rect = originalSprite.rect;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.O))  // Mở cửa
        {
            rect.width += openSpeed * Time.deltaTime;
            if (rect.width > originalSprite.rect.width)
                rect.width = originalSprite.rect.width;
        }
        else if (Input.GetKey(KeyCode.C))  // Đóng cửa
        {
            rect.width -= openSpeed * Time.deltaTime;
            if (rect.width < 0) rect.width = 0;
        }

        // Cập nhật Sprite với phần đã cắt
        spriteRenderer.sprite = Sprite.Create(
            originalSprite.texture,
            rect,
            new Vector2(0.5f, 0.5f)
        );
    }
}
