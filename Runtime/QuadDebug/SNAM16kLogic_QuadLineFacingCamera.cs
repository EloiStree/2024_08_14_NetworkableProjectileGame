using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

using Eloi.SNAM;
public class SNAM16kLogic_QuadLineFacingCamera : MonoBehaviour
{


    public MeshFilter m_meshFilter;
    public SkinnedMeshRenderer m_skinMeshRenderer;

    [Header("Debug")]
    public Mesh m_currentMesh;
    public STRUCTJOB_ProcessLineQuadFacingCamera m_job;
    public SNAM16K_ObjectBool m_quadToDisplay;
    public SNAM16K_ObjectVector3 m_quadLinePositionCurrent;
    public SNAM16K_ObjectVector3 m_quadLinePositionPrevious;
    public SNAM16K_ObjectFloat m_quadLineSizeRadius;
    public Transform m_objectToLookAt;
    public float m_skinnedMeshBoundDistance = ushort.MaxValue / 1000;
    public float m_radiusFactor = 1.0f;

    [Header("Memory Storage")]
    public NativeArray<Vector3> m_quadCornerPosition;


    private void Awake()
    {

        if (m_objectToLookAt == null && Camera.main)
            m_objectToLookAt = Camera.main.transform;
        m_quadCornerPosition = new NativeArray<Vector3>(SNAM16K.ARRAY_MAX_SIZE * 4, Allocator.Persistent);
        m_isInitiatized = false;
        RefreshQuadMeshPosition();
        Invoke("RefreshNativeArray", 1);
    }
    private void OnDestroy()
    {
        if (m_quadCornerPosition.IsCreated)
            m_quadCornerPosition.Dispose();
    }
    public void RefreshNativeArray()
    {
        m_job.m_quadPositionCurrent = m_quadLinePositionCurrent.GetNativeArrayHolder().GetNativeArray();
        m_job.m_quadPositionPrevious = m_quadLinePositionPrevious.GetNativeArrayHolder().GetNativeArray();
        m_job.m_capsuleRadius = m_quadLineSizeRadius.GetNativeArrayHolder().GetNativeArray();
        m_job.m_isActiveObject = m_quadToDisplay.GetNativeArrayHolder().GetNativeArray();
    }


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

      

        m_job.m_resultVerticepositions = m_quadCornerPosition;
        m_job.m_cameraUp = m_objectToLookAt.up; // on ZERO AXIS NEED CHANGE
        m_job.m_cameraRight = m_objectToLookAt.right; // on ZERO AXIS NEED CHANGE
        m_job.m_cameraForward = m_objectToLookAt.forward; // on ZERO AXIS NEED CHANGE
        m_job.m_hidePosition = new Vector3(-404, 404, 404);
        m_job.m_radiusFactor = m_radiusFactor;
        JobHandle jh = m_job.Schedule(SNAM16K.ARRAY_MAX_SIZE, 64);
        jh.Complete();
        m_currentMesh.SetVertices(m_job.m_resultVerticepositions);
        Bounds b = new Bounds(Vector3.zero, Vector3.one * m_skinnedMeshBoundDistance);
        m_skinMeshRenderer.bounds = b;




    }

    #region DONE




    private void Randomize32()
    {
        for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
        {
            float r = m_skinnedMeshBoundDistance / 2;
            m_quadToDisplay[i] = UnityEngine.Random.value > 0.5f;
            m_quadLinePositionCurrent[i] = new Vector3(UnityEngine.Random.Range(-r, r), UnityEngine.Random.Range(-r, r), UnityEngine.Random.Range(-r, r));
            m_quadLineSizeRadius[i] = UnityEngine.Random.Range(0.5f, 2f);
        }
    }


    private bool m_isInitiatized = false;
   
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

        m_job.m_quadPositionCurrent = m_quadLinePositionCurrent.GetNativeArrayHolder().GetNativeArray();
        m_job.m_quadPositionPrevious = m_quadLinePositionPrevious.GetNativeArrayHolder().GetNativeArray();
        m_job.m_capsuleRadius = m_quadLineSizeRadius.GetNativeArrayHolder().GetNativeArray();
        m_job.m_isActiveObject = m_quadToDisplay.GetNativeArrayHolder().GetNativeArray();
        m_job.m_resultVerticepositions = m_quadCornerPosition;
        m_job.m_hidePosition = new Vector3(-404,404,404);
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

    #endregion


    [BurstCompile(CompileSynchronously = true)]
    //[BurstCompile]
    public struct STRUCTJOB_ProcessLineQuadFacingCamera : IJobParallelFor
    {
      
        [ReadOnly] public NativeArray<bool> m_isActiveObject;
        [ReadOnly] public NativeArray<Vector3> m_quadPositionCurrent;
        [ReadOnly] public NativeArray<Vector3> m_quadPositionPrevious;
        [ReadOnly] public NativeArray<float> m_capsuleRadius;

        [NativeDisableParallelForRestriction]
        [WriteOnly] public NativeArray<Vector3> m_resultVerticepositions;
        public Vector3 m_hidePosition;
        public Vector3 m_cameraPosition;
        public Vector3 m_hidePoint;
        public Vector3 m_cameraUp;
        public Vector3 m_cameraRight;
        public Vector3 m_cameraForward;
        public float m_radiusFactor;
        //Just to test;
        public float m_rotationTest;

        public void Execute(int index)
        {
            
            bool isBulletUsed = m_isActiveObject[index];
            int vertexIndexPosition = index * 4;
            if (!isBulletUsed)
            {

                m_resultVerticepositions[vertexIndexPosition + 0]= m_hidePoint;
                m_resultVerticepositions[vertexIndexPosition + 1]= m_hidePoint;
                m_resultVerticepositions[vertexIndexPosition + 2]= m_hidePoint;
                m_resultVerticepositions[vertexIndexPosition + 3] = m_hidePoint;
                return;
            }
            else
            {
                Vector3 c = m_quadPositionCurrent[index];
                Vector3 p = m_quadPositionPrevious[index];

                Vector3 dir= (c - p).normalized;
                Vector3 middle = (c + p) *0.5f;
                Vector3 dirCameraCurrrent= (m_cameraPosition - c).normalized;
                Vector3 dirCameraPrevious = (m_cameraPosition - p).normalized;
                float radius = m_capsuleRadius[index];

                Vector3 crossTopLeft = Vector3.Cross(dir, dirCameraPrevious).normalized;
                Vector3 crossBottomLeft = -crossTopLeft;
                Vector3 crossTopRight = Vector3.Cross(dir,  dirCameraCurrrent).normalized;
                Vector3 crossBottomRight = -crossTopRight;

               Vector3 v0 = p + crossBottomLeft * radius * m_radiusFactor; //pbl 
               Vector3 v1 = c + crossBottomRight * radius * m_radiusFactor;  //pbr
               Vector3 v2 = p + crossTopLeft * radius * m_radiusFactor; //ptl 
               Vector3 v3 = c + crossTopRight * radius * m_radiusFactor;  //ptr



                m_resultVerticepositions[vertexIndexPosition + 0]  =v0 ;//pbl 
                m_resultVerticepositions[vertexIndexPosition + 1]  =v1 ;  //pbr
                m_resultVerticepositions[vertexIndexPosition + 2]  =v2 ; //ptl 
                m_resultVerticepositions[vertexIndexPosition + 3] = v3;  //ptr
            }
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


