using System;

public static class ConvertBool{
	public static int ToInt(this bool current){return current ? 1 : 0;}
	public static byte[] ToBytes(this bool current){return BitConverter.GetBytes(current);}
	public static byte ToByte(this bool current){return current.ToInt().ToByte();}
	public static short ToShort(this bool current){return current.ToInt().ToShort();}
	public static sbyte ToSignedByte(this bool current){return current.ToInt().ToSignedByte();}
	public static uint ToUnsignedInt(this bool current){return current.ToInt().ToUnsignedInt();}
	public static ushort ToUnsignedShort(this bool current){return current.ToInt().ToUnsignedShort();}
	public static string ToText(this bool current,bool ignoreDefault=false,bool defaultValue=false){
		return ignoreDefault && current == defaultValue ? null : (current ? "true" : "false");
	}
}