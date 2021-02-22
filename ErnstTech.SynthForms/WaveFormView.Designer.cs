
namespace ErnstTech.Synthesizer
{
    partial class WaveFormView
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
            this.panelWaveForm = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.txtZoomFactor = new System.Windows.Forms.ToolStripTextBox();
            this.btnFFTControl = new System.Windows.Forms.ToolStripSplitButton();
            this.miFFTOn = new System.Windows.Forms.ToolStripMenuItem();
            this.miFFTOff = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelWaveForm
            // 
            this.panelWaveForm.BackColor = System.Drawing.Color.White;
            this.panelWaveForm.Location = new System.Drawing.Point(0, 0);
            this.panelWaveForm.Name = "panelWaveForm";
            this.panelWaveForm.Size = new System.Drawing.Size(660, 266);
            this.panelWaveForm.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtZoomFactor,
            this.btnFFTControl});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(660, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // txtZoomFactor
            // 
            this.txtZoomFactor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.txtZoomFactor.Name = "txtZoomFactor";
            this.txtZoomFactor.Size = new System.Drawing.Size(100, 25);
            this.txtZoomFactor.Text = "100";
            this.txtZoomFactor.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtZoomFactor.ToolTipText = "The zoom-factor (in percent).";
            this.txtZoomFactor.Validating += new System.ComponentModel.CancelEventHandler(this.txtZoomFactor_Validating);
            this.txtZoomFactor.TextChanged += new System.EventHandler(this.txtZoomFactor_TextChanged);
            // 
            // btnFFTControl
            // 
            this.btnFFTControl.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFFTOn,
            this.miFFTOff});
            this.btnFFTControl.Name = "btnFFTControl";
            this.btnFFTControl.Text = "FFT";
            // 
            // miFFTOn
            // 
            this.miFFTOn.Name = "miFFTOn";
            this.miFFTOn.Text = "On";
            // 
            // miFFTOff
            // 
            this.miFFTOff.Name = "miFFTOff";
            this.miFFTOff.Text = "Off";
            // 
            // WaveFormView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 266);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panelWaveForm);
            this.Name = "WaveFormView";
            this.Text = "WaveFormView";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelWaveForm;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripTextBox txtZoomFactor;
        private System.Windows.Forms.ToolStripSplitButton btnFFTControl;
        private System.Windows.Forms.ToolStripMenuItem miFFTOn;
        private System.Windows.Forms.ToolStripMenuItem miFFTOff;

    }
}