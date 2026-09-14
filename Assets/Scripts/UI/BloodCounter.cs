using UnityEngine;
using TMPro;

public class BloodCounter : MonoBehaviour
{
    
    [SerializeField] private CurrencyHandler currencyHandler;
    [SerializeField] private TMP_Text bloodText;

    private void OnEnable(){
        if(currencyHandler != null){
            currencyHandler.OnBloodChanged += HandleBloodChange;
        }
    }

    private void OnDisable(){
        if(currencyHandler != null){
            currencyHandler.OnBloodChanged -= HandleBloodChange;
        }
    }

    private void HandleBloodChange(int current, int peak, int delta){
        bloodText.text = $"{current.ToString()} / {peak.ToString()}";
        UIShake.instance.Shake(15f, 15f);
    }

    void Start()
    {
        bloodText.text = $"{currencyHandler.GetBlood().ToString()} / {currencyHandler.GetPeakBlood().ToString()}";
    }
}
