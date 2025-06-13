using UnityUaal.Maui.Unity;

namespace UnityUaalMaui.Unity
{
    public static partial class UnityBridge
    {
        public static void Init()
        {
            // UnityContentReceiver has been removed.
            // You can reintroduce it later if Unity needs to call back into MAUI.
        }

        private static WeakReference<INativeUnityBridge> nativeBridgeReference;

        public static void RegisterNativeBridge(INativeUnityBridge nativeBridge)
        {
            if (nativeBridge == null)
            {
                nativeBridgeReference = null;
            }
            else
            {
                nativeBridgeReference = new WeakReference<INativeUnityBridge>(nativeBridge);
            }
        }

        public static void SendContent(string eventName, string eventContent)
        {
            if (string.IsNullOrWhiteSpace(eventName))
            {
                throw new ArgumentException($"'{nameof(eventName)}' cannot be null or whitespace.", nameof(eventName));
            }

            if (!eventName.All(char.IsLetterOrDigit))
            {
                throw new ArgumentException($"'{nameof(eventName)}' must be only alpha-numeric characters.", nameof(eventName));
            }

            if (nativeBridgeReference != null && nativeBridgeReference.TryGetTarget(out var nativeBridge))
            {
                try
                {
                    nativeBridge.SendContent(eventName, eventContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}