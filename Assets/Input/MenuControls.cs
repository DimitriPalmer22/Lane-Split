//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input/MenuControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @MenuControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @MenuControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MenuControls"",
    ""maps"": [
        {
            ""name"": ""Navigation"",
            ""id"": ""4d06aa9c-bc8a-4944-ba2e-ef4c75f0494a"",
            ""actions"": [
                {
                    ""name"": ""NextVehicle"",
                    ""type"": ""Button"",
                    ""id"": ""cc2e2cec-c4d6-443c-b53a-ac555af91a6c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PreviousVehicle"",
                    ""type"": ""Button"",
                    ""id"": ""0da0023c-e6f0-4c17-bc56-c42fe43d6f99"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SelectVehicle"",
                    ""type"": ""Button"",
                    ""id"": ""d9ea16ac-8d70-40cb-bc72-5b1a4de29356"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c5c9e3c6-f226-4714-a09b-ac1959ffc7b5"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextVehicle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c8a00122-8284-45b6-9ead-052fe9f22b6c"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PreviousVehicle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""594bc139-974c-4a03-a4c7-a3d82b26a769"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SelectVehicle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Navigation
        m_Navigation = asset.FindActionMap("Navigation", throwIfNotFound: true);
        m_Navigation_NextVehicle = m_Navigation.FindAction("NextVehicle", throwIfNotFound: true);
        m_Navigation_PreviousVehicle = m_Navigation.FindAction("PreviousVehicle", throwIfNotFound: true);
        m_Navigation_SelectVehicle = m_Navigation.FindAction("SelectVehicle", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Navigation
    private readonly InputActionMap m_Navigation;
    private List<INavigationActions> m_NavigationActionsCallbackInterfaces = new List<INavigationActions>();
    private readonly InputAction m_Navigation_NextVehicle;
    private readonly InputAction m_Navigation_PreviousVehicle;
    private readonly InputAction m_Navigation_SelectVehicle;
    public struct NavigationActions
    {
        private @MenuControls m_Wrapper;
        public NavigationActions(@MenuControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @NextVehicle => m_Wrapper.m_Navigation_NextVehicle;
        public InputAction @PreviousVehicle => m_Wrapper.m_Navigation_PreviousVehicle;
        public InputAction @SelectVehicle => m_Wrapper.m_Navigation_SelectVehicle;
        public InputActionMap Get() { return m_Wrapper.m_Navigation; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(NavigationActions set) { return set.Get(); }
        public void AddCallbacks(INavigationActions instance)
        {
            if (instance == null || m_Wrapper.m_NavigationActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_NavigationActionsCallbackInterfaces.Add(instance);
            @NextVehicle.started += instance.OnNextVehicle;
            @NextVehicle.performed += instance.OnNextVehicle;
            @NextVehicle.canceled += instance.OnNextVehicle;
            @PreviousVehicle.started += instance.OnPreviousVehicle;
            @PreviousVehicle.performed += instance.OnPreviousVehicle;
            @PreviousVehicle.canceled += instance.OnPreviousVehicle;
            @SelectVehicle.started += instance.OnSelectVehicle;
            @SelectVehicle.performed += instance.OnSelectVehicle;
            @SelectVehicle.canceled += instance.OnSelectVehicle;
        }

        private void UnregisterCallbacks(INavigationActions instance)
        {
            @NextVehicle.started -= instance.OnNextVehicle;
            @NextVehicle.performed -= instance.OnNextVehicle;
            @NextVehicle.canceled -= instance.OnNextVehicle;
            @PreviousVehicle.started -= instance.OnPreviousVehicle;
            @PreviousVehicle.performed -= instance.OnPreviousVehicle;
            @PreviousVehicle.canceled -= instance.OnPreviousVehicle;
            @SelectVehicle.started -= instance.OnSelectVehicle;
            @SelectVehicle.performed -= instance.OnSelectVehicle;
            @SelectVehicle.canceled -= instance.OnSelectVehicle;
        }

        public void RemoveCallbacks(INavigationActions instance)
        {
            if (m_Wrapper.m_NavigationActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(INavigationActions instance)
        {
            foreach (var item in m_Wrapper.m_NavigationActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_NavigationActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public NavigationActions @Navigation => new NavigationActions(this);
    public interface INavigationActions
    {
        void OnNextVehicle(InputAction.CallbackContext context);
        void OnPreviousVehicle(InputAction.CallbackContext context);
        void OnSelectVehicle(InputAction.CallbackContext context);
    }
}