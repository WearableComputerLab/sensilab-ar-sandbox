using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    
    [Range(0, 360)]
    public float viewAngle;

    public float meshResolution;
    public MeshFilter viewMeshFilter;
    public GameObject childFOVSphere;
    Mesh viewMesh;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }
    public Vector3 DirFromAngle(float angleInDegress, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegress += transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Sin(angleInDegress * Mathf.Deg2Rad), Mathf.Cos(angleInDegress * Mathf.Deg2Rad), 0 );
    }
    public void UpdateFOV(float dist, Vector3 clickPos)
    {
        rotateTowards(clickPos);
        childFOVSphere.transform.position = clickPos;
        viewRadius = dist;
    }
    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            viewPoints.Add(transform.position + DirFromAngle(angle, true) * viewRadius);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount -2) * 3];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    private void LateUpdate()
    {
       DrawFieldOfView();
    }

    // Rotates the FOV indicator in the z-axis as to correctly face the current click pos
    private void rotateTowards(Vector3 to)
    {
        if (to - transform.position != Vector3.zero)
        {
            Quaternion _lookRotation = Quaternion.LookRotation((to - transform.position), Vector3.forward);
            if (transform.position.x > to.x)
            {
                if (to.y > transform.position.y)
                {
                    _lookRotation = Quaternion.Euler(_lookRotation.eulerAngles.x, _lookRotation.eulerAngles.y, 630 - _lookRotation.eulerAngles.x);
                }
                else
                {
                    _lookRotation = Quaternion.Euler(_lookRotation.eulerAngles.x, _lookRotation.eulerAngles.y, (_lookRotation.eulerAngles.x * -1) - 90);
                }
            }
            else
                _lookRotation = Quaternion.Euler(_lookRotation.eulerAngles.x, _lookRotation.eulerAngles.y, _lookRotation.eulerAngles.x + 90);

            transform.rotation = _lookRotation;
        }
    }
}
