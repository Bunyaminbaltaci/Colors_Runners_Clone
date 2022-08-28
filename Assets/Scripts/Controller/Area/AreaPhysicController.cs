using System;
using Enums;
using Managers;
using Signals;
using UnityEngine;

namespace Controller
{
    public class AreaPhysicController : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables

        

        #endregion

        #region Serialized Variables

        [SerializeField] private AreaManager _manager;

        #endregion

        #region Private Variables
        

        #endregion

        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Collectable"))
            {
               PoolSignals.Instance.onSendPool?.Invoke(other.transform.parent.gameObject,PoolType.Collectable);
            }
        }
    }
}