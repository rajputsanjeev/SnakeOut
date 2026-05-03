using System;

public static class FloatArrayExtension
{
	public static float Pack(this float[] current)
	{
		var packed = 0;
		var amount = current.Length;
		var bitPrecision = 24 / amount;
		var intPrecision = (1 << bitPrecision) - 1;
		var slot = 0;
		for (var index = amount; index > 0; --index)
		{
			var shift = bitPrecision * (index - 1);
			packed |= (int)Math.Floor(current[slot] * intPrecision) << shift;
			++slot;
		}

		return packed * 0.0000001f;
	}
}