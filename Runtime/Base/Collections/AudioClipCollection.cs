using UnityEngine;

namespace Soar.Collections
{
    [CreateAssetMenu(fileName = "AudioClipCollection", menuName = MenuHelper.DefaultCollectionMenu + "AudioClipCollection")]
    public class AudioClipCollection : Collection<AudioClip>
    {
    }
}