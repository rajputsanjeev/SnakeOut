using UnityEngine;

namespace Framework
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "PrivilegeCycle", menuName = "Data/Privilege/Privilege Cycle (28 Days)")]
	public class PrivilegeCycleSet : ScriptableObject
	{
		public string cycleName = "Default Cycle";
		public PrivilegeStepData[] steps = new PrivilegeStepData[28]; // exactly 28

		public PrivilegeStepData GetStep(int index)
		{
			if (steps == null || index < 0 || index >= steps.Length) return null;
			return steps[index];
		}
	}

}