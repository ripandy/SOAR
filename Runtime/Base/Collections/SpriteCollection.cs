using UnityEngine;

namespace Soar.Collections
{
    [CreateAssetMenu(fileName = "SpriteCollection", menuName = MenuHelper.DefaultCollectionMenu + "SpriteCollection")]
    public class SpriteCollection : SoarList<Sprite>
    {
    }
}