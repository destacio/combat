using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using Hex = Barbar.HexGrid;

namespace qwerty
{
    public partial class Form1 : Form
    {
        private ObjectManager objectManager => gameLogic.objectManager;
        private readonly GameLogic gameLogic = new GameLogic(8,6);
        private readonly FieldPainter fieldPainter;
        System.Media.SoundPlayer player = new System.Media.SoundPlayer();
        
        public Form1()
        {
            player.SoundLocation = @"../../Sounds/laser1.wav";


            InitializeComponent();
            pictureMap.Width = this.gameLogic.BitmapWidth;
            pictureMap.Height = this.gameLogic.BitmapHeight;
            // i'll leave this as constants -> calculation from window size or placing in container later
            Width = pictureMap.Right + 25;
            Height = pictureMap.Bottom + 45;
            fieldPainter = new FieldPainter(this.gameLogic.BitmapWidth, this.gameLogic.BitmapHeight, objectManager, imageUpdater);
            ObjectManager.ObjectAnimated += this.fieldPainter.OnAnimationPending;
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
            int blueShipsCount = gameLogic.FirstPlayerShipCount;
            int redShipsCount = gameLogic.SecondPlayerShipCount;

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
            //fieldPainter.DrawField();
            //pictureMap.Image = fieldPainter.CurrentBitmap;
            //pictureMap.Refresh();
            if (!this.imageUpdater.IsBusy)
            {
                imageUpdater.RunWorkerAsync();
            }
            boxDescription.Text = gameLogic.ActiveShipDescription;
            UpdateShipCount();
        }

        private void btnEndTurn_Click(object sender, EventArgs e)
        {
            gameLogic.EndTurn();
            //fieldPainter.DrawField();
            //pictureMap.Image = fieldPainter.CurrentBitmap;
            //pictureMap.Refresh();
            if (!this.imageUpdater.IsBusy)
            {
                imageUpdater.RunWorkerAsync();
            }
            boxDescription.Text = gameLogic.ActiveShipDescription;
            lblTurn.Text = gameLogic.ActivePlayerDescription + "'s turn";
        }

        private void buttonDebug_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello from debug!");
        }

        private void imageUpdater_DoWork(object sender, DoWorkEventArgs e)
        {
            //this.btnEndTurn.Enabled = false;
            this.fieldPainter.UpdateBitmap();
            //this.btnEndTurn.Enabled = true;
        }

        private void imageUpdater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pictureMap.Refresh();
        }

        private void imageUpdater_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pictureMap.Refresh();
        }
    }
}
