using UnityEngine;
using UnityEngine.UIElements;
using System;

#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
#endif

namespace Aceline.RPG.Editor
{
    public class BranchCard : BaseCard
    {
        private BranchData branchData = new BranchData();
        
        public BranchData BranchData { get => branchData; set => branchData = value; }

        public BranchCard() { }

        public BranchCard(Vector2 position, BoardEditorWindow editorWindow, BoardGraphView graphView, bool createStartCondition = false)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Cards/BranchCardStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Branch";
            SetPosition(new Rect(position, defaultCardSize));
            cardGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("True", Port.Capacity.Single);
            AddOutputPort("False", Port.Capacity.Single);

            AddTopMenuButton();

            if (createStartCondition)
            {
                AddCondition();
            }
        }


        private void AddTopMenuButton()
        {
            ToolbarMenu menu = new ToolbarMenu();
            menu.text = "Add Condition";

            menu.menu.AppendAction("Condition", new Action<DropdownMenuAction>(x => AddCondition()));
            menu.AddToClassList("branchTopMenuButton");

            titleButtonContainer.Add(menu);
            RefreshExpandedState();
        }

        public void AddCondition(BranchCondition condition = null)
        {
            AddBranchCondition(branchData.Conditions, condition);
        }

    }
}
