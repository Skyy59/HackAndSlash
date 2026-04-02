using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Section : MonoBehaviour
{
    public enum Disposition { Horizontal, Vertical}

    [SerializeField] private Selectable[] sectionButtons;
    [SerializeField] private Disposition sectionDisposition;

    private void Start() 
    {
        AdjustNavigation();
    }

    [ContextMenu("Set Navigation")]
    public void AdjustNavigation()
    {
        if (sectionButtons.Length <= 0) return;

        List<Selectable> _currentButtons = new();

        for (int _i = 0; _i < sectionButtons.Length; _i++)
        {
            if (sectionButtons[_i].gameObject.activeInHierarchy) _currentButtons.Add(sectionButtons[_i]);
        }

        for (int _i = 0; _i < _currentButtons.Count; _i++)
        {
            Navigation _buttonNavigation = _currentButtons[_i].navigation;
            int _topIndex = _i - 1 < 0 ? _currentButtons.Count - 1 : _i - 1;
            int _botIndex = _i + 1 >= _currentButtons.Count ? 0 : _i + 1;

            switch (sectionDisposition)
            {
                case Disposition.Horizontal:

                    _buttonNavigation.selectOnLeft = _currentButtons[_topIndex];
                    _buttonNavigation.selectOnRight = _currentButtons[_botIndex];

                break;

                case Disposition.Vertical:

                    _buttonNavigation.selectOnUp = _currentButtons[_topIndex];
                    _buttonNavigation.selectOnDown = _currentButtons[_botIndex];

                break;
            }

            _currentButtons[_i].navigation = _buttonNavigation;
        }
    }
}
