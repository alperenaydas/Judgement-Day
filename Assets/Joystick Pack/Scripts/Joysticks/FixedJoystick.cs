﻿using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using Managers;
using UnityEngine;

public class FixedJoystick : Joystick
{
    protected PlayerController controller;


    private void OnEnable()
    {
        Initialize();
    }
    

    protected virtual void Initialize()
    {
        StartCoroutine(WaitForInitialize());
    }

    IEnumerator WaitForInitialize()
    {
        yield return new WaitForSeconds(Constants.SecondBatch);
        controller = SurroundingsManager.Instance.mainPlayer.GetComponent<PlayerController>();

    }
     protected virtual void Update()
    {
        if (InputManager.Instance.takeInput)
        {
            controller.Move(input);
        }
    }
}