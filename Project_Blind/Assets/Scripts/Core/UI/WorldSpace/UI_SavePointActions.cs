using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blind
{
    public class UI_SavePointActions : UI_WorldSpace
    {
        enum Buttons
        {
            Button_Store,
            Button_Rest,
            Button_Leave
        }
        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            InitEvents();
        }
        private void InitEvents()
        {
            Get<Button>((int)Buttons.Button_Store).gameObject.BindEvent(PushStoreButton);
            Get<Button>((int)Buttons.Button_Rest).gameObject.BindEvent(PushRestButton);
            Get<Button>((int)Buttons.Button_Leave).gameObject.BindEvent(PushLeaveButton);
        }
        private void PushStoreButton()
        {
            SoundManager.Instance.Play("Select");
            UIManager.Instance.ShowNormalUI<UI_Shop>();
        }
        private void PushRestButton()
        {
            SoundManager.Instance.Play("Select");
            SavePoint save = FindObjectOfType<SavePoint>();
            if (save != null) save.DoInteraction();
            UI_ScreenConversation ui = UIManager.Instance.ShowNormalUI<UI_ScreenConversation>();
            ui.SetName("��ž");
            ui.SetScriptTitle(Define.ScriptTitle.RestScript);
            ui.StopMove = false;
        }
        private void PushLeaveButton()
        {
            SoundManager.Instance.Play("Select");
            UIManager.Instance.CloseWorldSpaceUI(this);
        }
        private void OnDestroy()
        {
            PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
            if (player != null) player.UnTalk();
        }
    }
}

