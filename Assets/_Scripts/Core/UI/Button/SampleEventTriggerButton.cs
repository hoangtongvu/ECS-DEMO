using Core.MyEvent;

public class SampleEventTriggerButton : BaseButton
{
    protected override void OnClick()
    {
        SampleButtonEventManager.testEvent.Invoke(ButtonEventData.Default);
    }
}
