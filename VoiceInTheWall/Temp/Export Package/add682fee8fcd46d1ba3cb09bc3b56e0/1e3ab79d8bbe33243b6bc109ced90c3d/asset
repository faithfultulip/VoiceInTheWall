using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderInfo : MonoBehaviour
{

    public enum ColorChannels
    {
        r,g,b
    }

    public ColorChannels ColorChannel;
    public WormholeAnimation.ColorType ColorType;

    private void Start()
    {
        StartCoroutine(WaitandColor());

        
        
    }


    IEnumerator WaitandColor()
    {
        yield return new WaitForSeconds(1f);
        Color tempColor = GameObject.FindObjectOfType<WormholeAnimation>().GetColor(this.ColorType);
        
        if (this.ColorChannel == ColorChannels.r)
        {
            this.gameObject.GetComponent<Slider>().value = tempColor.r;
        }
        if (this.ColorChannel == ColorChannels.g)
        {
            this.gameObject.GetComponent<Slider>().value = tempColor.g;
        }
        if (this.ColorChannel == ColorChannels.b)
        {
            this.gameObject.GetComponent<Slider>().value = tempColor.b;
        }
    }
}
