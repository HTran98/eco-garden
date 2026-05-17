using EcoGarden.Board;
using UnityEngine;

namespace EcoGarden.Level
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private BoardController boardController;

        private void Reset()
        {
            boardController = FindAnyObjectByType<BoardController>();
        }

        private void Start()
        {
            if (boardController == null)
            {
                boardController = FindAnyObjectByType<BoardController>();
            }

            if (boardController != null && boardController.BoardState == null)
            {
                boardController.LoadLevel();
            }
        }
    }
}
