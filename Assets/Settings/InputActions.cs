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
                    ""name"": ""LeftMouseButton"",
                    ""type"": ""Button"",
                    ""id"": ""5ef0ee42-6261-41c3-9148-f13fe054d93c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""LeftMouseButtonReleased"",
                    ""type"": ""Button"",
                    ""id"": ""e7696ba5-e463-4f56-8afa-059903523c49"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=1)""
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
                },
                {
                    ""name"": ""CameraGrab"",
                    ""type"": ""Button"",
                    ""id"": ""c7bbea6c-6704-4973-bbf9-5e0386f831ee"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraZoomIn"",
                    ""type"": ""Button"",
                    ""id"": ""d2f426f4-88b2-4de5-b03e-db59922b6b94"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraZoomOut"",
                    ""type"": ""Button"",
                    ""id"": ""b4c796d5-7aee-4621-b85a-1823f5cd4ba3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraZoomReset"",
                    ""type"": ""Button"",
                    ""id"": ""23a08c5e-464b-49a7-aec2-073669289e40"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ScreenCapture"",
                    ""type"": ""Button"",
                    ""id"": ""edbe66af-b2e9-4198-9315-9103978f1059"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
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
                },
                {
                    ""name"": """",
                    ""id"": ""c510bfe4-f378-4fc9-aff0-a2399947b771"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""LeftMouseButtonReleased"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""53040979-6b33-46d8-aa75-9273c1d5bec5"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomIn"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""985a17c8-ee7f-48fb-b64b-9c39f250ac27"",
                    ""path"": ""<Keyboard>/alt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomIn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""742a3e7c-bef4-4f34-b986-b14ae1ba5420"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomIn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
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
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""063bf738-8eba-43e6-ad77-778a7276c919"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomOut"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""7eb649f8-a397-4867-82b4-03657c1b7cb3"",
                    ""path"": ""<Keyboard>/alt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomOut"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""15724b85-23cb-4271-92c8-fde112922487"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomOut"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Button With One Modifier"",
                    ""id"": ""9b297a6f-8d51-4646-ba97-ec10d4805ad7"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomReset"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""175ab088-c423-4661-9ebc-8a09ee7d57fc"",
                    ""path"": ""<Keyboard>/alt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomReset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""45a291bc-c69b-4647-8e9d-94f53b070d20"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraZoomReset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b83b3795-18be-4308-8ee5-451f3da3ffed"",
                    ""path"": ""<Keyboard>/insert"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardAndMouse"",
                    ""action"": ""ScreenCapture"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
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
        m_Game_LeftMouseButton = m_Game.FindAction("LeftMouseButton", throwIfNotFound: true);
        m_Game_LeftMouseButtonReleased = m_Game.FindAction("LeftMouseButtonReleased", throwIfNotFound: true);
        m_Game_MousePosition = m_Game.FindAction("MousePosition", throwIfNotFound: true);
        m_Game_WASD = m_Game.FindAction("WASD", throwIfNotFound: true);
        m_Game_CameraGrab = m_Game.FindAction("CameraGrab", throwIfNotFound: true);
        m_Game_CameraZoomIn = m_Game.FindAction("CameraZoomIn", throwIfNotFound: true);
        m_Game_CameraZoomOut = m_Game.FindAction("CameraZoomOut", throwIfNotFound: true);
        m_Game_CameraZoomReset = m_Game.FindAction("CameraZoomReset", throwIfNotFound: true);
        m_Game_ScreenCapture = m_Game.FindAction("ScreenCapture", throwIfNotFound: true);
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
    private readonly InputAction m_Game_LeftMouseButton;
    private readonly InputAction m_Game_LeftMouseButtonReleased;
    private readonly InputAction m_Game_MousePosition;
    private readonly InputAction m_Game_WASD;
    private readonly InputAction m_Game_CameraGrab;
    private readonly InputAction m_Game_CameraZoomIn;
    private readonly InputAction m_Game_CameraZoomOut;
    private readonly InputAction m_Game_CameraZoomReset;
    private readonly InputAction m_Game_ScreenCapture;
    public struct GameActions
    {
        private @InputActions m_Wrapper;
        public GameActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftMouseButton => m_Wrapper.m_Game_LeftMouseButton;
        public InputAction @LeftMouseButtonReleased => m_Wrapper.m_Game_LeftMouseButtonReleased;
        public InputAction @MousePosition => m_Wrapper.m_Game_MousePosition;
        public InputAction @WASD => m_Wrapper.m_Game_WASD;
        public InputAction @CameraGrab => m_Wrapper.m_Game_CameraGrab;
        public InputAction @CameraZoomIn => m_Wrapper.m_Game_CameraZoomIn;
        public InputAction @CameraZoomOut => m_Wrapper.m_Game_CameraZoomOut;
        public InputAction @CameraZoomReset => m_Wrapper.m_Game_CameraZoomReset;
        public InputAction @ScreenCapture => m_Wrapper.m_Game_ScreenCapture;
        public InputActionMap Get() { return m_Wrapper.m_Game; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameActions set) { return set.Get(); }
        public void SetCallbacks(IGameActions instance)
        {
            if (m_Wrapper.m_GameActionsCallbackInterface != null)
            {
                @LeftMouseButton.started -= m_Wrapper.m_GameActionsCallbackInterface.OnLeftMouseButton;
                @LeftMouseButton.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnLeftMouseButton;
                @LeftMouseButton.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnLeftMouseButton;
                @LeftMouseButtonReleased.started -= m_Wrapper.m_GameActionsCallbackInterface.OnLeftMouseButtonReleased;
                @LeftMouseButtonReleased.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnLeftMouseButtonReleased;
                @LeftMouseButtonReleased.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnLeftMouseButtonReleased;
                @MousePosition.started -= m_Wrapper.m_GameActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnMousePosition;
                @WASD.started -= m_Wrapper.m_GameActionsCallbackInterface.OnWASD;
                @WASD.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnWASD;
                @WASD.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnWASD;
                @CameraGrab.started -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraGrab;
                @CameraGrab.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraGrab;
                @CameraGrab.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraGrab;
                @CameraZoomIn.started -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraZoomIn;
                @CameraZoomIn.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraZoomIn;
                @CameraZoomIn.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraZoomIn;
                @CameraZoomOut.started -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraZoomOut;
                @CameraZoomOut.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraZoomOut;
                @CameraZoomOut.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraZoomOut;
                @CameraZoomReset.started -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraZoomReset;
                @CameraZoomReset.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraZoomReset;
                @CameraZoomReset.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnCameraZoomReset;
                @ScreenCapture.started -= m_Wrapper.m_GameActionsCallbackInterface.OnScreenCapture;
                @ScreenCapture.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnScreenCapture;
                @ScreenCapture.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnScreenCapture;
            }
            m_Wrapper.m_GameActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LeftMouseButton.started += instance.OnLeftMouseButton;
                @LeftMouseButton.performed += instance.OnLeftMouseButton;
                @LeftMouseButton.canceled += instance.OnLeftMouseButton;
                @LeftMouseButtonReleased.started += instance.OnLeftMouseButtonReleased;
                @LeftMouseButtonReleased.performed += instance.OnLeftMouseButtonReleased;
                @LeftMouseButtonReleased.canceled += instance.OnLeftMouseButtonReleased;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
                @WASD.started += instance.OnWASD;
                @WASD.performed += instance.OnWASD;
                @WASD.canceled += instance.OnWASD;
                @CameraGrab.started += instance.OnCameraGrab;
                @CameraGrab.performed += instance.OnCameraGrab;
                @CameraGrab.canceled += instance.OnCameraGrab;
                @CameraZoomIn.started += instance.OnCameraZoomIn;
                @CameraZoomIn.performed += instance.OnCameraZoomIn;
                @CameraZoomIn.canceled += instance.OnCameraZoomIn;
                @CameraZoomOut.started += instance.OnCameraZoomOut;
                @CameraZoomOut.performed += instance.OnCameraZoomOut;
                @CameraZoomOut.canceled += instance.OnCameraZoomOut;
                @CameraZoomReset.started += instance.OnCameraZoomReset;
                @CameraZoomReset.performed += instance.OnCameraZoomReset;
                @CameraZoomReset.canceled += instance.OnCameraZoomReset;
                @ScreenCapture.started += instance.OnScreenCapture;
                @ScreenCapture.performed += instance.OnScreenCapture;
                @ScreenCapture.canceled += instance.OnScreenCapture;
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
        void OnLeftMouseButton(InputAction.CallbackContext context);
        void OnLeftMouseButtonReleased(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnWASD(InputAction.CallbackContext context);
        void OnCameraGrab(InputAction.CallbackContext context);
        void OnCameraZoomIn(InputAction.CallbackContext context);
        void OnCameraZoomOut(InputAction.CallbackContext context);
        void OnCameraZoomReset(InputAction.CallbackContext context);
        void OnScreenCapture(InputAction.CallbackContext context);
    }
}
