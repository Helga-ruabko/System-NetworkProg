namespace Lab4
{
    partial class Form1
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.btnAddToStart = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(1594, 679);
            this.webBrowser1.TabIndex = 0;
            // 
            // btnAddToStart
            // 
            this.btnAddToStart.Location = new System.Drawing.Point(793, 88);
            this.btnAddToStart.Name = "btnAddToStart";
            this.btnAddToStart.Size = new System.Drawing.Size(286, 78);
            this.btnAddToStart.TabIndex = 1;
            this.btnAddToStart.Text = "Add";
            this.btnAddToStart.UseVisualStyleBackColor = true;
            this.btnAddToStart.Click += new System.EventHandler(this.btnAddToStart_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(793, 193);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(286, 79);
            this.btnCopy.TabIndex = 2;
            this.btnCopy.Text = "Copy to file";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1594, 679);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnAddToStart);
            this.Controls.Add(this.webBrowser1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button btnAddToStart;
        private System.Windows.Forms.Button btnCopy;
    }
}

