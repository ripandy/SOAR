namespace Soar.Events
{
    public sealed class StringUnityEventBinder : UnityEventBinder<string>
    {
        protected override void Start()
        {
            base.Start();
            // NOTE: GameEvent<string> should be listened to by the base class.
            //       Other than that raise the event with the string representation of the GameEvent.
            if (gameEventToListen is GameEvent<string>) return;
            subscriptions.Add(gameEventToListen.Subscribe(() => onTypedGameEventRaised.Invoke(gameEventToListen.ToString())));
        }
    }
}