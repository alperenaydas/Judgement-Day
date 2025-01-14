﻿using System;
using System.Collections.Generic;
using Enums;
using Managers;
using Skills;
using UnityEngine;
using Photon.Pun;

namespace Controllers
{
    public class PlayerController : CharacterController
    {
        public float speed = 3;
        private Rigidbody rigidbody;
        private float rotationByMovementTimer = 1f;

        private LineRenderer lineRenderer;
        private float trailDistance = 2;

        private Animator animator;
        private Vector2 moveVector = Vector2.zero;

        private void Start()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            rigidbody = GetComponent<Rigidbody>();
            lineRenderer = GetComponentInChildren<LineRenderer>();
            animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            rotationByMovementTimer -= Time.deltaTime;
            KeyBoardInput();
        }

        private void KeyBoardInput() // Delete
        {
            var input = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
            if (input != Vector2.zero)
            {
                Move(input);
            }
        }
        

        public void Move(Vector2 input)
        {
            if (input == Vector2.zero)
            {
                animator.SetBool(Constants.CharacterRunAnimBool,false);
                return;
            }

            moveVector = input;
            animator.SetBool(Constants.CharacterRunAnimBool,true);

            transform.position += new Vector3(input.x,0,input.y) * (speed * Time.deltaTime);
            if (rotationByMovementTimer < 0)
            {
                transform.LookAt(transform.position + new Vector3(input.x*100,0,input.y*100));
            }
        }

        public void EndRotation()
        {
            lineRenderer.enabled = false;
            animator.SetFloat(Constants.CharacterRunDirectionXFloat,0f);
            animator.SetFloat(Constants.CharacterRunDirectionYFloat,1f);
        }
        
        public void Rotate(Vector2 input)
        {
            if (input == Vector2.zero)
            {
                return;
            }
            var angle = Utils.Angle360(moveVector, input);
            float dirY = Mathf.Cos(Mathf.Deg2Rad* angle);
            float dirX = Mathf.Sin(Mathf.Deg2Rad*angle);
            
            animator.SetFloat(Constants.CharacterRunDirectionXFloat,dirX);
            animator.SetFloat(Constants.CharacterRunDirectionYFloat,dirY);

            lineRenderer.enabled = true;
            
            transform.LookAt(transform.position + new Vector3(input.x*100,0,input.y*100));
            rotationByMovementTimer = 1f;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, trailDistance))
            {
                lineRenderer.SetPosition(1,new Vector3(hit.point.x,0.25f,hit.point.z));
            }
            else
            {
                var point = transform.position + transform.forward * trailDistance;
                lineRenderer.SetPosition(1, new Vector3(point.x,0.25f,point.z));
            }
            lineRenderer.SetPosition(0,new Vector3(transform.position.x,0.25f,transform.position.z));

        }

        public override void AddSkill(SkillSlots slots, GameObject skillObj)
        {
            GameObject skill = PhotonNetwork.Instantiate(Constants.SkillDataPath + skillObj.name, transform.position, transform.rotation);
            var skillMain = skill.GetComponent<SkillMain>();
            photonView.RPC("setParentSkill", RpcTarget.All, skill.GetComponent<PhotonView>().ViewID);
            //skillMain.transform.SetParent(transform);
            skillMain.transform.position = transform.position;
            skillMain.slot = slots;
            skills.Add(skillMain);
            SlotManager.Instance.AddSkill(skillMain);
        }


    }
}