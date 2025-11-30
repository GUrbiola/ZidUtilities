namespace ZidUtilities.TesterWin
{
    partial class Form8
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.grpAntiPatterns = new System.Windows.Forms.GroupBox();
            this.lblAntiPattern = new System.Windows.Forms.Label();
            this.btnMainThreadBlocked = new System.Windows.Forms.Button();
            this.grpAdvanced = new System.Windows.Forms.GroupBox();
            this.btnTimerBased = new System.Windows.Forms.Button();
            this.btnIndeterminate = new System.Windows.Forms.Button();
            this.btnErrorHandling = new System.Windows.Forms.Button();
            this.btnSequentialOps = new System.Windows.Forms.Button();
            this.grpCancellable = new System.Windows.Forms.GroupBox();
            this.btnCancelOperation = new System.Windows.Forms.Button();
            this.btnCancellable = new System.Windows.Forms.Button();
            this.grpRecommended = new System.Windows.Forms.GroupBox();
            this.lblRecommended = new System.Windows.Forms.Label();
            this.btnAsyncAwait = new System.Windows.Forms.Button();
            this.btnBackgroundManual = new System.Windows.Forms.Button();
            this.btnBackgroundThread = new System.Windows.Forms.Button();
            this.grpHeader = new System.Windows.Forms.GroupBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.pnlLogHeader = new System.Windows.Forms.Panel();
            this.btnTestResponsiveness = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.lblLog = new System.Windows.Forms.Label();
            this.pnlLeft.SuspendLayout();
            this.grpAntiPatterns.SuspendLayout();
            this.grpAdvanced.SuspendLayout();
            this.grpCancellable.SuspendLayout();
            this.grpRecommended.SuspendLayout();
            this.grpHeader.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlLogHeader.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlLeft
            //
            this.pnlLeft.AutoScroll = true;
            this.pnlLeft.Controls.Add(this.grpAntiPatterns);
            this.pnlLeft.Controls.Add(this.grpAdvanced);
            this.pnlLeft.Controls.Add(this.grpCancellable);
            this.pnlLeft.Controls.Add(this.grpRecommended);
            this.pnlLeft.Controls.Add(this.grpHeader);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Padding = new System.Windows.Forms.Padding(10);
            this.pnlLeft.Size = new System.Drawing.Size(400, 661);
            this.pnlLeft.TabIndex = 0;
            //
            // grpAntiPatterns
            //
            this.grpAntiPatterns.Controls.Add(this.lblAntiPattern);
            this.grpAntiPatterns.Controls.Add(this.btnMainThreadBlocked);
            this.grpAntiPatterns.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpAntiPatterns.Location = new System.Drawing.Point(10, 540);
            this.grpAntiPatterns.Name = "grpAntiPatterns";
            this.grpAntiPatterns.Size = new System.Drawing.Size(380, 90);
            this.grpAntiPatterns.TabIndex = 4;
            this.grpAntiPatterns.TabStop = false;
            this.grpAntiPatterns.Text = "Anti-Patterns (What NOT to do)";
            //
            // lblAntiPattern
            //
            this.lblAntiPattern.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAntiPattern.ForeColor = System.Drawing.Color.Red;
            this.lblAntiPattern.Location = new System.Drawing.Point(10, 53);
            this.lblAntiPattern.Name = "lblAntiPattern";
            this.lblAntiPattern.Size = new System.Drawing.Size(360, 30);
            this.lblAntiPattern.TabIndex = 1;
            this.lblAntiPattern.Text = "⚠ This will freeze the UI. Demonstrates incorrect usage.";
            //
            // btnMainThreadBlocked
            //
            this.btnMainThreadBlocked.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnMainThreadBlocked.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMainThreadBlocked.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMainThreadBlocked.ForeColor = System.Drawing.Color.White;
            this.btnMainThreadBlocked.Location = new System.Drawing.Point(10, 23);
            this.btnMainThreadBlocked.Name = "btnMainThreadBlocked";
            this.btnMainThreadBlocked.Size = new System.Drawing.Size(360, 27);
            this.btnMainThreadBlocked.TabIndex = 0;
            this.btnMainThreadBlocked.Text = "❌ Main Thread Blocked (WRONG!)";
            this.btnMainThreadBlocked.UseVisualStyleBackColor = false;
            this.btnMainThreadBlocked.Click += new System.EventHandler(this.btnMainThreadBlocked_Click);
            //
            // grpAdvanced
            //
            this.grpAdvanced.Controls.Add(this.btnTimerBased);
            this.grpAdvanced.Controls.Add(this.btnIndeterminate);
            this.grpAdvanced.Controls.Add(this.btnErrorHandling);
            this.grpAdvanced.Controls.Add(this.btnSequentialOps);
            this.grpAdvanced.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpAdvanced.Location = new System.Drawing.Point(10, 395);
            this.grpAdvanced.Name = "grpAdvanced";
            this.grpAdvanced.Size = new System.Drawing.Size(380, 145);
            this.grpAdvanced.TabIndex = 3;
            this.grpAdvanced.TabStop = false;
            this.grpAdvanced.Text = "Advanced Scenarios";
            //
            // btnTimerBased
            //
            this.btnTimerBased.Location = new System.Drawing.Point(10, 109);
            this.btnTimerBased.Name = "btnTimerBased";
            this.btnTimerBased.Size = new System.Drawing.Size(360, 27);
            this.btnTimerBased.TabIndex = 3;
            this.btnTimerBased.Text = "Timer-Based Updates (Main Thread)";
            this.btnTimerBased.UseVisualStyleBackColor = true;
            this.btnTimerBased.Click += new System.EventHandler(this.btnTimerBased_Click);
            //
            // btnIndeterminate
            //
            this.btnIndeterminate.Location = new System.Drawing.Point(10, 80);
            this.btnIndeterminate.Name = "btnIndeterminate";
            this.btnIndeterminate.Size = new System.Drawing.Size(360, 27);
            this.btnIndeterminate.TabIndex = 2;
            this.btnIndeterminate.Text = "Indeterminate Progress";
            this.btnIndeterminate.UseVisualStyleBackColor = true;
            this.btnIndeterminate.Click += new System.EventHandler(this.btnIndeterminate_Click);
            //
            // btnErrorHandling
            //
            this.btnErrorHandling.Location = new System.Drawing.Point(10, 51);
            this.btnErrorHandling.Name = "btnErrorHandling";
            this.btnErrorHandling.Size = new System.Drawing.Size(360, 27);
            this.btnErrorHandling.TabIndex = 1;
            this.btnErrorHandling.Text = "Error Handling Demo";
            this.btnErrorHandling.UseVisualStyleBackColor = true;
            this.btnErrorHandling.Click += new System.EventHandler(this.btnErrorHandling_Click);
            //
            // btnSequentialOps
            //
            this.btnSequentialOps.Location = new System.Drawing.Point(10, 22);
            this.btnSequentialOps.Name = "btnSequentialOps";
            this.btnSequentialOps.Size = new System.Drawing.Size(360, 27);
            this.btnSequentialOps.TabIndex = 0;
            this.btnSequentialOps.Text = "Sequential Operations";
            this.btnSequentialOps.UseVisualStyleBackColor = true;
            this.btnSequentialOps.Click += new System.EventHandler(this.btnSequentialOps_Click);
            //
            // grpCancellable
            //
            this.grpCancellable.Controls.Add(this.btnCancelOperation);
            this.grpCancellable.Controls.Add(this.btnCancellable);
            this.grpCancellable.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpCancellable.Location = new System.Drawing.Point(10, 305);
            this.grpCancellable.Name = "grpCancellable";
            this.grpCancellable.Size = new System.Drawing.Size(380, 90);
            this.grpCancellable.TabIndex = 2;
            this.grpCancellable.TabStop = false;
            this.grpCancellable.Text = "Cancellable Operation";
            //
            // btnCancelOperation
            //
            this.btnCancelOperation.Enabled = false;
            this.btnCancelOperation.Location = new System.Drawing.Point(10, 51);
            this.btnCancelOperation.Name = "btnCancelOperation";
            this.btnCancelOperation.Size = new System.Drawing.Size(360, 27);
            this.btnCancelOperation.TabIndex = 1;
            this.btnCancelOperation.Text = "Cancel Operation";
            this.btnCancelOperation.UseVisualStyleBackColor = true;
            this.btnCancelOperation.Click += new System.EventHandler(this.btnCancelOperation_Click);
            //
            // btnCancellable
            //
            this.btnCancellable.Location = new System.Drawing.Point(10, 22);
            this.btnCancellable.Name = "btnCancellable";
            this.btnCancellable.Size = new System.Drawing.Size(360, 27);
            this.btnCancellable.TabIndex = 0;
            this.btnCancellable.Text = "Start Cancellable Process";
            this.btnCancellable.UseVisualStyleBackColor = true;
            this.btnCancellable.Click += new System.EventHandler(this.btnCancellable_Click);
            //
            // grpRecommended
            //
            this.grpRecommended.Controls.Add(this.lblRecommended);
            this.grpRecommended.Controls.Add(this.btnAsyncAwait);
            this.grpRecommended.Controls.Add(this.btnBackgroundManual);
            this.grpRecommended.Controls.Add(this.btnBackgroundThread);
            this.grpRecommended.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpRecommended.Location = new System.Drawing.Point(10, 145);
            this.grpRecommended.Name = "grpRecommended";
            this.grpRecommended.Size = new System.Drawing.Size(380, 160);
            this.grpRecommended.TabIndex = 1;
            this.grpRecommended.TabStop = false;
            this.grpRecommended.Text = "Recommended Approaches";
            //
            // lblRecommended
            //
            this.lblRecommended.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecommended.ForeColor = System.Drawing.Color.Green;
            this.lblRecommended.Location = new System.Drawing.Point(10, 125);
            this.lblRecommended.Name = "lblRecommended";
            this.lblRecommended.Size = new System.Drawing.Size(360, 30);
            this.lblRecommended.TabIndex = 3;
            this.lblRecommended.Text = "✓ These approaches keep the UI responsive.";
            //
            // btnAsyncAwait
            //
            this.btnAsyncAwait.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnAsyncAwait.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAsyncAwait.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAsyncAwait.ForeColor = System.Drawing.Color.White;
            this.btnAsyncAwait.Location = new System.Drawing.Point(10, 80);
            this.btnAsyncAwait.Name = "btnAsyncAwait";
            this.btnAsyncAwait.Size = new System.Drawing.Size(360, 27);
            this.btnAsyncAwait.TabIndex = 2;
            this.btnAsyncAwait.Text = "⭐ Async/Await (Modern)";
            this.btnAsyncAwait.UseVisualStyleBackColor = false;
            this.btnAsyncAwait.Click += new System.EventHandler(this.btnAsyncAwait_Click);
            //
            // btnBackgroundManual
            //
            this.btnBackgroundManual.Location = new System.Drawing.Point(10, 51);
            this.btnBackgroundManual.Name = "btnBackgroundManual";
            this.btnBackgroundManual.Size = new System.Drawing.Size(360, 27);
            this.btnBackgroundManual.TabIndex = 1;
            this.btnBackgroundManual.Text = "Background Thread (Manual Control)";
            this.btnBackgroundManual.UseVisualStyleBackColor = true;
            this.btnBackgroundManual.Click += new System.EventHandler(this.btnBackgroundManual_Click);
            //
            // btnBackgroundThread
            //
            this.btnBackgroundThread.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnBackgroundThread.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBackgroundThread.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackgroundThread.ForeColor = System.Drawing.Color.White;
            this.btnBackgroundThread.Location = new System.Drawing.Point(10, 22);
            this.btnBackgroundThread.Name = "btnBackgroundThread";
            this.btnBackgroundThread.Size = new System.Drawing.Size(360, 27);
            this.btnBackgroundThread.TabIndex = 0;
            this.btnBackgroundThread.Text = "✓ Background Thread (Using Block)";
            this.btnBackgroundThread.UseVisualStyleBackColor = false;
            this.btnBackgroundThread.Click += new System.EventHandler(this.btnBackgroundThread_Click);
            //
            // grpHeader
            //
            this.grpHeader.Controls.Add(this.lblDescription);
            this.grpHeader.Controls.Add(this.lblTitle);
            this.grpHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpHeader.Location = new System.Drawing.Point(10, 10);
            this.grpHeader.Name = "grpHeader";
            this.grpHeader.Size = new System.Drawing.Size(380, 135);
            this.grpHeader.TabIndex = 0;
            this.grpHeader.TabStop = false;
            this.grpHeader.Text = "About This Demo";
            //
            // lblDescription
            //
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescription.Location = new System.Drawing.Point(3, 45);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Padding = new System.Windows.Forms.Padding(5);
            this.lblDescription.Size = new System.Drawing.Size(374, 87);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "This form demonstrates various ways to use ProcessingDialog.\r\n\r\n✓ Green buttons" +
    " show CORRECT approaches that keep UI responsive.\r\n❌ Red button shows INCORRECT" +
    " approach that freezes UI.\r\n\r\nWatch the log panel to see what\'s happening!";
            //
            // lblTitle
            //
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(3, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(374, 29);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ProcessingDialog Test Scenarios";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pnlRight
            //
            this.pnlRight.Controls.Add(this.txtLog);
            this.pnlRight.Controls.Add(this.pnlLogHeader);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(400, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Padding = new System.Windows.Forms.Padding(10);
            this.pnlRight.Size = new System.Drawing.Size(684, 661);
            this.pnlRight.TabIndex = 1;
            //
            // txtLog
            //
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.ForeColor = System.Drawing.Color.Lime;
            this.txtLog.Location = new System.Drawing.Point(10, 50);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(664, 601);
            this.txtLog.TabIndex = 1;
            this.txtLog.WordWrap = false;
            //
            // pnlLogHeader
            //
            this.pnlLogHeader.Controls.Add(this.btnTestResponsiveness);
            this.pnlLogHeader.Controls.Add(this.btnClearLog);
            this.pnlLogHeader.Controls.Add(this.lblLog);
            this.pnlLogHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLogHeader.Location = new System.Drawing.Point(10, 10);
            this.pnlLogHeader.Name = "pnlLogHeader";
            this.pnlLogHeader.Size = new System.Drawing.Size(664, 40);
            this.pnlLogHeader.TabIndex = 0;
            //
            // btnTestResponsiveness
            //
            this.btnTestResponsiveness.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestResponsiveness.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnTestResponsiveness.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestResponsiveness.ForeColor = System.Drawing.Color.White;
            this.btnTestResponsiveness.Location = new System.Drawing.Point(420, 7);
            this.btnTestResponsiveness.Name = "btnTestResponsiveness";
            this.btnTestResponsiveness.Size = new System.Drawing.Size(150, 27);
            this.btnTestResponsiveness.TabIndex = 2;
            this.btnTestResponsiveness.Text = "Test Responsiveness";
            this.btnTestResponsiveness.UseVisualStyleBackColor = false;
            this.btnTestResponsiveness.Click += new System.EventHandler(this.btnTestResponsiveness_Click);
            //
            // btnClearLog
            //
            this.btnClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearLog.Location = new System.Drawing.Point(577, 7);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(80, 27);
            this.btnClearLog.TabIndex = 1;
            this.btnClearLog.Text = "Clear";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // lblLog
            //
            this.lblLog.AutoSize = true;
            this.lblLog.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLog.Location = new System.Drawing.Point(3, 10);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(100, 21);
            this.lblLog.TabIndex = 0;
            this.lblLog.Text = "Activity Log";
            //
            // Form8
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 661);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
            this.Name = "Form8";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProcessingDialog - Comprehensive Test Scenarios";
            this.pnlLeft.ResumeLayout(false);
            this.grpAntiPatterns.ResumeLayout(false);
            this.grpAdvanced.ResumeLayout(false);
            this.grpCancellable.ResumeLayout(false);
            this.grpRecommended.ResumeLayout(false);
            this.grpHeader.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.PerformLayout();
            this.pnlLogHeader.ResumeLayout(false);
            this.pnlLogHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.GroupBox grpHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.GroupBox grpRecommended;
        private System.Windows.Forms.Button btnBackgroundThread;
        private System.Windows.Forms.Button btnBackgroundManual;
        private System.Windows.Forms.Button btnAsyncAwait;
        private System.Windows.Forms.GroupBox grpCancellable;
        private System.Windows.Forms.Button btnCancellable;
        private System.Windows.Forms.Button btnCancelOperation;
        private System.Windows.Forms.GroupBox grpAdvanced;
        private System.Windows.Forms.Button btnSequentialOps;
        private System.Windows.Forms.Button btnErrorHandling;
        private System.Windows.Forms.Button btnIndeterminate;
        private System.Windows.Forms.GroupBox grpAntiPatterns;
        private System.Windows.Forms.Button btnMainThreadBlocked;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Panel pnlLogHeader;
        private System.Windows.Forms.Label lblLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Label lblRecommended;
        private System.Windows.Forms.Label lblAntiPattern;
        private System.Windows.Forms.Button btnTestResponsiveness;
        private System.Windows.Forms.Button btnTimerBased;
    }
}
