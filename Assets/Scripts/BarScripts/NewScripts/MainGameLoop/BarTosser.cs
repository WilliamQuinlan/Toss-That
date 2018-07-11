﻿//Imports for Unity C# Programming
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This class represents step 1
*/
public class BarTosser : MonoBehaviour {
    /*
    Private Substate Variables
    */
    private enum state { waitforClick, waitforFlick, none, pause, paused, unpause};
    private state substate;
    private state savedState;
    /*
    Private Variables
    */
    //for toss mechanic
    private Vector3 touchPosition;
    /*
    Private Const Variables
    */
    //for toss mechanic
    private const float slowerX = 0.025F;
    private const float slowerY = 0.025F;
    private const float expanderZ = 1F;

    // Use this for initialization
    void Awake () {
        touchPosition = new Vector3(0, 0, 0);
        substate = BarTosser.state.waitforClick;
    }
	
	// Update is called once per frame
	void Update () {
        if (substate == BarTosser.state.waitforClick)
        {

        }
        else if (substate == BarTosser.state.waitforFlick)
        {
            if (Input.GetMouseButtonUp(0))
            {
                //When cube is flicked up play throw sound
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = Resources.Load("Sounds/ThrowSound") as AudioClip;
                audioSource.Play();
                //get the vector representing the difference between
                //the position of the mouse release and the position of the
                //mouse click.
                Vector2 deltaSwipe = Input.mousePosition - touchPosition;
                Debug.Log("curr pos:" + Input.mousePosition);
                Debug.Log("orig pos:" + touchPosition);
                Debug.Log("delta: " + deltaSwipe);

                //calculate the z velocity with the pythagorean theorm.
                float zVal = 0;
                if (deltaSwipe.y < 0)
                {
                    zVal = -1 * expanderZ * Mathf.Sqrt(Mathf.Pow(deltaSwipe.x * slowerX, 2) + Mathf.Pow(deltaSwipe.y * slowerY, 2));
                }
                else
                {
                    zVal = expanderZ * Mathf.Sqrt(Mathf.Pow(deltaSwipe.x * slowerX, 2) + Mathf.Pow(deltaSwipe.y * slowerY, 2));
                }
                Debug.Log("zVal: " + zVal);
                //set the iceObj's velocity for the ice cube's click and release.
                gameObject.GetComponent<Rigidbody>().velocity = new Vector3(deltaSwipe.x * slowerX,
                    deltaSwipe.y * slowerY, zVal);
                //allow the iceCube to move.
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
                gameObject.GetComponent<Rigidbody>().useGravity = true;

                GameObject.Find("Table").GetComponent<BarMelter>().stopMelter();

                gameObject.GetComponent<BarIceCollider>().enabled = true;
                //disable this script, wil set substate to none in OnDisable
                //no logic should come after this code block and after the if-elseif's here.
                gameObject.GetComponent<BarTosser>().enabled = false;

            }
        }
        else if (substate == BarTosser.state.none)
        {

        }
        else if (substate == BarTosser.state.pause)
        {
            substate = BarTosser.state.paused;
        }
        else if (substate == BarTosser.state.paused)
        {

        }
        else if (substate == BarTosser.state.unpause)
        {
            substate = savedState;
        }
    }
    /*
    Do not use onEnable. Because of the substate
    void OnEnable()
    {

    }
    */
    void OnDisable()
    {
        substate = BarTosser.state.none;
    }

    void OnMouseDown()
    {
        touchPosition = Input.mousePosition;
        substate = BarTosser.state.waitforFlick;
    }

    public void pauseGame()
    {
        savedState = substate;
        substate = BarTosser.state.pause;
    }
    public void unpauseGame()
    {
        substate = BarTosser.state.unpause;
    }
}