using UnityEngine;
using UnityEngine.UIElements;
using System;

#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
#endif

namespace Aceline.RPG.Editor
{
    public class EventCard : BaseCard
    {
        private EventData eventData = new EventData();

        public EventData EventData { get => eventData; set => eventData = value; }

        public EventCard() { }

        public EventCard(Vector2 pos, BoardEditorWindow editorWindow, BoardGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("USS/Cards/EventCardStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Event";
            SetPosition(new Rect(pos, defaultCardSize));
            cardGuid = Guid.NewGuid().ToString();

            AddInputPort("Input", Port.Capacity.Multi);
            AddOutputPort("Output", Port.Capacity.Single);

            AddTopMenuButton();
        }

        private void AddTopMenuButton()
        {
            ToolbarMenu menu = new ToolbarMenu();
            menu.AddToClassList("TopMenuButton");
            menu.text = "Add Event";

            menu.menu.AppendAction("Event Modifier", new Action<DropdownMenuAction>(x => AddModifier()));
            menu.menu.AppendAction("Event Card Set", new Action<DropdownMenuAction>(x => AddEventDataModifier()));

            titleButtonContainer.Add(menu);
        }

        public void AddModifier(EventModifier eventModifier = null)
        {
            AddEventModifier(eventData.Modifiers, eventModifier);
        }

        public void AddEventDataModifier(EventCardStat modifier = null)
        {
            EventCardStat newModifier = new EventCardStat();

            if (modifier != null)
            {
                newModifier.CardEvent = modifier.CardEvent;
            }
            eventData.EventCardSet.Add(newModifier);

            Box boxContainer = new Box();
            boxContainer.AddToClassList("EventBox");

            ObjectField objectField = GetNewCardSetField(newModifier, "EventObject");

            Button btn = GetNewButton("X", "DeleteButton");
            btn.clicked += () => {
                eventData.EventCardSet.Remove(newModifier);
                DeleteBox(boxContainer);
            };

            boxContainer.Add(objectField);
            boxContainer.Add(btn);

            mainContainer.Add(boxContainer);
            RefreshExpandedState();
        }

    }
}
