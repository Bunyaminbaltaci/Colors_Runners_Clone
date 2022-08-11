using UnityEngine;
using Signals;
using Managers;
using Enums;
using DG.Tweening;

namespace Controllers
{
    public class CollectablePhysicsController : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables

        public bool isTaken;

        #endregion

        #region Serializable Variables

        [SerializeField] private CollectableManager _collectableManager;
        [SerializeField]
        private GameObject collectableMeshObj;

        #endregion

        #region Private Variables

        [SerializeField]
        private SkinnedMeshRenderer _collectableSkinnedMeshRenderer;

        #endregion

        #endregion

        private void Awake()
        {
            GetReferences();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Collectable") && isTaken)
            {
                var otherPhysic = other.gameObject.GetComponent<CollectablePhysicsController>();
                if (!otherPhysic.isTaken)
                {
                    otherPhysic.isTaken = true;
                    StackSignals.Instance.onAddInStack?.Invoke(other.gameObject.transform.parent.gameObject);
                }
            }

            if (other.CompareTag("Gate"))
            {
                var otherMR = other.gameObject.transform.parent.GetComponentInChildren<MeshRenderer>();
                _collectableSkinnedMeshRenderer.material.color = otherMR.material.color;
            }

            if (other.CompareTag("CheckArea"))
            {
                var type = other.gameObject.GetComponentInParent<ColorCheckAreaManager>().areaType;
                switch (type)
                {
                    case ColorCheckAreaType.Drone:
                        CollectablesMovementInDrone();
                        break;
                    case ColorCheckAreaType.Turret:
                        //change animation state 
                        break;
                }
            }

            if (other.CompareTag("ColorCheck"))
            {
                StackSignals.Instance.onTransportInStack?.Invoke(transform.parent.gameObject);

                transform.parent.transform.DOMove(other.gameObject.transform.position, 1f);

            }
        }

        private void GetReferences()
        {
            _collectableSkinnedMeshRenderer = collectableMeshObj.GetComponentInChildren<SkinnedMeshRenderer>();
        }

        private void CollectablesMovementInDrone()
        {
            //ColorCheckAreaSignals.Instance.onDroneActive?.Invoke();

            //StackSignals.Instance.onDecreaseStack?.Invoke(this.gameObject);
        }
    }
}
