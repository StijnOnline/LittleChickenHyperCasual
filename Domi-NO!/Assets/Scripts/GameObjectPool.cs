﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool {
    private List<GameObject> pool;
    private Transform root;

    [SerializeField] private GameObject objectPrefab;

    /// <summary>
    /// Creates a GameObjectPool. When root is not set, it will be created.
    /// </summary> 
    public GameObjectPool(GameObject _objectPrefab, string _name, Transform _root = null) {
        objectPrefab = _objectPrefab;

        if(_root == null) {
            root = new GameObject().transform;
            root.name = _name;
        } else { root = _root; }

        pool = new List<GameObject>();

        for(int i = 0; i < 20; i++) {
            GameObject _newObject = GameObject.Instantiate(objectPrefab, root);
            pool.Add(_newObject);
            pool[i].SetActive(false);
        }
    }

    public GameObject GetNext() {
        if(pool.Count > 0) {
            GameObject _obj = pool[0];
            _obj.SetActive(true);
            pool.RemoveAt(0);
            return _obj;
        } else {
            GameObject _newObject = GameObject.Instantiate(objectPrefab, root);
            return _newObject;
        }
    }

    public void Return(GameObject obj) {
        obj.SetActive(false);
        pool.Add(obj);
    }

    public void Return(List<GameObject> list) {
        foreach(GameObject obj in list) {
            obj.SetActive(false);
            if(!pool.Contains(obj))
                pool.Add(obj);
        }
    }
}
