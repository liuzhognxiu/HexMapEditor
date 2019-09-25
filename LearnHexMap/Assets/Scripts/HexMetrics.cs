using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics
{

    //六边形的外径
    public const float outerRadius = 10f;

    //六边形内径
    public const float innerRadius = outerRadius * 0.866025404f;
    
    //外径转换为内径的因子数
    public const float outerToInner = 0.866025404f;
    
    //内径转换为外径的因子数
    public const float innerToOuter = 1f / outerToInner;

    //内纯色六边形比例
    public const float solidFactor = 0.8f;

    //需要混合颜色的区域
    public const float blendFactor = 1f - solidFactor;

    //阶梯之间的高度
    public const float elevationStep = 3f;

    //斜坡的阶地数量
    public const int terracesPerSlope = 2;

    //斜坡的阶梯的面数
    //         _/
    //      _ /
    //     /
    //两个阶地需要的面数是 N*2 +1
    public const int terraceSteps = terracesPerSlope * 2 + 1;


    //斜坡的水平方向的步骤
    //    ->  _/ 
    //   -> _/
    //     /
    // -> 指向的是_线   指斜坡之间相差的x和z轴的距离大小
    public const float horizontalTerraceStepSize = 1f / terraceSteps;

    //斜坡的竖直方向的步骤
    //         _/   <-
    //      _ /     <-
    //     /        <-
    // <-指向的是/线  指斜坡之间相差的Y轴的距离大小
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

    /// <summary>
    /// 用来取样的噪音贴图
    /// </summary>
    public static Texture2D noiseSource;

    /// <summary>
    /// 调整微扰的幅度
    /// </summary>
    public const float cellPerturbStrength = 4f;

    /// <summary>
    /// 噪声取样的大小
    /// </summary>
    public const float noiseScale = 0.003f;

    /// <summary>
    /// 对单元进行垂直上的微扰
    /// </summary>
    public const float elevationPerturbStrength = 1.5f;  

    public const int chunkSizeX = 5, chunkSizeZ = 5;
    
    /// <summary>
    /// 河床的高度
    /// </summary>
    public const float streamBedElevationOffset = -1f;


    public const float riverSurfaceElevationOffset = -0.5f;
    
    public const float waterElevationOffset = -0.5f;

    public static Color[] colors;
    
    /// <summary>
    /// 水面的岸过多，或者是需要隐藏海岸水的系数
    /// </summary>
    public const float waterFactor = 0.6f;
    
    /// <summary>
    /// 水面之间的桥梁（将水面链接到一起）
    /// </summary>
    public const float waterBlendFactor = 1f - waterFactor;
    
    /// <summary>
    /// 哈希网格
    /// </summary>
    public const int hashGridSize = 256;

    static HexHash[] hashGrid;
    
    public const float hashGridScale = 0.25f;
    
    
	
    /// <summary>
    /// 初始化哈希网格
    /// </summary>
    public static void InitializeHashGrid (int seed) {
        hashGrid = new HexHash[hashGridSize * hashGridSize];
        Random.State currentState = Random.state;
        Random.InitState(seed);
        for (int i = 0; i < hashGrid.Length; i++) {
            hashGrid[i] = HexHash.Create();
        }
        Random.state = currentState;
    }
    
    public static HexHash SampleHashGrid (Vector3 position) {
        int x = (int)(position.x * hashGridScale) % hashGridSize;
        if (x < 0) {
            x += hashGridSize;
        }
        int z = (int)(position.z * hashGridScale) % hashGridSize;
        if (z < 0) {
            z += hashGridSize;
        }
        return hashGrid[x + z * hashGridSize];
    }
    
    static float[][] featureThresholds = {
        new float[] {0.0f, 0.0f, 0.4f},
        new float[] {0.0f, 0.4f, 0.6f},
        new float[] {0.4f, 0.6f, 0.8f}
    };
						
    public static float[] GetFeatureThresholds (int level) {
        return featureThresholds[level];
    }
    
    public static Vector3 GetWaterBridge (HexDirection direction) {
        return (corners[(int)direction] + corners[(int)direction + 1]) *
               waterBlendFactor;
    }
    
    public static Vector3 GetFirstWaterCorner (HexDirection direction) {
        return corners[(int)direction] * waterFactor;
    }

    public static Vector3 GetSecondWaterCorner (HexDirection direction) {
        return corners[(int)direction + 1] * waterFactor;
    }
    
    static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius),
    };

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }

    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    /// <summary>
    /// 添加两个格子中间的桥梁
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) *
               blendFactor;
    }

     /// <summary>
     /// 计算阶梯之间的插值
     /// </summary>
     /// <param name="a"></param>
     /// <param name="b"></param>
     /// <param name="step"></param>
     /// <returns></returns>
    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        float h = step * horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }

    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }

    /// <summary>
    /// 获取两个格子之间的链接方式
    /// </summary>
    /// <param name="elevation1"></param>
    /// <param name="elevation2"></param>
    /// <returns></returns>
    public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
    {
        if (elevation1 == elevation2)
        {
            return HexEdgeType.Flat;
        }
        int delta = elevation2 - elevation1;
        if (delta == 1 || delta == -1)
        {
            return HexEdgeType.Slope;
        }
        return HexEdgeType.Cliff;
    }


    /// <summary>
    /// 通过纹理扰动
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vector3 Perturb(Vector3 position)
    {
        Vector4 sample = SampleNoise(position);
        position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
        position.y += sample.y * 2f - 1f * cellPerturbStrength;
        position.z += (sample.z * 2f - 1f) * cellPerturbStrength;
        return position;
    }

    //噪声扰动
    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(
            position.x * noiseScale,
            position.z * noiseScale
        );
    }

   
    /// <summary>
    /// 相邻两个角向量取平均数然后乘以纯色因子
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Vector3 GetSolidEdgeMiddle (HexDirection direction) {
        return
            (corners[(int)direction] + corners[(int)direction + 1]) *
            (0.5f * solidFactor);
    }
}
