
namespace Synthesizer
{
    partial class WaveFormView2
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
            this.components = new System.ComponentModel.Container();
            this.wavePlot = new ScottPlot.FormsPlot();
            this.SuspendLayout();
            // 
            // wavePlot
            // 
            this.wavePlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wavePlot.AutoScroll = true;
            this.wavePlot.AutoSize = true;
            this.wavePlot.Location = new System.Drawing.Point(0, 2);
            this.wavePlot.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.wavePlot.Name = "wavePlot";
            this.wavePlot.Size = new System.Drawing.Size(802, 452);
            this.wavePlot.TabIndex = 0;
            // 
            // WaveFormView2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.wavePlot);
            this.Name = "WaveFormView2";
            this.Text = "WaveFormView2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ScottPlot.FormsPlot wavePlot;
    }
}