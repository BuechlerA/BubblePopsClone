using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITopBehaviour : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI perfectText;

    private bool isSendingMessage = false;

    private void Awake()
    {
        scoreText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetScoreText(int score)
    {
        PunchTextMesh(scoreText);
        scoreText.text = score.ToString();
    }

    public void SendString(string message)
    {
        if (!isSendingMessage)
        {
            isSendingMessage = true;
            perfectText.gameObject.SetActive(true);
            PunchTextMesh(perfectText);
            perfectText.text = message;
            LeanTween.scale(perfectText.gameObject, new Vector3(0f, 0f), 4f).setEaseInOutElastic();
            isSendingMessage = false;
        }
    }

    private void PunchTextMesh(TextMeshProUGUI textToPunch)
    {
        if (!LeanTween.isTweening(textToPunch.gameObject))
        {
            LeanTween.scale(textToPunch.gameObject, new Vector3(1.5f, 1.5f), 0.3f).setEasePunch().setLoopClamp().setLoopCount(1);
        }
    }

}
