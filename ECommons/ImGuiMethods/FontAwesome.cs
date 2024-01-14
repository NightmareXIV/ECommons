using Dalamud.Interface;
using System;

namespace ECommons.ImGuiMethods;
public static class FontAwesome
{
    public static readonly FontAwesomeString Save = 0xf0c7;
    public static readonly FontAwesomeString Cross = 0xf00d;
    public static readonly FontAwesomeString Check = 0xF00C;
    public static readonly FontAwesomeString Info = 0xf05a;
    public static readonly FontAwesomeString Trash = 61944;
    public static readonly FontAwesomeString Plus = 61543;

    public class FontAwesomeString
    {
        int ID;
        public FontAwesomeString(int id)
        {
            this.ID = id;
        }

        public FontAwesomeString(FontAwesomeIcon id)
        {
            this.ID = (int)id;
        }

        public static implicit operator FontAwesomeIcon(FontAwesomeString s) => (FontAwesomeIcon)s.ID;
        public static implicit operator int(FontAwesomeString s) => s.ID;
        public static implicit operator FontAwesomeString(FontAwesomeIcon s) => new(s);
        public static implicit operator FontAwesomeString(int s) => new(s);
        public static implicit operator string(FontAwesomeString s) => ((char)s.ID).ToString();
        public static implicit operator FontAwesomeString(string s) 
        {
            if(s.Length != 1)
            {
                throw new ArgumentOutOfRangeException("String must contain exactly one character");
            }
            return s[0];
        }
    }
}
