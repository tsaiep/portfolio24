using UnityEngine;

public class InstancedIndirectRenderer : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public int instanceCount = 1000;
    public float spawnRange = 1f;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer dataBuffer;

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    struct MeshTransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public Vector4 Color;
    }

    void Start()
    {
        InitializeBuffers();
    }

    void InitializeBuffers()
    {
        // 初始化參數緩衝區
        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        if (mesh != null)
        {
            args[0] = (uint)mesh.GetIndexCount(0);
            args[1] = (uint)instanceCount;
            args[2] = (uint)mesh.GetIndexStart(0);
            args[3] = (uint)mesh.GetBaseVertex(0);
        }

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        // 初始化實例數據緩衝區
        MeshTransformData[] data = new MeshTransformData[instanceCount];
        for (int i = 0; i < instanceCount; i++)
        {
            data[i] = new MeshTransformData()
            {
                Position = Random.insideUnitSphere * spawnRange,
                Rotation = Random.rotation,
                Scale = Vector3.one,
                Color = new Vector4(Random.value, Random.value, Random.value, 1f)
            };
        }

        int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(MeshTransformData));
        dataBuffer = new ComputeBuffer(instanceCount, size);
        dataBuffer.SetData(data);

        // 將緩衝區設置到材質上
        material.SetBuffer("_PerInstanceData", dataBuffer);
    }

    void Update()
    {
        if (argsBuffer != null && dataBuffer != null)
        {
            Graphics.DrawMeshInstancedIndirect(mesh, 0, material, new Bounds(Vector3.zero, Vector3.one * 1000f), argsBuffer);
        }
    }

    void OnDisable()
    {
        if (argsBuffer != null)
        {
            argsBuffer.Release();
            argsBuffer = null;
        }
        if (dataBuffer != null)
        {
            dataBuffer.Release();
            dataBuffer = null;
        }
    }
}
