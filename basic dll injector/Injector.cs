using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static basic_dll_injector.InjectorImports;

namespace basic_dll_injector
{
    public static class Injector
    {
        public static void InjectDll(string dllFilePath, Process targetProcess)
        {
            IntPtr processHandle = OpenProcess((int)PROCESS_ALL_ACCESS, false, (uint)targetProcess.Id);
            if (processHandle == IntPtr.Zero)
            {
                MessageBox.Show("Failed to open process.");
                return;
            }

            byte[] dllPathBytes = Encoding.ASCII.GetBytes(dllFilePath + '\0');
            IntPtr pMemory = VirtualAllocEx(processHandle, IntPtr.Zero, (uint)dllPathBytes.Length, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
            if (pMemory == IntPtr.Zero)
            {
                MessageBox.Show("Failed to allocate memory in target process.");
                return;
            }

            uint bytesWritten;
            bool result = WriteProcessMemory(processHandle, pMemory, dllPathBytes, (uint)dllPathBytes.Length, out bytesWritten);
            if (!result || bytesWritten != dllPathBytes.Length)
            {
                MessageBox.Show("Failed to write process memory.");
                return;
            }

            IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtr remoteThread = CreateRemoteThread(processHandle, IntPtr.Zero, 0, loadLibraryAddr, pMemory, 0, out _);
            if (remoteThread == IntPtr.Zero)
            {
                MessageBox.Show("Failed to create remote thread.");
                VirtualFreeEx(processHandle, pMemory, 0, 0x8000);
                return;
            }

            WaitForSingleObject(remoteThread, INFINITE);
            VirtualFreeEx(processHandle, pMemory, 0, 0x8000);
            MessageBox.Show("DLL injected successfully.");
        }
    }

    public static class InjectorImports
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out uint lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        public const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        public const uint MEM_COMMIT = 0x00001000;
        public const uint MEM_RESERVE = 0x00002000;
        public const uint PAGE_READWRITE = 0x04;
        public const uint INFINITE = 0xFFFFFFFF;
    }
}
