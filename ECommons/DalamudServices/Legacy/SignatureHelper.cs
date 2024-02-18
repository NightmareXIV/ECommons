namespace ECommons.DalamudServices.Legacy;

public static class SignatureHelper
{
    public static void Initialise(object which, bool log = false)
    {
        Svc.Hook.InitializeFromAttributes(which);
    }
}
