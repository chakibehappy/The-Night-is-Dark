using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

namespace Aceline.RPG.Editor
{
    public class CardSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private BoardEditorWindow editorWindow;
        private BoardGraphView graphView;

        private Texture2D iconForSpacing;


        public void Configure(BoardEditorWindow editorWindow, BoardGraphView graphView)
        {
            this.editorWindow = editorWindow;
            this.graphView = graphView;

            iconForSpacing = new Texture2D(1, 1);
            iconForSpacing.SetPixel(0, 0, new Color(0, 0, 0, 0));
            iconForSpacing.Apply();
        }


        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Board Editor"), 0),
            new SearchTreeGroupEntry(new GUIContent("Cards"), 1),

            AddCardSearch("Dialogue Card", new DialogueCard()),
            AddCardSearch("Choice Card", new ChoiceCard()),
            AddCardSearch("Branch Card", new BranchCard()),
            AddCardSearch("Event Card", new EventCard()),
            AddCardSearch("Start Card", new StartCard()),
            AddCardSearch("End Card", new EndCard()),
        };

            return tree;
        }


        private SearchTreeEntry AddCardSearch(string cardName, BaseCard card)
        {
            SearchTreeEntry tmp = new SearchTreeEntry(new GUIContent(cardName, iconForSpacing))
            {
                level = 2,
                userData = card
            };
            return tmp;
        }


        public bool OnSelectEntry(SearchTreeEntry treeEntry, SearchWindowContext context)
        {
            Vector2 mousePos = editorWindow.rootVisualElement.ChangeCoordinatesTo(
                editorWindow.rootVisualElement.parent, context.screenMousePosition - editorWindow.position.position
            );

            Vector2 graphMousePos = graphView.contentViewContainer.WorldToLocal(mousePos);

            return CheckForCardType(treeEntry, graphMousePos);
        }


        private bool CheckForCardType(SearchTreeEntry searchTree, Vector2 pos)
        {
            switch (searchTree.userData)
            {
                case StartCard card:
                    graphView.AddElement(graphView.CreateStartCard(pos));
                    return true;

                case BranchCard card:
                    graphView.AddElement(graphView.CreateBranchCard(pos, true));
                    return true;

                case DialogueCard card:
                    graphView.AddElement(graphView.CreateDialogueCard(pos));
                    return true;

                case ChoiceCard card:
                    graphView.AddElement(graphView.CreateChoiceCard(pos));
                    return true;

                case EventCard card:
                    graphView.AddElement(graphView.CreateEventCard(pos));
                    return true;

                case EndCard card:
                    graphView.AddElement(graphView.CreateEndCard(pos));
                    return true;

                default:
                    break;
            }
            return false;
        }
    }
}
