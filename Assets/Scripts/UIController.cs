using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public TMP_Text reloadMessage;
    public Slider weaponTempSlider;
    public TMP_Text maxBullets;
    public TMP_Text countBulletsText;
    private void Awake()
    {
        instance = this;
    }
}
