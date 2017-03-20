namespace IWHMap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MercatorMap = new IWHMap.MercatorPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.MercatorMap)).BeginInit();
            this.SuspendLayout();
            // 
            // MercatorMap
            // 
            this.MercatorMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MercatorMap.Image = ((System.Drawing.Image)(resources.GetObject("MercatorMap.Image")));
            this.MercatorMap.Location = new System.Drawing.Point(0, 0);
            this.MercatorMap.Name = "MercatorMap";
            this.MercatorMap.MapImage = null;
            this.MercatorMap.Size = new System.Drawing.Size(400, 330);
            this.MercatorMap.TabIndex = 0;
            this.MercatorMap.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 330);
            this.Controls.Add(this.MercatorMap);
            this.Name = "MainForm";
            this.Text = "IWasHere Map";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MercatorMap)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MercatorPictureBox MercatorMap;
    }
}

