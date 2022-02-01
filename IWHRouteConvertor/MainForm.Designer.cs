
namespace IWHRouteConvertor
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonRead = new System.Windows.Forms.Button();
            this.buttonWrite = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboFormatList = new System.Windows.Forms.ComboBox();
            this.textRoute = new System.Windows.Forms.TextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.textClipboard = new System.Windows.Forms.TextBox();
            this.buttonReverse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonRead
            // 
            this.buttonRead.Location = new System.Drawing.Point(321, 287);
            this.buttonRead.Name = "buttonRead";
            this.buttonRead.Size = new System.Drawing.Size(75, 23);
            this.buttonRead.TabIndex = 1;
            this.buttonRead.Text = "Загрузить";
            this.buttonRead.UseVisualStyleBackColor = true;
            this.buttonRead.Click += new System.EventHandler(this.ButtonRead_Click);
            // 
            // buttonWrite
            // 
            this.buttonWrite.Location = new System.Drawing.Point(721, 287);
            this.buttonWrite.Name = "buttonWrite";
            this.buttonWrite.Size = new System.Drawing.Size(75, 23);
            this.buttonWrite.TabIndex = 2;
            this.buttonWrite.Text = "Выгрузить";
            this.buttonWrite.UseVisualStyleBackColor = true;
            this.buttonWrite.Click += new System.EventHandler(this.ButtonWrite_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(532, 290);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Формат";
            // 
            // comboFormatList
            // 
            this.comboFormatList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFormatList.Location = new System.Drawing.Point(587, 287);
            this.comboFormatList.Name = "comboFormatList";
            this.comboFormatList.Size = new System.Drawing.Size(128, 21);
            this.comboFormatList.TabIndex = 4;
            // 
            // textRoute
            // 
            this.textRoute.Location = new System.Drawing.Point(412, 25);
            this.textRoute.Multiline = true;
            this.textRoute.Name = "textRoute";
            this.textRoute.Size = new System.Drawing.Size(384, 256);
            this.textRoute.TabIndex = 6;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 320);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(808, 22);
            this.statusStrip.TabIndex = 7;
            this.statusStrip.Text = "statusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(793, 17);
            this.toolStripStatusLabel.Spring = true;
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textClipboard
            // 
            this.textClipboard.Location = new System.Drawing.Point(12, 25);
            this.textClipboard.Multiline = true;
            this.textClipboard.Name = "textClipboard";
            this.textClipboard.ReadOnly = true;
            this.textClipboard.Size = new System.Drawing.Size(384, 256);
            this.textClipboard.TabIndex = 8;
            // 
            // buttonReverse
            // 
            this.buttonReverse.Location = new System.Drawing.Point(412, 287);
            this.buttonReverse.Name = "buttonReverse";
            this.buttonReverse.Size = new System.Drawing.Size(75, 23);
            this.buttonReverse.TabIndex = 9;
            this.buttonReverse.Text = "Обратить";
            this.buttonReverse.UseVisualStyleBackColor = true;
            this.buttonReverse.Click += new System.EventHandler(this.ButtonReverse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Буфер обмена";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(744, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Маршрут";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 342);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonReverse);
            this.Controls.Add(this.textClipboard);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.textRoute);
            this.Controls.Add(this.comboFormatList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonWrite);
            this.Controls.Add(this.buttonRead);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Преобразователь маршрутов";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonRead;
        private System.Windows.Forms.Button buttonWrite;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboFormatList;
        private System.Windows.Forms.TextBox textRoute;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.TextBox textClipboard;
        private System.Windows.Forms.Button buttonReverse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

