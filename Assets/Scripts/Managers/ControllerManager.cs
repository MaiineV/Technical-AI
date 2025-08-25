using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public enum InputNames
    {
        Move,
    }
    
    public enum ActionMapType
    {
        Player,
        UI
    }

    public static class ControllerManager
    {
        private static readonly InputSystem_Actions _inputSystem;
        private static readonly Dictionary<ActionMapType, InputActionMap> ActionMaps = new();

        private static ActionMapType _currentMapType;
        
        static ControllerManager()
        {
            _inputSystem = new InputSystem_Actions();

            ActionMaps[ActionMapType.Player] = _inputSystem.Player;
            ActionMaps[ActionMapType.UI]     = _inputSystem.UI;

            ActionMaps[ActionMapType.UI].Disable();
            ActionMaps[ActionMapType.Player].Enable();
            _currentMapType = ActionMapType.Player;
        }

        /// <summary>
        /// You have to define the Input Type (Move, Look, etc.) <see cref="InputNames" />,
        /// and an <see cref="Action" /> function with an <see cref="InputAction.CallbackContext" /> by params.
        /// </summary>
        public static void AddPerformanceEvent(InputNames inputNames,
            Action<InputAction.CallbackContext> action, ActionMapType actionMapType = ActionMapType.Player)
        {
            var actionName = inputNames.ToString();
            var inputAction = ActionMaps[actionMapType].FindAction(actionName, throwIfNotFound: false);
            if (inputAction == null)
            {
                Debug.LogWarning($"Can't find \"{actionName}\" in the ActionMap \"Player\".");
                return;
            }

            inputAction.performed += action;
        }

        /// <summary>
        /// You have to define the Input Type (Move, Look, etc.) <see cref="InputNames" />,
        /// and an <see cref="Action" /> function with an <see cref="InputAction.CallbackContext" /> by params.
        /// </summary>
        public static void AddCancelEvent(InputNames inputNames, 
            Action<InputAction.CallbackContext> action, ActionMapType actionMapType = ActionMapType.Player)
        {
            var actionName = inputNames.ToString();
            var inputAction = ActionMaps[actionMapType].FindAction(actionName, throwIfNotFound: false);
            if (inputAction == null)
            {
                Debug.LogWarning($"Can't find \"{actionName}\" in the ActionMap \"Player\".");
                return;
            }

            inputAction.canceled += action;
        }

        //TODO: Add remove for each case
        public static void RemoveEvent(InputNames inputNames, Action<InputAction.CallbackContext> action,
            ActionMapType actionMapType = ActionMapType.Player)
        {
            var actionName = inputNames.ToString();
            var inputAction = ActionMaps[actionMapType].FindAction(actionName, throwIfNotFound: false);
            if (inputAction == null)
            {
                Debug.LogWarning($"Can't find \"{actionName}\" in the ActionMap \"Player\".");
                return;
            }

            inputAction.performed -= action;
        }

        public static void ChangeInputMap(ActionMapType newMapType)
        {
            if (_currentMapType == newMapType)
                return;

            ActionMaps[_currentMapType].Disable();
            ActionMaps[newMapType].Enable();
            _currentMapType = newMapType;
        }
    }
}