using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

namespace Aceline.RPG.Editor
{
    public class BoardGraphView : GraphView
    {
        private string styleSheetName = "USS/BoardGraphView/BoardGraphViewStyleSheet";
        private BoardEditorWindow editorWindow;
        private CardSearchWindow searchWindow;


        public BoardGraphView(BoardEditorWindow editorWindow)
        {
            this.editorWindow = editorWindow;

            StyleSheet tempStyleSheet = Resources.Load<StyleSheet>(styleSheetName);
            styleSheets.Add(tempStyleSheet);

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddSearchWindow();
        }


        private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<CardSearchWindow>();
            searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }


        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach((port) =>
            {
                if (startPort != port && startPort.node != port.node && startPort.direction != port.direction && startPort.portColor == port.portColor)
                {
                    compatiblePorts.Add(port);
                }
            });
            return compatiblePorts;
        }


        public void ReloadLanguage()
        {
            List<BaseCard> allNodes = nodes.ToList().Where(node => node is BaseCard).Cast<BaseCard>().ToList();
            foreach (BaseCard node in allNodes)
            {
                node.ReloadLanguage();
            }
        }


        #region Creating Cards
        public StartCard CreateStartCard(Vector2 position)
        {
            return new StartCard(position, editorWindow, this);
        }

        public BranchCard CreateBranchCard(Vector2 position, bool createStartCondition)
        {
            return new BranchCard(position, editorWindow, this, createStartCondition);
        }

        public DialogueCard CreateDialogueCard(Vector2 position)
        {
            return new DialogueCard(position, editorWindow, this);
        }

        public EventCard CreateEventCard(Vector2 position)
        {
            return new EventCard(position, editorWindow, this);
        }

        public ChoiceCard CreateChoiceCard(Vector2 position)
        {
            return new ChoiceCard(position, editorWindow, this);
        }

        public EndCard CreateEndCard(Vector2 position)
        {
            return new EndCard(position, editorWindow, this);
        }
        #endregion
    }
}
