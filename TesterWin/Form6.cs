using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ZidUtilities.CommonCode.Win.Controls;

namespace TesterWin
{
    /// <summary>
    /// Test form for VerticalProgressBar and ToastForm controls
    /// </summary>
    public partial class Form6 : Form
    {
        private Timer progressTimer;
        private int currentProgress = 0;
        private bool increasing = true;

        public Form6()
        {
            InitializeComponent();
            SetupDemo();
        }

        private void SetupDemo()
        {
            // Setup progress timer for automatic progress bar animation
            progressTimer = new Timer();
            progressTimer.Interval = 100;
            progressTimer.Tick += ProgressTimer_Tick;

            // Set initial states
            chkUseGradient.Checked = false;
            chkUseBackgroundGradient.Checked = false;
            cmbGradientMode.SelectedIndex = 0;

            // Set initial progress bar colors
            btnProgressColor.BackColor = verticalProgressBar1.BarColor;
            btnBackgroundColor.BackColor = verticalProgressBar1.BackgroundColor;
            btnGradientStart.BackColor = verticalProgressBar1.GradientStartColor;
            btnGradientEnd.BackColor = verticalProgressBar1.GradientEndColor;
            btnBgGradientStart.BackColor = verticalProgressBar1.BackgroundGradientStartColor;
            btnBgGradientEnd.BackColor = verticalProgressBar1.BackgroundGradientEndColor;

            // Toast form setup
            cmbToastType.DataSource = Enum.GetValues(typeof(ToastType));
            cmbToastType.SelectedItem = ToastType.Info;

            cmbAnimation.DataSource = Enum.GetValues(typeof(ToastAnimation));
            cmbAnimation.SelectedItem = ToastAnimation.Default;

            cmbVerticalPos.DataSource = Enum.GetValues(typeof(VerticalPosition));
            cmbVerticalPos.SelectedItem = VerticalPosition.Top;

            cmbHorizontalPos.DataSource = Enum.GetValues(typeof(HorizontalPosition));
            cmbHorizontalPos.SelectedItem = HorizontalPosition.Right;

            numDuration.Value = 3000;
        }

        #region Progress Bar Events

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            verticalProgressBar1.Value = trackBar1.Value;
            lblProgressValue.Text = $"{trackBar1.Value}%";
        }

        private void btnAnimate_Click(object sender, EventArgs e)
        {
            if (progressTimer.Enabled)
            {
                progressTimer.Stop();
                btnAnimate.Text = "Start Animation";
            }
            else
            {
                progressTimer.Start();
                btnAnimate.Text = "Stop Animation";
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (increasing)
            {
                currentProgress += 2;
                if (currentProgress >= 100)
                {
                    currentProgress = 100;
                    increasing = false;
                }
            }
            else
            {
                currentProgress -= 2;
                if (currentProgress <= 0)
                {
                    currentProgress = 0;
                    increasing = true;
                }
            }

            trackBar1.Value = currentProgress;
            verticalProgressBar1.Value = currentProgress;
            lblProgressValue.Text = $"{currentProgress}%";
        }

        private void chkUseGradient_CheckedChanged(object sender, EventArgs e)
        {
            verticalProgressBar1.UseGradient = chkUseGradient.Checked;
            panelGradientColors.Enabled = chkUseGradient.Checked;
        }

        private void chkUseBackgroundGradient_CheckedChanged(object sender, EventArgs e)
        {
            verticalProgressBar1.UseBackgroundGradient = chkUseBackgroundGradient.Checked;
            panelBgGradientColors.Enabled = chkUseBackgroundGradient.Checked;
        }

        private void cmbGradientMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGradientMode.SelectedIndex >= 0)
            {
                verticalProgressBar1.GradientMode = (LinearGradientMode)cmbGradientMode.SelectedIndex;
            }
        }

        private void btnProgressColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = verticalProgressBar1.BarColor;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    verticalProgressBar1.BarColor = cd.Color;
                    btnProgressColor.BackColor = cd.Color;
                }
            }
        }

        private void btnBackgroundColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = verticalProgressBar1.BackgroundColor;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    verticalProgressBar1.BackgroundColor = cd.Color;
                    btnBackgroundColor.BackColor = cd.Color;
                }
            }
        }

        private void btnGradientStart_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = verticalProgressBar1.GradientStartColor;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    verticalProgressBar1.GradientStartColor = cd.Color;
                    btnGradientStart.BackColor = cd.Color;
                }
            }
        }

        private void btnGradientEnd_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = verticalProgressBar1.GradientEndColor;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    verticalProgressBar1.GradientEndColor = cd.Color;
                    btnGradientEnd.BackColor = cd.Color;
                }
            }
        }

        private void btnBgGradientStart_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = verticalProgressBar1.BackgroundGradientStartColor;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    verticalProgressBar1.BackgroundGradientStartColor = cd.Color;
                    btnBgGradientStart.BackColor = cd.Color;
                }
            }
        }

        private void btnBgGradientEnd_Click(object sender, EventArgs e)
        {
            using (ColorDialog cd = new ColorDialog())
            {
                cd.Color = verticalProgressBar1.BackgroundGradientEndColor;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    verticalProgressBar1.BackgroundGradientEndColor = cd.Color;
                    btnBgGradientEnd.BackColor = cd.Color;
                }
            }
        }

        #endregion

        #region Toast Form Events

        private void btnShowToast_Click(object sender, EventArgs e)
        {
            ToastType toastType = (ToastType)cmbToastType.SelectedItem;
            ToastAnimation animation = (ToastAnimation)cmbAnimation.SelectedItem;
            VerticalPosition vPos = (VerticalPosition)cmbVerticalPos.SelectedItem;
            HorizontalPosition hPos = (HorizontalPosition)cmbHorizontalPos.SelectedItem;
            int duration = (int)numDuration.Value;
            bool useParentRef = chkUseParentReference.Checked;

            string title = txtToastTitle.Text;
            string message = txtToastMessage.Text;

            if (string.IsNullOrWhiteSpace(title))
                title = "Test Toast";
            if (string.IsNullOrWhiteSpace(message))
                message = "This is a test toast message!";

            Toaster.ShowToast(this, title, message, toastType, hPos, vPos, duration, useParentRef, animation);
        }

        private void btnShowAllAnimations_Click(object sender, EventArgs e)
        {
            // Show all animation types in sequence
            ToastType toastType = (ToastType)cmbToastType.SelectedItem;
            int duration = (int)numDuration.Value;

            var animations = new[]
            {
                ToastAnimation.Fade,
                ToastAnimation.SlideDown,
                ToastAnimation.SlideUp,
                ToastAnimation.SlideLeft,
                ToastAnimation.SlideRight
            };

            var positions = new[]
            {
                new { V = VerticalPosition.Top, H = HorizontalPosition.Right },
                new { V = VerticalPosition.Top, H = HorizontalPosition.Center },
                new { V = VerticalPosition.Bottom, H = HorizontalPosition.Center },
                new { V = VerticalPosition.Center, H = HorizontalPosition.Right },
                new { V = VerticalPosition.Center, H = HorizontalPosition.Left }
            };

            for (int i = 0; i < animations.Length; i++)
            {
                var animation = animations[i];
                var pos = positions[i];

                System.Threading.Tasks.Task.Delay(i * 200).ContinueWith(_ =>
                {
                    this.Invoke(new Action(() =>
                    {
                        Toaster.ShowToast(this, $"{animation} Animation",
                            $"Demonstrating {animation} animation", toastType,
                            pos.H, pos.V, duration, false, animation);
                    }));
                });
            }
        }

        private void btnTestAllTypes_Click(object sender, EventArgs e)
        {
            // Show all toast types in sequence
            var types = new[]
            {
                ToastType.Primary,
                ToastType.Secondary,
                ToastType.Success,
                ToastType.Danger,
                ToastType.Warning,
                ToastType.Info,
                ToastType.Light,
                ToastType.Dark
            };

            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                System.Threading.Tasks.Task.Delay(i * 500).ContinueWith(_ =>
                {
                    this.Invoke(new Action(() =>
                    {
                        Toaster.ShowToast(this, $"{type} Toast",
                            $"This is a {type} toast message", type,
                            HorizontalPosition.Right, VerticalPosition.Top,
                            (int)numDuration.Value, false, ToastAnimation.SlideLeft);
                    }));
                });
            }
        }

        private void btnTestStacking_Click(object sender, EventArgs e)
        {
            // Test stacking by showing multiple toasts at the same position
            ToastType toastType = (ToastType)cmbToastType.SelectedItem;
            VerticalPosition vPos = (VerticalPosition)cmbVerticalPos.SelectedItem;
            HorizontalPosition hPos = (HorizontalPosition)cmbHorizontalPos.SelectedItem;
            int duration = (int)numDuration.Value;

            // Show 5 toasts at the same position to demonstrate stacking
            for (int i = 1; i <= 5; i++)
            {
                int index = i; // Capture for closure
                System.Threading.Tasks.Task.Delay((i - 1) * 200).ContinueWith(_ =>
                {
                    this.Invoke(new Action(() =>
                    {
                        Toaster.ShowToast(this, $"Stacked Toast #{index}",
                            $"This demonstrates automatic stacking. Toast #{index} of 5",
                            toastType, hPos, vPos, duration, false, ToastAnimation.Fade);
                    }));
                });
            }
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (progressTimer != null)
            {
                progressTimer.Stop();
                progressTimer.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}
