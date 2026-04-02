using System;
using UnityEngine;

public class CanvasMovement : MonoBehaviour
{
    #region Variables

    public CanvasGroup canvasGroup = null;

    public Vector2 angle = Vector2.zero;
    public bool reverseOut = false;

    [SerializeField] private bool isShowing = false;
    
    private float effectDuration = 0;

    private RectTransform _rectTransform;
    private Vector2 initialLocalPosition = Vector2.zero;

    private float timer = 0;

    private bool forward = false;

    private bool backward = false;

    #endregion
    
    #region Events

    private void OnValidate()
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        if (_rectTransform) initialLocalPosition = _rectTransform.localPosition;
    }

    private void Start ( )
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        if (_rectTransform) initialLocalPosition = _rectTransform.localPosition;
        effectDuration = CanvasCurves.Instance.fadeInCurve.keys [ CanvasCurves.Instance.fadeInCurve.keys.Length - 1 ].time;
    }

    private void Update ( )
    {
        if ( forward )
        {
            if ( timer > 0 )
            {
                timer -= Time.unscaledDeltaTime;
                timer = Mathf.Clamp ( timer, 0, effectDuration );

                _rectTransform.localPosition = initialLocalPosition + angle.normalized * CanvasCurves.Instance.fadeInCurve.Evaluate ( effectDuration - timer );
                canvasGroup.alpha = ( effectDuration - timer ) / effectDuration;
            }
        }

        else if ( backward )
        {
            if ( timer > 0 )
            {
                timer -= Time.unscaledDeltaTime;
                timer = Mathf.Clamp ( timer, 0, effectDuration );

                _rectTransform.localPosition = initialLocalPosition + ( reverseOut ? angle.normalized : -angle.normalized ) * CanvasCurves.Instance.fadeOutCurve.Evaluate ( effectDuration - timer );
                canvasGroup.alpha = 1 - ( ( effectDuration - timer ) / effectDuration );
            }
        }
    }

    #endregion
    
    #region Methods

    public void Call_Effect ( )
    {
        forward = true;
        backward = false;
        isShowing = true;
        Effect ( );
    }

    public void Call_Effect_Out ( )
    {
        backward = true;
        forward = false;
        isShowing = false;
        Effect ( );
    }

    private void Effect ( )
    {
        timer = effectDuration;
        canvasGroup.alpha = 0;
    }

    public bool GetState()
    {
        return isShowing;
    }

    #endregion
}
