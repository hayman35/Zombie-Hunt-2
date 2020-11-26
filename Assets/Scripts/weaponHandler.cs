using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponHandler : MonoBehaviour
{
    public static weaponHandler instance;

    [SerializeField] private WeaponScript curentGun;

    //All the weapons in the inventory
	GameObject[] weapons = new GameObject[2];

    private int cur = 0;

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
        }
    }

    private void Start() 
    {
        weapons[0] = curentGun.gameObject;
    }

    private void Update() 
    {
        //Switch weapon on TAB
		if (Input.GetKeyDown (KeyCode.Tab)) {
			StartCoroutine ("SwitchWeapon");
		}
    }


    public void PickUpGun(WeaponScript gun)
    {
        
        weapons[1] = Instantiate(gun.gameObject,transform);
        weapons[1].SetActive(false);
        
    }

    IEnumerator SwitchWeapon () {
		//Play ther current weapon's Lower animation
		weapons [cur].GetComponent<Animator> ().CrossFade ("Lower",0.15f);

		//Give it time to finish
		yield return new WaitForSeconds (0.5f);

		//Disable the current weapon
		weapons [cur].SetActive(false);

		//Go to the next weapon in the array. If we reach the end of the array, go back to the start
		cur++;
		if (cur >= weapons.Length)
			cur = 0;

		//Activate the new current weapon
		weapons [cur].SetActive(true);

		//Play ther current weapon's Raise animation
		weapons [cur].GetComponent<Animator> ().CrossFade ("Raise",0f);
	}

}
