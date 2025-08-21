using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soar.Variables.Sample
{
    public class DataEditor : MonoBehaviour
    {
        [SerializeField] private CustomVariable customVariable;

        [SerializeField] private Toggle toggle;
        [SerializeField] private TMP_InputField intInputField;
        [SerializeField] private TMP_InputField floatInputField;
        [SerializeField] private TMP_InputField stringInputField;
        [SerializeField] private Button[] intEditButtons;
        [SerializeField] private Slider floatSlider;

        private IDisposable subscription;

        private void Start()
        {
            UpdateData(customVariable);
            subscription = customVariable.Subscribe(UpdateData);
        }

        private void OnEnable()
        {
            toggle.onValueChanged.AddListener(UpdateBoolField);
            intInputField.onValueChanged.AddListener(UpdateIntField);
            floatInputField.onValueChanged.AddListener(UpdateFloatField);
            stringInputField.onValueChanged.AddListener(UpdateStringField);
            intEditButtons[0].onClick.AddListener(SubtractIntField);
            intEditButtons[1].onClick.AddListener(AddIntField);
            floatSlider.onValueChanged.AddListener(UpdateFloatField);
        }
        
        private void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(UpdateBoolField);
            intInputField.onValueChanged.RemoveListener(UpdateIntField);
            floatInputField.onValueChanged.RemoveListener(UpdateFloatField);
            stringInputField.onValueChanged.RemoveListener(UpdateStringField);
            intEditButtons[0].onClick.RemoveListener(SubtractIntField);
            intEditButtons[1].onClick.RemoveListener(AddIntField);
            floatSlider.onValueChanged.RemoveListener(UpdateFloatField);
        }

        private void UpdateBoolField(bool isOn)
        {
            var v = customVariable.Value;
            v.boolField = isOn;
            customVariable.Value = v;
        }

        private void AddIntField() => UpdateIntField(customVariable.Value.intField + 1);
        private void SubtractIntField() => UpdateIntField(customVariable.Value.intField - 1);
        
        private void UpdateIntField(string value)
        {
            if (!int.TryParse(value, out var result)) return;
            UpdateIntField(result);
        }
        
        private void UpdateIntField(int value)
        {
            var v = customVariable.Value;
            v.intField = value;
            customVariable.Value = v;
        }
        
        private void UpdateFloatField(string value)
        {
            if (!float.TryParse(value, out var result)) return;
            UpdateFloatField(result);
        }
        
        private void UpdateFloatField(float value)
        {
            var v = customVariable.Value;
            v.floatField = value;
            customVariable.Value = v;
        }
        
        private void UpdateStringField(string value)
        {
            var v = customVariable.Value;
            v.stringField = value;
            customVariable.Value = v;
        }

        private void UpdateData(CustomStruct data)
        {
            toggle.SetIsOnWithoutNotify(data.boolField);
            intInputField.SetTextWithoutNotify(data.intField.ToString());
            floatInputField.SetTextWithoutNotify(data.floatField.ToString(CultureInfo.InvariantCulture));
            stringInputField.SetTextWithoutNotify(data.stringField);
            floatSlider.SetValueWithoutNotify(data.floatField);
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
        }
    }
}