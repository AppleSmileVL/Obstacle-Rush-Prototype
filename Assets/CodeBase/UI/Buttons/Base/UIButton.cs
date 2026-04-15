using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] protected bool interactable = true;

    private bool focused = false;
    public bool Focused => focused;

    public bool Interactable
    {
        get => interactable;
        set => interactable = value;
    }

    public UnityEvent onClick;

    public event UnityAction<UIButton> PointerEnter;
    public event UnityAction<UIButton> PointerExit;
    public event UnityAction<UIButton> PointerClick;
    public event UnityAction<UIButton> FocusEnter;


    public virtual void SetFocus()
    {
        if (interactable == false) return;

        if (!focused)
        {
            focused = true;
            FocusEnter?.Invoke(this);
        }
    }

    public virtual void SetUnFocus()
    {
        if (interactable == false) return;

        focused = false;
    }

    public virtual void Submit()
    {
        if (interactable == false) return;

        PointerClick?.Invoke(this);
        onClick?.Invoke();          
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (interactable == false) return;

        PointerEnter?.Invoke(this);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (interactable == false) return;

        PointerExit?.Invoke(this);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
       
        PointerClick?.Invoke(this);
        
        onClick.Invoke();
    }
}
