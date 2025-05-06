using System;
using UnityEngine;

public class Lure : MonoBehaviour
{
    public static event Action<Transform> OnLureCreated;

    private void Awake()
    {
        OnLureCreated?.Invoke(transform);
    }

}
