using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Aceline.RPG.Test
{
    public class GetGameBoardData : MonoBehaviour
    {
        public GameBoard gameBoard;

        protected BaseData GetCardByGuid(string targetGuid)
        {
            return gameBoard.AllDatas.Find(card => card.CardGuid == targetGuid);
        }

        protected BaseData GetCardByCardPort(StatPort targetPort)
        {
            return gameBoard.AllDatas.Find(card => card.CardGuid == targetPort.InputGuid);
        }

        protected BaseData GetNextCard(BaseData cardData)
        {
            CardLinkData cardLinkData = gameBoard.CardLinkDatas.Find(edge => edge.BaseCardGuid == cardData.CardGuid);

            return GetCardByGuid(cardLinkData.TargetCardGuid);
        }
    }

}