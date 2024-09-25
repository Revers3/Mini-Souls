using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class winShowScript : MonoBehaviour
{
    //this button does not do anything on click, it is only a button for the sake of being able to run scripts
    //these are set in unity
    public Button winButton;
    public Button retryButton;
    public Button exitButton;
    // Start is called before the first frame update
    //disables this button
    void Start()
    {
        winButton.gameObject.SetActive(false);
        
    }
    //activates the win button, try again button, and exit button on the final screen
    public void winShow(Button victoryButton)
    {
        victoryButton.gameObject.SetActive(true);
        victoryButton.interactable=true;
        retryButton.interactable = true;
        retryButton.gameObject.SetActive(true);
        exitButton.interactable = true;
        exitButton.gameObject.SetActive(true);

    }
}
