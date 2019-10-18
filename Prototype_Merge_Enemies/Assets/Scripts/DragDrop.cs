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
            RaycastHit2D itemHit = Physics2D.Raycast(touchedPos, Vector2.zero, (LayerMask.NameToLayer("Item") | LayerMask.NameToLayer("Item_NoCollision")));
            
            if(touch.phase == TouchPhase.Began) {
                draggingObject = itemHit.transform.GetComponent<Item>();

                int mask = ~(LayerMask.NameToLayer("Item") | LayerMask.NameToLayer("Item_NoCollision"));
                RaycastHit2D interatableHit = Physics2D.Raycast(touchedPos, Vector2.zero, );
                Interactable i = interatableHit.transform.GetComponent<Interactable>();
                if (draggingObject != null && i != null)
                {
                    i.PickItem(draggingObject);
                }                                  
            }

            if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                //test where Item is dropped
                RaycastHit2D interatableHit = Physics2D.Raycast(touchedPos, Vector2.zero, ~(LayerMask.NameToLayer("Item") | LayerMask.NameToLayer("Item_NoCollision")));
                Interactable i = interatableHit.transform.GetComponent<Interactable>();
                if(draggingObject != null && i != null) {
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
