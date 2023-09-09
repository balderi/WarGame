namespace WarGame.Models.GameObjects
{
    public class GameObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PositionX { get; set; }
        public int PositionY { get; set; }
    }
}
