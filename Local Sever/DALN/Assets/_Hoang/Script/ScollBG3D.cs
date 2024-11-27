using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScollBG3D : MonoBehaviour
{
    // Start is called before the first frame update
   
    public float scrollSpeed = 1f;
    private Renderer bgRenderer;


    void Start()
    {
      
        bgRenderer = GetComponent<Renderer>();

        

    }

    void Update()
    {
        bgRenderer.material.mainTextureOffset += new Vector2(scrollSpeed * Time.deltaTime, 0f);
    }

   

   
}
