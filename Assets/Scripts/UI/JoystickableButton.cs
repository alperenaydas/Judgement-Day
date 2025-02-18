﻿using System;
using System.Collections;
using Enums;
using Managers;
using Skills;
//using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class JoystickableButton : FixedJoystick
    {
        private Image icon;
        private bool isJoystickable = true;
        private SkillSlots slot;
        private GameObject handleButton;
        private bool disabled = true;

        private SkillMain skill = null;
        private GameObject disablePanel;
        private float rangeReminder = 0;

        public void SetJoystickable(bool joystickable)
        {
            isJoystickable = joystickable;
            if (!isJoystickable)
            {
                HandleRange = 0;
            }
            else
            {
                HandleRange = 0.5f;
            }

            rangeReminder = HandleRange;
        }

        public void SetDisable(bool disable)
        {
            disabled = disable;
            disablePanel.SetActive(disable);
            if (disable)
            {
                HandleRange = 0;
            }
            else
            {
                HandleRange = rangeReminder;
            }
        }

        public void Clear()
        {
            try
            {
                icon.sprite = null;
                isJoystickable = true;
                disabled = true;
                skill = null;
                rangeReminder = 0;
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        public void AddSkill(SkillMain skillMain)
        {
            skill = skillMain;
            disabled = false;
            SetJoystickable(skill.skillData.hasDirection);
            icon.sprite = skillMain.skillData.icon;
            SetDisable(false);
        }
        
        protected override void Initialize()
        {
            
            handleButton = transform.GetChild(0).gameObject;
            icon = handleButton.transform.GetChild(0).GetComponent<Image>();
            slot = transform.GetComponent<JoystickableInit>().slot;
            SetJoystickable(isJoystickable);
            
            disablePanel = transform.Find("Disable").gameObject;

            rangeReminder = HandleRange;

            SetDisable(true);
            base.Initialize();
        }

        protected override void Update()
        {
            if (InputManager.Instance.takeInput && !disabled && isJoystickable)
            {
                controller.Rotate(input);
            }
        }
        

        IEnumerator CoolDown()
        {
            float curTime = skill.skillData.coolDownTime;
            float totalTime = curTime;
            Image disableImage = disablePanel.GetComponent<Image>();
            SetDisable(true);
            while (curTime>0)
            {
                disableImage.fillAmount = curTime / totalTime;
                curTime -= Time.deltaTime;
                yield return null;
            }
            disableImage.fillAmount = 1;
            SetDisable(false);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if(!InputManager.Instance.takeInput) return;
            if (!disabled)
            {
                skill.Action(SurroundingsManager.Instance.mainPlayer);
                StartCoroutine(CoolDown());
                controller.EndRotation();
            }
            base.OnPointerUp(eventData);
        }
    }

    
}
