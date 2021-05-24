using Dalamud.Game;
using Dalamud.Game.Internal;
using System;
using System.Runtime.InteropServices;

namespace ZoomHackPlugin
{
    class AddressResolver : BaseAddressResolver
    {
        public IntPtr ResolvedAddress { get; private set; }

        public IntPtr CameraPotisionAddress { get; private set; }

        protected override void Setup64Bit(SigScanner sig)
        {
            // 48 89 5c 24 08   MOV     qword ptr [RSP  + local_res8 ],RBX
            // 48 89 6c 24 10   MOV     qword ptr [RSP  + local_res10 ],RBP
            // 48 89 74 24 18   MOV     qword ptr [RSP  + local_res18 ],RSI
            // 48 89 7c 24 20   MOV     qword ptr [RSP  + local_res20 ],RDI
            // 41 56            PUSH    R14
            // 48 83 ec 60      SUB     RSP ,0x60
            // 48 8b d9         MOV     RBX ,RCX
            // 48 8d 0d 5c fb   LEA     RCX ,[ g_ControlSystem_CameraManager ]
            // 9e 01

            this.ResolvedAddress = sig.ScanText("48 89 5c 24 08 48 89 6c 24 10 48 89 74 24 18 48 89 7c 24 20 41 56 48 83 ec 60 48 8b d9 48 8d 0d");
            var offset = Marshal.ReadInt32(ResolvedAddress + 32);
            var A = sig.ResolveRelativeAddress(ResolvedAddress + 36, offset);
            this.CameraPotisionAddress = Marshal.ReadIntPtr(A);

        }
    }
}
