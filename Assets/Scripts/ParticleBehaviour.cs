using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehaviour : MonoBehaviour
{
    public void Init(Color initColor)
    {
        var main = GetComponent<ParticleSystem>().main;
        main.startColor = initColor;
        GetComponent<ParticleSystem>().Play();
    }
}
