using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLoader : MonoBehaviour {
    [SerializeField] private GameObject BlockPrefab;
    [SerializeField] private GameObject CornerPrefab;
    [SerializeField] private Path path;
    private List<Transform> nodes;

    public static GameObjectPool BlockPool;
    public static GameObjectPool CornerPool;



    void Start() {
        BlockPool = new GameObjectPool(BlockPrefab, "BlockPool");
        CornerPool = new GameObjectPool(CornerPrefab, "CornerPool");

        nodes = path.GetPath();

        BlockPool.GetNext().transform.position = nodes[0].position;

        for(int i = 1; i <= nodes.Count - 1; i++) {
            GameObject g;
            Vector3 pos1 = nodes[i - 1].position;
            Vector3 pos2 = nodes[i].position;

            float length = (pos1 - pos2).magnitude;

            for(float j = 1; j < length; j++) {
                g = BlockPool.GetNext();
                g.transform.position = pos1 + (pos2 - pos1).normalized * j;
            }

            if(i != nodes.Count - 1) {
                Vector3 dir = pos1 - nodes[i + 1].position;
                Vector3 rotation = new Vector3(90, 0, 0); ;
                if((dir.x < 0 ^ dir.z > 0)) { rotation += new Vector3(0, 90, 0); }
                if((dir.x < 0 ^ (pos2 - pos1).x == 0)) { rotation += new Vector3(0, 180, 0); }

                g = CornerPool.GetNext();
                g.transform.position = pos2;
                g.transform.rotation = Quaternion.Euler(rotation);
            }
        }

        BlockPool.GetNext().transform.position = nodes[nodes.Count - 1].position;
    }
}
