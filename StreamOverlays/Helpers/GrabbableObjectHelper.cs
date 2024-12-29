using System.Reflection;

namespace com.github.zehsteam.StreamOverlays.Helpers;

internal static class GrabbableObjectHelper
{
    private static readonly FieldInfo _deactivatedField;

    static GrabbableObjectHelper()
    {
        _deactivatedField = typeof(GrabbableObject).GetField("deactivated", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
    
    public static bool IsDeactivated(GrabbableObject grabbableObject)
    {
        if (_deactivatedField != null)
        {
            return _deactivatedField.GetValue(grabbableObject) as bool? ?? false;
        }

        return false;
    }
}
