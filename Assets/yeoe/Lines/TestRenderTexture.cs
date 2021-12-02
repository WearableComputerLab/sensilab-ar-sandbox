using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARSandbox
{
    public class TestRenderTexture : MonoBehaviour
    {
        public FireSimulation.FireSimulation fire;
        public Material mat;
        // Start is called before the first frame update
        void Start()
        {
            mat = GetComponent<MeshRenderer>().material;
        }

        // Update is called once per frame
        void Update()
        {
            //if (mat.mainTexture != fire.fireBreakMaskTex)
            //{
            //    mat.mainTexture = fire.fireBreakMaskTex;
            //}
        }
    }
}
