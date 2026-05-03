using System.Collections.Generic;
using UnityEngine;
using Framework;



namespace Framework
{
    public static class TicTacLineUtility
    {
		public static List<int[]> GetAllGroups(
		   int width, int height, int groupSize)
		{
			// Same logic as your old GetAllWinningLines,
			// but generic for groups
			return TicTacLineUtility.GetAllWinningLines(
				width, height, groupSize
			);
		}

		public static List<int[]> GetAllWinningLines(
    		int width, int height, int winCount)
    	{
    		var lines = new List<int[]>();
    
    		// Horizontal
    		for (int y = 0; y < height; y++)
    			for (int x = 0; x <= width - winCount; x++)
    				lines.Add(BuildLine(x, y, 1, 0, width, winCount));
    
    		// Vertical
    		for (int x = 0; x < width; x++)
    			for (int y = 0; y <= height - winCount; y++)
    				lines.Add(BuildLine(x, y, 0, 1, width, winCount));
    
    		// Diagonal \
    		for (int x = 0; x <= width - winCount; x++)
    			for (int y = 0; y <= height - winCount; y++)
    				lines.Add(BuildLine(x, y, 1, 1, width, winCount));
    
    		// Diagonal /
    		for (int x = 0; x <= width - winCount; x++)
    			for (int y = winCount - 1; y < height; y++)
    				lines.Add(BuildLine(x, y, 1, -1, width, winCount));
    
    		return lines;
    	}
    
    	static int[] BuildLine(
    		int x, int y, int dx, int dy, int width, int count)
    	{
    		int[] line = new int[count];
    		for (int i = 0; i < count; i++)
    			line[i] = (y + dy * i) * width + (x + dx * i);
    		return line;
    	}
		// =========================================================
		// 🔥 NEW OVERLOAD (THIS IS THE ONLY ADDITION)
		// =========================================================
		public static List<int[]> PickNonOverlappingLines(
			List<int[]> allLines,
			int requiredLines,
			HashSet<int> externalUsed)
		{
			// 1️⃣ Split lines by direction (OLD LOGIC)
			var horizontal = new List<int[]>();
			var vertical = new List<int[]>();
			var diagonal = new List<int[]>();

			foreach (var line in allLines)
			{
				int first = line[0];
				int second = line[1];
				int diff = Mathf.Abs(second - first);

				if (diff == 1)
					horizontal.Add(line);
				else if (diff > 1 && diff % line.Length == 0)
					vertical.Add(line);
				else
					diagonal.Add(line);
			}

			// 2️⃣ Shuffle each bucket (OLD LOGIC)
			Shuffle(horizontal);
			Shuffle(vertical);
			Shuffle(diagonal);

			// 3️⃣ Interleave buckets (OLD LOGIC)
			var buckets = new List<List<int[]>> { horizontal, vertical, diagonal };

			var result = new List<int[]>();
			int bucketIndex = 0;

			while (result.Count < requiredLines)
			{
				bool addedAny = false;

				foreach (var bucket in buckets)
				{
					if (bucketIndex >= bucket.Count)
						continue;

					var line = bucket[bucketIndex];

					bool conflict = false;
					foreach (int c in line)
					{
						if (externalUsed.Contains(c))
						{
							conflict = true;
							break;
						}
					}

					if (conflict)
						continue;

					result.Add(line);
					foreach (int c in line)
						externalUsed.Add(c);

					addedAny = true;

					if (result.Count == requiredLines)
						break;
				}

				if (!addedAny)
					break;

				bucketIndex++;
			}

			return result;
		}

		static void Shuffle(List<int[]> list)
    	{
    		for (int i = 0; i < list.Count; i++)
    		{
    			int r = Random.Range(i, list.Count);
    			(list[i], list[r]) = (list[r], list[i]);
    		}
    	}
    
    }
    
}