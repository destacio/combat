﻿using System;
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
using qwerty.Objects;

namespace qwerty
{
    public partial class Form1 : Form
    {
        public Bitmap combatBitmap;

        //combatMap cMap = new combatMap(8, 6);  // создаем поле боя с указанной размерностью

        private CombatMap cMap => objectManager.CombatMap;
        private List<Ship> allShips => objectManager.Ships;

        ObjectManager objectManager = new ObjectManager(8, 6);
        int select = -1; // служебная переменная, пока сам не знаю на кой хер она мне, но пусть будет. да.
        int activePlayer = 1; // ход 1-ого или 2-ого игрока
        Ship activeShip = null; // выделенное судно
        //List<Ship> allShips = new List<Ship>();
        System.Media.SoundPlayer player = new System.Media.SoundPlayer();
        

        
        public Form1()
        {
            player.SoundLocation = @"../../Sounds/laser1.wav";

            InitializeComponent();
            Draw();

            UpdateShipCount();
#if !DEBUG
            buttonDebug.Visible = false;
#endif
        }

        public bool UpdateShipCount()
        {
            int blueShipsCount = objectManager.FirstPlayerShipCount;
            int redShipsCount = objectManager.SecondPlayerShipCount;

            if (blueShipsCount == 0 || redShipsCount == 0)
            {
                txtBlueShips.Text = "";
                txtRedShips.Text = "";
                label1.Text = "GAME OVER!";
                return false;
            }
            txtBlueShips.Text = "" + blueShipsCount;
            txtRedShips.Text = "" + redShipsCount;
            return true;
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

            for (int i = 0; i < cMap.Cells.Count; i++)
            {
                generalPen = PurplePen;
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
                                double x1 = cMap.Cells[activeShip.boxId].x;
                                double y1 = cMap.Cells[activeShip.boxId].y;
                                double x2 = cMap.Cells[allShips[count].boxId].x;
                                double y2 = cMap.Cells[allShips[count].boxId].y;
                                double range;
                                range = Math.Sqrt((x2 - x1) * (x2 - x1) + ((y2 - y1) * (y2 - y1)) * 0.35);

                                if ((int)range <= activeShip.EquippedWeapon.attackRange)
                                {
                                    g.DrawPolygon(redPen, cMap.Cells[allShips[count].boxId].CellPoints);
                                }
                            }
                        }
                    }
                }

                g.DrawPolygon(PurplePen, cMap.Cells[i].CellPoints);

                if (activeShip != null && activeShip.boxId == i)
                {
                    g.DrawPolygon(activeShipAriaPen, cMap.Cells[i].CellPoints);

                }

                if (cMap.Cells[i].spaceObject != null)
                {
                    if (cMap.Cells[i].spaceObject.player == 1)
                        brush = blueBrush;
                    else if (cMap.Cells[i].spaceObject.player == 2)
                        brush = redBrush;
                    else brush = grayBrush;

                    // this call is as dumb as it could ever be
                    objectManager.drawSpaceShit(i, ref combatBitmap);

                }
#if DEBUG
                g.DrawString(cMap.Cells[i].id.ToString(), new Font("Arial", 8.0F), Brushes.Yellow, new PointF(cMap.Cells[i].xpoint1 + 20, cMap.Cells[i].ypoint1 + 10));
                g.DrawString(cMap.Cells[i].x.ToString(), new Font("Arial", 8.0F), Brushes.DeepSkyBlue, new PointF(cMap.Cells[i].xpoint1 + 10, cMap.Cells[i].ypoint1 + 10));
                g.DrawString(cMap.Cells[i].y.ToString(), new Font("Arial", 8.0F), Brushes.Green, new PointF(cMap.Cells[i].xpoint1 + 40, cMap.Cells[i].ypoint1 + 10));
#endif
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
                shipx = cMap.Cells[activeShip.boxId].x;  // координаты выделенного корабля
                shipy = cMap.Cells[activeShip.boxId].y;

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
            /*for (int count = 0; count < (int)Math.Abs(angle); count += 5)
            {
                activeShip.shipRotate(5 * (int)(angle / Math.Abs(angle)));
                Draw();
            }*/
            activeShip.Rotate(angle);
            Draw();
        }
        public void resetShipRotate(double angle)
        {
            /*for (int count = 1; count < (int)Math.Abs(angle); count += 5)
            {
                
                activeShip.shipRotate(-5 * (int)(angle / Math.Abs(angle)));
                Draw();
            }*/
            activeShip.Rotate(-angle);
            Draw();
        }
        private void pictureMap_MouseClick(object sender, MouseEventArgs e)
        {

            for (int i = 0; i < cMap.Cells.Count; i++)
            {

                if ((e.X > cMap.Cells[i].xpoint2) &&
                    (e.X < cMap.Cells[i].xpoint3) &&
                    (e.Y > cMap.Cells[i].ypoint2) &&
                    (e.Y < cMap.Cells[i].ypoint6))
                {
                    select = i;


                    if (activeShip == null && cMap.Cells[select].spaceObject != null)
                    {
                        if (cMap.Cells[select].spaceObject != null)
                        {
                            if (activePlayer == cMap.Cells[select].spaceObject.player)
                            {
                                boxDescription.Text = cMap.Cells[select].spaceObject.Description;
                                activeShip = (Ship)cMap.Cells[select].spaceObject;

                                Draw();
                            }
                            else
                            {
                                Draw();
                                boxDescription.Text = cMap.Cells[i].spaceObject.Description;
                            }
                        }
                    }


                // Если до этого ткнули по дружественному судну
                    else if (activeShip != null)
                    {

                        // если выбранная клетка пуста - определяем возможность перемещения 
                        if (activeShip.actionsLeft > 0 && cMap.Cells[select].spaceObject == null)
                        {
                            if (cMap.Cells[activeShip.boxId].IsNeighborCell(cMap.Cells[select].x, cMap.Cells[select].y))
                            {
                                double rotateAngle;

                                rotateAngle = attackAngleSearch(cMap.Cells[select].x, cMap.Cells[select].y);

                                doShipRotate(rotateAngle);

                                int range, dx, x1, x2, y1, y2;

                                x1 = cMap.Cells[activeShip.boxId].xcenter;
                                y1 = cMap.Cells[activeShip.boxId].ycenter;
                                x2 = cMap.Cells[select].xcenter;
                                y2 = cMap.Cells[select].ycenter;

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

                                activeShip.Move(cMap, activeShip.boxId, select);

                                resetShipRotate(rotateAngle);

                                boxDescription.Text = activeShip.Description;

                                if (activeShip.actionsLeft == 0) activeShip = null;
                                Draw();

                                break;
                            }
                        }
                        else if (cMap.Cells[select].spaceObject != null)
                        {
                            if (cMap.Cells[select].spaceObject.player == activePlayer)
                            {
                                boxDescription.Text = cMap.Cells[select].spaceObject.Description;
                                activeShip = (Ship)cMap.Cells[select].spaceObject;

                                Draw();
                                break;
                            }

                            // просчет возможности атаки 

                            else if (cMap.Cells[select].spaceObject.player != activePlayer)
                            {
                                int flag = 0;
                                int a = activeShip.boxId;

                                double x1 = cMap.Cells[a].x;
                                double y1 = cMap.Cells[a].y;
                                double x2 = cMap.Cells[select].x;
                                double y2 = cMap.Cells[select].y;
                                double range;

                                // определяем расстояние между объектами

                                range = Math.Sqrt((x2 - x1) * (x2 - x1) + ((y2 - y1) * (y2 - y1)) * 0.35);

                                if(activeShip.EquippedWeapon.attackRange >= (int)range)
                                {
                                    flag = 1; // устанавливаем флаг, если расстояние не превышает дальности атаки
                                }
                                if (flag == 1)
                                {
                                    if(activeShip.actionsLeft >= activeShip.EquippedWeapon.energyСonsumption)  // если у корабля остались очки действий
                                    {
                                        double angle, targetx, targety;

                                        targetx = cMap.Cells[select].x;
                                        targety = cMap.Cells[select].y;

                                        angle = attackAngleSearch(targetx, targety);

                                        // поворачиваем корабль на угол angle
                                        doShipRotate(angle);
                                        
                                        // отрисовка атаки
                                        Thread.Sleep(150);

                                        if (activeShip.Attack(cMap, cMap.Cells[select].id, ref combatBitmap, player, ref pictureMap))
                                            UpdateShipCount();
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
            if (!UpdateShipCount())
            {
                return;
            }

            if (activePlayer == 1) activePlayer = 2;
            else activePlayer = 1;

            lblTurn.Text = "Ходит " + activePlayer + "-й игрок";

            activeShip = null;

            objectManager.EndTurn();

            Draw();
        }

        private void buttonDebug_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello from debug!");
        }
    }
}
