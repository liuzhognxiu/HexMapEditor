using UnityEngine;

public static class Bezier
{
    /// <summary>
    /// 计算曲线上的点
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 GetPoint(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float r = 1f - t;
        return r * r * a + 2f * r * t * b + t * t * c;
    }

    /// <summary>
    /// 根据曲线的倒数，计算单位点上的朝向
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 GetDerivative(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return 2f * ((1f - t) * (b - a) + t * (c - b));
    }
}