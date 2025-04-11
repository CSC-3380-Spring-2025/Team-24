using System;
using UnityEngine;

public class Lure : MonoBehaviour
{
    public static event Action<Transform> OnLureCreated;
    public static event Action OnLureDestroyed;

    private void Awake()
    {
        OnLureCreated?.Invoke(transform);
    }

    private void OnDestroy()
    {
        OnLureDestroyed?.Invoke();
    }
}
