#if (DEBUGFORMS || RELEASEFORMS)
using System.Windows.Forms;

namespace ECommons.Automation;
public partial class WindowsKeypress
{
    public static bool SendKeypress(Keys key) => SendKeypress((int)key);
    public static void SendMousepress(Keys key) => SendMousepress((int)key);
}
#endif