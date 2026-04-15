using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UIButtonSound : MonoBehaviour
{
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip hover;

    private new AudioSource audio;

    private UIButton[] uIbutton;

    private float lastHoverTime;

    private void Start()
    {
        audio = GetComponent<AudioSource>();

        uIbutton = GetComponentsInChildren<UIButton>(true);

        for (int i = 0; i < uIbutton.Length; i++)
        {
            uIbutton[i].PointerEnter += OnPointerEnter;
            uIbutton[i].FocusEnter += OnPointerEnter;
            uIbutton[i].PointerClick += OnPointerClick;
        }
    }

    private void OnDestroy()
    {
        if (uIbutton == null) return;

        for (int i = 0; i < uIbutton.Length; i++)
        {
            if (uIbutton[i] != null)
            {
                uIbutton[i].PointerEnter -= OnPointerEnter;
                uIbutton[i].FocusEnter -= OnPointerEnter;
                uIbutton[i].PointerClick -= OnPointerClick;
            }
        }
    }

    private void OnPointerClick(UIButton arg0)
    {
        audio.PlayOneShot(click);
    }

    private void OnPointerEnter(UIButton arg0)
    {
        if (Time.unscaledTime - lastHoverTime < 0.05f) return;
        lastHoverTime = Time.unscaledTime;
        audio.PlayOneShot(hover);
    }
}
