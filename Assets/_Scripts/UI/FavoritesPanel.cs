using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.UI
{
    class FavoritesPanel : MonoBehaviour, IPanel
    {
        public static FavoritesPanel Instance;

        [SerializeField] private GameObject Panel;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        void Start()
        {
            MainMenuManagerUI.Instance.CloseablePanels += this.doSomething;
        }

        protected virtual void doSomething(object sender, EventArgs args)
        {
            this.Disable();
        }

        public void Enable()
        {
            Panel.SetActive(true);
        }

        public void Disable()
        {
            Panel.SetActive(false);
        }

        public void Init()
        {

        }

        public void Refresh()
        {

        }
    }
}
