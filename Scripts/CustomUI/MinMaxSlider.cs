using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace CustomUiElements
{
    public class MinMaxSlider : MonoBehaviour
    {
        public Action<float, float> OnMinMaxValueChanged;
        private const float m_NormalisedHandleWidth = 0.1f;
        //The offset required so the handles do not intersect relative to the min/max slider value
        private float m_fCurrentOffsetValue;

        [Header("Common settings")]
        public int DecimalPlaces = 2;
        [SerializeField]
        private float MinimumValue = 0;
        [SerializeField]
        private float MaximumValue = 1;
        public bool UseWholeNumbers = false;

        [Header("Configuration for MinSlider")]
        private Slider m_MinSlider;

        [Header("Configuration for MaxSlider")]
        private Slider m_MaxSlider;

        private RectTransform m_Fill;

        private float m_fFillSize;

        // Properties
        public float CurrentLowerValue
        {
            get { return m_MinSlider.value; }
        }
        public float CurrentUpperValue
        {
            get { return m_MaxSlider.value; }
        }

        void Awake()
        {
            m_Fill = transform.SearchComponent<RectTransform>("Fill");
            m_fFillSize = m_Fill.rect.size.x;
            Transform minSlider = transform.Search("MinSlider");
            m_MinSlider = minSlider.gameObject.ForceComponent<Slider>();
            Transform maxSlider = transform.Search("MaxSlider");
            m_MaxSlider = maxSlider.gameObject.ForceComponent<Slider>();

            // Adjust Max/Min values for both sliders
            m_MinSlider.wholeNumbers = UseWholeNumbers;
            m_MaxSlider.wholeNumbers = UseWholeNumbers;

            m_MinSlider.onValueChanged.AddListener(UpdateMin);
            m_MaxSlider.onValueChanged.AddListener(UpdateMax);

            SetMinMaxValues(MinimumValue, MaximumValue);
        }

        public void SetMinMaxValues(float min, float max)
        {
            MinimumValue = min;
            MaximumValue = max;
            m_MinSlider.minValue = MinimumValue;
            m_MinSlider.maxValue = MaximumValue;
            m_MaxSlider.minValue = MinimumValue;
            m_MaxSlider.maxValue = MaximumValue;

            m_fCurrentOffsetValue = (MaximumValue - MinimumValue) * m_NormalisedHandleWidth;

            OnUpdate();
        }

        private void UpdateMin(float min)
        {
            if (min + m_fCurrentOffsetValue >= m_MaxSlider.value)
            {
                // invalid
                min = m_MaxSlider.value - m_fCurrentOffsetValue;
            }

            m_MinSlider.value = min;
            OnUpdate();
        }

        private void UpdateMax(float max)
        {
            if (max - m_fCurrentOffsetValue <= m_MinSlider.value)
            {
                // invalid value
                max = m_MinSlider.value + m_fCurrentOffsetValue;
            }

            m_MaxSlider.value = max;
            OnUpdate();
        }

        private void OnUpdate()
        {
            OnMinMaxValueChanged?.Invoke(m_MinSlider.value, m_MaxSlider.value);

            Vector2 offsetMin = m_Fill.offsetMin;
            float normalisedMin = Mathf.InverseLerp(MinimumValue, MaximumValue, m_MinSlider.value);
            offsetMin.x = Mathf.Lerp(0f, m_fFillSize, normalisedMin);
            m_Fill.offsetMin = offsetMin;

            Vector2 offsetMax = m_Fill.offsetMax;
            float normalisedMax = Mathf.InverseLerp(MinimumValue, MaximumValue, m_MaxSlider.value);
            offsetMax.x = Mathf.Lerp(-m_fFillSize, 0f, normalisedMax);
            m_Fill.offsetMax = offsetMax;
        }
    }
}