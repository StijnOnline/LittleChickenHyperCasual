using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    Item draggingObject;
    public float delta = 0.2f;

    void Update()
    {
        if(Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);

            Vector3 touchedPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
            Ray touchRay = new Ray(Vector3.forward, touchedPos);
            RaycastHit2D hit = Physics2D.Raycast(touchedPos, Vector2.zero, (LayerMask.NameToLayer("Item") | LayerMask.NameToLayer("Item_NoCollision")));
            
            if(touch.phase == TouchPhase.Began) {
                draggingObject = hit.transform.GetComponent<Item>();                    
            }

            if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) { 
                //test where Item is dropped
                hit = Physics2D.Raycast(touchedPos, Vector2.zero, ~(LayerMask.NameToLayer("Item") | LayerMask.NameToLayer("Item_NoCollision")));
                Interactable i = hit.transform.GetComponent<Interactable>();
                if(i != null) {
                    i.DropItem(draggingObject);
                }

                draggingObject = null;
            }

            if(draggingObject.transform != null) {
                draggingObject.transform.position = Vector3.Lerp(draggingObject.transform.position, touchedPos, delta);
            }
        }
    }
}
