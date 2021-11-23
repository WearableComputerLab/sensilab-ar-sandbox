using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARSandbox
{
    public class AnnotationsManager : MonoBehaviour
    {
        public GameObject LinePrefab;
        public HandInput HandInput;
        public CalibrationManager CalibrationManager;
        public Toggle DrawToggle;
        public Toggle HideToggle;
        public static bool canDrawAnnotations = false;

        // Z Pos of annotations drawn to maintain above topography labels
        private const float Z_POS_OFFSET = 150;
        private GameObject currentLine;
        private LineRenderer lineRenderer;
        private List<Vector2> touchPositions;
        private List<GameObject> lines;

        // Previous canDraw state when isShowing is toggled as to allow easy return
        // to drawing from hide annotations if drawing was previously enabled.
        private bool oldCanDrawAnnotations = false;

        private bool isShowing = true;
        private Color currentColour;

        private void OnEnable()
        {
            touchPositions = new List<Vector2>();
            lines = new List<GameObject>();
            HandInput.OnGesturesReady += OnGesturesReady;
            CalibrationManager.OnCalibration += OnCalibration;

            oldCanDrawAnnotations = false;
            isShowing = true;
            canDrawAnnotations = false;
            DrawToggle.isOn = false;
            HideToggle.isOn = false;
        }
        private void OnDisable()
        {
            HandInput.OnGesturesReady -= OnGesturesReady;
            CalibrationManager.OnCalibration -= OnCalibration;

            DestroyLines();
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
            if (HandInput.GetCurrentGestures().Count != 0 && canDrawAnnotations)
            {
                gesture = HandInput.GetCurrentGestures()[0];
                if (currentLine == null)
                {
                    CreateLine(gesture.WorldPosition);
                }
                else
                {
                    UpdateLine(gesture.WorldPosition);
                }
            }
            else
            {
                currentLine = null;
                lineRenderer = null;
            }
        }
        private void CreateLine(Vector3 position)
        {
            position.z = Z_POS_OFFSET;
            currentLine = null;
            currentLine = Instantiate(LinePrefab, position, Quaternion.identity);
            currentLine.GetComponent<LineRenderer>().material.color = currentColour;

            lines.Add(currentLine.gameObject);
            lineRenderer = currentLine.GetComponent<LineRenderer>();
            touchPositions.Clear();
            touchPositions.Add(position);
            touchPositions.Add(position);
            lineRenderer.SetPosition(0, touchPositions[0]);
            lineRenderer.SetPosition(1, touchPositions[1]);
        }
        private void UpdateLine(Vector3 newTouchPos)
        {
            if (touchPositions != null && currentLine != null)
            {
                newTouchPos.z = Z_POS_OFFSET;
                Vector3 tempTouchPos = newTouchPos;
                if (Vector3.Distance(tempTouchPos, touchPositions[touchPositions.Count - 1]) > .1f)
                {
                    touchPositions.Add(newTouchPos);
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, newTouchPos);
                }
            }
        }
        public void ToggleCanDraw()
        {
            canDrawAnnotations = DrawToggle.isOn;
            if (canDrawAnnotations)
            {
                HideToggle.isOn = false;
                isShowing = true;
                foreach (GameObject line in lines)
                {
                    line.gameObject.SetActive(isShowing);
                }
            }
        }
        public void ToggleShowing()
        {
            isShowing = !HideToggle.isOn;

            // Hide annotations
            if (!isShowing)
            {
                oldCanDrawAnnotations = canDrawAnnotations;
                DrawToggle.isOn = false;
                canDrawAnnotations = false;
            }

            // Showing annotations
            else
            {
                if (oldCanDrawAnnotations == true)
                {
                    canDrawAnnotations = true;
                    DrawToggle.isOn = true;
                }
            }
            foreach (GameObject line in lines)
            {
                line.gameObject.SetActive(isShowing);
            }
        }
        public void SetColour(String newColourHex)
        {
            ColorUtility.TryParseHtmlString("#" + newColourHex, out currentColour);
        }
        public void DestroyLines()
        {
            foreach (GameObject line in lines)
            {
                Destroy(line);
            }
            lines.Clear();
        }

    }
}
