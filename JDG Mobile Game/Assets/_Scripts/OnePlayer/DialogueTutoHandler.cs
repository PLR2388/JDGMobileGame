using System;

namespace OnePlayer
{
    public class DialogueTutoHandler: StaticInstance<DialogueTutoHandler>
    {
        public int CurrentDialogIndex { get; private set; } = 0;

        protected override void Awake()
        {
            base.Awake();
            DialogueUI.DialogIndex.AddListener(SaveDialogIndex);
        }

        private void OnDestroy()
        {
            DialogueUI.DialogIndex.RemoveListener(SaveDialogIndex);
        }

        private void SaveDialogIndex(int index)
        {
            CurrentDialogIndex = index;
        }
    }
}