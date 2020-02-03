using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int score;

    public GameStates currentGameState;

    public UITopBehaviour UIContainer;

    public StringList messageStrings;
    private string[] Messages;

    private void Awake()
    {
        currentGameState = GameStates.Aiming;
        score = 0;
        Messages = messageStrings.messages;
    }

    public void AddScore(int poppedBubbleValue)
    {
        score += poppedBubbleValue;
        UIContainer.SetScoreText(score);
        if (poppedBubbleValue >= 32)
        {
            UIContainer.SendString(Messages[Random.Range(0, Messages.Length)]);
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(0);
    }

    public enum GameStates
    {
        Aiming,
        Shooting,
        Merging,
    }

    //Cheating/Debug Functions
    public void ForcePerfect()
    {
        GameObject[] allBubbles = GameObject.FindGameObjectsWithTag("Bubble");
        foreach (var item in allBubbles)
        {
            item.GetComponent<BubbleBehaviour>().Explode();
        }
    }

    public void ForceIncrement()
    {
        GameObject[] allBubbles = GameObject.FindGameObjectsWithTag("Bubble");
        foreach (var item in allBubbles)
        {
            item.GetComponent<BubbleBehaviour>().Increment();
        }
    }
}
