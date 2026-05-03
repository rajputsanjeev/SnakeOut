using UnityEngine;
using UnityEngine.UI;
using Framework;
using Framework.Core;



namespace Framework
{
    [System.Serializable]
    public class TutorialBaseSave : ISaveObject
    {
        public bool isActive;
        public bool isFinished;

        public int progress;

        public void Flush()
        {

        }
    }
}
