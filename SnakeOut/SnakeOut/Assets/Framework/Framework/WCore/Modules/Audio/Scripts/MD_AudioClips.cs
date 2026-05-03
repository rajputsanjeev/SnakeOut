using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    [CreateAssetMenu(fileName = "Audio Clips", menuName = "Data/Core/Audio Clips")]
    public class AudioClips : ScriptableObject
    {
        [BoxGroup("UI", "UI")]
        public AudioClip buttonSound;
        public AudioClip closeButtonSound;
        public AudioClip ClaimSound;

        [BoxGroup("Gameplay", "Gameplay")]
        public AudioClip blockPick;
        
        [BoxGroup("Gameplay")]
        public AudioClip blockDestroy;

        [BoxGroup("Gameplay")]
        public AudioClip win;
        
        [BoxGroup("Gameplay")]
        public AudioClip lose;

        [BoxGroup("Gameplay")]
        public AudioClip revive;

        [BoxGroup("Gameplay")]
        public AudioClip actionDone;

		[BoxGroup("Error")]
		public AudioClip actionError;
	}
}

// -----------------
// Audio Controller v 0.4
// -----------------