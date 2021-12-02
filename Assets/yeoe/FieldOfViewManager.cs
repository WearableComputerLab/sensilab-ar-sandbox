using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARSandbox
{
    public class FieldOfViewManager : MonoBehaviour  {

        public Sandbox Sandbox;
        public HandInput HandInput;
        public CalibrationManager CalibrationManager;
        public GameObject FOVIndicatorPrefab;
        public Slider UI_FOVAngleSlider;
        public bool canDraw = false;

        private Vector3 firstTouch;
        private Vector3 currentHoldTouch;
        private FieldOfView currentFOV;
        private float fovAngle = 90;
        private List<GameObject> allFOVsGO;
        private void Start()
        {
            CalibrationManager.OnCalibration += OnCalibration;
            Sandbox.OnSandboxReady += OnSandboxReady;
            UI_FOVAngleSlider.value = fovAngle;
            allFOVsGO = new List<GameObject>();
        }

        private void OnEnable()
        {
            HandInput.OnGesturesReady += OnGesturesReady;
            CalibrationManager.OnCalibration += OnCalibration;
            firstTouch = Vector3.zero;
            currentHoldTouch = Vector3.zero;
        }
        private void OnDisable()
        {
            HandInput.OnGesturesReady -= OnGesturesReady;
            CalibrationManager.OnCalibration -= OnCalibration;
        }
        private void OnCalibration()
        {
            HandInput.OnGesturesReady -= OnGesturesReady;
            Sandbox.OnSandboxReady += OnSandboxReady;

        }

        // Called only after a successful calibration.
        private void OnSandboxReady()
        {
            HandInput.OnGesturesReady += OnGesturesReady;
        }

        private void OnGesturesReady()
        {
            HandInputGesture gesture;
            if (HandInput.GetCurrentGestures().Count != 0 && canDraw)
            {                
                gesture = HandInput.GetCurrentGestures()[0];
                if (!gesture.OutOfBounds)
                {
                    // First UI Touch Down
                    if (firstTouch == Vector3.zero)
                    {
                        firstTouch = gesture.WorldPosition;
                        GameObject FOVGameObject = Instantiate(FOVIndicatorPrefab, gesture.WorldPosition, FOVIndicatorPrefab.gameObject.transform.rotation);
                        allFOVsGO.Add(FOVGameObject);

                        currentFOV = FOVGameObject.GetComponent<FieldOfView>();
                        currentFOV.viewAngle = fovAngle;
                    }
                    // UI Touch Hold
                    else
                    {
                        currentHoldTouch = new Vector3(gesture.WorldPosition.x, gesture.WorldPosition.y, firstTouch.z);

                        // Distance from first UI touch down
                        float dist = Vector3.Distance(firstTouch, currentHoldTouch);
                        currentFOV.UpdateFOV(dist, currentHoldTouch);
                    }
                }
            }
            else
            {
                firstTouch = Vector3.zero;
                currentHoldTouch = Vector3.zero;
                currentFOV = null;
            }
        }

        public void UI_ChangeFOVRadius(float radius)
        {
            fovAngle = radius;
            foreach (GameObject fovGO in allFOVsGO)
            {
                FieldOfView fov = fovGO.GetComponent<FieldOfView>();
                fov.viewAngle = radius;
            }
        }

        public void UI_DestroyFOVs()
        {
            foreach (GameObject fovGO in allFOVsGO)
            {
                Destroy(fovGO);
            }
            allFOVsGO.Clear();
        }
    }
}