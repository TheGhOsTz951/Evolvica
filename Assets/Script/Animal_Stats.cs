using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animal Stats", menuName = "Evolvica/New Animal Stats", order = 1)]
public class Animal_Stats : ScriptableObject
{
    [SerializeField, Tooltip("How much Life Point an Animal has")]
    public int health = 1;

    [SerializeField]
    public float stamina = 10f;

    public float hunger = 100;

    [SerializeField]
    public float awareness = 100f;

    public float wanderZone = 50f;

    public float speed = 5f;

    public float angularSpeed = 1f;
}
