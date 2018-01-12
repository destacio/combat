using Hex = Barbar.HexGrid;

namespace qwerty.Objects
{
    class Meteor : SpaceObject
    {
        public int explodeDmg;
        public string staticDescription = "Moving meteor";
        public int xdirection;
        public int ydirection;
		public readonly HexagonNeighborDirection MovementDirection;

		public Meteor(Hex.OffsetCoordinates meteorCoordinates, int health, int dmg, HexagonNeighborDirection movementDirection)
        {
			this.MovementDirection = movementDirection;
			this.ObjectCoordinates = meteorCoordinates;
			objectType = ObjectType.Meteor;
            player = 0;
            maxHealth = health;
            currentHealth = maxHealth;
            explodeDmg = dmg;
        }

        public override string Description => staticDescription + "\nУрон при попадании\n в корабль: " + explodeDmg
                                              + "\nhp - " + currentHealth
                                              + "\nНаправление: \n" + MovementDirection;
    }
}
