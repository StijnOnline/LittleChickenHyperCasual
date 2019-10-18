using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    void DropItem(Item i);
    void PickItem(Item draggingObject);
}
