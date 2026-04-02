using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelUI : MonoBehaviour
{

    [Header("Panel UI")] 
    [Header("References")] 
    // [SerializeField] private string headerKey = "";
    // [SerializeField] private TextMeshProUGUI headerText = null;
    [SerializeField] private GameObject initialButton = null;
    public GameObject InitialButton => initialButton;
    [SerializeField] private CanvasMovement canvasMovement = null;
    [SerializeField] private CanvasGroup canvasGroup = null;
    [SerializeField] private bool isInteractable = false;


    private void OnValidate()
    {
        if (!canvasMovement) canvasMovement = GetComponent<CanvasMovement>();
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public virtual void EnableCanvas()
    {
        if (IsActive()) return;
        canvasGroup.interactable = isInteractable;
        canvasGroup.blocksRaycasts = isInteractable;
        if (canvasMovement) canvasMovement.Call_Effect();
        else canvasGroup.alpha = 1;
        EventSystem.current.SetSelectedGameObject(initialButton);
    }

    public virtual void DisableCanvas()
    {
        if (!IsActive()) return;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        if (canvasMovement) canvasMovement.Call_Effect_Out();
        else canvasGroup.alpha = 0;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public bool IsActive()
    {
        return canvasGroup.alpha == 1f ? true : false;
    }
}
