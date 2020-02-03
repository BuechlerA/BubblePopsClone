using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBubbleBehaviour : MonoBehaviour
{
    [ContextMenu("Punch")]
    public void Punch()
    {
        LeanTween.scale(gameObject, new Vector3(1.2f, 1.2f), 0.5f).setEasePunch().setLoopOnce();
    }
}
