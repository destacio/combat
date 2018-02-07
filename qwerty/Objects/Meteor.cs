using Hex = Barbar.HexGrid;

namespace qwerty.Objects
{
    class Meteor : SpaceObject
    {
        public int explodeDmg;
        public string staticDescription = "Moving meteor";
        public readonly HexagonNeighborDirection MovementDirection;

        public Meteor(Hex.OffsetCoordinates meteorCoordinates, int health, int dmg, HexagonNeighborDirection movementDirection): base(Player.None, health, ObjectType.Meteor, 1)
        {
            this.MovementDirection = movementDirection;
            this.ObjectCoordinates = meteorCoordinates;
            this.currentHealth = this.MaxHealth;
            this.explodeDmg = dmg;
        }

        public override string Description => this.staticDescription + "\nУрон при попадании\n в корабль: " + this.explodeDmg
                                              + "\nhp - " + this.currentHealth
                                              + "\nНаправление: \n" + this.MovementDirection;

        public override void Rotate(double angle)
        {
            throw new System.NotImplementedException();
        }
    }
}
