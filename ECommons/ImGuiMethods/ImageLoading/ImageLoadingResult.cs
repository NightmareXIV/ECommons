using Dalamud.Interface.Internal;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;

namespace ECommons.ImGuiMethods.ImageLoading;

internal class ImageLoadingResult
{
    internal ISharedImmediateTexture? ImmediateTexture;
    internal IDalamudTextureWrap? TextureWrap;
    internal IDalamudTextureWrap? Texture => ImmediateTexture?.GetWrapOrDefault() ?? TextureWrap;
    internal bool IsCompleted = false;

    public ImageLoadingResult(ISharedImmediateTexture? immediateTexture)
    {
        ImmediateTexture = immediateTexture;
    }

    public ImageLoadingResult(IDalamudTextureWrap? textureWrap)
    {
        TextureWrap = textureWrap;
    }

    public ImageLoadingResult()
    {
    }
}
