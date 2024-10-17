using FFXIVClientStructs.FFXIV.Client.UI;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    /// <summary>
    /// Portraits editor addon
    /// </summary>
    public unsafe class BannerEditor : AddonMasterBase<AddonBannerEditor>
    {
        public BannerEditor(nint addon) : base(addon) { }
        public BannerEditor(void* addon) : base(addon) { }

        public void Save() => ClickButtonIfEnabled(Addon->SaveButton);
        public void Close() => ClickButtonIfEnabled(Addon->CloseButton);
    }
}
