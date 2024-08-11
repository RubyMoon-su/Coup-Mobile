using UnityEngine;
using UnityEngine.UI;

namespace Coup_Mobile.InGame.GameManager.Ui
{
    public class UI_MiniMap_Control : UI_Control
    {
        private Camera miniMap_Camera;
        private RawImage projector_Control;

        public UI_MiniMap_Control(GameUiManager gameUiManager) : base(gameUiManager)
        {

        }

        protected override void Install_System()
        {
            GameObject MiniMap_GameObject = GameObject.Find("MiniMap_UI").gameObject;

            miniMap_Camera = MiniMap_GameObject.transform.GetComponentInChildren<Camera>();

            projector_Control = MiniMap_GameObject.transform.GetComponentInChildren<RawImage>();

            this.install_Complate = true;
        }

        public override void OnInteractive_UI()
        {
            throw new System.NotImplementedException();
        }
    }
}