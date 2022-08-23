using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HudManager : MonoBehaviour
{
    [Header("UI Fields")]
    [SerializeField] Slider fuelSlider;
    [SerializeField] TextMeshProUGUI wrongWayText;
    [SerializeField] TextMeshProUGUI lapText;
    [SerializeField] TextMeshProUGUI finishText;
    [Header("Text templates")]
    [SerializeField] string lapTextFormat = "LAP {0}/{1}";
    [SerializeField] string finishTextFormat = "FINISH!";
    public bool goingWrongWay;
    // Start is called before the first frame update
    void Start()
    {
        finishText.enabled=false;
    }

    // Update is called once per frame
    void Update()
    {
        wrongWayText.enabled = goingWrongWay;
    }

    public void UpdateFuelRemaining(float remainingFraction)
    {
        fuelSlider.value = remainingFraction;
    }

    public void UpdateLapText(int currentLap, int totalLaps,int position=0)
    {
        if(currentLap <= totalLaps)
        {
            lapText.text=string.Format(lapTextFormat,currentLap,totalLaps);
        }
        else
        {
            lapText.text = finishTextFormat;
            finishText.enabled = true;
            
            finishText.text = $"{finishTextFormat}\n{GetPlaceText(position)} Place!" ;
        }
    }

    private string GetPlaceText(int position)
    {
        switch(position)
        {
            case 0:
            return "1st";
            case 1:
            return "2nd";
            case 2:
            return "3rd";
        }
        return position+"th";
    }
}
