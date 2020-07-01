using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class HexCellShaderData : MonoBehaviour
{
    private Texture2D cellTexture;

    /// <summary>
    /// 声明颜色缓冲区，所有的单元格都使用颜色缓冲
    /// </summary>
    private Color32[] cellTextureData;
    /// <summary>
    /// 初始化cell对应的纹理数据，在线性空间内使用RBGA纹理，不需要minmap
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void Initialize(int x, int z)
    {
        if (cellTexture)
        {
            cellTexture.Resize(x, z);
        }
        else
        {
            cellTexture = new Texture2D(
                x, z, TextureFormat.RGBA32, false, true
            );
            cellTexture.filterMode = FilterMode.Point;
            cellTexture.wrapMode = TextureWrapMode.Clamp;
            Shader.SetGlobalTexture("_HexCellData", cellTexture);
        }

        Shader.SetGlobalVector(
            "_HexCellData_TexelSize",
            new Vector4(1f / x, 1f / z, x, z)
        );

        if (cellTextureData == null || cellTextureData.Length != x * z)
        {
            cellTextureData = new Color32[x * z];
        }
        else
        {
            for (int i = 0; i < cellTextureData.Length; i++)
            {
                cellTextureData[i] = new Color32(0, 0, 0, 0);
            }
        }

        enabled = true;
    }


    /// <summary>
    /// 刷新地图Cell单位的纹理数据
    /// </summary>
    /// <param name="cell"></param>
    public void RefreshTerrain(HexCell cell)
    {
        cellTextureData[cell.Index].a = (byte)cell.TerrainTypeIndex;
        enabled = true;
    }

    void LateUpdate()
    {
        cellTexture.SetPixels32(cellTextureData);
        cellTexture.Apply();
        enabled = false;
    }

    public void RefreshVisibility(HexCell cell)
    {
        cellTextureData[cell.Index].r = cell.IsVisible ? (byte)255 : (byte)0;
        enabled = true;
    }
}
