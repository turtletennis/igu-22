using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burnable : MonoBehaviour
{
    [Header("Temperatures")]
    [SerializeField] float startTemperature = 20f;
    [SerializeField] float burningTemperature = 30f;
    [SerializeField] float burntTemperature = 100f;
    [SerializeField] float wetTemperature = 10f;
    [Header("Colours")]
    [SerializeField] Color burningColor = new Color(0.9f,0.2f,0f);
    [SerializeField] Color wetColor = new Color(0.9f,0.2f,0f);
    [SerializeField] Color burntColor = new Color(0.3f,0.3f,0.3f);
    Color baseColor;


    Collider2D collider;
    float temperature;
    bool burnt;
    
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        temperature = startTemperature;
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
    }

    public void ChangeTemperature(float amount)
    {
        if(!burnt)
        {
            temperature+=amount;
            temperature = Mathf.Clamp(temperature, 0, float.MaxValue);
            //Debug.Log($"Temperature changed to {temperature}");
            if(temperature >= burntTemperature)
            {
                spriteRenderer.color= burntColor;
                burnt=true;
            }
            else
            {
                spriteRenderer.color = CalculateColor(temperature);
            }
        }
    }

    Color CalculateColor(float temperature)
    {
        Color newColor = baseColor;
        if(temperature < wetTemperature)
        {
            float factor = (wetTemperature - temperature) / wetTemperature;
            Vector4 colorDiff = wetColor - baseColor;
            newColor = (Vector4) baseColor + colorDiff * factor;
        }
        else if(temperature > burningTemperature)
        {
            float burningRange = burntTemperature - burningTemperature;
            float factor = (temperature - burningTemperature ) / burningRange;
            Vector4 colorDiff = burningColor - baseColor;
            newColor = (Vector4) baseColor + colorDiff * factor;
            //Debug.Log($"burningRange={burningRange} factor={factor} colorDiff = {colorDiff} newColor = {newColor}");
        }
        
        return newColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
