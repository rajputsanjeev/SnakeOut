using UnityEngine;

namespace Framework
{
	[CreateAssetMenu(fileName = "RequiredLevel", menuName = "RequiredLevel/RequiredLevelData")]
	public class RequiredLevelScriptableObject : ScriptableObject
	{
		public int ShowAfterLevel;
		public bool IsShowWhenLevelNotReach;
	}
}
