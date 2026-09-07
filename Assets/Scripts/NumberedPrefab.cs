using UnityEngine;
using TMPro;

public class NumberedPrefab : MonoBehaviour
{
    private TMP_Text numberLabel;

    void Awake()
    {
        numberLabel = GetComponentInChildren<TMP_Text>();
    }

    public void SetNumber(int num)
    {
        if(numberLabel != null)
        {
            numberLabel.text = num.ToString();
        }
    }
}
