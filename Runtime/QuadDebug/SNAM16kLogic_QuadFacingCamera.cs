
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Eloi.SNAM;

public class SNAM16kLogic_QuadFacingCamera : MonoBehaviour
{
    public MeshFilter m_meshFilter;
    public SkinnedMeshRenderer m_skinMeshRenderer;
    

    [Header("Debug")]
    public Mesh m_currentMesh;
    public ProcessBulletsForParallels m_job;
    public SNAM16K_ObjectBool m_quadToDisplay;
    public SNAM16K_ObjectVector3 m_quadPosition;
    public SNAM16K_ObjectFloat m_quadSizeRadius;
    public NativeArray<Vector3> m_verticePosition;
    public Transform m_objectToLookAt;
    public float m_skinnedMeshBoundDistance = ushort.MaxValue / 1000;

    private void Awake()
    {

        if (m_objectToLookAt == null && Camera.main)
            m_objectToLookAt = Camera.main.transform;
        m_verticePosition = new NativeArray<Vector3>(SNAM16K.ARRAY_MAX_SIZE * 4, Allocator.Persistent);
        RefreshQuadMeshPosition();
        Hum();
        Invoke("RefreshNativeArray", 1);

        RefreshNativeArray();
    }

    private void RefreshNativeArray()
    {
       m_job.m_quadPosition = m_quadPosition.GetNativeArrayHolder().GetNativeArray();
       m_job.m_squareSize = m_quadSizeRadius.GetNativeArrayHolder().GetNativeArray();
       m_job.m_isActiveObject = m_quadToDisplay.GetNativeArrayHolder().GetNativeArray();

    }

    private void Hum()
    {
        if (m_skinMeshRenderer != null)
        {
            Bounds b = new Bounds(Vector3.zero, Vector3.one * m_skinnedMeshBoundDistance);
            m_skinMeshRenderer.bounds = b;
            m_skinMeshRenderer.updateWhenOffscreen = true;
            m_skinMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            m_skinMeshRenderer.receiveShadows = false;
            m_skinMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        }
    }

    private void OnDestroy()
    {
        if (m_verticePosition.IsCreated)
            m_verticePosition.Dispose();
    }






    private void Randomize32()
    {
        for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
        {
            float r = m_skinnedMeshBoundDistance / 2;
            m_quadToDisplay[i] = UnityEngine.Random.value > 0.5f;
            m_quadPosition[i] = new Vector3(UnityEngine.Random.Range(-r, r), UnityEngine.Random.Range(-r, r), UnityEngine.Random.Range(-r, r));
            m_quadSizeRadius[i] = UnityEngine.Random.Range(0.5f, 2f);
        }
    }


    private bool m_isInitiatized = false;
    public void RefreshQuadMeshPosition()
    {
        if (!m_isInitiatized)
        {
            m_isInitiatized = true;
            InitWithCount();
        }

        Hum();
        if (m_objectToLookAt == null)
            m_objectToLookAt = Camera.main.transform;
        m_job.m_cameraPosition = transform.InverseTransformPoint(m_objectToLookAt.position);
        m_job.m_maxQuad = SNAM16K.ARRAY_MAX_SIZE;
        m_job.m_resultVerticePositions = m_verticePosition;
        m_job.m_radiusFactor = m_radiusFactor;
        JobHandle jh = m_job.Schedule(SNAM16K.ARRAY_MAX_SIZE, 64);
        jh.Complete();
        m_currentMesh.SetVertices(m_job.m_resultVerticePositions);
        Bounds b = new Bounds(Vector3.zero, Vector3.one * m_skinnedMeshBoundDistance);
        m_skinMeshRenderer.bounds = b;


    }

    public void InitWithCount()
    {
        m_currentMesh = new Mesh();
        m_currentMesh.name = "Bullet as square at far distance";
        Vector3[] vertices = new Vector3[SNAM16K.ARRAY_MAX_SIZE * 4];
        Vector2[] uvs = new Vector2[SNAM16K.ARRAY_MAX_SIZE * 4];
        int[] triangles = new int[SNAM16K.ARRAY_MAX_SIZE * 6];

        m_currentMesh.bounds = new Bounds(Vector3.zero, Vector3.one * m_skinnedMeshBoundDistance);

        for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
        {
            int iuv = i * 4;
            int itv = i * 6;
            uvs[iuv + 0] = new Vector2(0, 0);
            uvs[iuv + 1] = new Vector2(1, 0);
            uvs[iuv + 2] = new Vector2(0, 1);
            uvs[iuv + 3] = new Vector2(1, 1);
            triangles[itv + 0] = iuv + 0;
            triangles[itv + 1] = iuv + 2;
            triangles[itv + 2] = iuv + 1;
            triangles[itv + 3] = iuv + 2;
            triangles[itv + 4] = iuv + 3;
            triangles[itv + 5] = iuv + 1;
        }
        m_currentMesh.SetVertices(vertices);
        m_currentMesh.SetUVs(0, uvs);
        m_currentMesh.SetTriangles(triangles, 0);
        m_meshFilter.sharedMesh = m_currentMesh;
        m_meshFilter.mesh = m_currentMesh;
        if (m_skinMeshRenderer != null)
        {

            m_skinMeshRenderer.sharedMesh = m_currentMesh;
        }


        m_job = new ProcessBulletsForParallels();
        m_job.m_quadPosition = m_quadPosition.GetNativeArrayHolder().GetNativeArray();
        m_job.m_squareSize = m_quadSizeRadius.GetNativeArrayHolder().GetNativeArray();
        m_job.m_resultVerticePositions = m_verticePosition;
        m_job.m_isActiveObject = m_quadToDisplay.GetNativeArrayHolder().GetNativeArray();
        m_job.m_hidePosition = new Vector3(-404, 404, 404);
        m_job.m_radiusFactor = m_radiusFactor;
        m_job.m_maxQuad = SNAM16K.ARRAY_MAX_SIZE;
        ;

        IgnoreWeighBoneError();

    }

    private void IgnoreWeighBoneError()
    {

        // Get the mesh from the SkinnedMeshRenderer
        Mesh mesh = m_skinMeshRenderer.sharedMesh;

        // Create an array for bone weights (one bone influences all vertices)
        BoneWeight[] boneWeights = new BoneWeight[mesh.vertexCount];

        // Create the bind pose for the single bone (which is the transform of the object itself)
        Matrix4x4[] bindPoses = new Matrix4x4[1]; // Only one bone

        // The bind pose is the inverse of the object's local-to-world matrix
        bindPoses[0] = transform.worldToLocalMatrix * transform.localToWorldMatrix;

        // Assign bone weights to each vertex (all vertices are influenced by this single bone)
        for (int i = 0; i < boneWeights.Length; i++)
        {
            // Assign all vertices to be controlled by the single bone at index 0
            boneWeights[i].boneIndex0 = 0;  // Single bone index
            boneWeights[i].weight0 = 1f;    // Full weight of 1 for the only bone
        }

        // Assign the bone weights and bind poses to the mesh
        mesh.boneWeights = boneWeights;
        mesh.bindposes = bindPoses;

        // Assign the object's transform as the single bone
        m_skinMeshRenderer.bones = new Transform[] { transform };

        // Now the mesh is influenced by its own transform as a bone
    }

    public float m_radiusFactor = 1.2f;

       [BurstCompile(CompileSynchronously = true)]

    // [BurstCompile]
    public struct ProcessBulletsForParallels : IJobParallelFor
    {
        public int m_maxQuad;


        [ReadOnly] public NativeArray<bool> m_isActiveObject;
        [ReadOnly] public NativeArray<Vector3> m_quadPosition;
        [ReadOnly] public NativeArray<float> m_squareSize;
        [NativeDisableParallelForRestriction]
        [WriteOnly] public NativeArray<Vector3> m_resultVerticePositions;
        public Vector3 m_hidePosition;
        public Vector3 m_cameraPosition;
        Quaternion m_pbl;
        Quaternion m_pbr;
        Quaternion m_ptl;
        Quaternion m_ptr;

        public float m_radiusFactor;

        public void Execute(int index)
        {
            if (index >= m_maxQuad)
                return;
            bool isBulletUsed = m_isActiveObject[index];
            int vertexIndexPosition = index * 4;
            if (!isBulletUsed)
            {

                m_resultVerticePositions[vertexIndexPosition + 0] = Vector3.zero;
                m_resultVerticePositions[vertexIndexPosition + 1] = Vector3.zero;
                m_resultVerticePositions[vertexIndexPosition + 2] = Vector3.zero;
                m_resultVerticePositions[vertexIndexPosition + 3] = Vector3.zero;
                return;
            }
            else
            {
                float radius = m_squareSize[index];
                float squareSize = (float)Math.Sqrt(radius * radius + radius * radius);
                Vector3 dir = -(m_quadPosition[index] - m_cameraPosition);
                Quaternion cQ = Quaternion.LookRotation(dir, Vector3.up);
                Vector3 position = m_quadPosition[index];

                SetBorderLocal();
                m_resultVerticePositions[vertexIndexPosition + 0] = position + ((cQ * m_pbl) * Vector3.forward * squareSize * m_radiusFactor);
                m_resultVerticePositions[vertexIndexPosition + 1] = position + ((cQ * m_pbr) * Vector3.forward * squareSize * m_radiusFactor);
                m_resultVerticePositions[vertexIndexPosition + 2] = position + ((cQ * m_ptl) * Vector3.forward * squareSize * m_radiusFactor);
                m_resultVerticePositions[vertexIndexPosition + 3] = position + ((cQ * m_ptr) * Vector3.forward * squareSize * m_radiusFactor);



            }
        }


        public void SetBorderLocal()
        {
            this.m_pbl = Quaternion.LookRotation(new Vector3(1, -1, 0), Vector3.up);
            this.m_pbr = Quaternion.LookRotation(new Vector3(-1, -1, 0), Vector3.up);
            this.m_ptl = Quaternion.LookRotation(new Vector3(1, 1, 0), Vector3.up);
            this.m_ptr = Quaternion.LookRotation(new Vector3(-1, 1, 0), Vector3.up);
        }
        public static Vector3 Rotated(Vector3 vector, Quaternion rotation, Vector3 pivot = default(Vector3))
        {
            return rotation * (vector - pivot) + pivot;
        }

        public static Vector3 Rotated(Vector3 vector, Vector3 rotation, Vector3 pivot = default(Vector3))
        {
            return Rotated(vector, Quaternion.Euler(rotation), pivot);
        }

        public static Vector3 Rotated(Vector3 vector, float x, float y, float z, Vector3 pivot = default(Vector3))
        {
            return Rotated(vector, Quaternion.Euler(x, y, z), pivot);
        }


    }
}


