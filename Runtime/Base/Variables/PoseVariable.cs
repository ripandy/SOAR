using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "PoseVariable", menuName = MenuHelper.DefaultVariableMenu + "Pose")]
    public sealed class PoseVariable : VariableCore<Pose>
    {
    }
}