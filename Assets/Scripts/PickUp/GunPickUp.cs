using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickUp : MonoBehaviour, ILootable
{
    public WeaponScript gun;
    
    public void OnEndLook()
    {
        
    }

    public void OnInteract()
    {
        weaponHandler.instance.PickUpGun(gun);
        Destroy(gameObject);
    }

    public void OnStartLook()
    {
        
    }

}
