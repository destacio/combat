using System.Collections.Generic;
using System.Drawing;
using Barbar.HexGrid;

namespace qwerty.Objects
{
    public enum ObjectType
    {
        Ship, Meteor
    }

    abstract class SpaceObject
    {
        public ObjectType objectType; // подробности смотри в константах
        public Player player; // 0,1,2 ..0 - нейтральные объекты 
        public int boxId; // ячейка, в которой находится
        public CubeCoordinates ObjectCoordinates;
        public int maxHealth; // hit points
        public int currentHealth;

        public abstract string Description { get; }

        public abstract void drawSpaceShit(ref CombatMap cMap, ref System.Drawing.Bitmap bmap);

        public List<PointF> PolygonPoints;                                            

        public int maxActions; // максимальное количество действий на одном ходу
        public int actionsLeft; // оставшееся количество действий
    }
}
