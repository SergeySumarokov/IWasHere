
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
            this.ButtonRead = new System.Windows.Forms.Button();
            this.ButtonWrite = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.routeText = new System.Windows.Forms.TextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.clipboardText = new System.Windows.Forms.TextBox();
            this.ButtonReverse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonRead
            // 
            this.ButtonRead.Location = new System.Drawing.Point(321, 287);
            this.ButtonRead.Name = "ButtonRead";
            this.ButtonRead.Size = new System.Drawing.Size(75, 23);
            this.ButtonRead.TabIndex = 1;
            this.ButtonRead.Text = "Загрузить";
            this.ButtonRead.UseVisualStyleBackColor = true;
            this.ButtonRead.Click += new System.EventHandler(this.ButtonRead_Click);
            // 
            // ButtonWrite
            // 
            this.ButtonWrite.Location = new System.Drawing.Point(721, 287);
            this.ButtonWrite.Name = "ButtonWrite";
            this.ButtonWrite.Size = new System.Drawing.Size(75, 23);
            this.ButtonWrite.TabIndex = 2;
            this.ButtonWrite.Text = "Выгрузить";
            this.ButtonWrite.UseVisualStyleBackColor = true;
            this.ButtonWrite.Click += new System.EventHandler(this.ButtonWrite_Click);
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
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(587, 287);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(128, 21);
            this.comboBox1.TabIndex = 4;
            // 
            // routeText
            // 
            this.routeText.Location = new System.Drawing.Point(412, 25);
            this.routeText.Multiline = true;
            this.routeText.Name = "routeText";
            this.routeText.Size = new System.Drawing.Size(384, 256);
            this.routeText.TabIndex = 6;
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
            this.toolStripStatusLabel.Size = new System.Drawing.Size(781, 17);
            this.toolStripStatusLabel.Spring = true;
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // clipboardText
            // 
            this.clipboardText.Location = new System.Drawing.Point(12, 25);
            this.clipboardText.Multiline = true;
            this.clipboardText.Name = "clipboardText";
            this.clipboardText.ReadOnly = true;
            this.clipboardText.Size = new System.Drawing.Size(384, 256);
            this.clipboardText.TabIndex = 8;
            // 
            // ButtonReverse
            // 
            this.ButtonReverse.Location = new System.Drawing.Point(412, 287);
            this.ButtonReverse.Name = "ButtonReverse";
            this.ButtonReverse.Size = new System.Drawing.Size(75, 23);
            this.ButtonReverse.TabIndex = 9;
            this.ButtonReverse.Text = "Обратить";
            this.ButtonReverse.UseVisualStyleBackColor = true;
            this.ButtonReverse.Click += new System.EventHandler(this.ButtonReverse_Click);
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
            this.Controls.Add(this.ButtonReverse);
            this.Controls.Add(this.clipboardText);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.routeText);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButtonWrite);
            this.Controls.Add(this.ButtonRead);
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
        private System.Windows.Forms.Button ButtonRead;
        private System.Windows.Forms.Button ButtonWrite;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox routeText;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.TextBox clipboardText;
        private System.Windows.Forms.Button ButtonReverse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

