using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gaia
{
    public class PhotoModeColorPicker : MonoBehaviour
    {
        public Color m_value;
        public Image m_currentColor;
        public Image m_lastColor;
        public Slider m_sliderR;
        public Image m_currentRedColor;
        public Slider m_sliderG;
        public Image m_currentGreenColor;
        public Slider m_sliderB;
        public Image m_currentBlueColor;
        public Text m_selectedNameText;

        public UnityAction m_onChanged;

        private void Awake()
        {
            if (m_sliderR != null)
            {
                m_sliderR.onValueChanged.AddListener(SetRedValue);
            }
            if (m_sliderG != null)
            {
                m_sliderG.onValueChanged.AddListener(SetGreenValue);
            }
            if (m_sliderB != null)
            {
                m_sliderB.onValueChanged.AddListener(SetBlueValue);
            }
        }
        private void Update()
        {
            Refresh();
        }
        public void SetColorValue(Color color)
        {
            m_value = color;
            if (m_sliderR != null)
            {
                m_sliderR.SetValueWithoutNotify(color.r);
                if (m_currentRedColor != null)
                {
                    Color newColor = m_value;
                    newColor = Color.red;
                    newColor.r *= color.r;
                    newColor.a = 1f;
                    m_currentRedColor.color = newColor;
                }
            }
            if (m_sliderG != null)
            {
                m_sliderG.SetValueWithoutNotify(color.g);
                if (m_currentGreenColor != null)
                {
                    Color newColor = m_value;
                    newColor = Color.green;
                    newColor.g *= color.g;
                    newColor.a = 1f;
                    m_currentGreenColor.color = newColor;
                }
            }
            if (m_sliderB != null)
            {
                m_sliderB.SetValueWithoutNotify(color.b);
                if (m_currentBlueColor != null)
                {
                    Color newColor = m_value;
                    newColor = Color.blue;
                    newColor.b *= color.b;
                    newColor.a = 1f;
                    m_currentBlueColor.color = newColor;
                }
            }
            Refresh();
            this.transform.SetAsLastSibling();
        }
        public void SetLastColorValue(Color color)
        {
            if (m_lastColor != null)
            {
                Color newColor = color;
                newColor.a = 1f;
                m_lastColor.color = newColor;
            }
        }
        public void SetCurrentFocusedName(string name)
        {
            if (m_selectedNameText != null)
            {
                m_selectedNameText.text = "(" + name + ")";
            }
        }
        public void CloseColorPicker()
        {
            gameObject.SetActive(false);
            if (PhotoMode.Instance != null)
            {
                PhotoMode.Instance.m_updateColorPickerRef = false;
            }
        }
        public void RefColor(ref Color color, ref Button currentValue)
        {
            color = m_value;
            if (currentValue != null)
            {
                ColorBlock colorBlock = currentValue.colors;
                colorBlock.normalColor = m_value;
                colorBlock.selectedColor = m_value;
                currentValue.colors = colorBlock;
            }
        }
        public void Refresh()
        {
            UpdateCurrentColors();
        }
        private void UpdateCurrentColors()
        {
            if (m_currentColor != null)
            {
                Color newColor = m_value;
                newColor.a = 1f;
                m_currentColor.color = newColor;
            }
        }
        public void SetRedValue(float value)
        {
            m_value.r = value;
            if (m_currentRedColor != null)
            {
                Color newColor = m_value;
                newColor = Color.red;
                newColor.r *= value;
                newColor.a = 1f;
                m_currentRedColor.color = newColor;
                if (m_onChanged != null)
                {
                    m_onChanged.Invoke();
                }
            }
        }
        public void SetGreenValue(float value)
        {
            m_value.g = value;
            if (m_currentGreenColor != null)
            {
                Color newColor = m_value;
                newColor = Color.green;
                newColor.g *= value;
                newColor.a = 1f;
                m_currentGreenColor.color = newColor;
                if (m_onChanged != null)
                {
                    m_onChanged.Invoke();
                }
            }
        }
        public void SetBlueValue(float value)
        {
            m_value.b = value;
            if (m_currentBlueColor != null)
            {
                Color newColor = m_value;
                newColor = Color.blue;
                newColor.b *= value;
                newColor.a = 1f;
                m_currentBlueColor.color = newColor;
                if (m_onChanged != null)
                {
                    m_onChanged.Invoke();
                }
            }
        }
        public void UpdateOnChangedMethod(UnityAction onChanged)
        {
            m_onChanged = onChanged;
        }
        public void ResetColorValue()
        {
            if (m_lastColor != null)
            {
                if (PhotoMode.Instance != null)
                {
                    m_sliderR.value = m_lastColor.color.r;
                    m_sliderG.value = m_lastColor.color.g;
                    m_sliderB.value = m_lastColor.color.b;

                    PhotoMode.Instance.UpdateColorPicker();
                    if (m_onChanged != null)
                    {
                        m_onChanged.Invoke();
                    }
                }
            }
        }
    }
}