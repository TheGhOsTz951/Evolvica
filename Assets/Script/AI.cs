using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI : MonoBehaviour
{
    public enum AI_type {PLANT, ANIMAL};
    [SerializeField]
    public AI_type AI_TYPE = AI_type.ANIMAL; 

    private static List<AI> allAI = new List<AI>();

    public static List<AI> AllAI
    {
        get { return allAI; }
    }

    public bool isAnimal()
    {
        return AI_TYPE == AI_type.ANIMAL ? true : false;
    }
}
