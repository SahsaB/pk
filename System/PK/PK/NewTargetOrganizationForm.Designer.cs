﻿namespace PK
{
    partial class NewTargetOrganizationForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.rtbOrganizationName = new System.Windows.Forms.RichTextBox();
            this.btSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(103, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Название целевой организации:";
            // 
            // rtbOrganizationName
            // 
            this.rtbOrganizationName.Location = new System.Drawing.Point(12, 56);
            this.rtbOrganizationName.Name = "rtbOrganizationName";
            this.rtbOrganizationName.Size = new System.Drawing.Size(343, 64);
            this.rtbOrganizationName.TabIndex = 1;
            this.rtbOrganizationName.Text = "";
            // 
            // btSave
            // 
            this.btSave.Location = new System.Drawing.Point(152, 129);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 2;
            this.btSave.Text = "Сохранить";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // NewTargetOrganizationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 164);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.rtbOrganizationName);
            this.Controls.Add(this.label1);
            this.Name = "NewTargetOrganizationForm";
            this.Text = "Создание целевой организации";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtbOrganizationName;
        private System.Windows.Forms.Button btSave;
    }
}