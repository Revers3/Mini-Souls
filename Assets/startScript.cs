using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startScript : MonoBehaviour
{
    /*--this is the script attached to the try again, start, and exit buttons--*/
    //these are the functions attached to the buttons being clicks
    //they have a built in tool for buttons, so they do not need to be added to this script


    //assigned by unity in building, so it scene will know which one to call
    public int sceneBuildIndex;
    //this one is set to the first area, and is assinged to both the try again and start buttons to start the game over
    public void startGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
        
    }
    //function for quitting the application
    public void QuitGame()
    {
        
        Application.Quit();
    }





}
