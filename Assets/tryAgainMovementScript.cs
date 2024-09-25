using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tryAgainMovementScript : MonoBehaviour
{
    /*--this script is for the exit, and try again buttons--*/
    
    public Button thisButton;
    // Start is called before the first frame update
    //disables the buttons on start
    void Start()
    {
        thisButton.gameObject.SetActive(false);
        thisButton.interactable = false;

    }
   
    //when called from the player death, it reenables the buttons
    public void EnableRetryButton(Button retryButton)
    {
        retryButton.interactable = true;
        retryButton.gameObject.SetActive(true);
    }

    public void EnableExitButton(Button exitButton)
    {
        exitButton.interactable = true;
        exitButton.gameObject.SetActive(true);
    }
    
}
