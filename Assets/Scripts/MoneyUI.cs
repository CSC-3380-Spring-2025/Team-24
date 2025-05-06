using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public int money;
    public TMP_Text moneyText;
    public GameObject moneyPanel;

    public void SetMoneyText(int money){
        moneyText.text = money.ToString();
    }
    private void Start(){
        SetMoneyText(money);
    }
}
