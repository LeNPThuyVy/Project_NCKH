namespace QLBGX
{
    partial class QLBGX
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
            this.btnQLX = new System.Windows.Forms.Button();
            this.btnQLRaVao = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnQLX
            // 
            this.btnQLX.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQLX.Image = global::QLBGX.Properties.Resources.icons8_car_24;
            this.btnQLX.Location = new System.Drawing.Point(692, 216);
            this.btnQLX.Margin = new System.Windows.Forms.Padding(0);
            this.btnQLX.Name = "btnQLX";
            this.btnQLX.Padding = new System.Windows.Forms.Padding(0, 50, 0, 0);
            this.btnQLX.Size = new System.Drawing.Size(381, 201);
            this.btnQLX.TabIndex = 0;
            this.btnQLX.Text = "QUẢN LÍ XE";
            this.btnQLX.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnQLX.UseVisualStyleBackColor = true;
            this.btnQLX.Click += new System.EventHandler(this.btnQLX_Click);
            // 
            // btnQLRaVao
            // 
            this.btnQLRaVao.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQLRaVao.Image = global::QLBGX.Properties.Resources.icons8_monitor_24;
            this.btnQLRaVao.Location = new System.Drawing.Point(108, 216);
            this.btnQLRaVao.Margin = new System.Windows.Forms.Padding(0);
            this.btnQLRaVao.Name = "btnQLRaVao";
            this.btnQLRaVao.Padding = new System.Windows.Forms.Padding(0, 50, 0, 0);
            this.btnQLRaVao.Size = new System.Drawing.Size(381, 201);
            this.btnQLRaVao.TabIndex = 0;
            this.btnQLRaVao.Text = "QUẢN LÍ RA-VÀO";
            this.btnQLRaVao.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnQLRaVao.UseVisualStyleBackColor = true;
            this.btnQLRaVao.Click += new System.EventHandler(this.btnQLRaVao_Click);
            // 
            // QLBGX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 703);
            this.Controls.Add(this.btnQLX);
            this.Controls.Add(this.btnQLRaVao);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "QLBGX";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QLBGX";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnQLRaVao;
        private System.Windows.Forms.Button btnQLX;
    }
}

