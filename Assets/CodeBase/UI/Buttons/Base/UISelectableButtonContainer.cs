using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UISelectableButtonContainer : MonoBehaviour
{
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private LayoutGroup layoutGroup;
    [SerializeField] private bool enableKeyboardNavigation = true;

    public UnityEvent onCancel;

    public bool Interactable = true;
    public void SetInteractable(bool interactable) => Interactable = interactable;

    private UISelectableButton[] buttons;
    private int selectButtonIndex = 0;
    private GridLayoutGroup gridLayout;
    private VerticalLayoutGroup verticalLayout;

    private void Start()
    {
        buttons = buttonsContainer.GetComponentsInChildren<UISelectableButton>();

        if (buttons.Length == 0)
        {
            Debug.LogError("No buttons found in the container.");
            return;
        }

        gridLayout = layoutGroup as GridLayoutGroup;
        verticalLayout = layoutGroup as VerticalLayoutGroup;

        foreach (var button in buttons) 
            button.PointerEnter += SelectButton;

        if (Interactable)
            buttons[selectButtonIndex].SetFocus();
    }

    private void OnDestroy()
    {
        if (buttons == null) return;

        foreach (var button in buttons)
        {
            if (button != null)
            {
                button.PointerEnter -= SelectButton;
            }
        }
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy || !Interactable || !enableKeyboardNavigation)
            return;

        int newIndex = selectButtonIndex;

        ISettingControl settingControl = buttons[selectButtonIndex].GetComponent<ISettingControl>();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            onCancel?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (settingControl != null)
            {
                settingControl.Decrement();
                return;
            }
            newIndex = selectButtonIndex - 1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (settingControl != null)
            {
                settingControl.Increment();
                return;
            }
            newIndex = selectButtonIndex + 1;
        }

        CanvasGroup cg = GetComponentInParent<CanvasGroup>();
        if (cg != null && (cg.alpha <= 0 || !cg.interactable))
            return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (buttons != null && selectButtonIndex >= 0 && selectButtonIndex < buttons.Length)
            {
                if (buttons[selectButtonIndex].Interactable)
                {
                    buttons[selectButtonIndex].Submit();
                }
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            onCancel?.Invoke();
            return;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeSettingValue(false);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeSettingValue(true);
        }

        if (gridLayout != null)
            HandleGridNavigation();
        else if (verticalLayout != null)
            HandleVerticalNavigation();
    }

    private void ChangeSettingValue(bool isIncrement)
    {
        if (buttons[selectButtonIndex] is ISettingControl control)
        {
            if (isIncrement) control.Increment();
            else control.Decrement();
        }
    }

    private void SelectButton(UIButton button) 
    {
        if (!Interactable) return;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == button)
            {
                SetFocusAt(i);
                break;
            }
        }

        buttons[selectButtonIndex].SetUnFocus();            
        selectButtonIndex = Array.IndexOf(buttons, button);
        buttons[selectButtonIndex].SetFocus();              
    }

    private void SetFocusAt(int index)
    {
        if (index < 0 || index >= buttons.Length) return;

        buttons[selectButtonIndex].SetUnFocus();
        selectButtonIndex = index;
        buttons[selectButtonIndex].SetFocus();
    }

    public void SelectNext() => SetFocusAt((selectButtonIndex + 1) % buttons.Length);
    public void SelectPrevious() => SetFocusAt((selectButtonIndex - 1 + buttons.Length) % buttons.Length);

    private void HandleVerticalNavigation() 
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            SelectPrevious();
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            SelectNext();
    }

    private void HandleGridNavigation() 
    {
        int cols = DetectGridColumns();                           
        int rows = Mathf.CeilToInt((float)buttons.Length / cols); 
        int row = selectButtonIndex / cols;                       
        int col = selectButtonIndex % cols;                       
        int newIndex = -1;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) 
            newIndex = ((row - 1 + rows) % rows) * cols + col;                       
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            newIndex = ((row + 1) % rows) * cols + col;                              
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            newIndex = selectButtonIndex - 1;                                        
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            newIndex = selectButtonIndex + 1;                                        
        else
            return;

        newIndex = ((newIndex % buttons.Length) + buttons.Length) % buttons.Length;

        if (newIndex != selectButtonIndex) 
            SetFocusAt(newIndex);
    }

    private int DetectGridColumns()
    {
        if (buttons.Length < 2) 
            return 1;

        RectTransform firstRect = buttons[0].GetComponent<RectTransform>();
        if (firstRect == null)
            return 1;

        float firstY = firstRect.anchoredPosition.y;
        int cols = 1;

        foreach (var button in buttons) 
        {
            if (button == buttons[0]) continue;

            RectTransform rect = button.GetComponent<RectTransform>();
            if (rect != null && Mathf.Abs(firstY - rect.anchoredPosition.y) < 5f)
                cols++; 
            else
                break;
        }

        return cols;
    }
}