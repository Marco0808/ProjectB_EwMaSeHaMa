// GENERATED AUTOMATICALLY FROM 'Assets/Settings/InputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputActions : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""Menu"",
            ""id"": ""b7b7e075-0000-4b84-af7d-49ba447cfe18"",
            ""actions"": [
                {
                    ""name"": ""Continue"",
                    ""type"": ""Button"",
                    ""id"": ""bdebfa20-7922-426c-8be0-acb41cfeeaaa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Back"",
                    ""type"": ""Button"",
                    ""id"": ""fa3f103b-8a06-47eb-8d03-91cf3f16819e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""35fc703b-8bac-47d7-be30-f75734aaa7d6"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Continue"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""339147cf-1ef2-48d0-9630-87e2eca1866e"",
                    ""path"": ""<Keyboard>/backspace"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""Back"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Game"",
            ""id"": ""a2bb38cc-41b6-4ffb-8d52-25289e471230"",
            ""actions"": [
                {
                    ""name"": ""CameraGrab"",
                    ""type"": ""Button"",
                    ""id"": ""c7bbea6c-6704-4973-bbf9-5e0386f831ee"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LeftMouseButton"",
                    ""type"": ""Button"",
                    ""id"": ""5ef0ee42-6261-41c3-9148-f13fe054d93c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""ca7f5320-aa93-4da3-b9e8-a8680a601245"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""WASD"",
                    ""type"": ""Value"",
                    ""id"": ""8927bddb-3c71-470b-a31d-e017f2b9fa43"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""71a8e467-3532-4fb0-a8b8-0dcd05f80f58"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""CameraGrab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1dfe184f-3d31-4b40-a49e-51e62395e9ea"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""CameraGrab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f2ddbdb2-3eaa-43b2-8a1b-9aff0935cc51"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cbf3d026-3dab-448e-be7e-f02595599e75"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""LeftMouseButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""f31b7f3d-d2be-41b4-af6f-9f8e9f7f891c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""212dff35-cc0d-432c-931d-53cb5f33eda0"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""80d14913-ce3b-44a8-b1ea-54f351c1dd8c"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a5dfa689-89e2-4e79-90ef-5c0067c5bd01"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ab9947e4-1925-4125-99a6-5f12d8f159ac"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""WASD"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyboardAndMouse"",
            ""bindingGroup"": ""KeyboardAndMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_Continue = m_Menu.FindAction("Continue", throwIfNotFound: true);
        m_Menu_Back = m_Menu.FindAction("Back", throwIfNotFound: true);
        // Game
        m_Game = asset.FindActionMap("Game", throwIfNotFound: true);
        m_Game_CameraGrab = m_Game.FindAction("CameraGrab", throwIfNotFound: true);
        m_Game_LeftMouseButton = m_Game.FindAction("LeftMouseButton", throwIfNotFound: true);
        m_Game_MousePosition = m_Game.FindAction("MousePosition", throwIfNotFound: true);
        m_Game_WASD = m_Game.FindAction("WASD", throwIfNotFound: true);
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

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_Continue;
    private readonly InputAction m_Menu_Back;
    public struct MenuActions
    {
        private @InputActions m_Wrapper;
        public MenuActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Continue => m_Wrapper.m_Menu_Continue;
        public InputAction @Back => m_Wrapper.m_Menu_Back;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @Continue.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnContinue;
                @Continue.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnContinue;
                @Continue.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnContinue;
                @Back.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                @Back.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
                @Back.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnBack;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Continue.started += instance.OnContinue;
                @Continue.performed += instance.OnContinue;
                @Continue.canceled += instance.OnContinue;
                @Back.started += instance.OnBack;
                @Back.performed += instance.OnBack;
                @Back.canceled += instance.OnBack;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);

    // Game
    private readonly InputActionMap m_Game;
    private IGameActions m_GameActionsCallbackInterface;
    private readonly InputAction m_Game_CameraGrab;
    private readonly InputAction m_Game_LeftMouseButton;
    private readonly InputAction m_Game_MousePosition;
    private readonly InputAction m_Game_WASD;
    public struct GameActions
    {
        private @InputActions m_Wrapper;
        public GameActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @CameraGrab => m_Wrapper.m_Game_CameraGrab;
        public InputAction @LeftMouseButton => m_Wrapper.m_Game_LeftMouseButton;
        public InputAction @MousePosition => m_Wrapper.m_Game_MousePosition;
        public InputAction @WASD => m_Wrapper.m_Game_WASD;
        public InputActionMap Get() { return m_Wrapper.m_Game; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameActions set) { return set.Get(); }
        public void SetCallbacks(IGameActions instance)
        {
            if (m_Wrapper.m_GameActionsCallbackInterface != null)
            {
                @CameraGrab.started -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraGrab;
                @CameraGrab.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraGrab;
                @CameraGrab.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraGrab;
                @LeftMouseButton.started -= m_Wrapper.m_GameActionsCallbackInterface.OnLeftMouseButton;
                @LeftMouseButton.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnLeftMouseButton;
                @LeftMouseButton.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnLeftMouseButton;
                @MousePosition.started -= m_Wrapper.m_GameActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnMousePosition;
                @WASD.started -= m_Wrapper.m_GameActionsCallbackInterface.OnWASD;
                @WASD.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnWASD;
                @WASD.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnWASD;
            }
            m_Wrapper.m_GameActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CameraGrab.started += instance.OnCameraGrab;
                @CameraGrab.performed += instance.OnCameraGrab;
                @CameraGrab.canceled += instance.OnCameraGrab;
                @LeftMouseButton.started += instance.OnLeftMouseButton;
                @LeftMouseButton.performed += instance.OnLeftMouseButton;
                @LeftMouseButton.canceled += instance.OnLeftMouseButton;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
                @WASD.started += instance.OnWASD;
                @WASD.performed += instance.OnWASD;
                @WASD.canceled += instance.OnWASD;
            }
        }
    }
    public GameActions @Game => new GameActions(this);
    private int m_KeyboardAndMouseSchemeIndex = -1;
    public InputControlScheme KeyboardAndMouseScheme
    {
        get
        {
            if (m_KeyboardAndMouseSchemeIndex == -1) m_KeyboardAndMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardAndMouse");
            return asset.controlSchemes[m_KeyboardAndMouseSchemeIndex];
        }
    }
    public interface IMenuActions
    {
        void OnContinue(InputAction.CallbackContext context);
        void OnBack(InputAction.CallbackContext context);
    }
    public interface IGameActions
    {
        void OnCameraGrab(InputAction.CallbackContext context);
        void OnLeftMouseButton(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnWASD(InputAction.CallbackContext context);
    }
}
