using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Dashboard
{
    public class DashboardUI : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup login_layer;
        [SerializeField]
        public InputField broker_url;
        [SerializeField]
        public InputField username;
        [SerializeField]
        public InputField password;

        [SerializeField]
        private CanvasGroup data_layer;
        [SerializeField]
        public Text temperature;
        [SerializeField]
        public Text humidity;
        [SerializeField]
        public CustomToggle led;
        [SerializeField]
        public CustomToggle pump;

        private Tween twen_fade;

        void Start() {
            broker_url.text = "mqttserver.tk";
            username.text = "bkiot";
            password.text = "12345678";
        }

        public ControlData GetLedControlData()
        {
            ControlData data = new ControlData();
            data.device = "LED";
            data.status = led.toggle.isOn ? "ON" : "OFF";
            return data;
        }

        public ControlData GetPumpControlData()
        {
            ControlData data = new ControlData();
            data.device = "PUMP";
            data.status = pump.toggle.isOn ? "ON" : "OFF";
            return data;
        }

        public void UpdateStatusData(StatusData status_data)
        {
            temperature.text = float.Parse(status_data.temperature) + "°C";
            humidity.text = float.Parse(status_data.humidity) + "°%";
        }

        public void Fade(CanvasGroup _canvas, float endValue, float duration, TweenCallback onFinish)
        {
            if (twen_fade != null)
            {
                twen_fade.Kill(false);
            }

            twen_fade = _canvas.DOFade(endValue, duration);
            twen_fade.onComplete += onFinish;
        }

        public void FadeIn(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 1f, duration, () =>
            {
                _canvas.interactable = true;
                _canvas.blocksRaycasts = true;
            });
        }

        public void FadeOut(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 0f, duration, () =>
            {
                _canvas.interactable = false;
                _canvas.blocksRaycasts = false;
            });
        }

        IEnumerator _IESwitchLayer()
        {
            if (login_layer.interactable == true)
            {
                FadeOut(login_layer, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(data_layer, 0.25f);
            }
            else
            {
                FadeOut(data_layer, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(login_layer, 0.25f);
            }
        }

        public void SwitchLayer()
        {
            StartCoroutine(_IESwitchLayer());
        }
    }
}