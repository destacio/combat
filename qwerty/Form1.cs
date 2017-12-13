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
using Hex = Barbar.HexGrid;

namespace qwerty
{
    public partial class Form1 : Form
    {
        public Bitmap combatBitmap;

        private CombatMap cMap => _objectManager.CombatMap;
        private List<Ship> allShips => _objectManager.Ships;

        private readonly ObjectManager _objectManager = new ObjectManager(8, 6);
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
            UpdateUi();    

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

        public void UpdateUi()
        {
            combatBitmap = new Bitmap(pictureMap.Width, pictureMap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(combatBitmap);
            g.FillRectangle(Brushes.Black, 0, 0, combatBitmap.Width, combatBitmap.Height); //рисуем фон окна

            Pen redPen = new Pen(Color.Red, 3);
            Pen grayPen = new Pen(Color.Gray, 3);
            Pen PurplePen = new Pen(Color.Purple);
            Pen activeShipAriaPen = new Pen(Color.Purple, 5);

            for (int i = 0; i < cMap.Cells.Count; i++)
            {
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
                                    g.DrawPolygon(redPen, cMap.GetHexagonCorners(cMap.Cells[allShips[count].boxId]));
                                }
                            }
                        }
                    }
                }
                
                g.DrawPolygon(PurplePen, cMap.GetHexagonCorners(cMap.Cells[i]));

                if (_activeShip != null && _activeShip.boxId == i)
                {
                    g.DrawPolygon(activeShipAriaPen, cMap.GetHexagonCorners(cMap.Cells[i]));

                }

                if (cMap.Cells[i].spaceObject != null)
                {
                    // this call is as dumb as it could ever be
                    _objectManager.drawSpaceShit(i, ref combatBitmap);

                }
#if DEBUG
                g.DrawString(cMap.Cells[i].id.ToString(), new Font("Arial", 8.0F), Brushes.Yellow, PointF.Add(cMap.Cells[i].CellPoints[3], new Size(10, 10)));
                g.DrawString($"({cMap.Cells[i].x}, {cMap.Cells[i].y})", new Font("Arial", 8.0F), Brushes.DeepSkyBlue, PointF.Add(cMap.Cells[i].CellPoints[3], new Size(30, 10)));
#endif
            }
                pictureMap.Image = combatBitmap;
                pictureMap.Refresh();
            
        }   

        public void RotateShip(double angle)
        {
            /*for (int count = 0; count < (int)Math.Abs(angle); count += 5)
            {
                activeShip.shipRotate(5 * (int)(angle / Math.Abs(angle)));
                Draw();
            }*/
            _activeShip.Rotate(angle);
            UpdateUi();
        }

        private void pictureMap_MouseClick(object sender, MouseEventArgs e)
        {
            var selectedCell = cMap.GetCellByPixelCoordinates(e.X, e.Y);
            if (selectedCell == null)
            {
                return;
            }

            if (_activeShip == null)
            {
                // Nothing active and nothing to be activated
                if (selectedCell.spaceObject == null) return;

                if (_activePlayer == selectedCell.spaceObject.player)
                {
                    _activeShip = (Ship)selectedCell.spaceObject;
                }
                boxDescription.Text = selectedCell.spaceObject.Description;
                UpdateUi();
            }
            else
            {
                // если выбранная клетка пуста - определяем возможность перемещения 
                if (selectedCell.spaceObject == null)
                {
                    if (_activeShip.actionsLeft <= 0) return;
                    if (cMap.AreNeighbors(new Hex.OffsetCoordinates(cMap.Cells[_activeShip.boxId].x, cMap.Cells[_activeShip.boxId].y), new Hex.OffsetCoordinates(selectedCell.x, selectedCell.y)))
                    {
                        var rotateAngle = cMap.GetAngle(_activeShip.boxId, selectedCell.id, _activePlayer);

                        RotateShip(rotateAngle);

                        var x1 = cMap.Cells[_activeShip.boxId].CellCenter.X;
                        var y1 = cMap.Cells[_activeShip.boxId].CellCenter.Y;
                        var x2 = selectedCell.CellCenter.X;
                        var y2 = selectedCell.CellCenter.Y;

                        var oldPoints = new List<PointF>(_activeShip.PolygonPoints);

                        var range = (int)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
                        var dx = range / 15;
                        int step = 15;

                        var deltax = (x2 - x1) / step;
                        var deltay = (y2 - y1) / step;
                                
                        for (int count1 = 0; count1 < range - 10; count1 += dx)
                        {
                            for (var i = 0; i < _activeShip.PolygonPoints.Count; i++)
                            {
                                _activeShip.PolygonPoints[i] = new PointF(_activeShip.PolygonPoints[i].X + deltax,
                                    _activeShip.PolygonPoints[i].Y + deltay);
                            }
                            Thread.Sleep(5);
                            UpdateUi(); 
                        } 

                        _activeShip.PolygonPoints = oldPoints;

                        _activeShip.Move(cMap, _activeShip.boxId, selectedCell.id);

                        RotateShip(-rotateAngle);

                        boxDescription.Text = _activeShip.Description;

                        if (_activeShip.actionsLeft == 0) _activeShip = null;
                        UpdateUi();
                    }
                }
                else
                {
                    if (selectedCell.spaceObject.player == _activePlayer)
                    {
                        boxDescription.Text = selectedCell.spaceObject.Description;
                        _activeShip = (Ship)selectedCell.spaceObject;

                        UpdateUi();
                    }

                    // просчет возможности атаки 

                    else
                    {
                        int a = _activeShip.boxId;

                        double x1 = cMap.Cells[a].x;
                        double y1 = cMap.Cells[a].y;
                        double x2 = selectedCell.x;
                        double y2 = selectedCell.y;

                        // определяем расстояние между объектами

                        var range = Math.Sqrt((x2 - x1) * (x2 - x1) + ((y2 - y1) * (y2 - y1)) * 0.35);

                        if (_activeShip.EquippedWeapon.attackRange < (int) range || _activeShip.actionsLeft < _activeShip.EquippedWeapon.energyСonsumption)
                        {
                            // another object is out of range or requires more energy than is left
                            return;
                        }

                        var angle = cMap.GetAngle(_activeShip.boxId, selectedCell.id, _activePlayer);

                        // поворачиваем корабль на угол angle
                        RotateShip(angle);
                                        
                        // отрисовка атаки
                        Thread.Sleep(150);

                        if (_activeShip.Attack(cMap, selectedCell.id, ref combatBitmap, player, ref pictureMap))
                            UpdateShipCount();
                        UpdateUi();

                        // возвращаем корабль в исходное положение
                        RotateShip(-angle);

                        // убираем подсветку с корабля, если у него не осталось очков передвижений
                        if (_activeShip.actionsLeft == 0)
                        {
                            _activeShip = null;
                            UpdateUi();
                        }
                    }
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

            UpdateUi();
        }

        private void buttonDebug_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello from debug!");
        }
    }
}
