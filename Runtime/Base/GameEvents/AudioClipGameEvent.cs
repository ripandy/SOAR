using UnityEngine;

namespace Soar.Events
{
    [CreateAssetMenu(fileName = "AudioClipGameEvent", menuName = MenuHelper.DefaultGameEventMenu + "AudioClip")]
    public class AudioClipGameEvent : GameEvent<AudioClip>
    {
    }
}