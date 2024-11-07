using UnityEngine;

namespace Soar.Variables
{
    [CreateAssetMenu(fileName = "CameraVariable", menuName = MenuHelper.DefaultVariableMenu + "Camera")]
    public sealed class CameraVariable : Variable<Camera>
    {
        [SerializeField] private CameraFallback fallbackType;

        public override Camera Value
        {
            get
            {
                if (base.Value == null && fallbackType != CameraFallback.Null)
                {
                    switch (fallbackType)
                    {
                        case CameraFallback.Main:
                            base.Value = Camera.main;
                            break;
                        case CameraFallback.Current:
                            base.Value = Camera.current;
                            break;
                    }
                }

                return base.Value;
            }
            set => base.Value = value;
        }
        
        public Transform Transform => Value.transform;
    }

    internal enum CameraFallback
    {
        Null,
        Main,
        Current
    }
}