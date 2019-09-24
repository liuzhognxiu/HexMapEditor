
/// <summary>
/// 代表六边形对应的六个方向
/// </summary>
public enum HexDirection
{

	NE, E, SE, SW, W, NW,
}


public static class HexDirectionExtensions
{
	public static HexDirection Opposite(this HexDirection direction)
	{
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}

	/// <summary>
	/// 获取前一个方向的格子
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public static HexDirection Previous(this HexDirection direction)
	{
		return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
	}

	/// <summary>
	/// 获取下一个方向的格子
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public static HexDirection Next(this HexDirection direction)
	{
		return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
	}
    
	/// <summary>
	/// 获取前一个方向的前一个方向
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public static HexDirection Previous2 (this HexDirection direction) {
		direction -= 2;
		return direction >= HexDirection.NE ? direction : (direction + 6);
	}
 
	/// <summary>
	/// 获取下一个方向的下一个方向
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public static HexDirection Next2 (this HexDirection direction) {
		direction += 2;
		return direction <= HexDirection.NW ? direction : (direction - 6);
	}

}