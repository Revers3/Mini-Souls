using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    //script for the portal object that allows the player to advance in the game
    //value determined by scenes in unity
    //this is the variable that stores the next scene to load the next level
    public int NextsceneBuildIndex;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if colliding with the player object, it will load the next level
        if (collision.gameObject.CompareTag("Player"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(NextsceneBuildIndex, LoadSceneMode.Single);
        }
    }


}
