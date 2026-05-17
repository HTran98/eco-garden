using EcoGarden.Items;

namespace EcoGarden.Board
{
    public sealed class BoardCell
    {
        public GridPosition Position { get; }
        public CellKind Kind { get; set; }
        public ObstacleKind ObstacleKind { get; set; }
        public BoardItem Item { get; set; }
        public ProducerRuntime Producer { get; set; }

        public BoardCell(GridPosition position, CellKind kind)
        {
            Position = position;
            Kind = kind;
            ObstacleKind = ObstacleKind.None;
        }

        public bool IsPlayable
        {
            get { return Kind == CellKind.Empty || Kind == CellKind.Producer || Kind == CellKind.NpcOrderPoint; }
        }

        public bool CanReceiveItem
        {
            get { return (Kind == CellKind.Empty || Kind == CellKind.NpcOrderPoint) && Item == null; }
        }
    }
}
