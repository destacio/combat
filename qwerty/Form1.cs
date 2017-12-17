using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        private CombatMap cMap => _objectManager.CombatMap;
        private List<Ship> allShips => _objectManager.Ships;

        private readonly ObjectManager _objectManager = new ObjectManager(8, 6);
        private readonly GameLogic gameLogic = new GameLogic(8,6);
        private readonly FieldPainter fieldPainter;
        private Player _activePlayer = Player.FirstPlayer; // ход 1-ого или 2-ого игрока
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
            fieldPainter = new FieldPainter(cMap.FieldWidthPixels, cMap.FieldHeightPixels, _objectManager);
            fieldPainter.DrawField();
            pictureMap.Image = fieldPainter.CurrentBitmap;
            pictureMap.Refresh();

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

        private void pictureMap_MouseClick(object sender, MouseEventArgs e)
        {
            gameLogic.HandleFieldClick(e.Location);
            fieldPainter.DrawField();
            pictureMap.Refresh();
        }

        private void btnEndTurn_Click(object sender, EventArgs e)
        {
            gameLogic.EndTurn();
            fieldPainter.DrawField();
            pictureMap.Refresh();
        }

        private void buttonDebug_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello from debug!");
        }
    }
}
