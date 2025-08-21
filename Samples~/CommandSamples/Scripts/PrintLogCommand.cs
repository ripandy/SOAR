using UnityEngine;

namespace Soar.Commands.Sample
{
    [CreateAssetMenu(fileName = "PrintLogCommand", menuName = MenuHelper.DefaultCommandMenu + "PrintLogCommand")]
    public class PrintLogCommand : Command
    {
        [SerializeField] private string tag;
        [SerializeField] private string toPrint = "Debugging...";
        [SerializeField] private Color color = Color.white;
        [SerializeField] private bool withCounter;

        private int counter;
        
        public override void Execute()
        {
            var colorString =
                ((byte)(color.r * 255)).ToString("x2") +
                ((byte)(color.g * 255)).ToString("x2") +
                ((byte)(color.b * 255)).ToString("x2");
            Debug.Log($"<color=#{colorString}>{(string.IsNullOrEmpty(tag) ? "" : $"[{tag}] ")}{toPrint}{(withCounter ? $" {counter++}" : "")}</color>");
        }
    }
}