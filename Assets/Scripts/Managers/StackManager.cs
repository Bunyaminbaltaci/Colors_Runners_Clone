using UnityEngine;
using Controllers;
using Signals;
using Data.UnityObject;
using Commands;
using Datas.ValueObject;
using System.Collections.Generic;
using System.Collections;
using Data.ValueObject;
using Enums;

namespace Managers
{
    public class StackManager : MonoBehaviour
    {
        #region Self Veriables

        #region Public Veriables

        [Header("Data")] public StackData StackData;
        
        #endregion

        #region Serilazible Veriables

        [SerializeField] private List<GameObject> stackList = new List<GameObject>();
        [SerializeField] private GameObject levelHolder;
        [SerializeField] private StackManager stackManager;
       

        #endregion

        #region Private Veriables

        private CollectableAddOnStackCommand _collectableAddOnStackCommand;
        private StackLerpMovementCommand _stackLerpMovementCommand;
        private StackScaleCommand _stackScaleCommand;
        private StackJumpCommand _stackJumpCommand;
        private CollectableRemoveOnStackCommand _collectableRemoveOnStackCommand;
        private TransportInStack _transportInStack;
        private CollectableAnimSetCommand _collectableAnimSetCommand;
        private Transform _playerManager;
        private float _stackScore;
        
        #endregion

        #endregion

        #region Event Subscription

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            StackSignals.Instance.onAddInStack += OnAddInStack;
            StackSignals.Instance.onRemoveInStack += OnRemoveInStack;
            StackSignals.Instance.onTransportInStack += OnTransportInStack;
            StackSignals.Instance.onSendStackList += OnSendStacklistToPlayer;
            StackSignals.Instance.onSetCollectableAnimState += OnCollectableAnimState;
            // StackSignals.Instance.onStackJumpPlatform += OnStackJumpPlatform;
            CoreGameSignals.Instance.onReset += OnReset;
            CoreGameSignals.Instance.onPlay += OnPlay;
        }


        private void UnsubscribeEvents()
        {
            StackSignals.Instance.onAddInStack -= OnAddInStack;
            StackSignals.Instance.onRemoveInStack -= OnRemoveInStack;
            StackSignals.Instance.onTransportInStack -= OnTransportInStack;
            StackSignals.Instance.onSendStackList -= OnSendStacklistToPlayer;
            StackSignals.Instance.onSetCollectableAnimState -= OnCollectableAnimState;
            // StackSignals.Instance.onStackJumpPlatform += OnStackJumpPlatform;
            CoreGameSignals.Instance.onReset -= OnReset;
            CoreGameSignals.Instance.onPlay -= OnPlay;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion


        private StackData GetStackData()
        {
            return Resources.Load<CD_Stack>("Data/CD_Stack").Data;
        }
        
        private void Awake()
        {
            GetReferences();
        }
        
        private void Update()
        {
            if (!_playerManager)
                return;
            transform.position = new Vector3(0, 0, _playerManager.position.z - StackData.StackOffset);
            _stackLerpMovementCommand.Execute(ref _playerManager);
        }

        private void GetReferences()
        {
            StackData = GetStackData();
            _collectableAddOnStackCommand = new CollectableAddOnStackCommand(ref stackManager, ref stackList,ref StackData);
            _stackLerpMovementCommand = new StackLerpMovementCommand(ref stackList, ref StackData);
            _stackScaleCommand = new StackScaleCommand(ref stackList, ref StackData);
            _collectableRemoveOnStackCommand = new CollectableRemoveOnStackCommand(ref stackList, ref stackManager, ref levelHolder,ref StackData);
            _transportInStack = new TransportInStack(ref stackList,ref stackManager,ref levelHolder,ref StackData);
            _collectableAnimSetCommand = new CollectableAnimSetCommand();
            _stackJumpCommand = new StackJumpCommand(ref stackList, ref StackData);
        }
        private void FindPlayer()
        {
            if (!_playerManager)
            {
                _playerManager = FindObjectOfType<PlayerManager>().transform;
            }
        }
        private void OnAddInStack(GameObject obj)
        {
            StartCoroutine(_stackScaleCommand.Execute());
            _collectableAnimSetCommand.Execute(obj,CollectableAnimationStates.Run);
            _collectableAddOnStackCommand.Execute(obj);
        }

        private void OnRemoveInStack(GameObject obj)
        {
            _collectableRemoveOnStackCommand.Execute(obj);

        }

        // private void OnStackJumpPlatform()
        // {
        //     StartCoroutine(_stackJumpCommand.Execute(StackData.StackJumpDistance,StackData.StackJumpDuration));
        // }
        private void OnTransportInStack(GameObject obj)
        {
            StartCoroutine(_transportInStack.Execute(obj));
        }
        private void Initialized()
        {
            for (int i = 0; i < stackList.Count; i++)
            {
                CollectableAnimSet(stackList[i]);
            }
        }

        public void CollectableAnimSet(GameObject obj)
        {
            _collectableAnimSetCommand.Execute(obj,CollectableAnimationStates.Run);
        }
        private void OnCollectableAnimState(GameObject obj, CollectableAnimationStates animState)
        {
            Debug.Log(animState.ToString()+ " " + animState);
            _collectableAnimSetCommand.Execute(obj,animState);
        }

        public void OnSendStacklistToPlayer(List<GameObject> _stackList)
        {
            for (int i = 0; i < stackList.Count; i++)
            {
                _stackList.Add(stackList[i]);
            }
        }

        private void OnPlay()
        {
            FindPlayer();
            Initialized();
        }
        private void OnReset()
        {
            stackList.Clear();
            stackList.TrimExcess();
        }
    }
}