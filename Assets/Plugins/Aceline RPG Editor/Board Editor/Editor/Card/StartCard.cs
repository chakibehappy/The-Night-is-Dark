using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

namespace Aceline.RPG.Editor
{
    public class StartCard : BaseCard
    {
        private StartData startData = new StartData();

        public StartData StartData { get => startData; set => startData = value; }

        public StartCard() { }

        public StartCard(Vector2 _position, BoardEditorWindow _editorWindow, BoardGraphView _graphView)
        {
            editorWindow = _editorWindow;
            graphView = _graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Cards/StartCardStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Start";
            SetPosition(new Rect(_position, defaultCardSize));
            cardGuid = Guid.NewGuid().ToString();

            AddOutputPort("Output", Port.Capacity.Single);

            RefreshExpandedState();
            RefreshPorts();
        }

    }
}
