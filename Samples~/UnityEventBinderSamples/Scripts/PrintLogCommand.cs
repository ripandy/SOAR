using Soar;
using Soar.Commands;
using UnityEngine;

[CreateAssetMenu(fileName = "PrintLogCommand", menuName = MenuHelper.DefaultCommandMenu + "PrintLogCommand")]
public class PrintLogCommand : Command<string>
{
    [SerializeField] private string message = "Hello, World!";
    
    public override void Execute(string param)
    {
        var msg = string.IsNullOrEmpty(param) ? message : param;
        Debug.Log($"[{GetType().Name}:{name}] {msg}");
    }
}
