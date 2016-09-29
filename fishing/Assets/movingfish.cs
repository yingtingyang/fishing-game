using UnityEngine;
using System.Collections;

public class movingfish : MonoBehaviour {
    //const is optionnal, but save time for process
    //declare state constant
    private const int WAITING_ON_LURE = 1;
    private const int FOLLOWING_LURE = 2;
    private const int CAUGHT_ON_LURE = 3;

    //state variable to determine behavior
    private int state = WAITING_ON_LURE;

    private Transform LurePosition;
    private float fishspeed = 5;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (state == WAITING_ON_LURE){
            //DEFAULT
        }
        else if (state == FOLLOWING_LURE) {
            //a lure is within range, swim toward it
            if (LurePosition != null)
            {
                transform.LookAt(LurePosition);
                transform.position += transform.forward * Time.deltaTime * fishspeed;
            }
        }
        else if (state == CAUGHT_ON_LURE)
        {
            transform.position = LurePosition.position;
            //the fish caught the lure and is parent to it
        }
	}

    void OnTriggerEnter(Collider otherCollider)
        {
        state = FOLLOWING_LURE;
        LurePosition = otherCollider.transform;
        }

    void OnTriggerExit()
        {
        state = WAITING_ON_LURE;
        }
    
    void OnCollisionEnter(Collision collisionData)
        {
        state = CAUGHT_ON_LURE;
        LurePosition = collisionData.transform;

        }
   
}
