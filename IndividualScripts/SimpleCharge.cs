using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SagesUtilsRelease
{
    public class SimpleCharge : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Value your charge will start at.")]
        public float StartValue = 0f;
        [Tooltip("Value your charge will reach before stopping.")]
        public float GoalValue = 100f;

        [Header("Add Over Time")]
        [Tooltip("Continously adds an undesignated value every update over the span of duration in seconds.")]
        public bool AddOverTime;
        [Tooltip("Amount of time it takes to reach your maximum value from your starting value.")]
        public float Duration;

        [Header("Add By Time")]
        [Tooltip("Adds a set value every set amount of seconds until it reaches its goal.")]
        public bool AddByTime;
        [Tooltip("Amount that is added every X seconds.")]
        public float Add;
        [Tooltip("Frequency in which the above value is added, in seconds.")]
        public float Every;

        [Header("Text")]
        [Tooltip("Will display the current value of your charge on a given text.")]
        public bool UseText = true;
        public TextMeshProUGUI Text;
        [Tooltip("Text that is added before your number.")]
        public string Prefix;
        [Tooltip("Text that is added after your number.")]
        public string Suffix;
        public enum roundToTheNearest
        {
            WholeNumber,
            Tenth,
            Hundredth,
            Thousandth
        }
        public roundToTheNearest decimalPlace;
        [Tooltip("Will automatically add a space after the prefix text; between the prefix and number.")]
        public bool AutoAddSpace1st = true;
        [Tooltip("Will automatically add a space before the suffix text; between the number and suffix.")]
        public bool AutoAddSpace2nd = true;

        [Header("Meter")]
        [Tooltip("Will change the fill amount of a given image based on the current value of your charge.")]
        public bool UseMeter = true;
        public Image Meter;
        [Tooltip("Allows the color of your meter to be gradually changed over time based on your current charge.")]
        public bool UseColor = true;
        [Tooltip("Color that is used to display a low charge.")]
        public Color Low;
        [Tooltip("Color that is used to display a high charge.")]
        public Color High;

        public UltrakillEvent OnFullMeter;

        private float currentValue;
        private string space;
        private bool currentlyActive = false;
        private int rounded;
        private float roundMultiplier;
        private bool Waiting = true;

        void Start()
        {
            currentValue = StartValue;

            if (Text == null && UseText)
                Text = gameObject.GetComponent<TextMeshProUGUI>();

            if (Meter == null && UseMeter)
                Meter = gameObject.GetComponent<Image>();

            space = " ";

            if (decimalPlace == roundToTheNearest.WholeNumber)
            {
                rounded = 1;
                roundMultiplier = 1;
            }
            else if (decimalPlace == roundToTheNearest.Tenth)
            {
                rounded = 10;
                roundMultiplier = 0.1f;
            }
            else if (decimalPlace == roundToTheNearest.Hundredth)
            {
                rounded = 100;
                roundMultiplier = 0.01f;
            }
            else if (decimalPlace == roundToTheNearest.Thousandth)
            {
                rounded = 1000;
                roundMultiplier = 0.001f;
            }
        }

        void Update()
        {
            if (currentlyActive)
            {
                if (AddOverTime && !AddByTime)
                {
                    if (Time.deltaTime > 0)
                        currentValue += ((GoalValue - StartValue) / Duration / 60f);
                }

                AssignTextValue();
                AssignMeterValue();
            }

            if (currentlyActive && currentValue >= GoalValue)
            {
                currentlyActive = false;
                currentValue = GoalValue;
                OnFullMeter.Invoke();
                AssignTextValue();
            }

            if (AddByTime && !AddOverTime)
            {
                if (currentlyActive && Waiting)
                {
                    Waiting = false;
                    Invoke(nameof(AddToValue), Every);
                }
                else if (!currentlyActive)
                    Waiting = true;
            }

        }

        void AssignTextValue()
        {
            if (UseText)
            {
                var roundResult = ((Mathf.Round(currentValue * rounded)) * roundMultiplier).ToString();
                if (currentValue > GoalValue)
                    roundResult = GoalValue.ToString();

                if (!AutoAddSpace1st && !AutoAddSpace2nd)
                    Text.text = Prefix + roundResult + Suffix;
                else if (!AutoAddSpace1st && AutoAddSpace2nd)
                    Text.text = Prefix + roundResult + space + Suffix;
                else if (AutoAddSpace1st && !AutoAddSpace2nd)
                    Text.text = Prefix + space + roundResult + Suffix;
                else if (AutoAddSpace1st && AutoAddSpace2nd)
                    Text.text = Prefix + space + roundResult + space + Suffix;
            }
        }

        void AssignMeterValue()
        {
            if (UseMeter)
            {
                Meter.fillAmount = currentValue / GoalValue;
            }
            if (UseColor)
            {
                Meter.color = Color.Lerp(Low, High, (currentValue / GoalValue));
            }
        }

        public void Activate(bool active)
        {
            currentlyActive = active;
        }

        void AddToValue()
        {
            if (currentlyActive)
            {
                currentValue += Add;
                Invoke(nameof(AddToValue), Every);
            }
            else
                return;
        }

        public void ChangeCharge(float change)
        {
            if (change > 0)
                currentValue += change;
            else if (change < 0)
                currentValue -= change;
        }
    }
}
