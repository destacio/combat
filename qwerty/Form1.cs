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
using qwerty.Objects;

namespace qwerty
{
    public partial class Form1 : Form
    {
        public Bitmap combatBitmap;

        private CombatMap cMap => _objectManager.CombatMap;
        private List<Ship> allShips => _objectManager.Ships;

        private readonly ObjectManager _objectManager = new ObjectManager(8, 6);
        private int _selectedCellId = -1;
        private int _activePlayer = 1; // ход 1-ого или 2-ого игрока
        private Ship _activeShip = null; // выделенное судно
        System.Media.SoundPlayer player = new System.Media.SoundPlayer();
        

        
        public Form1()
        {
            player.SoundLocation = @"../../Sounds/laser1.wav";

            InitializeComponent();
            pictureMap.Width = _objectManager.FieldWidth;
            pictureMap.Height = _objectManager.FieldHeight;
            // i'll leave this as constants -> calculation from window size or placing in container later
            Width = pictureMap.Right + 25;
            Height = pictureMap.Bottom + 45;
            Draw();

            UpdateShipCount();
#if !DEBUG
            buttonDebug.Visible = false;
#endif
        }

        public bool UpdateShipCount()
        {
            int blueShipsCount = _objectManager.FirstPlayerShipCount;
            int redShipsCount = _objectManager.SecondPlayerShipCount;

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

                if (_activeShip != null)
                {
                    for (int count = 0; count < allShips.Count; count++)
                    {
                        if (allShips[count].player != _activePlayer && allShips[count].boxId >= 0)
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
                                double x1 = cMap.Cells[_activeShip.boxId].x;
                                double y1 = cMap.Cells[_activeShip.boxId].y;
                                double x2 = cMap.Cells[allShips[count].boxId].x;
                                double y2 = cMap.Cells[allShips[count].boxId].y;
                                double range;
                                range = Math.Sqrt((x2 - x1) * (x2 - x1) + ((y2 - y1) * (y2 - y1)) * 0.35);

                                if ((int)range <= _activeShip.EquippedWeapon.attackRange)
                                {
                                    g.DrawPolygon(redPen, cMap.Cells[allShips[count].boxId].CellPoints);
                                }
                            }
                        }
                    }
                }

                g.DrawPolygon(PurplePen, cMap.Cells[i].CellPoints);

                if (_activeShip != null && _activeShip.boxId == i)
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
                    _objectManager.drawSpaceShit(i, ref combatBitmap);

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

            if (_activeShip != null)
            {
                shipx = cMap.Cells[_activeShip.boxId].x;  // координаты выделенного корабля
                shipy = cMap.Cells[_activeShip.boxId].y;

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
                    if (_activePlayer == 2) angle = -angle;

                }
                else // находим угол, на который нужно повернуть корабль (если он не равен 90 градусов)
                {
                    angle = Math.Atan((targety - shipy) / (targetx - shipx)) * 180 / Math.PI;
                }
                // дальше идет коррекция, не пытайся разобраться как это работает, просто оставь как есть
                if (_activePlayer == 1)
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
                else if (_activePlayer == 2)
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
            _activeShip.Rotate(angle);
            Draw();
        }
        public void resetShipRotate(double angle)
        {
            /*for (int count = 1; count < (int)Math.Abs(angle); count += 5)
            {
                
                activeShip.shipRotate(-5 * (int)(angle / Math.Abs(angle)));
                Draw();
            }*/
            _activeShip.Rotate(-angle);
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
                    _selectedCellId = i;


                    if (_activeShip == null && cMap.Cells[_selectedCellId].spaceObject != null)
                    {
                        if (cMap.Cells[_selectedCellId].spaceObject != null)
                        {
                            if (_activePlayer == cMap.Cells[_selectedCellId].spaceObject.player)
                            {
                                boxDescription.Text = cMap.Cells[_selectedCellId].spaceObject.Description;
                                _activeShip = (Ship)cMap.Cells[_selectedCellId].spaceObject;

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
                    else if (_activeShip != null)
                    {

                        // если выбранная клетка пуста - определяем возможность перемещения 
                        if (_activeShip.actionsLeft > 0 && cMap.Cells[_selectedCellId].spaceObject == null)
                        {
                            if (cMap.Cells[_activeShip.boxId].IsNeighborCell(cMap.Cells[_selectedCellId].x, cMap.Cells[_selectedCellId].y))
                            {
                                double rotateAngle;

                                rotateAngle = attackAngleSearch(cMap.Cells[_selectedCellId].x, cMap.Cells[_selectedCellId].y);

                                doShipRotate(rotateAngle);

                                int range, dx, x1, x2, y1, y2;

                                x1 = cMap.Cells[_activeShip.boxId].xcenter;
                                y1 = cMap.Cells[_activeShip.boxId].ycenter;
                                x2 = cMap.Cells[_selectedCellId].xcenter;
                                y2 = cMap.Cells[_selectedCellId].ycenter;

                                List<int> xold = new List<int>();
                                List<int> yold = new List<int>();

                                // запоминаем координаты
                                for (int n = 0; n < _activeShip.xpoints.Count; n++ )
                                {
                                    xold.Add(_activeShip.xpoints[n]);
                                    yold.Add(_activeShip.ypoints[n]);
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
                                    for (int j = 0; j < _activeShip.xpoints.Count; j++)
                                    {
                                        _activeShip.xpoints[j] += deltax;
                                        _activeShip.ypoints[j] += deltay;
                                    }
                                    Thread.Sleep(5);
                                    Draw(); 
                                } 
                                // восстанавливаем исходные координаты (смещение корабля по х и y, если быть точнее)
                                for (int n = 0; n < _activeShip.xpoints.Count; n++)
                                {
                                    _activeShip.xpoints[n] = xold[n];
                                    _activeShip.ypoints[n] = yold[n];
                                }

                                _activeShip.Move(cMap, _activeShip.boxId, _selectedCellId);

                                resetShipRotate(rotateAngle);

                                boxDescription.Text = _activeShip.Description;

                                if (_activeShip.actionsLeft == 0) _activeShip = null;
                                Draw();

                                break;
                            }
                        }
                        else if (cMap.Cells[_selectedCellId].spaceObject != null)
                        {
                            if (cMap.Cells[_selectedCellId].spaceObject.player == _activePlayer)
                            {
                                boxDescription.Text = cMap.Cells[_selectedCellId].spaceObject.Description;
                                _activeShip = (Ship)cMap.Cells[_selectedCellId].spaceObject;

                                Draw();
                                break;
                            }

                            // просчет возможности атаки 

                            else if (cMap.Cells[_selectedCellId].spaceObject.player != _activePlayer)
                            {
                                int flag = 0;
                                int a = _activeShip.boxId;

                                double x1 = cMap.Cells[a].x;
                                double y1 = cMap.Cells[a].y;
                                double x2 = cMap.Cells[_selectedCellId].x;
                                double y2 = cMap.Cells[_selectedCellId].y;
                                double range;

                                // определяем расстояние между объектами

                                range = Math.Sqrt((x2 - x1) * (x2 - x1) + ((y2 - y1) * (y2 - y1)) * 0.35);

                                if(_activeShip.EquippedWeapon.attackRange >= (int)range)
                                {
                                    flag = 1; // устанавливаем флаг, если расстояние не превышает дальности атаки
                                }
                                if (flag == 1)
                                {
                                    if(_activeShip.actionsLeft >= _activeShip.EquippedWeapon.energyСonsumption)  // если у корабля остались очки действий
                                    {
                                        double angle, targetx, targety;

                                        targetx = cMap.Cells[_selectedCellId].x;
                                        targety = cMap.Cells[_selectedCellId].y;

                                        angle = attackAngleSearch(targetx, targety);

                                        // поворачиваем корабль на угол angle
                                        doShipRotate(angle);
                                        
                                        // отрисовка атаки
                                        Thread.Sleep(150);

                                        if (_activeShip.Attack(cMap, cMap.Cells[_selectedCellId].id, ref combatBitmap, player, ref pictureMap))
                                            UpdateShipCount();
                                        Draw();

                                        // возвращаем корабль в исходное положение
                                        resetShipRotate(angle);

                                        // убираем подсветку с корабля, если у него не осталось очков передвижений
                                        if (_activeShip.actionsLeft == 0)
                                        {
                                            _activeShip = null;
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

            if (_activePlayer == 1) _activePlayer = 2;
            else _activePlayer = 1;

            lblTurn.Text = "Ходит " + _activePlayer + "-й игрок";

            _activeShip = null;

            _objectManager.EndTurn();

            Draw();
        }

        private void buttonDebug_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello from debug!");
        }
    }
}
