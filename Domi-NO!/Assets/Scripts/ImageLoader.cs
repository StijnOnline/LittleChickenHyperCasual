using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageLoader : MonoBehaviour
{
    [SerializeField] private Texture2D image;
    void Start()
    {
        for(int i = 0; i < image.width; i++)
            for(int j = 0; j < image.height; j++) {
                Color pixel = image.GetPixel(i, j);

                Debug.Log(pixel);
            }
        
    }
}
