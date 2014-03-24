using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Media;

namespace qwerty
{
    public partial class Form1 : Form
    {
        public Bitmap combatBitmap;

        combatMap cMap = new combatMap(8, 6);  // создаем поле боя с указанной размерностью
        ObjectManager objectManager = new ObjectManager();
        int select = -1; // служебная переменная, пока сам не знаю на кой хер она мне, но пусть будет. да.
        int activePlayer = 1; // ход 1-ого или 2-ого игрока
        Ship activeShip = null; // выделенное судно
        List<Ship> allShips = new List<Ship>();
        List<Meteor> meteors = new List<Meteor>();
        int blueShipsCount;
        int redShipsCount;
        System.Media.SoundPlayer player = new System.Media.SoundPlayer();
        

        
        public Form1()
        {
            player.SoundLocation = @"../../Sounds/laser1.wav";

            Ship penumbra = shipCreate(Constants.SCOUT, 1, Constants.LIGHT_LASER);
            allShips.Add(penumbra);
            Ship holycow = shipCreate(Constants.SCOUT, 1, Constants.LIGHT_LASER);
            allShips.Add(holycow);
            Ship leroy = shipCreate(Constants.ASSAULTER, 1, Constants.HEAVY_LASER);
            allShips.Add(leroy);


            Ship pandorum = shipCreate(Constants.SCOUT, 2, Constants.LIGHT_LASER);
            allShips.Add(pandorum);
            Ship exodar = shipCreate(Constants.SCOUT, 2, Constants.LIGHT_LASER);
            allShips.Add(exodar);
            Ship neveria = shipCreate(Constants.ASSAULTER, 2, Constants.HEAVY_LASER);
            allShips.Add(neveria);    

            objectManager.meteorCreate(cMap);

            // расставляем корабли по полю, синие - слева, красные - справа
            for (int count = 0; count < allShips.Count; count++ )
            {
                allShips[count].placeShip(ref cMap);
            }

            InitializeComponent();
            Draw();

            shipsCount();
    
        }

        Ship shipCreate(int type, int p, int wpn)
        {
            Weapon weapon = null;
            switch(wpn)
            {
                case Constants.LIGHT_LASER:
                    weapon = new wpnLightLaser();
                    break;
                case Constants.HEAVY_LASER:
                    weapon = new WpnHeavyLaser();
                    break;
            }
            Ship newShip = null;
            switch (type)
            {
                case Constants.SCOUT:
                    newShip = new ShipScout(p, weapon);
                    break;
                case Constants.ASSAULTER:
                    newShip = new ShipAssaulter(p, weapon);
                    break;
            }
            return newShip;
        } 

        public void shipsCount()
        {
            blueShipsCount = 0;
            redShipsCount = 0;
            for (int count = 0; count < allShips.Count; count++)
            {

                if (allShips[count].player == 1)
                    blueShipsCount++;
                else if (allShips[count].player == 2)
                    redShipsCount++;
            }
            txtBlueShips.Text = "" + blueShipsCount;
            txtRedShips.Text = "" + redShipsCount;
        }

        public void drawLaser(int x1, int y1, int x2, int y2)
        {
            // закоменченные строки делают отрисовку с восстановлением фона. Расписал по шагам:

            Graphics g = Graphics.FromImage(combatBitmap);
            //Rectangle rect;  //  --- размер изображения
            //Bitmap oldImage;  //  --- переменная, в которую его засунем

            Pen laserPen1 = new Pen(Color.Orange, 2);

            player.Play();
            for (int i = 0; i < 5; i++)
            {
                // --- 1) находим размер изображения
                //rect = new Rectangle(0, 0, combatBitmap.Width, combatBitmap.Height); 
                // --- 2) клонируем наш битмап
                //oldImage = combatBitmap.Clone(rect, combatBitmap.PixelFormat);
             
                g.DrawLine(laserPen1, new Point(x1, y1), new Point(x2 + i, y2));

                

                pictureMap.Image = combatBitmap;
                pictureMap.Refresh();

                // --- 3) отрисовываем тот битмам, который сохранили выше
                //g.DrawImage(oldImage, 0, 0);
                

                Thread.Sleep(35);
            }

        }
        public void Draw()
        {
            combatBitmap = new Bitmap(pictureMap.Width, pictureMap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(combatBitmap);
            g.FillRectangle(Brushes.Black, 0, 0, combatBitmap.Width, combatBitmap.Height); //рисуем фон окна

            Pen generalPen;
            Pen redPen = new Pen(Color.Red, 3);
            Pen grayPen = new Pen(Color.Gray, 3);
            Pen PurplePen = new Pen(Color.Purple);
            Pen activeShipAriaPen = new Pen(Color.Purple, 5);

            SolidBrush redBrush = new SolidBrush(Color.Red);
            SolidBrush blueBrush = new SolidBrush(Color.Blue);
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush grayBrush = new SolidBrush(Color.Gray);
            SolidBrush activeShipBrush = new SolidBrush(Color.DarkGreen);
            SolidBrush mediumPurpleBrush = new SolidBrush(Color.MediumPurple);
            SolidBrush brush;



            for (int i = 0; i < cMap.boxes.Count; i++)
            {
                generalPen = PurplePen;
                Point[] myPointArrayHex = {  //точки для отрисовки шестиугольника
                        new Point(cMap.boxes[i].xpoint1, cMap.boxes[i].ypoint1),
                        new Point(cMap.boxes[i].xpoint2, cMap.boxes[i].ypoint2),
                        new Point(cMap.boxes[i].xpoint3, cMap.boxes[i].ypoint3),
                        new Point(cMap.boxes[i].xpoint4, cMap.boxes[i].ypoint4),
                        new Point(cMap.boxes[i].xpoint5, cMap.boxes[i].ypoint5),
                        new Point(cMap.boxes[i].xpoint6, cMap.boxes[i].ypoint6)
                };
                // Если выделили судно с очками передвижения, подсвечиваем его и соседние клетки

                if (activeShip != null)
                {
                    for (int count = 0; count < allShips.Count; count++)
                    {
                        if (allShips[count].player != activePlayer && allShips[count].boxId >= 0)
                        {
                            if (allShips[count].player == 0)
                            {
                                // рисовать ли рамку вокруг нейтральных объектов
                                // в зоне досягаемости? пока решили что нет,
                                // но заглушка осталась
                                int x1;
                            }
                            else
                            {
                                double x1 = cMap.boxes[activeShip.boxId].x;
                                double y1 = cMap.boxes[activeShip.boxId].y;
                                double x2 = cMap.boxes[allShips[count].boxId].x;
                                double y2 = cMap.boxes[allShips[count].boxId].y;
                                double range;
                                range = Math.Sqrt((x2 - x1) * (x2 - x1) + ((y2 - y1) * (y2 - y1)) * 0.35);
                                //range = Math.Max(Math.Abs(x2-x1), Math.Ceiling( Math.Abs(y2-y1)/2+1));
                                if ((int)range <= activeShip.equippedWeapon.attackRange)
                                {
                                    Point[] myPointArrayHex99 = {  //точки для отрисовки шестиугольника
                                        new Point(cMap.boxes[allShips[count].boxId].xpoint1, cMap.boxes[allShips[count].boxId].ypoint1),
                                        new Point(cMap.boxes[allShips[count].boxId].xpoint2, cMap.boxes[allShips[count].boxId].ypoint2),
                                        new Point(cMap.boxes[allShips[count].boxId].xpoint3, cMap.boxes[allShips[count].boxId].ypoint3),
                                        new Point(cMap.boxes[allShips[count].boxId].xpoint4, cMap.boxes[allShips[count].boxId].ypoint4),
                                        new Point(cMap.boxes[allShips[count].boxId].xpoint5, cMap.boxes[allShips[count].boxId].ypoint5),
                                        new Point(cMap.boxes[allShips[count].boxId].xpoint6, cMap.boxes[allShips[count].boxId].ypoint6)
                                     };
                                    g.DrawPolygon(redPen, myPointArrayHex99);
                                }
                            }
                        }
                    }
                }

                g.DrawPolygon(PurplePen, myPointArrayHex);

                if (activeShip != null && activeShip.boxId == i)
                {
                    g.DrawPolygon(activeShipAriaPen, myPointArrayHex);

                }

                if (cMap.boxes[i].spaceObject != null)
                {
                    if (cMap.boxes[i].spaceObject.player == 1)
                        brush = blueBrush;
                    else if (cMap.boxes[i].spaceObject.player == 2)
                        brush = redBrush;
                    else brush = grayBrush;

                    cMap.boxes[i].spaceObject.drawSpaceShit(ref cMap, ref combatBitmap);
                    /*
                    Point[] myPointArray = {
                     
                        new Point(cMap.boxes[i].xcenter + cMap.boxes[i].spaceObject.xpoints[0], cMap.boxes[i].ycenter + cMap.boxes[i].spaceObject.ypoints[0]),
                        new Point(cMap.boxes[i].xcenter + cMap.boxes[i].spaceObject.xpoints[1], cMap.boxes[i].ycenter + cMap.boxes[i].spaceObject.ypoints[1]),
                        new Point(cMap.boxes[i].xcenter + cMap.boxes[i].spaceObject.xpoints[2], cMap.boxes[i].ycenter + cMap.boxes[i].spaceObject.ypoints[2]),
                        new Point(cMap.boxes[i].xcenter + cMap.boxes[i].spaceObject.xpoints[3], cMap.boxes[i].ycenter + cMap.boxes[i].spaceObject.ypoints[3]),
                        new Point(cMap.boxes[i].xcenter + cMap.boxes[i].spaceObject.xpoints[4], cMap.boxes[i].ycenter + cMap.boxes[i].spaceObject.ypoints[4])
                
                    }; 
                    g.FillPolygon(brush, myPointArray);
                    g.DrawString(cMap.boxes[i].spaceObject.actionsLeft.ToString(), new Font("Arial", 8.0F), Brushes.Blue, new PointF(cMap.boxes[i].xpoint1 + 25, cMap.boxes[i].ypoint1 + 15));
                    */


                }
                /*else if (cMap.boxes[i].spaceObject != null && cMap.boxes[i].spaceObject.objectType == Constants.METEOR)
                {
                    if (cMap.boxes[i].spaceObject.boxId != -1)
                    {
                        g.FillEllipse(grayBrush, cMap.boxes[i].xpoint1 + 17, cMap.boxes[i].ypoint1 - 12, 25, 25);
                    }
                } */

                //g.DrawString(cMap.boxes[i].id.ToString(), new Font("Arial", 8.0F), Brushes.Green, new PointF(cMap.boxes[i].xpoint1 + 20, cMap.boxes[i].ypoint1 + 10));

                g.DrawString(cMap.boxes[i].x.ToString(), new Font("Arial", 8.0F), Brushes.Green, new PointF(cMap.boxes[i].xpoint1 + 10, cMap.boxes[i].ypoint1 + 10));
                g.DrawString(cMap.boxes[i].y.ToString(), new Font("Arial", 8.0F), Brushes.Green, new PointF(cMap.boxes[i].xpoint1 + 40, cMap.boxes[i].ypoint1 + 10));
                /*if(cMap.boxes[i].spaceObject != null && cMap.boxes[i].spaceObject.boxId != -1)
                    g.DrawString(cMap.boxes[i].spaceObject.currentHealth.ToString(), new Font("Arial", 8.0F), Brushes.Red, new PointF(cMap.boxes[i].xpoint1 + 20, cMap.boxes[i].ypoint1 - 25));
            }*/
            }
                pictureMap.Image = combatBitmap;
                pictureMap.Refresh();
            
        }

        public double attackAngleSearch(double targetx, double targety)
        {
            double angle = 0;
            double shipx, shipy;

            if (activeShip != null)
            {
                shipx = cMap.boxes[activeShip.boxId].x;  // координаты выделенного корабля
                shipy = cMap.boxes[activeShip.boxId].y;

                if (shipx == targetx) // избегаем деления на ноль
                {
                    if (shipy > targety)
                    {
                        angle = -90;
                    }
                    else
                    {
                        angle = 90;
                    }
                    if (activePlayer == 2) angle = -angle;

                }
                else // находим угол, на который нужно повернуть корабль (если он не равен 90 градусов)
                {
                    angle = Math.Atan((targety - shipy) / (targetx - shipx)) * 180 / Math.PI;
                }
                // дальше идет коррекция, не пытайся разобраться как это работает, просто оставь как есть
                if (activePlayer == 1)
                {
                    if (shipy == targety && shipx > targetx)
                    {
                        angle = 180;
                    }
                    else if (shipx > targetx && shipy < targety)
                    {
                        angle += 180;
                    }
                    else if (shipx > targetx && shipy > targety)
                    {
                        angle = angle - 180;
                    }
                }
                else if (activePlayer == 2)
                {
                    if (shipy == targety && shipx < targetx)
                    {
                        angle = 180;
                    }
                    else if (shipx < targetx && shipy < targety)
                    {
                        angle -= 180;
                    }
                    else if (shipx < targetx && shipy > targety)
                    {
                        angle += 180;
                    }
                }

                if (angle > 150) angle = 150;
                else if (angle < -150) angle = -150;
            }
            return angle;
        }

        public void doShipRotate(double angle)
        {
            for (int count = 0; count < (int)Math.Abs(angle); count += 5)
            {
                activeShip.shipRotate(5 * (int)(angle / Math.Abs(angle)));
                Draw();
            }
        }
        public void resetShipRotate(double angle)
        {
            //Thread.Sleep(100);
            for (int count = 1; count < (int)Math.Abs(angle); count += 5)
            {
                
                activeShip.shipRotate(-5 * (int)(angle / Math.Abs(angle)));
                Draw();
            }
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


                    if (activeShip == null && cMap.boxes[select].spaceObject != null)
                    {
                        if (cMap.boxes[select].spaceObject != null)
                        {
                            if (activePlayer == cMap.boxes[select].spaceObject.player)
                            {
                                boxDescription.Text = cMap.boxes[select].spaceObject.description();
                                activeShip = (Ship)cMap.boxes[select].spaceObject;

                                Draw();
                            }
                            else
                            {
                                Draw();
                                boxDescription.Text = cMap.boxes[i].spaceObject.description();
                            }
                        }
                    }


                // Если до этого ткнули по дружественному судну
                    else if (activeShip != null)
                    {

                        // если выбранная клетка пуста - определяем возможность перемещения 
                        if (activeShip.actionsLeft > 0 && cMap.boxes[select].spaceObject == null)
                        {
                            int flag = 0;
                            // перемещение на одну клетку вверх
                            int a = activeShip.boxId;
                            if (a + 1 == select && a % cMap.height != cMap.height - 1)
                            {
                                flag = 1;
                            }
                            // перемещение на одну клетку вниз
                            else if (a - 1 == select && a % cMap.height != 0)
                            {
                                flag = 1;
                            }
                            // перемещение на клетку справа вверху
                            else if (cMap.boxes[a].y != 0 && cMap.boxes[a].x != cMap.width
                                && cMap.boxes[a].x + 1 == cMap.boxes[select].x
                                && cMap.boxes[a].y - 1 == cMap.boxes[select].y)
                            {
                                flag = 1;
                            }
                            // перемещение на клетку справа внизу
                            else if (cMap.boxes[a].y + 1 != cMap.height * 2 && cMap.boxes[a].x != cMap.width
                                && cMap.boxes[a].x + 1 == cMap.boxes[select].x
                                && cMap.boxes[a].y + 1 == cMap.boxes[select].y)
                            {
                                flag = 1;
                            }
                            // перемещение на клетку слева вверху
                            else if (cMap.boxes[a].y != 0 && cMap.boxes[a].x != 0
                                && cMap.boxes[a].x - 1 == cMap.boxes[select].x
                                && cMap.boxes[a].y - 1 == cMap.boxes[select].y)
                            {
                                flag = 1;
                            }
                            // перемещение на клетку слева внизу
                            else if (cMap.boxes[a].y + 1 != cMap.height * 2 && cMap.boxes[a].x != 0
                                && cMap.boxes[a].x - 1 == cMap.boxes[select].x
                                && cMap.boxes[a].y + 1 == cMap.boxes[select].y)
                            {
                                flag = 1;
                            }
                            if (flag == 1)
                            {
                                double rotateAngle;

                                rotateAngle = attackAngleSearch(cMap.boxes[select].x, cMap.boxes[select].y);

                                doShipRotate(rotateAngle);

                                int range, dx, x1, x2, y1, y2;

                                x1 = cMap.boxes[activeShip.boxId].xcenter;
                                y1 = cMap.boxes[activeShip.boxId].ycenter;
                                x2 = cMap.boxes[select].xcenter;
                                y2 = cMap.boxes[select].ycenter;

                                List<int> xold = new List<int>();
                                List<int> yold = new List<int>();

                                // запоминаем координаты
                                for (int n = 0; n < activeShip.xpoints.Count; n++ )
                                {
                                    xold.Add(activeShip.xpoints[n]);
                                    yold.Add(activeShip.ypoints[n]);
                                }

                                range = (int)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
                                dx = range / 15;
                                int deltax;
                                int deltay;
                                int step = 15;

                                deltax = (x2 - x1) / step;
                                deltay = (y2 - y1) / step;
                                
                                for (int count1 = 0; count1 < range - 10; count1 += dx)
                                {
                                    for (int j = 0; j < activeShip.xpoints.Count; j++)
                                    {
                                        activeShip.xpoints[j] += deltax;
                                        activeShip.ypoints[j] += deltay;
                                    }
                                    Thread.Sleep(5);
                                    Draw(); 
                                } 
                                // восстанавливаем исходные координаты (смещение корабля по х и y, если быть точнее)
                                for (int n = 0; n < activeShip.xpoints.Count; n++)
                                {
                                    activeShip.xpoints[n] = xold[n];
                                    activeShip.ypoints[n] = yold[n];
                                }

                                activeShip.moveShip(cMap, a, select);

                                resetShipRotate(rotateAngle);

                                boxDescription.Text = activeShip.description();

                                if (activeShip.actionsLeft == 0) activeShip = null;
                                Draw();

                                break;
                            }
                        }
                        else if (cMap.boxes[select].spaceObject != null)
                        {
                            if (cMap.boxes[select].spaceObject.player == activePlayer)
                            {
                                boxDescription.Text = cMap.boxes[select].spaceObject.description();
                                activeShip = (Ship)cMap.boxes[select].spaceObject;

                                Draw();
                                break;
                            }

                            // просчет возможности атаки 

                            else if (cMap.boxes[select].spaceObject.player != activePlayer)
                            {
                                int flag = 0;
                                int a = activeShip.boxId;

                                double x1 = cMap.boxes[a].x;
                                double y1 = cMap.boxes[a].y;
                                double x2 = cMap.boxes[select].x;
                                double y2 = cMap.boxes[select].y;
                                double range;

                                // определяем расстояние между объектами

                                range = Math.Sqrt((x2 - x1) * (x2 - x1) + ((y2 - y1) * (y2 - y1)) * 0.35);

                                if(activeShip.equippedWeapon.attackRange >= (int)range)
                                {
                                    flag = 1; // устанавливаем флаг, если расстояние не превышает дальности атаки
                                }
                                if (flag == 1)
                                {
                                    if(activeShip.actionsLeft >= activeShip.equippedWeapon.energyСonsumption)  // если у корабля остались очки действий
                                    {
                                        double angle, targetx, targety;

                                        targetx = cMap.boxes[select].x;
                                        targety = cMap.boxes[select].y;

                                        angle = attackAngleSearch(targetx, targety);

                                        // поворачиваем корабль на угол angle
                                        doShipRotate(angle);
                                        
                                        // отрисовка атаки
                                        Thread.Sleep(150);

                                        

                                        /* drawLaser(cMap.boxes[activeShip.boxId].xcenter + activeShip.xpoints[2], 
                                            cMap.boxes[activeShip.boxId].ycenter + activeShip.ypoints[2], 
                                            cMap.boxes[select].xcenter, cMap.boxes[select].ycenter); */

                                        //activeShip.equippedWeapon.doAttack(cMap.boxes[select].xcenter, cMap.boxes[select].ycenter);

                                        if (activeShip.attack(cMap, cMap.boxes[select].id, ref combatBitmap, player, ref pictureMap) == 1)
                                            shipsCount();
                                        Draw();

                                        // возвращаем корабль в исходное положение
                                        resetShipRotate(angle);

                                        // убираем подсветку с корабля, если у него не осталось очков передвижений
                                        if (activeShip.actionsLeft == 0)
                                        {
                                            activeShip = null;
                                            Draw();
                                        }
                                        flag = 0;

                                        break;
                                    } 
                                }
                            } 

                            }
                        }
                    break;
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

            objectManager.moveMeteors(cMap);
            
            if(objectManager.whether2createMeteor() == 1)
            {
                objectManager.meteorCreate(cMap);
            }

            Draw();
        }

    }
}
