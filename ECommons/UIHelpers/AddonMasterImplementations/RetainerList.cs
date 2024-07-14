﻿using ECommons.Automation;
using ECommons.UIHelpers.AtkReaderImplementations;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.UIHelpers.AddonMasterImplementations;
public partial class AddonMaster
{
    public unsafe class RetainerList : AddonMasterBase<AddonRetainerList>
    {
        public RetainerList(nint addon) : base(addon)
        {
        }

        public RetainerList(void* addon) : base(addon)
        {
        }

        public Entry[] Retainers
        {
            get
            {
                var reader = new ReaderRetainerList(Base);
                var entries = new Entry[reader.Retainers.Count];
                for (var i = 0; i < entries.Length; i++)
                {
                    entries[i] = new(Base, reader.Retainers[i], i);
                }
                return entries;
            }
        }

        public class Entry(AtkUnitBase* Base, ReaderRetainerList.Retainer handle, int index) : ReaderRetainerList.Retainer(handle.AtkReaderParams.UnitBase, handle.AtkReaderParams.BeginOffset)
        {
            public int Index => index;
            public bool Select()
            {
                if (IsActive)
                {
                    Callback.Fire(Base, true, 2, (uint)index, Callback.ZeroAtkValue, Callback.ZeroAtkValue);
                    return true;
                }
                return false;
            }
        }

        public void Retainer1() => Retainers[0].Select();
        public void Retainer2() => Retainers[1].Select();
        public void Retainer3() => Retainers[2].Select();
        public void Retainer4() => Retainers[3].Select();
        public void Retainer5() => Retainers[4].Select();
        public void Retainer6() => Retainers[5].Select();
        public void Retainer7() => Retainers[6].Select();
        public void Retainer8() => Retainers[7].Select();
        public void Retainer9() => Retainers[8].Select();
        public void Retainer10() => Retainers[9].Select();
    }
}
