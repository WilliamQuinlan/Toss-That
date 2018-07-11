﻿//Imports for Unity C# Programming
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This class is used for Cup Tipping Behavior
The Table object is linked to this script.

This Script's Behavior:
Checks if the cup tipped on the table.
*/
public class BarCupFall : MonoBehaviour {
    /*
    Substate Variables
    */
    private enum state { fell, none, pause, paused, unpause };
    private state substate;
    private state savedState;
    private Vector3 savedVelocity;
    /*
    Private Members - used for re-initialization on 
    a Scene Change.
    */
    private static float[] cupOri = { 0, 0, 0 };
    private static GameObject cup = null;
	/*
	These varibles are for the random respawning and location of the glassware
	*/
	public Transform spawnLocation;
	public GameObject[] glasswarePrefab;

    /*
    This is a UnityEngine function defined by MonoBehaviour.
    This function only activates when the Scene is loaded, even if the script is disabled.
    If the scene is reloaded, we need to make our own function: RepeatStart.

    This function initializes all member variables.
    */

	/*
	This Coroutine makes sure when the glassware is falls it waits and respawns a new cup from a random selection of prefabs
	* */
	private IEnumerator WaitForIt()
	{
		Destroy (cup);
		yield return new WaitForSeconds (1f);
		GameObject prefabClone = (GameObject)Instantiate (glasswarePrefab[Random.Range(0,glasswarePrefab.Length)], spawnLocation.transform.position, Quaternion.Euler (0, 0, 0));
		prefabClone.name = "CupRigidbody";
		cup = GameObject.Find("CupRigidbody");

	}

    void Awake()
    {
        substate = BarCupFall.state.none;
		cup = GameObject.Find ("CupRigidbody");
        //save cup's original position.
        cupOri[0] = cup.transform.position.x;
        cupOri[1] = cup.transform.position.y;
        cupOri[2] = cup.transform.position.z;
    }

    
    /*
    This is a UnityEngine function defined by MonoBehaviour.
    This function activates every 16.67 milliseconds (for 60 frames-per-second).

    Here, the Update function checks what this script's substate is, then takes the
    appopriate action.
    */
    void Update()
    {

		if (substate == BarCupFall.state.fell)
        {
			//StartCoroutine(WaitForIt ());
            //GameObject.Find("gameOverCanvas").GetComponent<GameOver>().GameOverSet();
            substate = BarCupFall.state.none;
        }
        else if (substate == BarCupFall.state.none)
        {

        }
        else if (substate == BarCupFall.state.pause)
        {
            cup.GetComponent<Rigidbody>().useGravity = false;
            cup.GetComponent<Rigidbody>().isKinematic = true;
            savedVelocity = cup.GetComponent<Rigidbody>().velocity;
            cup.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            
            substate = BarCupFall.state.paused;
        }
        else if (substate == BarCupFall.state.paused)
        {

        }
        else if (substate == BarCupFall.state.unpause)
        {
            cup.GetComponent<Rigidbody>().useGravity = true;
            cup.GetComponent<Rigidbody>().isKinematic = false;
            cup.GetComponent<Rigidbody>().velocity = savedVelocity;

            substate = savedState;
        }	
    }


    /*
    This is a Unity Event Handler defined by MonoBehaviour.
    It activates whenever a GameObject
    with Collider col enters the Collider of this script's gameobject (a cup).
    */
    void OnCollisionEnter(Collision col)
    {
        /*
        This conditional requires some explanation. 
        col.gameObject.name refers to the name of the parent object that collided with
        the Table.
        col.collider.gameObject.name refers to the exact child of the parent object that
        collided with the Table.

        We want the game over to be thrown when the cup tips. However, CupRB (22) is the 
        collider for the bottom of the cup. We don't want the game to end if the bottom of the cup
        touches the table (because it's underwhelming). But if any other part touches the table,
        then we activate the Game Over in MainGameLoop.
        */
        if (col.gameObject.name == "CupRigidbody" && col.collider.gameObject.name != "CupRB (22)")
        {
            substate = BarCupFall.state.fell;
            return;
        }
    }
    public void pauseGame()
    {
        savedState = substate;
        substate = BarCupFall.state.pause;
    }
    public void unpauseGame()
    {
        substate = BarCupFall.state.unpause;
    }
}