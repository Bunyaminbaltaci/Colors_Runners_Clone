﻿using UnityEngine;
using System.Collections;
using RootMotion;
using RootMotion.FinalIK;
using DG.Tweening;

namespace Controllers
{
    
    public class TurretController : MonoBehaviour
    {
       
        [System.Serializable]
        public class Part
        {
            public Transform transform; 
            private RotationLimit rotationLimit; 

         
            public void AimAt(Transform target)
            {
                transform.LookAt(new Vector3(target.position.x, target.position.y + 0.3f, target.position.z),
                    transform.up);

               
                if (rotationLimit == null)
                {
                    rotationLimit = transform.GetComponent<RotationLimit>();
                    rotationLimit.Disable();
                }

           
                rotationLimit.Apply();
            }
        }

        public Transform targetPlayer; 
        public Transform targetRandom;
        public Part[] parts;

        private void Start()
        {
            targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
            targetRandom = GameObject.FindGameObjectWithTag("Target").transform;
            StartCoroutine(SetTarget());
        }

        private IEnumerator SetTarget()
        {
            while (true)
            {
                float xValue = Random.Range(-1f, 1f);
                float yValue = Random.Range(0.5f, 1f);
                float zValue = Random.Range(-1f, 1f);
                targetRandom.DOLocalMove(new Vector3(xValue, yValue, zValue), 1f);
                yield return new WaitForSeconds(1.1f);
            }
        }

        public bool isTargetPlayer;

        void Update()
        {
            if (isTargetPlayer)
            {
                foreach (Part part in parts) part.AimAt(targetPlayer);
                return;
            }

            foreach (Part part in parts) part.AimAt(targetRandom);
        }

        private void ShootPlayer()
        {
        }
    }
}