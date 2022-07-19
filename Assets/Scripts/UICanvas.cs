using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    [SerializeField] private Button createButton;
    [SerializeField] private Button matRedButton;
    [SerializeField] private Button matGreenButton;
    [SerializeField] private Button matBlueButton;

    public void Init(Action onCreate, Action onRed, Action onGreen, Action onBlue)
    {
        createButton.onClick.AddListener(() => onCreate());
        matRedButton.onClick.AddListener(() => onRed());
        matGreenButton.onClick.AddListener(() => onGreen());
        matBlueButton.onClick.AddListener(() => onBlue());
    }
}
