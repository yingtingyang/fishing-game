using UnityEngine;
using System.Collections;

public class changeparent : MonoBehaviour {
    public Transform thingToFollow;
    public float speed = 1;
    // Use this for initialization
    void Start()
    {
        	transform.parent = GameObject.Find ("Fish").transform;
        // this is how to change the hierarchy dynamically in Unity 
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(thingToFollow);
        transform.position += transform.forward * Time.deltaTime * speed;
    }
}
