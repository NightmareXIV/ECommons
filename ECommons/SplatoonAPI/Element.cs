using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.SplatoonAPI
{
    public class Element
    {
        int Version;
        internal object Instance;
        public Element(int type)
        {
            Version = Splatoon.Version;
            Instance = Splatoon.Instance.GetType().Assembly.CreateInstance("Splatoon.Element", false, BindingFlags.Default, null, new object[] { type }, null, null);
        }

        public bool IsValid()
        {
            return Version == Splatoon.Version;
        }

        public bool Enabled
        {
            get
            {
                return (bool)Instance.GetType().GetField("Enabled").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("Enabled").SetValue(Instance, value);
            }
        }

        /// <summary>
        /// 0: Object at fixed coordinates |
        /// 1: Object relative to actor position | 
        /// 2: Line between two fixed coordinates | 
        /// 3: Line relative to object pos | 
        /// 4: Cone relative to object position |
        /// 5: Cone at fixed coordinates
        /// </summary>
        public int type
        {
            get
            {
                return (int)Instance.GetType().GetField("type").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("type").SetValue(Instance, value);
            }
        }

        public float refX
        {
            get
            {
                return (float)Instance.GetType().GetField("refX").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("refX").SetValue(Instance, value);
            }
        }

        public float refY
        {
            get
            {
                return (float)Instance.GetType().GetField("refY").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("refY").SetValue(Instance, value);
            }
        }

        public float refZ
        {
            get
            {
                return (float)Instance.GetType().GetField("refZ").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("refZ").SetValue(Instance, value);
            }
        }

        public float offX
        {
            get
            {
                return (float)Instance.GetType().GetField("offX").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("offX").SetValue(Instance, value);
            }
        }

        public float offY
        {
            get
            {
                return (float)Instance.GetType().GetField("offY").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("offY").SetValue(Instance, value);
            }
        }

        public float offZ
        {
            get
            {
                return (float)Instance.GetType().GetField("offZ").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("offZ").SetValue(Instance, value);
            }
        }

        public float radius
        {
            get
            {
                return (float)Instance.GetType().GetField("radius").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("radius").SetValue(Instance, value);
            }
        }

        public uint color
        {
            get
            {
                return (uint)Instance.GetType().GetField("color").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("color").SetValue(Instance, value);
            }
        }

        public float thicc
        {
            get
            {
                return (float)Instance.GetType().GetField("thicc").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("thicc").SetValue(Instance, value);
            }
        }

        public float FillStep
        {
            get
            {
                return (float)Instance.GetType().GetField("FillStep").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("FillStep").SetValue(Instance, value);
            }
        }

        public bool tether
        {
            get
            {
                return (bool)Instance.GetType().GetField("tether").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("tether").SetValue(Instance, value);
            }
        }

        public bool Filled
        {
            get
            {
                return (bool)Instance.GetType().GetField("Filled").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("Filled").SetValue(Instance, value);
            }
        }

        public bool includeRotation
        {
            get
            {
                return (bool)Instance.GetType().GetField("includeRotation").GetValue(Instance);
            }
            set
            {
                Instance.GetType().GetField("includeRotation").SetValue(Instance, value);
            }
        }
    }
}
