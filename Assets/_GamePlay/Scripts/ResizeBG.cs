using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeBG : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private SpriteRenderer spriteRenderer;
    void Start()
    {
        ResizeBackground();
    }

    void ResizeBackground()
    {
        
        float cameraHeight = cam.orthographicSize * 2;
        float cameraWidth = cameraHeight * cam.aspect;

        
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        float scaleX = cameraWidth / spriteWidth;
        float scaleY = cameraHeight / spriteHeight;

        transform.localScale = new Vector3(scaleX, scaleY * 2, 1f);
    }
}
