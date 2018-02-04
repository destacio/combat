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
        private readonly SoundPlayer soundPlayer = new SoundPlayer();
        
        public Form1()
        {
            InitializeComponent();
            pictureMap.Width = this.gameLogic.BitmapWidth;
            pictureMap.Height = this.gameLogic.BitmapHeight;
            // i'll leave this as constants -> calculation from window size or placing in container later
            Width = pictureMap.Right + 25;
            Height = pictureMap.Bottom + 45;
            fieldPainter = new FieldPainter(this.gameLogic.BitmapWidth, this.gameLogic.BitmapHeight, objectManager);
            ObjectManager.ObjectAnimated += this.fieldPainter.OnAnimationPending;
            ObjectManager.SoundPlayed += this.OnSoundEffect;
            this.fieldPainter.BitmapUpdated += this.OnBitmapUpdated;
            fieldPainter.DrawField();
            pictureMap.Image = fieldPainter.CurrentBitmap;
            pictureMap.Refresh();
            lblTurn.Text = gameLogic.ActivePlayerDescription + "'s turn";

            this.UpdateShipCount();
#if !DEBUG
            buttonDebug.Visible = false;
#endif
        }

        public void UpdateShipCount()
        {
            int blueShipsCount = gameLogic.FirstPlayerShipCount;
            int redShipsCount = gameLogic.SecondPlayerShipCount;

            if (blueShipsCount == 0 || redShipsCount == 0)
            {
                txtBlueShips.Text = "";
                txtRedShips.Text = "";
                label1.Text = "GAME OVER!";
                return;
            }
            txtBlueShips.Text = $"{blueShipsCount}";
            txtRedShips.Text = $"{redShipsCount}";
        }

        private void pictureMap_MouseClick(object sender, MouseEventArgs e)
        {
            gameLogic.HandleFieldClick(e.Location);
            this.fieldPainter.UpdateBitmap();
            this.pictureMap.Refresh();
            boxDescription.Text = gameLogic.ActiveShipDescription;
            this.UpdateShipCount();
        }

        private void btnEndTurn_Click(object sender, EventArgs e)
        {
            gameLogic.EndTurn();
            this.fieldPainter.UpdateBitmap();
            this.pictureMap.Refresh();
            boxDescription.Text = gameLogic.ActiveShipDescription;
            lblTurn.Text = gameLogic.ActivePlayerDescription + "'s turn";
            this.UpdateShipCount();
        }

        private void buttonDebug_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello from debug!");
        }
       
        private void OnBitmapUpdated(object sender, EventArgs e)
        {
            pictureMap.Refresh();
        }

        private void OnSoundEffect(object sender, SoundEventArgs e)
        {
            if (!checkBoxAudio.Checked)
            {
                return;
            }
            this.soundPlayer.Stream = e.AudioStream;
            this.soundPlayer.Play();
        }
    }
}
