using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace qwerty
{
    abstract class SpaceShit
    {
        public int objectType;
        public int player; // 0,1,2 ..0 - нейтральные объекты 
        public int boxId; // ячейка, в которой находится
        public int maxHealth; // hit points
        public int currentHealth;
        public abstract string description(); // описание объекта

        public List<int> xpoints = new List<int>();
        public List<int> ypoints = new List<int>();

        public int maxActions; // максимальное количество действий на одном ходу
        public int actionsLeft; // оставшееся количество действий
    }
}
