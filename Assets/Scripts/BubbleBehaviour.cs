using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BubbleBehaviour : MonoBehaviour
{
    public int bubbleValue;
    private int power;

    public GameObject bubbleParticle;

    private TextMeshProUGUI valueText;
    private SpriteRenderer bubbleSprite;

    public AudioClip bubblePopSound;
    public AudioClip bubbleIncrementSound;
    private AudioSource bubbleAudioSource;

    public ColorList colorValues;
    private Color[] Colors;

    private Collider2D bubbleCollider;
    public LayerMask collisionLayer;
    private bool isExploded = false;

    private GameManager gameManager;

    private void Awake()
    {
        valueText = GetComponentInChildren<TextMeshProUGUI>();
        bubbleSprite = GetComponent<SpriteRenderer>();
        bubbleAudioSource = GetComponent<AudioSource>();
        bubbleCollider = GetComponent<CircleCollider2D>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Colors = colorValues.Colors;
        //Init(11);
    }

    public void Init(int powerRange)
    {
        power = GetRandomPowerValue(powerRange);
        bubbleValue = (int)Mathf.Pow(2, power);
        SetValueText();
        SetValueColor();
    }

    private void SetValueText()
    {
        valueText.text = bubbleValue.ToString();
    }

    private void SetValueColor()
    {
        bubbleSprite.color = Colors[power-1];
    }

    [ContextMenu("Increment")]
    public void Increment()
    {
        if (bubbleValue <= 2024)
        {
            bubbleValue *= 2;
            power++;
            TweenPunch();
            SetValueText();
            SetValueColor();
            PlaySound(bubbleIncrementSound);

            Merge();            
        }
        if (bubbleValue >= 2048)
        {         
            Explode();
        }
    }

    [ContextMenu("Merge")]
    public void Merge()
    {
        var bubbles = GetNeighbours();

        foreach (var item in bubbles)
        {        
            if (item.GetComponent<BubbleBehaviour>().bubbleValue == bubbleValue)
            {
                LeanTween.move(gameObject, item.transform.position, 0.3f).setEaseInBack();
                item.GetComponent<BubbleBehaviour>().Increment();
                CheckForConnections();
                Explode();
            }
        }

        
    }

    [ContextMenu("Explode")]
    public void Explode()
    {      
        if (!isExploded)
        {
            isExploded = true;

            if (bubbleValue >= 2048)
            {
                //Run RippleEffect Shader
                Camera.main.GetComponent<RipplePostProcessor>().RippleEffect(transform.position);
                //Run Confetti Effect
                gameManager.GetComponent<ConfettiCannonBehaviour>().ConfettiApplause();
                gameManager.GetComponent<ConfettiCannonBehaviour>().FireConfettiCannons();

                var explodeList = GetNeighbours();

                foreach (var item in explodeList)
                {
                    item.gameObject.GetComponent<BubbleBehaviour>().Explode();
                }
            }
            if (bubbleValue < 2048)
            {
                var neighbourList = GetNeighbours();

                foreach (var item in neighbourList)
                {
                    item.gameObject.GetComponent<BubbleBehaviour>().CheckForConnections();
                }
            }
            if (bubbleValue >= 32)
            {
                gameManager.GetComponent<ConfettiCannonBehaviour>().FireConfettiCannons();
            }

            LeanTween.scale(gameObject, new Vector3(0f, 0f), 0.5f).setEaseOutCirc();

            //If this gets null exception bubbles move into empty fields
            GetComponentInChildren<TrailRenderer>().enabled = false;

            GameObject particle = Instantiate(bubbleParticle, transform.position, Quaternion.identity);
            particle.GetComponent<ParticleBehaviour>().Init(Colors[power-1]);

            gameManager.AddScore(bubbleValue);

            PlaySound(bubblePopSound);
            Destroy(gameObject, 0.5f);
        }
        else
        {
            return;
        }
    }

    public void Move(Vector3[] positions, Vector3 gridPosition)
    {
        //Debug.Log(positions.Length);

        if (positions.Length == 2)
        {
            LeanTween.move(gameObject, positions[1], 0.2f).setEaseLinear();
            LeanTween.move(gameObject, gridPosition, 0.2f).setEaseOutBack().setDelay(0.2f).setOnComplete(Merge);
        }
        if (positions.Length == 3)
        {
            LeanTween.move(gameObject, positions[1], 0.2f).setEaseLinear();
            LeanTween.move(gameObject, positions[2], 0.2f).setEaseLinear();
            LeanTween.move(gameObject, gridPosition, 0.2f).setEaseOutBack().setDelay(0.2f).setOnComplete(Merge);
        }
    }

    [ContextMenu("CheckForConnection")]
    public void CheckForConnections()
    {
        if (GetNeighbours().Length <= 0)
        {
            Drop();
        }
    }

    [ContextMenu("CheckIfConnectedToCeiling")]
    public void CheckIfConnectedToCeiling()
    {
        RaycastHit2D leftTopHit;
        RaycastHit2D rightTopHit;

        gameObject.layer = LayerMask.NameToLayer("Ignore");
        leftTopHit = Physics2D.Raycast(transform.position, new Vector2(-0.2f, 0.2f).normalized, 1 << LayerMask.NameToLayer("Bubbles"));
        rightTopHit = Physics2D.Raycast(transform.position, new Vector2(0.2f, 0.2f).normalized, 1 << LayerMask.NameToLayer("Bubbles"));
        gameObject.layer = LayerMask.NameToLayer("Bubbles");

        Debug.Log(leftTopHit.collider);
        Debug.Log(rightTopHit.collider);

        Debug.DrawRay(transform.position, new Vector2(-0.2f, 0.2f).normalized, Color.green, 2f);
        Debug.DrawRay(transform.position, new Vector2(0.2f, 0.2f).normalized, Color.green, 2f);
    }

    [ContextMenu("Drop")]
    public void Drop()
    {
        if (!GetComponent<Rigidbody2D>())
        {
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>() as Rigidbody2D;
            rb.gravityScale = 1f;
        }       
        transform.SetParent(null);
        gameObject.layer = LayerMask.NameToLayer("Ignore");
    }

    private void PlaySound(AudioClip soundClip)
    {
        if (bubbleAudioSource.isPlaying)
        {
            bubbleAudioSource.Stop();
        }
        bubbleAudioSource.PlayOneShot(soundClip);
    }

    public void TweenPunch()
    {
        if (!LeanTween.isTweening(gameObject) && !LeanTween.isTweening(valueText.gameObject))
        {
            LeanTween.scale(gameObject, new Vector3(1.5f, 1.5f), 0.5f).setEasePunch().setLoopOnce();
            LeanTween.scale(valueText.gameObject, new Vector3(2f, 2f), 0.5f).setEasePunch().setLoopOnce();
        }
    }

    private Collider2D[] GetNeighbours()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore");
        var List = Physics2D.OverlapCircleAll(transform.position, 0.5f, collisionLayer);
        gameObject.layer = LayerMask.NameToLayer("Bubbles");
        return List;
    }

    private int GetRandomPowerValue(int range)
    {
        int result = Random.Range(1, range);
        //int result = (int)Mathf.Pow(2, randomPower);  
        return result;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Edge"))
        {
            Explode();
        }
    }
}
