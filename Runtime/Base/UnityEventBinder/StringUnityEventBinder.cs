namespace Soar.Events
{
    public sealed class StringUnityEventBinder : UnityEventBinder<string>
    {
        protected override void Start()
        {
            subscriptions.Add(gameEventToListen.Subscribe(() => onTypedGameEventRaised.Invoke(gameEventToListen.ToString())));
        }
    }
}