using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
public class SellPanel : MonoBehaviour
{
    public TextMeshProUGUI cost;
    private float value;
    public Transform point;
    public GameObject item;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Display(GameObject item)
    {
        item.transform.position = point.TransformPoint(point.transform.position);

        if(item.GetComponent<SerializableFishItem>() != null) 
        {
            value = item.GetComponent<SerializableFishItem>().value;
            cost.text = value + "$";
        }

        else if(item.GetComponent<SerializableEquipmentItem>() != null) 
        {
            value = item.GetComponent<SerializableEquipmentItem>().value;
            cost.text = value + "$";
        }
        else 
        {
            Debug.Log("You're missing a script damn");

        }
    }
}
