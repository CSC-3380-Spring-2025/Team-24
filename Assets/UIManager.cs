using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private static GameObject objectInstance;
    void Awake()
    {
        DontDestroyOnLoad(this);

        if (objectInstance == null)
        {
            objectInstance = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
