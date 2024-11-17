using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 使用 Graphics.DrawMeshInstanced 來實例化並渲染多個網格實例。
/// </summary>
public class InstancedMeshSpawner : MonoBehaviour
{
    // 公開變數，可在 Unity 編輯器中設置
    [Header("Mesh and Material Settings")]
    [Tooltip("要實例化的網格")]
    public Mesh mesh;

    [Tooltip("用於渲染網格的材質")]
    public Material material;

    [Header("Instance Settings")]
    [Tooltip("要生成的實例數量")]
    public int spawnCount = 1000;

    [Tooltip("生成實例的位置半徑範圍")]
    public float spawnRadius = 50f;

    // 內部變數
    private List<Matrix4x4> matrices;
    private MaterialPropertyBlock propertyBlock;

    /// <summary>
    /// 初始化實例變換矩陣和材質屬性塊。
    /// </summary>
    void Start()
    {
        // 檢查必要的資源是否已設置
        if (mesh == null)
        {
            Debug.LogError("Mesh 未設置！");
            enabled = false;
            return;
        }

        if (material == null)
        {
            Debug.LogError("Material 未設置！");
            enabled = false;
            return;
        }

        // 初始化矩陣列表
        matrices = new List<Matrix4x4>(spawnCount);

        // 隨機生成每個實例的位置、旋轉和縮放
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 position = new Vector3(0,0,0);
            Quaternion rotation = new Quaternion(0,0,0,0);
            Vector3 scale =new Vector3(1,1,1);
            matrices.Add(Matrix4x4.TRS(position, rotation, scale));
        }

        // 初始化材質屬性塊（可選）
        propertyBlock = new MaterialPropertyBlock();
        // 如果需要為每個實例設置不同的材質屬性，可以在這裡設置
        // 例如，隨機顏色：
        /*
        Color[] colors = new Color[spawnCount];
        for (int i = 0; i < spawnCount; i++)
        {
            colors[i] = Random.ColorHSV();
        }
        propertyBlock.SetVectorArray("_Color", colors);
        */
    }

    /// <summary>
    /// 每幀渲染實例化的網格。
    /// </summary>
    void Update()
    {
        if (matrices == null || matrices.Count == 0)
            return;

        // 設定每批次最大實例數量
        const int batchSize = 1023;

        // 計算需要的批次數量
        int totalBatches = Mathf.CeilToInt((float)spawnCount / batchSize);

        for (int i = 0; i < totalBatches; i++)
        {
            // 計算當前批次的實例數量
            int count = Mathf.Min(batchSize, spawnCount - i * batchSize);

            // 創建當前批次的矩陣數組
            Matrix4x4[] batchMatrices = new Matrix4x4[count];
            matrices.CopyTo(i * batchSize, batchMatrices, 0, count);

            // 渲染當前批次的實例
            Graphics.DrawMeshInstanced(mesh, 0, material, batchMatrices, count, propertyBlock);
        }
    }
}
