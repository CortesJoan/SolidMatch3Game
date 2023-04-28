using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowCapture : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, int nFlags);
    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowDC(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
    [DllImport("dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern IntPtr SetActiveWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    private const int SWP_SHOWWINDOW = 0x0040;
    private const int SWP_HIDEWINDOW = 0x0080;
    private const int SW_RESTORE = 9;

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public string processName = "notepad";
    public RenderTexture renderTexture;
    public Material materialToApplyTheTexture;
    public string materialTextureName = "_EmissionMap";

    private Texture2D _capturedTexture;

    private void Update()
    {
        var process = Process.GetProcessesByName(processName)[0];
        if (process != null)
        {
            CaptureWindow(process.MainWindowHandle);
            materialToApplyTheTexture.SetTexture(materialTextureName, renderTexture);
        }
    }

    private void CaptureWindow(IntPtr hWnd)
    {
        bool wasMinimized = IsIconic(hWnd);
        if (wasMinimized)
        {
            GetWindowRect(hWnd, out RECT originalRect);
            SetWindowPos(hWnd, IntPtr.Zero, -9999, -9999, originalRect.Right - originalRect.Left, originalRect.Bottom - originalRect.Top, SWP_SHOWWINDOW);
        }
        ShowWindow(hWnd, SW_RESTORE);
        SetActiveWindow(hWnd);

        if (DwmGetWindowAttribute(hWnd, (int)DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, out RECT rect, Marshal.SizeOf(typeof(RECT))) != 0)
            return;

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        if (renderTexture == null || renderTexture.width != width || renderTexture.height != height)
        {
            if (renderTexture != null)
                Destroy(renderTexture);
            renderTexture = new RenderTexture(width, height, 24);
        }

        if (_capturedTexture == null || _capturedTexture.width != width || _capturedTexture.height != height)
        {
            if (_capturedTexture != null)
                Destroy(_capturedTexture);
            _capturedTexture = new Texture2D(width, height, TextureFormat.BGRA32, false);
        }

        var windowDC = GetWindowDC(hWnd);
        if (windowDC == IntPtr.Zero)
            return;

        var bmp = new System.Drawing.Bitmap(width, height, PixelFormat.Format32bppArgb);
        var graphics = System.Drawing.Graphics.FromImage(bmp);
        var hdcBitmap = graphics.GetHdc();

        PrintWindow(hWnd, hdcBitmap, 0);

        graphics.ReleaseHdc(hdcBitmap);
        graphics.Dispose();

        var bmpRect = new System.Drawing.Rectangle(0, 0, width, height);
        var bmpData = bmp.LockBits(bmpRect, ImageLockMode.ReadOnly, bmp.PixelFormat);

        _capturedTexture.LoadRawTextureData(bmpData.Scan0, width * height * 4);
        _capturedTexture.Apply();

        bmp.UnlockBits(bmpData);
        bmp.Dispose();

        Graphics.Blit(_capturedTexture, renderTexture);
        if (wasMinimized)
        {
            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_HIDEWINDOW);
        }
    }

    private void OnDestroy()
    {
        if (_capturedTexture != null)
        {
            Destroy(_capturedTexture);
        }
    }

    enum DWMWINDOWATTRIBUTE
    {
        DWMWA_NCRENDERING_ENABLED = 1,
        DWMWA_NCRENDERING_POLICY,
        DWMWA_TRANSITIONS_FORCEDISABLED,
        DWMWA_ALLOW_NCPAINT,
        DWMWA_CAPTION_BUTTON_BOUNDS,
        DWMWA_NONCLIENT_RTL_LAYOUT,
        DWMWA_FORCE_ICONIC_REPRESENTATION,
        DWMWA_FLIP3D_POLICY,
        DWMWA_EXTENDED_FRAME_BOUNDS,
        DWMWA_HAS_ICONIC_BITMAP,
        DWMWA_DISALLOW_PEEK,
        DWMWA_EXCLUDED_FROM_PEEK,
        DWMWA_CLOAK,
        DWMWA_CLOAKED,
        DWMWA_FREEZE_REPRESENTATION,
        DWMWA_LAST
    }
}