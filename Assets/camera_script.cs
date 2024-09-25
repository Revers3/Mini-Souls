using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_script : MonoBehaviour
{
    //this script is attached to the camera, so the transform is the camera following the player
    //unity is storing a reference to the player object here
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //every frame it is going to match the position of the players, to follow it
        transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
    }
}
