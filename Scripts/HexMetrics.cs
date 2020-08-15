using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

public class HexMetrics : MonoBehaviour
{
    public const int terracesPerSlope = 2;
    //用于高度插值
    public const int terraceSteps = terracesPerSlope * 2 + 1;
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
    public const float horizontalTerraceStepSize = 1f / terraceSteps;
    //用于六边形交界过渡
    public const float solidFactor = 0.75f;
    public const float elevationStep = 5f;
    public const float blendFactor = 1f - solidFactor;
    //用于确认六边形顶点位置
    public const float outerRadius = 10f;
    public const float innerRadius = outerRadius * 0.866025404f;
    //节点扰动 噪音纹理
    public static Texture2D noiseSource;
    //节点的噪音扰动强度
    public const float cellPerturbStrength = 5f;
    //缩放节点采集坐标，使噪音更加平滑
    public const float noiseScale = 0.003f;
    //高度的扰动强度
    public const float elevationPerturbStrength = 1.5f;
    static Vector3[] corners = {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
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
    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) *
            blendFactor;
    }
    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }
    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }
    //基于两个海拔高度来推导连接类型。
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
    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(position.x* noiseScale, position.z* noiseScale);
    }
}
