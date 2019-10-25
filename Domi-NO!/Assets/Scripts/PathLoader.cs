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
            if(length == 1) {
                g = CornerPool.GetNext();
                g.transform.position = pos2;
            } else {
                for(float j = 1; j < length; j++) {
                    g = BlockPool.GetNext();
                    g.transform.position = pos1 + (pos2 - pos1).normalized * j;
                }
                if(i != nodes.Count - 1) {
                    Vector3 toNext = nodes[i+1].position - nodes[i].position;
                    Quaternion rotation = Quaternion.identity;
                    //if((pos1 - pos2).z > 0 ^ toNext.x > 0) { rotation = Quaternion.Euler(90,90,0); }
                    //if((pos1 - pos2).z != 0 ^ toNext.x < 0) { rotation = Quaternion.Euler(90,90,0); }
                    //if(toNext.z > 0) { rotation = Quaternion.Euler(90,90,0); }

                    g = CornerPool.GetNext();
                    g.transform.position = pos2;
                    g.transform.rotation = rotation;
                }
            }            
        }

        BlockPool.GetNext().transform.position = nodes[nodes.Count - 1].position;
    }
}
