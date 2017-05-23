using System.Drawing;

namespace qwerty.Objects
{
    class Meteor : SpaceObject
    {
        public int explodeDmg;
        public string staticDescription = "Moving meteor";
        public int xdirection;
        public int ydirection;
        public Meteor(int box, int health, int dmg, int x, int y)
        {
            boxId = box;
            objectType = ObjectType.Meteor;
            player = 0;
            maxHealth = health;
            currentHealth = maxHealth;
            explodeDmg = dmg;
            xdirection = x;
            ydirection = y;
            
        }

        public override void drawSpaceShit(ref CombatMap cMap, ref System.Drawing.Bitmap bmap)
        {
            Graphics g = Graphics.FromImage(bmap);
            SolidBrush grayBrush = new SolidBrush(Color.Gray);
            g.FillEllipse(grayBrush, cMap.Cells[boxId].xpoint1 + 17, cMap.Cells[boxId].ypoint1 - 12, 25, 25);
            g.DrawString(currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, new PointF(cMap.Cells[boxId].xpoint1 + 20, cMap.Cells[boxId].ypoint1 - 25));
        }

        public void move(CombatMap cMap)
        {
                int newx;
                int newy;
                int pointB;

                newx = cMap.Cells[boxId].x + xdirection;
                newy = cMap.Cells[boxId].y + ydirection;

                if (newx < 0 || newx > cMap.FieldWidth - 1
                    || newy < 0 || newy > cMap.FieldHeight * 2 - 1)
                {
                    cMap.Cells[boxId].spaceObject.player = -1;
                    cMap.Cells[boxId].spaceObject = null;
                    boxId = -1;
                }
                else
                {
                    pointB = cMap.GetCellByCellCoordinates(newx, newy).id;

                    if (cMap.Cells[pointB].spaceObject == null)
                    {
                        cMap.Cells[boxId].spaceObject = null;
                        cMap.Cells[pointB].spaceObject = this;
                        boxId = cMap.Cells[pointB].id;
                    }
                    else
                    {
                        cMap.Cells[pointB].spaceObject.currentHealth -= explodeDmg;
                        if (cMap.Cells[pointB].spaceObject.currentHealth <= 0)
                        {
                            cMap.Cells[pointB].spaceObject.player = -1;
                            cMap.Cells[pointB].spaceObject.boxId = -1;
                            cMap.Cells[pointB].spaceObject = null;
                        }
                        cMap.Cells[boxId].spaceObject.player = -1;
                        cMap.Cells[boxId].spaceObject = null;
                        boxId = -1;
                    }
                    
                }
        }
        public override string Description
        {
            get
            {
                string x = "";
                string y = "";

                switch (xdirection)
                {
                    case -1:
                        x = "left ";
                        break;
                    case 1:
                        x = "right ";
                        break;
                }
                switch (ydirection)
                {
                    case -1:
                        y = "top ";
                        break;
                    case 1:
                        y = "bottom ";
                        break;
                }

                return staticDescription + "\nУрон при попадании\n в корабль: " + explodeDmg
                       + "\nhp - " + currentHealth
                       + "\nНаправление: \n" + x + y;
            }
        }
    }
}
