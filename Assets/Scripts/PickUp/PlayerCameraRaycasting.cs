using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraRaycasting : MonoBehaviour
{
   public float raycastDistance = 7f; 
   private ILootable currentItem;

   private void Update() 
   {
       HandleRayCast();
       if(Input.GetKeyDown(KeyCode.E))
       {
           if(currentItem != null)
           {
           currentItem.OnInteract();
           }
       }
   }

    private void HandleRayCast()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance))
        {
            ILootable lootable = hit.collider.GetComponent<ILootable>();

            // if player is looking at the lootable item 
            if(lootable != null)
            {
                if(lootable == currentItem)
                {return;}
                // looking at another lootable item
                else if(currentItem != null)
                {
                    currentItem.OnEndLook();
                    currentItem = lootable;
                    currentItem.OnStartLook();
                }
                else
                {
                    currentItem = lootable;
                    currentItem.OnStartLook();
                }
            }
            // Not lootable
            else 
            {
                if(currentItem != null)
                {
                    currentItem.OnEndLook();
                    currentItem = null;
                }
            } 
        } // raycast 
        // raycast is false
        else
        {
            if(currentItem != null)
            {
                currentItem.OnEndLook();
                currentItem = null;
            }

            
        }
    }
} // main
