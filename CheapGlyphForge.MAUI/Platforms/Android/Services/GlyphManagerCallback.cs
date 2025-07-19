// CheapGlyphForge.MAUI/Platforms/Android/Services/GlyphManagerCallback.cs
using Android.Content;
using Com.Nothing.Ketchum;
using System.Diagnostics;

namespace CheapGlyphForge.MAUI.Platforms.Android.Services;

public partial class AndroidInterfaceService
{
    private class GlyphManagerCallback(AndroidInterfaceService service) : Java.Lang.Object, GlyphManager.ICallback
    {
        private readonly AndroidInterfaceService _service = service;

        public void OnServiceConnected(ComponentName? componentName)
        {
            Debug.WriteLine("AndroidInterfaceService: Glyph service connected");
            _service.IsConnected = true;
            _service.ConnectionChanged?.Invoke(_service, true);
            _service._connectionTcs?.SetResult(true);
        }

        public void OnServiceDisconnected(ComponentName? componentName)
        {
            Debug.WriteLine("AndroidInterfaceService: Glyph service disconnected");
            _service.IsConnected = false;
            _service.IsSessionOpen = false;
            _service.ConnectionChanged?.Invoke(_service, false);
            _service._connectionTcs?.SetResult(false);
        }
    }
}