using UnityEngine;

namespace Framework
{
	public abstract class GetCurrentLevelAbstract : GenericSingletonClass<GetCurrentLevelAbstract>
	{
		protected int Level;

		public abstract void SetCurrenTLevel();

		public int GetLevel()
		{
			SetCurrenTLevel();
			return Level;
		}
	}
}
