﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blind
{
    public class UI_Setting : UI_Base
    {
        int _currCursor;
        const int SIZE = 5;
        const int BUTTON_CNT = 3;
        Define.Resolution[] _resolutions = new Define.Resolution[SIZE];

        private bool _isInputKeyChange = false;

        [SerializeField] private GameObject GraphicsSetting;
        [SerializeField] private GameObject AudioSetting;
        [SerializeField] private GameObject KeyBindsSetting;
        private GameObject _currActiveSetting = null;
        private GameObject[] _settings = new GameObject[BUTTON_CNT];

        [SerializeField] private Sprite Button_On;
        [SerializeField] private Sprite Button_Off;

        [SerializeField] private Sprite Button_Graphics_NonClicked;
        [SerializeField] private Sprite Button_Graphics_Clicked;
        [SerializeField] private Sprite Button_Audio_NonClicked;
        [SerializeField] private Sprite Button_Audio_Clicked;
        [SerializeField] private Sprite Button_KeyBinds_NonClicked;
        [SerializeField] private Sprite Button_KeyBinds_Clicked;

        [SerializeField] private Type _type;

        //[SerializeField] private GameObject testKeyChange;
        [SerializeField] private GameObject parentUI;

        private Dictionary<Define.KeyAction, KeyCode> newKeyDict;
        private Dictionary<Texts, Define.KeyAction> Enum2KeyAction = new Dictionary<Texts, Define.KeyAction>();

        GameData _gameData = null;

        private Texts _currKeyText = Texts.Unknown;
        public enum Type
        {
            Main,
            Menu
        }
        #region Enums
        enum Texts
        {
            Unknown,

            Text_ScreenSizeValue,

            Text_BeforeChange_RightMove,
            Text_BeforeChange_LeftMove,
            Text_BeforeChange_Jump,
            Text_BeforeChange_JumpDown,
            Text_BeforeChange_Dash,
            Text_BeforeChange_Attack,
            Text_BeforeChange_Skill,
            Text_BeforeChange_UseItem,
            Text_BeforeChange_ChangeSlot,
            Text_BeforeChange_Reflect,
            Text_BeforeChange_Wave,
            Text_BeforeChange_Interaction,

            Text_AfterChange_RightMove,
            Text_AfterChange_LeftMove,
            Text_AfterChange_Jump,
            Text_AfterChange_JumpDown,
            Text_AfterChange_Dash,
            Text_AfterChange_Attack,
            Text_AfterChange_Skill,
            Text_AfterChange_UseItem,
            Text_AfterChange_ChangeSlot,
            Text_AfterChange_Reflect,
            Text_AfterChange_Wave,
            Text_AfterChange_Interaction
        }
        enum Images
        {
            Button_Graphics,
            Button_Audio,
            Button_KeyBinds,

            Button_Resolution_LeftArrow,
            Button_Resolution_RightArrow,

            Button_WindowModeOnOff,
            Button_ScreenVibrationOnOff,

            Button_Refresh,
            Button_Save,
        }
        enum Sliders
        {
            Slider_MotionEffect,

            Slider_MasterVolume,
            Slider_SoundEffect,
            Slider_Bgm,
        }
        #endregion

        struct ImageInfo
        {
            public int width;
            public int height;
            public Sprite sprite;
        }
        struct ImageInfoSet
        {
            public ImageInfo click;
            public ImageInfo nonClick;
        }

        private ImageInfo _Button_Graphics_NonClicked;
        private ImageInfo _Button_Graphics_Clicked;
        private ImageInfo _Button_Audio_NonClicked;
        private ImageInfo _Button_Audio_Clicked;
        private ImageInfo _Button_KeyBinds_NonClicked;
        private ImageInfo _Button_KeyBinds_Clicked;

        ImageInfoSet[] _imageInfos;
        public override void Init()
        {
            Bind<Text>(typeof(Texts));
            Bind<Image>(typeof(Images));
            Bind<Slider>(typeof(Sliders));

            UIManager.Instance.KeyInputEvents -= HandleKeyInput;
            UIManager.Instance.KeyInputEvents += HandleKeyInput;

            _gameData = DataManager.Instance.GameData;

            InitResolution();
            InitTexts();
            InitSliders();
            InitEvents();

            _settings[(int)Images.Button_Graphics] = GraphicsSetting;
            _settings[(int)Images.Button_Audio] = AudioSetting;
            _settings[(int)Images.Button_KeyBinds] = KeyBindsSetting;

            _Button_Graphics_NonClicked = new ImageInfo() { width = 132, height = 51, sprite = Button_Graphics_NonClicked };
            _Button_Graphics_Clicked = new ImageInfo() { width = 773, height = 71, sprite = Button_Graphics_Clicked };

            _Button_Audio_NonClicked = new ImageInfo() { width = 138, height = 50, sprite = Button_Audio_NonClicked };
            _Button_Audio_Clicked = new ImageInfo() { width = 773, height = 70, sprite = Button_Audio_Clicked };

            _Button_KeyBinds_NonClicked = new ImageInfo() { width = 130, height = 50, sprite = Button_KeyBinds_NonClicked };
            _Button_KeyBinds_Clicked = new ImageInfo() { width = 773, height = 71, sprite = Button_KeyBinds_Clicked };

            _imageInfos = new ImageInfoSet[BUTTON_CNT];

            _imageInfos[(int)Images.Button_Graphics].nonClick = _Button_Graphics_NonClicked;
            _imageInfos[(int)Images.Button_Graphics].click = _Button_Graphics_Clicked;
            _imageInfos[(int)Images.Button_Audio].nonClick = _Button_Audio_NonClicked;
            _imageInfos[(int)Images.Button_Audio].click = _Button_Audio_Clicked;
            _imageInfos[(int)Images.Button_KeyBinds].nonClick = _Button_KeyBinds_NonClicked;
            _imageInfos[(int)Images.Button_KeyBinds].click = _Button_KeyBinds_Clicked;

            _currCursor = (int)Images.Button_Graphics;

            Get<Image>((int)Images.Button_Graphics).gameObject.BindEvent(() => ChangeCursor((int)Images.Button_Graphics));
            Get<Image>((int)Images.Button_Audio).gameObject.BindEvent(() => ChangeCursor((int)Images.Button_Audio));
            Get<Image>((int)Images.Button_KeyBinds).gameObject.BindEvent(() => ChangeCursor((int)Images.Button_KeyBinds));

            GraphicsSetting.SetActive(false);
            AudioSetting.SetActive(false);
            KeyBindsSetting.SetActive(false);

            ChangeCursor((int)Images.Button_Graphics);

            Get<Text>((int)Texts.Text_AfterChange_RightMove).gameObject.BindEvent(() => PushChangeKeyButton(Texts.Text_AfterChange_RightMove));
            Get<Text>((int)Texts.Text_AfterChange_LeftMove).gameObject.BindEvent(() => PushChangeKeyButton(Texts.Text_AfterChange_LeftMove));
            Get<Text>((int)Texts.Text_AfterChange_Jump).gameObject.BindEvent(() => PushChangeKeyButton(Texts.Text_AfterChange_Jump));
            Get<Text>((int)Texts.Text_AfterChange_JumpDown).gameObject.BindEvent(() => PushChangeKeyButton(Texts.Text_AfterChange_JumpDown));
            Get<Text>((int)Texts.Text_AfterChange_Dash).gameObject.BindEvent(() => PushChangeKeyButton(Texts.Text_AfterChange_Dash));
            Get<Text>((int)Texts.Text_AfterChange_Attack).gameObject.BindEvent(() => PushChangeKeyButton(Texts.Text_AfterChange_Attack));
            Get<Text>((int)Texts.Text_AfterChange_Skill).gameObject.BindEvent(() => PushChangeKeyButton(Texts.Text_AfterChange_Skill));
            Get<Text>((int)Texts.Text_AfterChange_UseItem).gameObject.BindEvent(() => PushChangeKeyButton(Texts.Text_AfterChange_UseItem));
            Get<Text>((int)Texts.Text_AfterChange_ChangeSlot).gameObject.BindEvent(() => PushChangeKeyButton(Texts.Text_AfterChange_ChangeSlot));
            Get<Text>((int)Texts.Text_AfterChange_Reflect).gameObject.BindEvent(() => PushChangeKeyButton(Texts.Text_AfterChange_Reflect));
            Get<Text>((int)Texts.Text_AfterChange_Wave).gameObject.BindEvent(() => PushChangeKeyButton(Texts.Text_AfterChange_Wave));
            Get<Text>((int)Texts.Text_AfterChange_Interaction).gameObject.BindEvent(() => PushChangeKeyButton(Texts.Text_AfterChange_Interaction));

            Get<Image>((int)Images.Button_Refresh).gameObject.BindEvent(InitKeySetting);
            Get<Image>((int)Images.Button_Save).gameObject.BindEvent(SaveKeySetting);

            Enum2KeyAction.Add(Texts.Text_AfterChange_RightMove, Define.KeyAction.RightMove);
            Enum2KeyAction.Add(Texts.Text_AfterChange_LeftMove, Define.KeyAction.LeftMove);
            Enum2KeyAction.Add(Texts.Text_AfterChange_Jump, Define.KeyAction.Jump);
            Enum2KeyAction.Add(Texts.Text_AfterChange_JumpDown, Define.KeyAction.DownJump);
            Enum2KeyAction.Add(Texts.Text_AfterChange_Dash, Define.KeyAction.Dash);
            Enum2KeyAction.Add(Texts.Text_AfterChange_Attack, Define.KeyAction.Attack);
            Enum2KeyAction.Add(Texts.Text_AfterChange_Skill, Define.KeyAction.Skill);
            Enum2KeyAction.Add(Texts.Text_AfterChange_UseItem, Define.KeyAction.ItemT);
            Enum2KeyAction.Add(Texts.Text_AfterChange_ChangeSlot, Define.KeyAction.ChangeSlot);
            Enum2KeyAction.Add(Texts.Text_AfterChange_Reflect, Define.KeyAction.Paring);
            Enum2KeyAction.Add(Texts.Text_AfterChange_Wave, Define.KeyAction.Wave);
            Enum2KeyAction.Add(Texts.Text_AfterChange_Interaction, Define.KeyAction.Interaction);

            //testKeyChange.SetActive(false);

            InitBeforeChangeTexts();
        }
        private void OnEnable()
        {
            UIManager.Instance.KeyInputEvents -= HandleKeyInput;
            UIManager.Instance.KeyInputEvents += HandleKeyInput;

            ChangeCursor((int)Images.Button_Graphics);
        }
        private void HandleKeyInput()
        {
            if (!Input.anyKey)
                return;

            if (_uiNum != UIManager.Instance.UINum)
                return;

            if (_isInputKeyChange) return;

            if (Input.GetKeyDown(KeyCode.Escape) && _type == Type.Main)
            {
                PushCloseButton();
                return;
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                ChangeCursor((_currCursor + 1) % BUTTON_CNT);
                return;
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                ChangeCursor((_currCursor - 1 + BUTTON_CNT) % BUTTON_CNT);
                return;
            }
        }
        #region Initialize
        private void InitResolution()
        {
            _resolutions[0].width = 960;
            _resolutions[0].height = 540;

            _resolutions[1].width = 1280;
            _resolutions[1].height = 720;

            _resolutions[2].width = 1920;
            _resolutions[2].height = 1080;
            
            _resolutions[3].width = 2560;
            _resolutions[3].height = 1440;
            
            _resolutions[4].width = 3840;
            _resolutions[4].height = 2160;

            UIManager.Instance.Resolution = _resolutions[_gameData.resolutionIndex];
            UIManager.Instance.IsWindowMode = _gameData.windowMode;
        }
        private void InitTexts()
        {
            Get<Text>((int)Texts.Text_ScreenSizeValue).text =
               $"{_resolutions[_gameData.resolutionIndex].width} * {_resolutions[_gameData.resolutionIndex].height}";
        }
        private void InitSliders()
        {
            // 사운드 관련
            Get<Slider>((int)Sliders.Slider_MasterVolume).value = _gameData.mastetVolume;
            Get<Slider>((int)Sliders.Slider_Bgm).value = _gameData.bgmVolume;
            Get<Slider>((int)Sliders.Slider_SoundEffect).value = _gameData.effectVolume;
            Get<Slider>((int)Sliders.Slider_MasterVolume).onValueChanged.AddListener(delegate { ChangeMasterVolume(); });
            Get<Slider>((int)Sliders.Slider_Bgm).onValueChanged.AddListener(delegate { ChangeVolume(Define.Sound.Bgm); });
            Get<Slider>((int)Sliders.Slider_SoundEffect).onValueChanged.AddListener(delegate { ChangeVolume(Define.Sound.Effect); });
            // 이펙트 관련 TODO
            Get<Slider>((int)Sliders.Slider_MotionEffect).value = _gameData.motionEffect;
            Get<Slider>((int)Sliders.Slider_MotionEffect).onValueChanged.AddListener(delegate { ChangeMotionEffect(); });
            // 키셋팅 관련 TODO
            // 해상도 관련 TODO
        }
        private void InitEvents()
        {
            // 사운드 관련
            // 이펙트 관련 TODO
            Get<Image>((int)Images.Button_ScreenVibrationOnOff).gameObject.BindEvent(PushVibrationOnOffButton, Define.UIEvent.Click);
            // 키셋팅 관련 TODO
            // 해상도 관련 TODO
            Get<Image>((int)Images.Button_Resolution_RightArrow).gameObject.BindEvent(PushRightButton, Define.UIEvent.Click);
            Get<Image>((int)Images.Button_Resolution_LeftArrow).gameObject.BindEvent(PushLeftButton, Define.UIEvent.Click);
            Get<Image>((int)Images.Button_WindowModeOnOff).gameObject.BindEvent(PushWindowModeButton, Define.UIEvent.Click);
        }
        #endregion
        #region UI Event
        private void PushCloseButton()
        {
            //SoundManager.Instance.StopBGM();
            DataManager.Instance.SaveGameData();
            UIManager.Instance.KeyInputEvents -= HandleKeyInput;
            UIManager.Instance.KeyInputEvents -= HandleKeyChangeInput;
            UIManager.Instance.CloseNormalUI(this);
        }
        #endregion
        public void CloseUI()
        {
            UIManager.Instance.KeyInputEvents -= HandleKeyInput;
        }
        #region Sound Event
        private void ChangeMasterVolume()
        {
            SoundManager.Instance.ChangeMasterVolume(Get<Slider>((int)Sliders.Slider_MasterVolume).value);
        }
        private void ChangeVolume(Define.Sound sound)
        {
            switch (sound)
            {
                case Define.Sound.Bgm:
                    SoundManager.Instance.ChangeVolume(Define.Sound.Bgm, Get<Slider>((int)Sliders.Slider_Bgm).value);
                    break;
                case Define.Sound.Effect:
                    SoundManager.Instance.ChangeVolume(Define.Sound.Effect, Get<Slider>((int)Sliders.Slider_SoundEffect).value);
                    break;
            }
        }
        #endregion
        #region Effect Event
        private void PushVibrationOnOffButton()
        {
            SoundManager.Instance.Play("Select");
            if (_gameData.vibration)
            {
                Get<Image>((int)Images.Button_ScreenVibrationOnOff).sprite = Button_Off;
                _gameData.vibration = false;
                //DataManager.Instance.GameData.vibration = false;
            }
            else
            {
                Get<Image>((int)Images.Button_ScreenVibrationOnOff).sprite = Button_On;
                _gameData.vibration = true;
                //DataManager.Instance.GameData.vibration = true;
            }
        }
        private void ChangeMotionEffect()
        {
            _gameData.motionEffect = Get<Slider>((int)Sliders.Slider_MotionEffect).value;
        }
        #endregion
        #region ScreenEvent
        private void PushRightButton()
        {
            SoundManager.Instance.Play("Select");
            _gameData.resolutionIndex = (_gameData.resolutionIndex + 1) % SIZE;
            Get<Text>((int)Texts.Text_ScreenSizeValue).text =
                $"{_resolutions[_gameData.resolutionIndex].width} * {_resolutions[_gameData.resolutionIndex].height}";
            UIManager.Instance.Resolution = _resolutions[_gameData.resolutionIndex];
        }
        private void PushLeftButton()
        {
            SoundManager.Instance.Play("Select");
            _gameData.resolutionIndex = (_gameData.resolutionIndex - 1 + SIZE) % SIZE;
            Get<Text>((int)Texts.Text_ScreenSizeValue).text =
               $"{_resolutions[_gameData.resolutionIndex].width} * {_resolutions[_gameData.resolutionIndex].height}";
            UIManager.Instance.Resolution = _resolutions[_gameData.resolutionIndex];
        }
        private void PushWindowModeButton()
        {
            SoundManager.Instance.Play("Select");
            if (_gameData.windowMode)
            {
                _gameData.windowMode = false;
                Get<Image>((int)Images.Button_WindowModeOnOff).sprite = Button_Off;
                UIManager.Instance.IsWindowMode = _gameData.windowMode;
            }
            else
            {
                _gameData.windowMode = true;
                Get<Image>((int)Images.Button_WindowModeOnOff).sprite = Button_On;
                UIManager.Instance.IsWindowMode = _gameData.windowMode;
            }
        }
        #endregion
        private void ClearCurrActiveSetting()
        {
            if (_currActiveSetting != null)
            {
                _currActiveSetting.SetActive(false);
                _currActiveSetting = null;
            }
        }
        private void EnterCursor(int idx)
        {
            _currCursor = idx;
            Image currImage = Get<Image>(idx);
            ImageInfo imageInfo = _imageInfos[idx].click;
            currImage.sprite = imageInfo.sprite;
            currImage.rectTransform.sizeDelta = new Vector2(imageInfo.width, imageInfo.height);

            ClearCurrActiveSetting();
            _settings[idx].SetActive(true);
            _currActiveSetting = _settings[idx];
            if (_currActiveSetting == KeyBindsSetting)
            {
                RefreshKeyBindUI();
            }
        }
        private void ExitCursor(int idx)
        {
            Image currImage = Get<Image>(idx);
            ImageInfo imageInfo = _imageInfos[idx].nonClick;
            currImage.sprite = imageInfo.sprite;
            currImage.rectTransform.sizeDelta = new Vector2(imageInfo.width, imageInfo.height);
        }
        private void ChangeCursor(int nextCursor)
        {
            SoundManager.Instance.Play("CursorMove");
            ExitCursor(_currCursor);
            EnterCursor(nextCursor);
        }

        private void PushChangeKeyButton(Texts type)
        {

            UIManager.Instance.KeyInputEvents -= HandleKeyChangeInput;
            UIManager.Instance.KeyInputEvents += HandleKeyChangeInput;

            ControllInput(true);

            _currKeyText = type;
            // 텍스트가 눌리면 입력을 받는다. TODO
            // 입력을 한번 받으면 그만 받게한다 TODO

        }

        private void HandleKeyChangeInput()
        {
            if (Input.anyKeyDown)
            {
                foreach(KeyCode k in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(k))
                    {
                        foreach(KeyCode key in newKeyDict.Values)
                        {
                            if (key == k)
                            {
                                UIManager.Instance.KeyInputEvents -= HandleKeyChangeInput;
                                ControllInput(false);
                                return;
                            }
                        }

                        Define.KeyAction action = Enum2KeyAction[_currKeyText];

                        newKeyDict[action] = k;
                        Get<Text>((int)_currKeyText).text = Enum.GetName(typeof(KeyCode), (int)k);

                        UIManager.Instance.KeyInputEvents -= HandleKeyChangeInput;
                        ControllInput(false);
                    }
                }
            }
        }

        private void ControllInput(bool changeNow)
        {
            _isInputKeyChange = changeNow;
            //testKeyChange.SetActive(changeNow);
            if (parentUI == null) return;
            parentUI.GetComponent<UI_Menu>().CanInput = !changeNow;
        }

        private void InitBeforeChangeTexts()
        {
            Dictionary<Define.KeyAction, KeyCode> dict = InputController.Instance.CurrKeyActions;
            foreach(Define.KeyAction action in dict.Keys)
            {
                KeyCode key = dict[action];
                string keyText = Enum.GetName(typeof(KeyCode), dict[action]);
                switch (action)
                {
                    case Define.KeyAction.RightMove:
                        Get<Text>((int)Texts.Text_BeforeChange_RightMove).text = keyText;
                        Get<Text>((int)Texts.Text_AfterChange_RightMove).text = keyText;
                        break;
                    case Define.KeyAction.LeftMove:
                        Get<Text>((int)Texts.Text_BeforeChange_LeftMove).text = keyText;
                        Get<Text>((int)Texts.Text_AfterChange_LeftMove).text = keyText;
                        break;
                    case Define.KeyAction.Jump:
                        Get<Text>((int)Texts.Text_BeforeChange_Jump).text = keyText;
                        Get<Text>((int)Texts.Text_AfterChange_Jump).text = keyText;
                        break;
                    case Define.KeyAction.DownJump:
                        Get<Text>((int)Texts.Text_BeforeChange_JumpDown).text = keyText;
                        Get<Text>((int)Texts.Text_AfterChange_JumpDown).text = keyText;
                        break;
                    case Define.KeyAction.Dash:
                        Get<Text>((int)Texts.Text_BeforeChange_Dash).text = keyText;
                        Get<Text>((int)Texts.Text_AfterChange_Dash).text = keyText;
                        break;
                    case Define.KeyAction.Attack:
                        Get<Text>((int)Texts.Text_BeforeChange_Attack).text = keyText;
                        Get<Text>((int)Texts.Text_AfterChange_Attack).text = keyText;
                        break;
                    case Define.KeyAction.Skill:
                        Get<Text>((int)Texts.Text_BeforeChange_Skill).text = keyText;
                        Get<Text>((int)Texts.Text_AfterChange_Skill).text = keyText;
                        break;
                    case Define.KeyAction.ItemT:
                        Get<Text>((int)Texts.Text_BeforeChange_UseItem).text = keyText;
                        Get<Text>((int)Texts.Text_AfterChange_UseItem).text = keyText;
                        break;
                    case Define.KeyAction.ChangeSlot:
                        Get<Text>((int)Texts.Text_BeforeChange_ChangeSlot).text = keyText;
                        Get<Text>((int)Texts.Text_AfterChange_ChangeSlot).text = keyText;
                        break;
                    case Define.KeyAction.Paring:
                        Get<Text>((int)Texts.Text_BeforeChange_Reflect).text = keyText;
                        Get<Text>((int)Texts.Text_AfterChange_Reflect).text = keyText;
                        break;
                    case Define.KeyAction.Wave:
                        Get<Text>((int)Texts.Text_BeforeChange_Wave).text = keyText;
                        Get<Text>((int)Texts.Text_AfterChange_Wave).text = keyText;
                        break;
                    case Define.KeyAction.Interaction:
                        Get<Text>((int)Texts.Text_BeforeChange_Interaction).text = keyText;
                        Get<Text>((int)Texts.Text_AfterChange_Interaction).text = keyText;
                        break;
                }
            }
        }

        private void RefreshKeyBindUI()
        {
            InitBeforeChangeTexts();
            newKeyDict = new Dictionary<Define.KeyAction, KeyCode>(InputController.Instance.CurrKeyActions);
        }

        private void InitKeySetting()
        {
            InputController.Instance.CurrKeyActions = new Dictionary<Define.KeyAction, KeyCode>(InputController.Instance.InitialKeyActions);
            RefreshKeyBindUI();
            InputController.Instance.ReKetSet();
        }
        private void SaveKeySetting()
        {
            InputController.Instance.CurrKeyActions = newKeyDict;
            RefreshKeyBindUI();
            InputController.Instance.ReKetSet();
        }
    }
}