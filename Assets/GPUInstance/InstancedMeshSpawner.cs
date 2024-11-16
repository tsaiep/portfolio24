using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class InstancedMeshSpawner : MonoBehaviour
{
    [Header("Mesh and Material Settings")]
    public Mesh mesh;
    public Material material;

    [Header("Instance Settings")]
    public int spawnCount = 1000;
    public float spawnRadius = 50f;

    void Update()
    {
        if (mesh == null || material == null)
            return;

        // 設定每批次最大實例數量
        const int batchSize = 1023;
        int totalBatches = Mathf.CeilToInt((float)spawnCount / batchSize);

        for (int i = 0; i < totalBatches; i++)
        {
            int count = Mathf.Min(batchSize, spawnCount - i * batchSize);
            Matrix4x4[] batchMatrices = new Matrix4x4[count];

            // 這裡不需要預先計算變換矩陣
            // 只需傳遞實例 ID 或其他必要資訊

            Graphics.DrawMeshInstanced(mesh, 0, material, batchMatrices, count);
        }
    }
}