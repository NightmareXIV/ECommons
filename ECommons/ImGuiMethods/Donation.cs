using Dalamud.Interface.Colors;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.ImGuiMethods
{
    public static class Donation
    {
        public static void PrintDonationInfo()
        {
            ImGui.PushTextWrapPos();
            ImGui.TextColored(ImGuiColors.DalamudRed, "Attention! Malware programs may replace crypto wallet address inside your clipboard. ALWAYS double-check destination address before sending any funds.");
            ImGui.PopTextWrapPos();
            ImGuiEx.ButtonCopy("Bitcoin (BTC): $COPY", "bc1qwzh7mc3glcdemyg9xpvr7cfuc2nxl8u87x73e4");
            ImGuiEx.ButtonCopy("USDT (TRC20): $COPY", "TBNN99wdCzPX4HavCjiooq3NjvujLgqfoK");
            ImGuiEx.ButtonCopy("Litecoin (LTC): $COPY", "ltc1qrgc802qzdez2q2v6ds293qrglfzj2kvwm5dl4f");
            ImGuiEx.ButtonCopy("Ethereum (ETC): $COPY", "0xA46D5cD23C7586b0817413682cdeCC8E3CdB590F");
            ImGuiEx.ButtonCopy("Binance BUSD (BEP20): $COPY", "0xA46D5cD23C7586b0817413682cdeCC8E3CdB590F");
        }
    }
}
