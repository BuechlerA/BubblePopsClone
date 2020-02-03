using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShooterBehaviour : MonoBehaviour
{
    public GameObject bubbleObject;
    private GameObject shotBubble;
    private int bubbleValueInitPowerRange = 9;

    public GameObject bubblePreviewSprite;
    private GameObject bubblePreview;

    public Transform nextBubbleTransform;

    private LineRenderer lr;
    private GameManager gameManager;
    
    public int maxReflectionCount;
    [SerializeField]
    private int currentReflectionCount;

    [SerializeField]
    private List<Vector3> lrPositions;
    private Vector2 shooterPos;
    private Transform gridPos;

    public LayerMask rayCollisionLayers;

    private bool isBubbleAvailable = false;
    

    private void Start()
    {
        lr = GetComponentInChildren<LineRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        shooterPos = transform.position;

        bubblePreview = Instantiate(bubblePreviewSprite, transform) as GameObject;
    }

    private void Update()
    {
        if (gameManager.currentGameState == GameManager.GameStates.Aiming)
        {
            if (Input.GetMouseButton(0))
            {
                lr.enabled = true;
                Aim();
                CreateBubble();
            }
            if (Input.GetMouseButtonUp(0))
            {
                lr.enabled = false;
                bubblePreview.GetComponent<SpriteRenderer>().enabled = false;
                ShootBubble();
            }
        }
    }

    private void Aim()
    {
        Vector2 directionVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionVector.normalized);
        Debug.DrawRay(transform.position, directionVector, Color.cyan);

        currentReflectionCount = 0;
        lrPositions.Clear();
        lrPositions.Add(transform.position);

        if (hit)
        {
            if (hit.collider.gameObject.tag == "Wall")
            {
                Reflect(transform.position, hit);
            }
            else
            {
                lrPositions.Add(hit.point);
                if (hit.collider.gameObject.tag == "Point")
                {
                    AimPreview(hit.collider);
                    gridPos = hit.transform;
                }
            }
        }
        else
        {
            lrPositions.Add(shooterPos + (directionVector - shooterPos).normalized);
        }

        lr.positionCount = lrPositions.Count;
        lr.SetPositions(lrPositions.ToArray());
    }

    private void Reflect(Vector2 position, RaycastHit2D hit)
    {
        if (currentReflectionCount > maxReflectionCount)
        {
            return;
        }
        else
        {
            lrPositions.Add(hit.point);
            currentReflectionCount++;

            Vector2 inDirection = (hit.point - position).normalized;
            Vector2 newDirection = Vector2.Reflect(inDirection, hit.normal);

            var newHit = Physics2D.Raycast(hit.point + (newDirection * 0.1f), newDirection * 100f);
            if (newHit)
            {
                Reflect(hit.point, newHit);
                if (newHit.collider.gameObject.tag == "Point")
                {
                    AimPreview(newHit.collider);
                    gridPos = newHit.transform;
                }
            }
            else
            {
                lrPositions.Add(hit.point + newDirection);
            }
        }
    }

    private void AimPreview(Collider2D collision)
    {
        if (collision.gameObject.tag == "Point" && !collision.gameObject.GetComponent<HexCellBehaviour>().isCellPopulated)
        {
            bubblePreview.GetComponent<SpriteRenderer>().enabled = true;
            bubblePreview.transform.position = collision.transform.position;
            bubblePreview.GetComponent<PreviewBubbleBehaviour>().Punch();
        }
        if (collision.gameObject.tag == "Point" && collision.gameObject.GetComponent<HexCellBehaviour>().isCellPopulated)
        {
            bubblePreview.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void CreateBubble()
    {     
        if (!isBubbleAvailable)
        {
            isBubbleAvailable = true;

            shotBubble = Instantiate(bubbleObject, transform) as GameObject;
            shotBubble.GetComponent<BubbleBehaviour>().Init(bubbleValueInitPowerRange);
            shotBubble.GetComponent<CircleCollider2D>().enabled = false;
        }
        else
        {
            return;
        }
    }

    private void ShootBubble()
    {
        if (lrPositions.Count >= 2)
        {
            shotBubble.GetComponent<BubbleBehaviour>().Move(lrPositions.ToArray(), gridPos.position);
            shotBubble.GetComponent<CircleCollider2D>().enabled = true;
            shotBubble.transform.SetParent(gridPos);

            isBubbleAvailable = false;
        }
        else
        {
            return;
        }
    }
}
