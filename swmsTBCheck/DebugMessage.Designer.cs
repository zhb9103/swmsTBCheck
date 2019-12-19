namespace swmsTBCheck
{
    partial class DebugMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugMessage));
            this.textBoxShowMessage = new System.Windows.Forms.TextBox();
            this.buttonGenOrder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxShowMessage
            // 
            this.textBoxShowMessage.Location = new System.Drawing.Point(12, 12);
            this.textBoxShowMessage.Multiline = true;
            this.textBoxShowMessage.Name = "textBoxShowMessage";
            this.textBoxShowMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxShowMessage.Size = new System.Drawing.Size(204, 161);
            this.textBoxShowMessage.TabIndex = 0;
            // 
            // buttonGenOrder
            // 
            this.buttonGenOrder.Location = new System.Drawing.Point(60, 179);
            this.buttonGenOrder.Name = "buttonGenOrder";
            this.buttonGenOrder.Size = new System.Drawing.Size(91, 28);
            this.buttonGenOrder.TabIndex = 1;
            this.buttonGenOrder.Text = "产生订单";
            this.buttonGenOrder.UseVisualStyleBackColor = true;
            this.buttonGenOrder.Click += new System.EventHandler(this.buttonGenOrder_Click);
            // 
            // DebugMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(228, 219);
            this.Controls.Add(this.buttonGenOrder);
            this.Controls.Add(this.textBoxShowMessage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DebugMessage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "调试信息";
            this.Load += new System.EventHandler(this.DebugMessage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox textBoxShowMessage;
        private System.Windows.Forms.Button buttonGenOrder;
    }
}