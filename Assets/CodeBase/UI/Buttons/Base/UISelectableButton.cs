using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UISelectableButton : UIButton
{
    [SerializeField] private Image selectImage;

    public UnityEvent onSelect;
    public UnityEvent onUnSelect;

    public override void SetFocus()
    {
        base.SetFocus();

        selectImage.enabled = true;
        onSelect?.Invoke();
    }

    public override void SetUnFocus()
    {
        base.SetUnFocus();
        selectImage.enabled = false;
        onUnSelect?.Invoke();
    }   
}
