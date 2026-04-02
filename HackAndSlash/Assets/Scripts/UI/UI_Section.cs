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

        for (int _i = 0; _i < sectionButtons.Length; _i++)
        {
            Navigation _buttonNavigation = sectionButtons[_i].navigation;
            int _topIndex = _i - 1 < 0 ? sectionButtons.Length - 1 : _i - 1;
            int _botIndex = _i + 1 >= sectionButtons.Length ? 0 : _i + 1;

            switch (sectionDisposition)
            {
                case Disposition.Horizontal:

                    _buttonNavigation.selectOnLeft = sectionButtons[_topIndex];
                    _buttonNavigation.selectOnRight = sectionButtons[_botIndex];

                break;

                case Disposition.Vertical:

                    _buttonNavigation.selectOnUp = sectionButtons[_topIndex];
                    _buttonNavigation.selectOnDown = sectionButtons[_botIndex];

                break;
            }

            sectionButtons[_i].navigation = _buttonNavigation;
        }
    }
}
