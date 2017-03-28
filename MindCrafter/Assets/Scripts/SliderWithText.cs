using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Slider))]
    internal class SliderWithText : MonoBehaviour
    {
        [SerializeField]
        private string sliderText = "";

        private Slider slider;
        private Text text;

        internal Slider Component { get { return slider; } }
        private void Start()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(delegate { ValueChanged(slider.value); });
            text = GetComponentInChildren<Text>();
        }
        private void ValueChanged(float value)
        {
            text.text = sliderText + " " + value;
            slider.value = value;
        }

    }
}
