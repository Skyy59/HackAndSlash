using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private string textKey;
    
    private void OnValidate() 
    {
        if (!buttonText) buttonText = GetComponentInChildren<TextMeshProUGUI>();   
    }
}
