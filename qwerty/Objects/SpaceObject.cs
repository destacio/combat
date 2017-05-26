using System.Collections.Generic;
using System.Drawing;

namespace qwerty.Objects
{
    public enum ObjectType
    {
        Ship, Meteor
    }

    abstract class SpaceObject
    {
        public ObjectType objectType; // подробности смотри в константах
        public int player; // 0,1,2 ..0 - нейтральные объекты 
        public int boxId; // ячейка, в которой находится
        public int maxHealth; // hit points
        public int currentHealth;

        public abstract string Description { get; }

        public abstract void drawSpaceShit(ref CombatMap cMap, ref System.Drawing.Bitmap bmap);

        //public List<int> xpoints = new List<int>();  // координаты 5 точек, описывающих внешний вид объекта
        //public List<int> ypoints = new List<int>();  // вообще, по сути надо каждый типа корабля сделать классом, тогда 
                                                     // можно будет внешний вид задавать прямо здесь, массивом Point[]
        public List<PointF> PolygonPoints;                                            

        public int maxActions; // максимальное количество действий на одном ходу
        public int actionsLeft; // оставшееся количество действий
    }
}
