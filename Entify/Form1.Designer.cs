namespace Entify
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
            this.glassPane1 = new AeroGlass.GlassPane();
            this.listView1 = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.navigator1 = new Entify.Navigator();
            this.searchBox2 = new Entify.SearchBox();
            this.searchBox1 = new Entify.SearchBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // glassPane1
            // 
            this.glassPane1.AutoExtend = false;
            this.glassPane1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.glassPane1.DefaultColor = System.Drawing.Color.White;
            this.glassPane1.Dock = System.Windows.Forms.DockStyle.Top;
            this.glassPane1.Location = new System.Drawing.Point(0, 0);
            this.glassPane1.Name = "glassPane1";
            this.glassPane1.Size = new System.Drawing.Size(1109, 49);
            this.glassPane1.TabIndex = 8;
            // 
            // listView1
            // 
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.listView1.Location = new System.Drawing.Point(0, 49);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(212, 396);
            this.listView1.TabIndex = 10;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(212, 49);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(897, 396);
            this.panel1.TabIndex = 11;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(473, 361);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // navigator1
            // 
            this.navigator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.navigator1.Location = new System.Drawing.Point(13, 6);
            this.navigator1.Name = "navigator1";
            this.navigator1.Size = new System.Drawing.Size(61, 29);
            this.navigator1.TabIndex = 14;
            // 
            // searchBox2
            // 
            this.searchBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox2.BackColor = System.Drawing.Color.White;
            this.searchBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBox2.Location = new System.Drawing.Point(855, 12);
            this.searchBox2.Name = "searchBox2";
            this.searchBox2.Size = new System.Drawing.Size(242, 19);
            this.searchBox2.TabIndex = 12;
            // 
            // searchBox1
            // 
            this.searchBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox1.BackColor = System.Drawing.Color.White;
            this.searchBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchBox1.Location = new System.Drawing.Point(91, 12);
            this.searchBox1.Name = "searchBox1";
            this.searchBox1.Size = new System.Drawing.Size(758, 19);
            this.searchBox1.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1109, 445);
            this.Controls.Add(this.navigator1);
            this.Controls.Add(this.searchBox2);
            this.Controls.Add(this.searchBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.glassPane1);
            this.Name = "Form1";
            this.Text = "Entify";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AeroGlass.GlassPane glassPane1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Panel panel1;
        private SearchBox searchBox1;
        private Navigator navigator1;
        private System.Windows.Forms.Button button1;
        private SearchBox searchBox2;


    }
}

