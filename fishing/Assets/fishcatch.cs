using UnityEngine;
using System.Collections;

public class fishcatch : MonoBehaviour {
    public float lure;

    fishmovement fm;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
      

    }
     void OnTriggerEnter(Collider otherGOCollider)
    {
        if (otherGOCollider.gameObject.tag == "lure")
        {


            fm = GetComponent<fishmovement>();
            fm.enabled = false;
            transform.parent = otherGOCollider.transform;
        }
    }
}
