using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// using Valve.VR.InteractionSystem;

namespace Valve.VR.InteractionSystem
{
    public class Drill : MonoBehaviour
    {
        Hand hand;
        public ItemPackage itemPackage;
        public UnityEvent onDrillSpin, onShutDown;
        public Transform tip;
        public Text text;


        private void Start()
        {
            onDrillSpin.AddListener(CheckForScrewable);
            onShutDown.AddListener(HandleShutDown);
        }

        protected virtual void OnAttachedToHand(Hand attached)
        {
            hand = attached;
            Debug.Log("Attached hand " + hand);
            Debug.Log("Other hand " + hand.otherHand.currentAttachedObject);
        }

        void HandAttachedUpdate()
        {
            Debug.DrawRay(tip.transform.position, tip.forward * 100, Color.red);

            if (hand.GetGrabStarting() == GrabTypes.Pinch)
            {
                Debug.Log("Grabbed Pinch");
                Debug.Log("Drill spin");
                onDrillSpin.Invoke();
            }
        }

        private void CheckForScrewable()
        {
            if (Physics.Raycast(new Ray(tip.transform.position, tip.forward), out RaycastHit raycastHit, 100))
            {
                GameObject colliderGameObject = raycastHit.collider.gameObject;
                Debug.Log("Drill tip hit" + raycastHit.collider.gameObject.name);
                if (colliderGameObject.TryGetComponent(out Screwable screwable))
                {
                    hand.TriggerHapticPulse(100);
                    screwable.ScrewIn();
                    float spd = ShortestPerpendicularDistance(tip.position, colliderGameObject.transform.position,
                        transform.forward, colliderGameObject.transform.up);
                    float alpha = AngleBetween(tip.forward, -1 * colliderGameObject.transform.up);
                    Debug.Log("Distance " + spd);
                    Debug.Log("Angle " + alpha);
                    text.text = "Distance " + spd.ToString("0.####") + "-mm Angle " +
                                (alpha * Mathf.Rad2Deg).ToString("0.##") + "-deg";
                    // onScrewIn(s, alpha);
                }
            }
            else
            {
                Debug.Log("Drill tip missed");
            }
        }


        private void HandleShutDown()
        {
            if (hand == null) return;
            if (hand.otherHand.currentAttachedObject == null) return;
            if (hand.otherHand.currentAttachedObject.GetComponent<ItemPackage>() == null) return;
            if (hand.otherHand.currentAttachedObject.GetComponent<ItemPackage>().otherHandItemPrefab
                .GetType() == itemPackage.otherHandItemPrefab.GetType())
            {
                Debug.Log("Removing screw");
                hand.otherHand.DetachObject(hand.otherHand.currentAttachedObject);
            }
        }

        private static float AngleBetween(Vector3 direction1, Vector3 direction2)
        {
            float dot = Vector3.Dot(direction1, direction2);
            dot *= (1 / (direction1.magnitude * direction2.magnitude));
            return Mathf.Acos(dot);
        }

        private static float ShortestPerpendicularDistance(Vector3 origin1, Vector3 origin2, Vector3 direction1,
            Vector3 direction2)
        {
            Vector3 perpendicularDirection = Vector3.Cross(direction1, direction2);
            float distance = Vector3.Dot(perpendicularDirection, (origin1 - origin2)) /
                             perpendicularDirection.magnitude;
            return distance;
        }

        private void OnDestroy()
        {
            onShutDown?.Invoke();
        }
    }
}