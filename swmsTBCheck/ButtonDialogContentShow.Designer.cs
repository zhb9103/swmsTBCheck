namespace swmsTBCheck
{
    partial class ButtonDialogContentShow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ButtonDialogContentShow));
            this.listViewShowInfo = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.radioButtonIsFull = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonIsNotFull = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.labelCurrentOrderQuantity = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelOrderNumber = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listViewShowInfo
            // 
            this.listViewShowInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listViewShowInfo.FullRowSelect = true;
            this.listViewShowInfo.Location = new System.Drawing.Point(12, 50);
            this.listViewShowInfo.Name = "listViewShowInfo";
            this.listViewShowInfo.Size = new System.Drawing.Size(723, 305);
            this.listViewShowInfo.TabIndex = 1;
            this.listViewShowInfo.UseCompatibleStateImageBehavior = false;
            this.listViewShowInfo.View = System.Windows.Forms.View.Details;
            this.listViewShowInfo.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewShowInfo_ColumnClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "序号";
            this.columnHeader1.Width = 40;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "SKU";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 104;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "商品描述";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 302;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "EPC编码";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 199;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "分播状态";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader5.Width = 68;
            // 
            // radioButtonIsFull
            // 
            this.radioButtonIsFull.AutoSize = true;
            this.radioButtonIsFull.Location = new System.Drawing.Point(120, 18);
            this.radioButtonIsFull.Name = "radioButtonIsFull";
            this.radioButtonIsFull.Size = new System.Drawing.Size(47, 16);
            this.radioButtonIsFull.TabIndex = 2;
            this.radioButtonIsFull.TabStop = true;
            this.radioButtonIsFull.Text = "已满";
            this.radioButtonIsFull.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 14);
            this.label1.TabIndex = 3;
            this.label1.Text = "当前订单状态：";
            // 
            // radioButtonIsNotFull
            // 
            this.radioButtonIsNotFull.AutoSize = true;
            this.radioButtonIsNotFull.Location = new System.Drawing.Point(173, 18);
            this.radioButtonIsNotFull.Name = "radioButtonIsNotFull";
            this.radioButtonIsNotFull.Size = new System.Drawing.Size(47, 16);
            this.radioButtonIsNotFull.TabIndex = 2;
            this.radioButtonIsNotFull.TabStop = true;
            this.radioButtonIsNotFull.Text = "未满";
            this.radioButtonIsNotFull.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(610, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 14);
            this.label2.TabIndex = 3;
            this.label2.Text = "订单商品数量：";
            // 
            // labelCurrentOrderQuantity
            // 
            this.labelCurrentOrderQuantity.AutoSize = true;
            this.labelCurrentOrderQuantity.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelCurrentOrderQuantity.Location = new System.Drawing.Point(715, 18);
            this.labelCurrentOrderQuantity.Name = "labelCurrentOrderQuantity";
            this.labelCurrentOrderQuantity.Size = new System.Drawing.Size(15, 14);
            this.labelCurrentOrderQuantity.TabIndex = 4;
            this.labelCurrentOrderQuantity.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(274, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 14);
            this.label3.TabIndex = 5;
            this.label3.Text = "订单号：";
            // 
            // labelOrderNumber
            // 
            this.labelOrderNumber.AutoSize = true;
            this.labelOrderNumber.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelOrderNumber.Location = new System.Drawing.Point(337, 20);
            this.labelOrderNumber.Name = "labelOrderNumber";
            this.labelOrderNumber.Size = new System.Drawing.Size(55, 14);
            this.labelOrderNumber.TabIndex = 6;
            this.labelOrderNumber.Text = "label4";
            // 
            // ButtonDialogContentShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 367);
            this.Controls.Add(this.labelOrderNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelCurrentOrderQuantity);
            this.Controls.Add(this.radioButtonIsNotFull);
            this.Controls.Add(this.radioButtonIsFull);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewShowInfo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ButtonDialogContentShow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ButtonDialogContentShow";
            this.Load += new System.EventHandler(this.ButtonDialogContentShow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.ListView listViewShowInfo;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        public System.Windows.Forms.RadioButton radioButtonIsFull;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.RadioButton radioButtonIsNotFull;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label labelCurrentOrderQuantity;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label labelOrderNumber;
    }
}