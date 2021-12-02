using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARSandbox
{
    public class LineDrawingManager : MonoBehaviour
    {
        public Sandbox Sandbox;
        public GameObject AnnotationLinePrefab;
        public GameObject FireBreakPrefab;
        public HandInput HandInput;
        public CalibrationManager CalibrationManager;
        public Camera LineDrawingMaskCamera;
        public Toggle DrawAnnotationToggle;
        public Toggle HideAnnotationToggle;
        public static bool CanDrawAnnotations = false;
        public static bool CanDrawFireBreak = false;
        public float FireBreakThickness { get; private set; }

        // Z Pos of annotations drawn to maintain above topography labels
        private const float Z_POS_OFFSET = 150;
        private GameObject currentLine;
        private LineRenderer lineRenderer;
        private List<Vector2> touchPositions;
        private RenderTexture maskRT;

        private List<GameObject> annotations;
        private bool isShowing = true;
        private Color currentColour;

        // Previous canDraw state when isShowing is toggled as to allow easy return
        // to drawing from hide annotations if drawing was previously enabled.
        private bool oldCanDrawAnnotations = false;

        private List<GameObject> fireBreaks;     

        private void Start()
        {
            CalibrationManager.OnCalibration += OnCalibration;
            Sandbox.OnSandboxReady += OnSandboxReady;
        }

        private void OnDestroy()
        {
            if (maskRT != null)
            {
                maskRT.Release();
            }
        }

        private void OnEnable()
        {
            touchPositions = new List<Vector2>();
            annotations = new List<GameObject>();
            fireBreaks = new List<GameObject>();
            FireBreakThickness = 1f;
            HandInput.OnGesturesReady += OnGesturesReady;
            CalibrationManager.OnCalibration += OnCalibration;

            oldCanDrawAnnotations = false;
            isShowing = true;
            CanDrawAnnotations = false;
            DrawAnnotationToggle.isOn = false;
            HideAnnotationToggle.isOn = false;

            // Set default start colour to black
            ColorUtility.TryParseHtmlString("#" + "0D0C0C", out currentColour);
        }
        private void OnDisable()
        {
            HandInput.OnGesturesReady -= OnGesturesReady;
            CalibrationManager.OnCalibration -= OnCalibration;

            DestroyAll();
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
            CalibrationManager.SetUpDataCamera(LineDrawingMaskCamera);
            CreateMaskRT();
            LineDrawingMaskCamera.targetTexture = maskRT;
            Sandbox.SetAnnotationsMaskRT(maskRT);
        }
        private void OnGesturesReady()
        {
            HandInputGesture gesture;
            if (HandInput.GetCurrentGestures().Count != 0 && (CanDrawAnnotations || CanDrawFireBreak))
            {
                gesture = HandInput.GetCurrentGestures()[0];
                if (!gesture.OutOfBounds && currentLine == null)
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
            if (CanDrawFireBreak)
            {
                currentLine = Instantiate(FireBreakPrefab, position, Quaternion.identity);
                currentLine.GetComponent<LineRenderer>().startWidth = FireBreakThickness;
                currentLine.GetComponent<LineRenderer>().endWidth = FireBreakThickness;
                fireBreaks.Add(currentLine.gameObject);
            } else
            {
                currentLine = Instantiate(AnnotationLinePrefab, position, Quaternion.identity);
                currentLine.GetComponent<LineRenderer>().material.color = currentColour;
                annotations.Add(currentLine.gameObject);
            }
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
        public void UI_ToggleDrawAnnotation()
        {
            CanDrawAnnotations = DrawAnnotationToggle.isOn;
            if (CanDrawAnnotations)
            {
                HideAnnotationToggle.isOn = false;
                isShowing = true;
                CanDrawFireBreak = false;
                foreach (GameObject line in annotations)
                {
                    line.gameObject.SetActive(isShowing);
                }
            }
        }
        public void UI_ToggleShowing()
        {
            isShowing = !HideAnnotationToggle.isOn;

            // Hide annotations
            if (!isShowing)
            {
                oldCanDrawAnnotations = CanDrawAnnotations;
                DrawAnnotationToggle.isOn = false;
                CanDrawAnnotations = false;
            }

            // Showing annotations
            else
            {
                if (oldCanDrawAnnotations == true)
                {
                    CanDrawAnnotations = true;
                    DrawAnnotationToggle.isOn = true;
                }
            }
            foreach (GameObject line in annotations)
            {
                line.gameObject.SetActive(isShowing);
            }
        }

        public void UI_ToggleDrawFireBreak()
        {
            CanDrawFireBreak = !CanDrawFireBreak;
            if (CanDrawFireBreak)
            {
                CanDrawAnnotations = false;
                DrawAnnotationToggle.isOn = false;
            }
        }
        public void UI_SetColour(String newColourHex)
        {
            ColorUtility.TryParseHtmlString("#" + newColourHex, out currentColour);
            DrawAnnotationToggle.isOn = true;
            CanDrawAnnotations = true;
        }
        public void UI_ChangeFireBreakThickness(float thickness)
        {
            FireBreakThickness = thickness;
        }
        public void DestroyAnnotations()
        {
            foreach (GameObject annotation in annotations)
            {
                Destroy(annotation);
            }
            annotations.Clear();
        }        
        public void DestroyFireBreaks()
        {
            foreach (GameObject fireBreak in fireBreaks)
            {
                Destroy(fireBreak);
            }
            fireBreaks.Clear();
        }

        public void DestroyAll()
        {
            DestroyAnnotations();
            DestroyFireBreaks();
        }

        private void CreateMaskRT()
        {
            if (maskRT != null) maskRT.Release();
            SandboxDescriptor sandboxDescriptor = Sandbox.GetSandboxDescriptor();
            float aspectRatio = (float)sandboxDescriptor.DataSize.x / (float)sandboxDescriptor.DataSize.y;
            Point maskRT_Size = new Point(Mathf.CeilToInt(1080 * aspectRatio), 1080);

            maskRT = new RenderTexture(maskRT_Size.x, maskRT_Size.y, 0);
            maskRT.Create();
        }
    }
}
