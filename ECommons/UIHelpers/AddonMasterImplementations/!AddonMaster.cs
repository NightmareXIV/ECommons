using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
/// <summary>
/// A class that contains useful methods for addons.<br></br><br></br>
/// If you want to add an addon:<br></br>
/// - Inherit AddonMasterBase<br></br>
/// - Make sure that class is named exactly as addon;<br></br>
/// - Make sure that class is a nested class inside partial AddonMaster class;<br></br>
/// - Make sure to implement both nint and void* constructors<br></br>
/// <b>- Prefer NOT to use indexes; instead, create an array of entries and create internal method to click an item. See RetainerList and SelectString for examples.</b><br />
/// <b>Ideally, automatic method should run as much checks as possible to disallow selecting disabled entries, entering values outside of range into checkboxes, etc. If something allows doing that, do not rely on it as ultimately the goal is to only allow 100% valid inputs.</b>
/// </summary>
public partial class AddonMaster
{
    private AddonMaster() { }
}
