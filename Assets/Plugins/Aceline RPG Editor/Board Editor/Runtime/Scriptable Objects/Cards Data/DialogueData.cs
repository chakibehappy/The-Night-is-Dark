using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
#endif

namespace Aceline.RPG
{
    [System.Serializable]
    public class DialogueData : BaseData
    {
        public List<StatContainer> Containers { get; set; } = new List<StatContainer>();
        public List<StatName> Names = new List<StatName>();
        public List<StatDialogueText> Dialogues = new List<StatDialogueText>();
        public List<StatProfilePic> Images = new List<StatProfilePic>();
        public List<StatPort> Ports = new List<StatPort>();
    }

    [System.Serializable]
    public class StatContainer
    {
        public IntStat ID = new IntStat();
    }

    [System.Serializable]
    public class StatName : StatContainer
    {
        public TextStat CharacterName = new TextStat();
    }

    [System.Serializable]
    public class StatDialogueText : StatContainer
    {
#if UNITY_EDITOR
        public TextField TextField { get; set; }
        public ObjectField ObjectField { get; set; }
#endif
        public TextStat GuidID = new TextStat();
        public List<Languages<string>> Texts = new List<Languages<string>>();
        public List<Languages<AudioClip>> AudioClips = new List<Languages<AudioClip>>();
    }

    [System.Serializable]
    public class StatProfilePic : StatContainer
    {
        public ImageStat LeftSprite = new ImageStat();
        public ImageStat RightSprite = new ImageStat();
    }

    [System.Serializable]
    public class StatPort : StatContainer
    {
        public string PortGuid;
        public string InputGuid;
        public string OutputGuid;
    }
}
