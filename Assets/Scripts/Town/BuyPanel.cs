using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class BuyPanel : MonoBehaviour
{
    public TextMeshProUGUI cost;
    public TextMeshProUGUI currentMoney;
    private float value;
    public MoneyUI moneyUI;
    public GameObject buyButton;
    public GameObject moneyPanel;
    public GameObject buyPanelObject;
    //public SellFishSlot ;
    //I need to make a collection of buyable item slots populate into an array

    private GameObject inventory;

    public AudioSource yesClip;
    public AudioSource noClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
      
    }

    

}
