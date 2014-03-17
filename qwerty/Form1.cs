using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace qwerty
{
    public partial class Form1 : Form
    {
        public Bitmap combatBitmap;

        combatMap cMap = new combatMap(20, 10);  // создаем поле боя с указанной размерностью
        int select = -1; // служебная переменная, пока сам не знаю на кой хер она мне, но пусть будет. да.
        int activePlayer = 1; // ход 1-ого или 2-ого игрока
        Ship activeShip = null; // выделенное судно
        List<Ship> allShips = new List<Ship>();
        public Form1()
        {

            Ship penumbra = new Ship(Constants.SCOUT, 1);
            allShips.Add(penumbra);
            Ship holycow = new Ship(Constants.SCOUT, 1);
            allShips.Add(holycow);
            Ship leroy = new Ship(Constants.ASSAULTER, 1);
            allShips.Add(leroy);


            Ship pandorum = new Ship(Constants.SCOUT, 2);
            allShips.Add(pandorum);
            Ship exodar = new Ship(Constants.SCOUT, 2);
            allShips.Add(exodar);
            Ship neveria = new Ship(Constants.ASSAULTER, 2);
            allShips.Add(neveria);
          

            for (int count = 0; count < allShips.Count; count++ )
            {
                allShips[count].placeShip(ref cMap);
            }

            InitializeComponent();
            Draw();

    
        }

        public void Draw()//рисование карты без выделений
        {
            //labelDamage.Text = "";
            combatBitmap = new Bitmap(pictureMap.Width, pictureMap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(combatBitmap);
            g.FillRectangle(Brushes.Black, 0, 0, combatBitmap.Width, combatBitmap.Height);//рисуем фон окна

            Pen redPen = new Pen(Color.Red, 1);
            Pen PurplePen = new Pen(Color.Purple);

            SolidBrush redBrush = new SolidBrush(Color.Red);
            SolidBrush blueBrush = new SolidBrush(Color.Blue);
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush grayBrush = new SolidBrush(Color.Gray);
            SolidBrush mediumPurpleBrush = new SolidBrush(Color.MediumPurple);
            SolidBrush brush;

            

            for (int i = 0; i < cMap.boxes.Count; i++)
            {
                Point[] myPointArrayHex = {  //точки для рисования шестиугольника
                        new Point(cMap.boxes[i].xpoint1, cMap.boxes[i].ypoint1),
                        new Point(cMap.boxes[i].xpoint2, cMap.boxes[i].ypoint2),
                        new Point(cMap.boxes[i].xpoint3, cMap.boxes[i].ypoint3),
                        new Point(cMap.boxes[i].xpoint4, cMap.boxes[i].ypoint4),
                        new Point(cMap.boxes[i].xpoint5, cMap.boxes[i].ypoint5),
                        new Point(cMap.boxes[i].xpoint6, cMap.boxes[i].ypoint6)
            };
                // Если выделили судно с очками передвижения, подсвечиваем его и соседние клетки

                if (activeShip != null && activeShip.boxId == i)
                {
                    g.FillPolygon(mediumPurpleBrush, myPointArrayHex);

                    int k;
                    if (i % cMap.height != cMap.height - 1 && cMap.boxes[i + 1].spaceObject == null)
                    {
                        Point[] myPointArrayHex2 = {  //точки для рисования шестиугольника
                            new Point(cMap.boxes[i+1].xpoint1, cMap.boxes[i+1].ypoint1),
                            new Point(cMap.boxes[i+1].xpoint2, cMap.boxes[i+1].ypoint2),
                            new Point(cMap.boxes[i+1].xpoint3, cMap.boxes[i+1].ypoint3),
                            new Point(cMap.boxes[i+1].xpoint4, cMap.boxes[i+1].ypoint4),
                            new Point(cMap.boxes[i+1].xpoint5, cMap.boxes[i+1].ypoint5),
                            new Point(cMap.boxes[i+1].xpoint6, cMap.boxes[i+1].ypoint6)
                        };
                        g.FillPolygon(mediumPurpleBrush, myPointArrayHex2);
                    }

                    if (i % cMap.height != 0 && cMap.boxes[i - 1].spaceObject == null)
                    {
                        Point[] myPointArrayHex3 = {  //точки для рисования шестиугольника
                            new Point(cMap.boxes[i-1].xpoint1, cMap.boxes[i-1].ypoint1),
                            new Point(cMap.boxes[i-1].xpoint2, cMap.boxes[i-1].ypoint2),
                            new Point(cMap.boxes[i-1].xpoint3, cMap.boxes[i-1].ypoint3),
                            new Point(cMap.boxes[i-1].xpoint4, cMap.boxes[i-1].ypoint4),
                            new Point(cMap.boxes[i-1].xpoint5, cMap.boxes[i-1].ypoint5),
                            new Point(cMap.boxes[i-1].xpoint6, cMap.boxes[i-1].ypoint6)
                         };
                        g.FillPolygon(mediumPurpleBrush, myPointArrayHex3);
                        g.DrawPolygon(PurplePen, myPointArrayHex3);
                    }
                    if (i >= cMap.height)
                    {
                        if (cMap.boxes[i - cMap.height].spaceObject == null)
                        {
                            Point[] myPointArrayHex4 = {  //точки для рисования шестиугольника
                                new Point(cMap.boxes[i-cMap.height].xpoint1, cMap.boxes[i-cMap.height].ypoint1),
                                new Point(cMap.boxes[i-cMap.height].xpoint2, cMap.boxes[i-cMap.height].ypoint2),
                                new Point(cMap.boxes[i-cMap.height].xpoint3, cMap.boxes[i-cMap.height].ypoint3),
                                new Point(cMap.boxes[i-cMap.height].xpoint4, cMap.boxes[i-cMap.height].ypoint4),
                                new Point(cMap.boxes[i-cMap.height].xpoint5, cMap.boxes[i-cMap.height].ypoint5),
                                new Point(cMap.boxes[i-cMap.height].xpoint6, cMap.boxes[i-cMap.height].ypoint6)
                             };
                            g.FillPolygon(mediumPurpleBrush, myPointArrayHex4);
                            g.DrawPolygon(PurplePen, myPointArrayHex4);
                        }
                        if (((i - i % cMap.height) / cMap.height) % 2 == 0) k = -1;
                        else k = 1;

                        if (!((k == 1) && (i % cMap.height == cMap.height - 1)) && !((k == -1) && (i % cMap.height == 0)) && cMap.boxes[i - cMap.height + k].spaceObject == null)
                        {
                            Point[] myPointArrayHex5 = {  //точки для рисования шестиугольника
                                new Point(cMap.boxes[i-cMap.height+k].xpoint1, cMap.boxes[i-cMap.height+k].ypoint1),
                                new Point(cMap.boxes[i-cMap.height+k].xpoint2, cMap.boxes[i-cMap.height+k].ypoint2),
                                new Point(cMap.boxes[i-cMap.height+k].xpoint3, cMap.boxes[i-cMap.height+k].ypoint3),
                                new Point(cMap.boxes[i-cMap.height+k].xpoint4, cMap.boxes[i-cMap.height+k].ypoint4),
                                new Point(cMap.boxes[i-cMap.height+k].xpoint5, cMap.boxes[i-cMap.height+k].ypoint5),
                                new Point(cMap.boxes[i-cMap.height+k].xpoint6, cMap.boxes[i-cMap.height+k].ypoint6)
                            };
                            g.FillPolygon(mediumPurpleBrush, myPointArrayHex5);
                            g.DrawPolygon(PurplePen, myPointArrayHex5);
                        }
                    }

                    if (i < cMap.boxes.Count - cMap.height )
                    {
                        if (cMap.boxes[i + cMap.height].spaceObject == null)
                        {
                            Point[] myPointArrayHex6 = {  //точки для рисования шестиугольника
                                new Point(cMap.boxes[i+cMap.height].xpoint1, cMap.boxes[i+cMap.height].ypoint1),
                                new Point(cMap.boxes[i+cMap.height].xpoint2, cMap.boxes[i+cMap.height].ypoint2),
                                new Point(cMap.boxes[i+cMap.height].xpoint3, cMap.boxes[i+cMap.height].ypoint3),
                                new Point(cMap.boxes[i+cMap.height].xpoint4, cMap.boxes[i+cMap.height].ypoint4),
                                new Point(cMap.boxes[i+cMap.height].xpoint5, cMap.boxes[i+cMap.height].ypoint5),
                                new Point(cMap.boxes[i+cMap.height].xpoint6, cMap.boxes[i+cMap.height].ypoint6)
                            };
                            g.FillPolygon(mediumPurpleBrush, myPointArrayHex6);
                            g.DrawPolygon(PurplePen, myPointArrayHex6);
                        }
                    
                        if(((i - i % cMap.height)/cMap.height)%2 == 0) k = -1;
                        else k = 1;
                        if (i % cMap.height != 0 && !((k == 1) && (i % cMap.height == cMap.height - 1)) && cMap.boxes[i + cMap.height + k].spaceObject == null)
                        {
                                Point[] myPointArrayHex7 = {  //точки для рисования шестиугольника
                                new Point(cMap.boxes[i+cMap.height+k].xpoint1, cMap.boxes[i+cMap.height+k].ypoint1),
                                new Point(cMap.boxes[i+cMap.height+k].xpoint2, cMap.boxes[i+cMap.height+k].ypoint2),
                                new Point(cMap.boxes[i+cMap.height+k].xpoint3, cMap.boxes[i+cMap.height+k].ypoint3),
                                new Point(cMap.boxes[i+cMap.height+k].xpoint4, cMap.boxes[i+cMap.height+k].ypoint4),
                                new Point(cMap.boxes[i+cMap.height+k].xpoint5, cMap.boxes[i+cMap.height+k].ypoint5),
                                new Point(cMap.boxes[i+cMap.height+k].xpoint6, cMap.boxes[i+cMap.height+k].ypoint6)
                            };
                            g.FillPolygon(mediumPurpleBrush, myPointArrayHex7);
                            g.DrawPolygon(PurplePen, myPointArrayHex7);
                        }
                    }
                }
                g.DrawPolygon(PurplePen, myPointArrayHex);//рисование шестиугольника

                if (cMap.boxes[i].spaceObject != null)
                {
                    if (cMap.boxes[i].spaceObject.player == 1)
                        brush = blueBrush;
                    else if (cMap.boxes[i].spaceObject.player == 2)
                        brush = redBrush;
                    else brush = grayBrush;
                    
                    Point[] myPointArray = {
                     
                        new Point(cMap.boxes[i].xpoint2 + cMap.boxes[i].spaceObject.xpoints[0], cMap.boxes[i].ypoint2 + cMap.boxes[i].spaceObject.ypoints[0]),
                        new Point(cMap.boxes[i].xpoint2 + cMap.boxes[i].spaceObject.xpoints[1], cMap.boxes[i].ypoint2 + cMap.boxes[i].spaceObject.ypoints[1]),
                        new Point(cMap.boxes[i].xpoint2 + cMap.boxes[i].spaceObject.xpoints[2], cMap.boxes[i].ypoint2 + cMap.boxes[i].spaceObject.ypoints[2]),
                        new Point(cMap.boxes[i].xpoint2 + cMap.boxes[i].spaceObject.xpoints[3], cMap.boxes[i].ypoint2 + cMap.boxes[i].spaceObject.ypoints[3]),
                        new Point(cMap.boxes[i].xpoint2 + cMap.boxes[i].spaceObject.xpoints[4], cMap.boxes[i].ypoint2 + cMap.boxes[i].spaceObject.ypoints[4])
                
                    }; 
                    g.FillPolygon(brush, myPointArray);
                    g.DrawString(cMap.boxes[i].spaceObject.currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, new PointF(cMap.boxes[i].xpoint1 + 20, cMap.boxes[i].ypoint1 - 25));
                    g.DrawString(cMap.boxes[i].spaceObject.actionsLeft.ToString(), new Font("Arial", 8.0F), Brushes.Blue, new PointF(cMap.boxes[i].xpoint1 + 25, cMap.boxes[i].ypoint1 + 15));
                    
                }

                g.DrawString(i.ToString(), new Font("Arial", 8.0F), Brushes.Green, new PointF(cMap.boxes[i].xpoint1 + 10, cMap.boxes[i].ypoint1 + 10));
                
            }
            pictureMap.Image = combatBitmap;
            pictureMap.Refresh();

        }

        private void pictureMap_MouseClick(object sender, MouseEventArgs e)
        {

            for (int i = 0; i < cMap.boxes.Count; i++)
            {

                if ((e.X > cMap.boxes[i].xpoint2) &&
                    (e.X < cMap.boxes[i].xpoint3) &&
                    (e.Y > cMap.boxes[i].ypoint2) &&
                    (e.Y < cMap.boxes[i].ypoint6))
                {
                    select = i;
                   
                    if(cMap.boxes[i].spaceObject != null)
                    {
                        if(cMap.boxes[i].spaceObject.objectType == Constants.SHIP)
                        {
                            if(activePlayer == cMap.boxes[i].spaceObject.player)
                            {
                                
                                boxDescription.Text = cMap.boxes[i].spaceObject.description();
                                activeShip = (Ship)cMap.boxes[i].spaceObject;

                                

                                Draw();

                                break;
                               
                            }
                            else
                            {
                                //select = -1;

                                

                                Draw();
                                boxDescription.Text = cMap.boxes[i].spaceObject.description();
                            }
                        }
                       
                    }
                    
                    
                }

                // Если до этого ткнули по дружественному судну

                else if (activeShip != null)
                {
                    int a = activeShip.boxId;
                    if (a + 1 == select && a % cMap.height != cMap.height - 1)
                    {

                        activeShip.moveShip(ref cMap, a, select);
                        boxDescription.Text = activeShip.description();

                        if (activeShip.actionsLeft == 0) activeShip = null;
                        Draw();

                        break;
                    }
                    else if (a - 1 == select && a % cMap.height != 0)
                    {
                        activeShip.moveShip(ref cMap, a, select);
                        boxDescription.Text = activeShip.description();

                        if (activeShip.actionsLeft == 0) activeShip = null;
                        Draw();

                        break;
                    }
                }
            }
        }

        private void btnEndTurn_Click(object sender, EventArgs e)
        {
            if (activePlayer == 1) activePlayer = 2;
            else activePlayer = 1;

            lblTurn.Text = "Ходит " + activePlayer + "-й игрок";

            activeShip = null;

            for (int count = 0; count < allShips.Count; count++)
            {
                allShips[count].refill();
            }

            Draw();
        }
    }
}
