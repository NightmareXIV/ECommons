IPC provider side example:
```C#
public class IPCProvider()
{
    //Internal plugin name is MyPlugin
    public IPCProvider()
    {
        EzIPC.Init(this);
    }

    [EzIPC]
    public void MyIPCAction() //will register as MyPlugin.MyIPCAction
    {
        DoThings();
    }

    [EzIPC("RenamedFunction")]
    public int MyIPCFunction() //will register as MyPlugin.RenamedFunction
    {
        return DoOtherThings();
    }

    [EzIPCEvent] public Action Event; //Provides delegate for firing events
    [EzIPCEvent] public Action<int> AwesomeEvent;
}
```

IPC subscriber (consumer) side example:
```C#
public class IPCSubscriber()
{
    public IPCSubscriber()
    {
        EzIPC.Init(this, "MyPlugin");
    }

    [EzIPC] public readonly Action MyIPCAction; //retrieves delegate for MyPlugin.MyIPCAction
    [EzIPC("RenamedFunction")] public readonly Func<int> SinceItsRenamedFieldCanHaveAnyName;

    [EzIPC("OtherPlugin_OtherAction", applyPrefix:false)] public Action OtherPluginAction;
    //you can define full tag without prefix for plugins
    //that don't follow this standard naming convention or
    //if you want to get IPC from multiple plugins in one class

    [EzIPCEvent] //event subscription
    void Event()
    {
        DoSomething();
    }

    [EzIPCEvent]
    void AwesomeEvent(int a1)
    {
        DoSomethingElse(a1);
    }
}
```
