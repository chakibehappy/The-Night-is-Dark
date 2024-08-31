using System;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
#endif

namespace Aceline.RPG.Editor
{
    public class EndCard : BaseCard
    {
        private EndData endData = new EndData();

        public EndData EndData { get => endData; set => endData = value; }
        
        public EndCard() { }

        public EndCard(Vector2 _position, BoardEditorWindow _editorWindow, BoardGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Cards/EndCardStyleSheet");
            styleSheets.Add(styleSheet);

            title = "End";
            SetPosition(new Rect(_position, defaultCardSize));
            cardGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);

            EnumField enumField = GetNewEndTypeEnumField(endData.EndState);

            mainContainer.Add(enumField);
        }

        public override void LoadValueInToField()
        {
            if (EndData.EndState.EnumField != null)
                EndData.EndState.EnumField.SetValueWithoutNotify(EndData.EndState.Value);
        }

    }
}