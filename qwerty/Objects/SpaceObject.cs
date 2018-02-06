using System.Collections.Generic;
using System.Drawing;
using Barbar.HexGrid;

namespace qwerty.Objects
{
    public enum ObjectType
    {
        Ship, Meteor
    }

    public abstract class SpaceObject
    {
        public ObjectType objectType; // подробности смотри в константах
        public Player Owner; // 0,1,2 ..0 - нейтральные объекты 
        public OffsetCoordinates ObjectCoordinates;
        public int maxHealth; // hit points
        public int currentHealth;

        public abstract string Description { get; }
        public bool IsMoving { get; set; }

        public List<PointF> PolygonPoints;                                            

        public int maxActions; // максимальное количество действий на одном ходу
        public int actionsLeft; // оставшееся количество действий

        public abstract void Rotate(double angle);
    }
}
