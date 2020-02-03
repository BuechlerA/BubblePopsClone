using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCellBehaviour : MonoBehaviour
{
    public bool isCeiling = false;
    public bool isConnected = false;
    public bool isCellPopulated = false;

    private CircleCollider2D circleCollider;
    [SerializeField]
    private int populatedNeighbours;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        InvokeRepeating("DoCheckUp", 0.1f, 0.03f);
    }

    [ContextMenu("DoCheckUp")]
    public void DoCheckUp()
    {
        CheckIfPopulated();
        CheckIfConnected();

        if (isCellPopulated && isConnected)
        {
            CheckIfCeiling();
        }
    }

    private void CheckIfCeiling()
    {
        RaycastHit2D leftTopHit;
        RaycastHit2D rightTopHit;

        gameObject.layer = LayerMask.NameToLayer("Ignore");
        leftTopHit = Physics2D.Raycast(transform.position, new Vector2(-0.2f, 0.2f).normalized, 1 << LayerMask.NameToLayer("Ceiling"));
        rightTopHit = Physics2D.Raycast(transform.position, new Vector2(0.2f, 0.2f).normalized, 1 << LayerMask.NameToLayer("Ceiling"));
        gameObject.layer = LayerMask.NameToLayer("Points");

        //Debug.Log(leftTopHit.collider);
        //Debug.Log(rightTopHit.collider);

        if (leftTopHit.collider.gameObject.layer == LayerMask.NameToLayer("Ceiling") 
            || rightTopHit.collider.gameObject.layer == LayerMask.NameToLayer("Ceiling"))
        {
            isCeiling = true;
        }
        else
        {
            isCeiling = false;
        }

        Debug.DrawRay(transform.position, new Vector2(-0.1f, 0.1f), Color.green, 2f);
        Debug.DrawRay(transform.position, new Vector2(0.1f, 0.1f), Color.green, 2f);
    }

    private void CheckIfConnected()
    {
        populatedNeighbours = 0;

        var connections = Physics2D.OverlapCircleAll(transform.position, 0.3f, 1 << 10);

        foreach (var item in connections)
        {
            if (item.GetComponent<HexCellBehaviour>().isCellPopulated)
            {
                populatedNeighbours++;
            }
        }

        if (populatedNeighbours <= 0)
        {
            circleCollider.enabled = false;
        }
        else
        {
            circleCollider.enabled = true;
        }
    }

    private void CheckIfPopulated()
    {
        if (transform.childCount >= 1)
        {
            isCellPopulated = true;
        }
        else
        {
            isCellPopulated = false;
        }
    }

}
