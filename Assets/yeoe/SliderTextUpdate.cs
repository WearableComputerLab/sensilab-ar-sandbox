using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTextUpdate : MonoBehaviour
{
    public Text sliderValue;
    public Slider brushSizeSlider;
    
    void Update()
    {
        sliderValue.text = "Brush Size: "+brushSizeSlider.value.ToString();
    }
}
