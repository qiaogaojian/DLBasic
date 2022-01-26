using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    public Button button;
    public GameObject open;
    public GameObject close;
    public Sprite[] buttonImage;
    public void UpdateState(bool ison)
    {
        if (open) open.SetActive(ison);


        if (close) close.SetActive(!ison);
        if (buttonImage.Length > 0) button.image.sprite = buttonImage[ison ? 0 : 1];
    }
}
