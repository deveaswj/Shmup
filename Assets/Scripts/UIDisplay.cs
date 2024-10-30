using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDisplay : MonoBehaviour
{

    [Header("Health")]
    [SerializeField] Slider healthSlider;
    [SerializeField] Health playerHealth;
    [SerializeField] int warningPercentage = 40;
    [SerializeField] int dangerPercentage = 15;
    [SerializeField] Color healColor = Color.green;
    [SerializeField] Color warningColor = Color.yellow;
    [SerializeField] Color dangerColor = Color.red;
    [SerializeField] Color dangerColor2 = Color.magenta;
    Image healthSliderFill;
    int healthPercentage;
    int lastPercentage = 100;
    int healthChange = 0;
    Color normalColor;
    Color currentColor;
    Color alertColor;
    readonly string scoreFormat = new('0', 9);

    [Header("Score")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    void Awake()
    {
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        healthSliderFill = healthSlider.fillRect.GetComponent<Image>();
        normalColor = healthSliderFill.color;
        // Debug.Log("normalColor: " + normalColor);
        alertColor = normalColor;
    }

    void Update()
    {
        healthPercentage = playerHealth.GetHealthPercentage();
        healthSlider.value = healthPercentage;
        scoreText.text = scoreKeeper.GetScore().ToString(scoreFormat);

        currentColor = healthSliderFill.color;
        // 3 kinds of color change
        // heal: healColor
        // warning: warningColor
        // danger: dangerColor
        // in each case, lerp from currentColor to the alert color
        // the alert color could change again in mid-lerp

        // how much has health changed?
        healthChange = healthPercentage - lastPercentage;
        if (healthChange != 0)
        {
            Debug.Log("Health %: " + healthPercentage + ", change: " + healthChange);
        }

        // healColor and dangerColor2 are temporary colors
        // if we've lerped enough to reach one of these, fade back to another color
        // -- default to normal (can be overridden later if healthChange != 0)
        if (ColorsAreClose(currentColor, healColor)) alertColor = normalColor;
        if (ColorsAreClose(currentColor, dangerColor2)) alertColor = normalColor;

        if (healthChange < 0)
        {
            if (healthPercentage <= dangerPercentage)
            {
                // if already at red, go magenta
                alertColor = ColorsAreClose(alertColor, dangerColor) ? dangerColor2 : dangerColor;
            }
            else if (healthPercentage <= warningPercentage)
            {
                alertColor = warningColor;
            }
        }
        else if (healthChange > 0)
        {
            alertColor = healColor;
        }

        // lerp from wherever we are to the alert color
        if (alertColor != currentColor)
        {
            healthSliderFill.color = Color.Lerp(currentColor, alertColor, Time.deltaTime);
        }

        lastPercentage = healthPercentage;
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
