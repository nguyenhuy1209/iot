using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CustomToggle : MonoBehaviour
{
    public Toggle toggle;
    public RectTransform toogle_indicator;
    public Image background_image;
    public Color on_color;
    public Color off_color;

    private float onX;
    private float offX;

    void Awake()
    {
        offX = toogle_indicator.anchoredPosition.x;
        onX = -offX;
        toggle.onValueChanged.AddListener(OnSwitch);

        if (toggle.isOn) OnSwitch(true);
    }

    void OnSwitch(bool on) {
        ToggleColor(on);
        MoveIndicator(on);
    }

    private void ToggleColor(bool value) {
        background_image.color = value ? on_color : off_color;
    }

    private void MoveIndicator(bool value) {
        toogle_indicator.DOAnchorPosX(value ? onX : offX, 0.25f);
    }

    void OnDestroy() {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}
