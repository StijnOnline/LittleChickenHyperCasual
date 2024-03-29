﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageLoader : MonoBehaviour {
    public bool revealOnStart = false;

    [SerializeField] private Texture2D image;

    public static GameObjectPool dominoPool;
    [SerializeField] private GameObject dominoPrefab;

    void Start() {
        dominoPool = new GameObjectPool(dominoPrefab, "dominoPool",transform);

        for(int i = 0; i < image.width; i++) {
            for(int j = 0; j < image.height; j++) {
                Color pixel = image.GetPixel(i, j);

                if(pixel.a != 0) {
                    GameObject go = dominoPool.GetNext();
                    go.transform.localPosition = new Vector3(i - image.width / 2f,0, j - image.height / 2f);
                    Material mat = go.GetComponent<Renderer>().material;
                    mat.SetColor("_BaseColor", pixel);

                    go.SetActive(false);
                }

            }
        }
        if(revealOnStart) { Reveal(); }
    }

    public float Reveal() {
        StartCoroutine(RevealDelayed());
        return 0.1f * transform.childCount;
    }

    private IEnumerator RevealDelayed() {
        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(3f / (image.width * image.height));
        }
    }
}