using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDisplay : MonoBehaviour
{
    public class SliderProperties
    {
        public Slider slider;
        public Image fill;
        public int percentage = 100;
        public int lastPercentage = 100;
        public int change = 0;
        public Color currentColor;
        public Color alertColor;

        public SliderProperties(Slider slider, Image fill, Color alertColor)
        {
            this.slider = slider;
            this.fill = fill;
            this.alertColor = alertColor;
        }
    }

    [Header("Thresholds")]
    [SerializeField] int warningPercentage = 66;
    [SerializeField] int dangerPercentage = 33;

    [Header("Health")]
    [SerializeField] Slider healthSlider;
    [SerializeField] Health playerHealth;
    SliderProperties hSP;
    Image healthSliderFill;
    int healthPercentage;
    int lastHealth = 100;
    int healthChange = 0;

    [Header("Energy")]
    [SerializeField] Slider energySlider;
    [SerializeField] PlayerEnergy playerEnergy;
    SliderProperties eSP;
    Image energySliderFill;
    int energyPercentage;
    int lastEnergy = 100;
    int energyChange = 0;

    [Header("Slider Colors")]
    [SerializeField] Color healColor = Color.green;
    [SerializeField] Color warningColor = Color.yellow;
    [SerializeField] Color dangerColor = Color.red;
    [SerializeField] Color dangerColor2 = Color.magenta;
    Color normalColor;
    Color currentColorHealth;
    Color currentColorEnergy;
    Color alertColorHealth;
    Color alertColorEnergy;

    [Header("Score")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;
    readonly string scoreFormat = new('0', 9);

    void Awake()
    {
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        healthSliderFill = healthSlider.fillRect.GetComponent<Image>();
        energySliderFill = energySlider.fillRect.GetComponent<Image>();
        normalColor = healthSliderFill.color;
        // Debug.Log("normalColor: " + normalColor);
        alertColorHealth = normalColor;
        alertColorEnergy = normalColor;

        SetupHealthSlider();
        SetupEnergySlider();
    }

    void SetupHealthSlider()
    {
        Image fill = healthSlider.fillRect.GetComponent<Image>();
        hSP = new SliderProperties(healthSlider, fill, normalColor);
    }

    void SetupEnergySlider()
    {
        Image fill = energySlider.fillRect.GetComponent<Image>();
        eSP = new SliderProperties(energySlider, fill, normalColor);
    }

    void Update()
    {
        scoreText.text = scoreKeeper.GetScore().ToString(scoreFormat);
        // UpdateHealth();
        // UpdateEnergy();

        UpdateSlider(hSP, playerHealth.GetHealthPercentage());
        UpdateSlider(eSP, playerEnergy.GetEnergyPercentage());
    }

    void UpdateHealth()
    {
        healthPercentage = playerHealth.GetHealthPercentage();
        healthSlider.value = healthPercentage;

        currentColorHealth = healthSliderFill.color;
        // 3 kinds of color change
        // heal: healColor
        // warning: warningColor
        // danger: dangerColor
        // in each case, lerp from currentColor to the alert color
        // the alert color could change again in mid-lerp

        // how much has health changed?
        healthChange = healthPercentage - lastHealth;
        if (healthChange != 0)
        {
            Debug.Log("Health %: " + healthPercentage + ", change: " + healthChange);
        }

        // healColor and dangerColor2 are temporary colors
        // if we've lerped enough to reach one of these, fade back to another color
        // -- default to normal (can be overridden later if healthChange != 0)
        if (ColorsAreClose(currentColorHealth, healColor)) alertColorHealth = normalColor;
        if (ColorsAreClose(currentColorHealth, dangerColor2)) alertColorHealth = normalColor;

        if (healthChange < 0)
        {
            if (healthPercentage <= dangerPercentage)
            {
                // if already at red, go magenta
                alertColorHealth = ColorsAreClose(alertColorHealth, dangerColor) ? dangerColor2 : dangerColor;
            }
            else if (healthPercentage <= warningPercentage)
            {
                alertColorHealth = warningColor;
            }
        }
        else if (healthChange > 0)
        {
            alertColorHealth = healColor;
        }

        // lerp from wherever we are to the alert color
        if (alertColorHealth != currentColorHealth)
        {
            healthSliderFill.color = Color.Lerp(currentColorHealth, alertColorHealth, Time.deltaTime);
        }

        lastHealth = healthPercentage;
    }

    void UpdateEnergy()
    {
        energyPercentage = playerHealth.GetHealthPercentage();
        energySlider.value = energyPercentage;

        currentColorEnergy = energySliderFill.color;
        // 3 kinds of color change
        // heal: healColor
        // warning: warningColor
        // danger: dangerColor
        // in each case, lerp from currentColor to the alert color
        // the alert color could change again in mid-lerp

        // how much has energy changed?
        energyChange = energyPercentage - lastEnergy;
        if (energyChange != 0)
        {
            Debug.Log("Energy %: " + energyPercentage + ", change: " + energyChange);
        }

        // healColor and dangerColor2 are temporary colors
        // if we've lerped enough to reach one of these, fade back to another color
        // -- default to normal (can be overridden later if healthChange != 0)
        if (ColorsAreClose(currentColorEnergy, healColor)) alertColorEnergy = normalColor;
        if (ColorsAreClose(currentColorEnergy, dangerColor2)) alertColorEnergy = normalColor;

        if (energyChange < 0)
        {
            if (energyPercentage <= dangerPercentage)
            {
                // if already at red, go magenta
                alertColorEnergy = ColorsAreClose(alertColorEnergy, dangerColor) ? dangerColor2 : dangerColor;
            }
            else if (energyPercentage <= warningPercentage)
            {
                alertColorHealth = warningColor;
            }
        }
        else if (energyChange > 0)
        {
            alertColorEnergy = healColor;
        }

        // lerp from wherever we are to the alert color
        if (alertColorEnergy != currentColorEnergy)
        {
            energySliderFill.color = Color.Lerp(currentColorEnergy, alertColorEnergy, Time.deltaTime);
        }

        lastEnergy = energyPercentage;
    }

    void UpdateSlider(SliderProperties sp, int percentage)
    {
        sp.percentage = percentage;
        sp.slider.value = percentage;

        sp.currentColor = sp.fill.color;
        // 3 kinds of color change
        // heal: healColor
        // warning: warningColor
        // danger: dangerColor
        // in each case, lerp from currentColor to the alert color
        // the alert color could change again in mid-lerp

        // how much has health changed?
        sp.change = sp.percentage - sp.lastPercentage;

        // healColor and dangerColor2 are temporary colors
        // if we've lerped enough to reach one of these, fade back to another color
        // -- default to normal (can be overridden later if healthChange != 0)
        if (ColorsAreClose(sp.currentColor, healColor)) sp.alertColor = normalColor;
        if (ColorsAreClose(sp.currentColor, dangerColor2)) sp.alertColor = normalColor;

        if (sp.change < 0)
        {
            if (sp.percentage <= dangerPercentage)
            {
                // if already at red, go magenta
                sp.alertColor = ColorsAreClose(sp.alertColor, dangerColor) ? dangerColor2 : dangerColor;
            }
            else if (sp.percentage <= warningPercentage)
            {
                sp.alertColor = warningColor;
            }
        }
        else if (sp.change > 0)
        {
            sp.alertColor = healColor;
        }

        // lerp from wherever we are to the alert color
        if (sp.alertColor != sp.currentColor)
        {
            sp.fill.color = Color.Lerp(sp.currentColor, sp.alertColor, Time.deltaTime);
        }

        sp.lastPercentage = sp.percentage;
    }


    bool ColorsAreClose(Color color1, Color color2, float tolerance = 0.01f)
    {
        // Calculate the squared distance between color components
        float rDiff = color1.r - color2.r;
        float gDiff = color1.g - color2.g;
        float bDiff = color1.b - color2.b;
        float aDiff = color1.a - color2.a;

        float distanceSquared = rDiff * rDiff + gDiff * gDiff + bDiff * bDiff + aDiff * aDiff;

        // Compare to the squared tolerance to avoid unnecessary square root computation
        return distanceSquared < tolerance * tolerance;
    }
}
