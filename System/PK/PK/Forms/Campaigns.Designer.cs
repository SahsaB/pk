﻿namespace PK.Forms
{
    partial class Campaigns
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
            this.dgvCampaigns = new System.Windows.Forms.DataGridView();
            this.dgvCampaigns_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCampaigns_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCampaigns_Timing = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCampaigns_EduLevels = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCampaigns_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btCreatePriemComp = new System.Windows.Forms.Button();
            this.btUpdate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbCurrentCampaign = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCampaigns)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvCampaigns
            // 
            this.dgvCampaigns.AllowUserToAddRows = false;
            this.dgvCampaigns.AllowUserToDeleteRows = false;
            this.dgvCampaigns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCampaigns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvCampaigns_ID,
            this.dgvCampaigns_Name,
            this.dgvCampaigns_Timing,
            this.dgvCampaigns_EduLevels,
            this.dgvCampaigns_Status});
            this.dgvCampaigns.Location = new System.Drawing.Point(12, 76);
            this.dgvCampaigns.MultiSelect = false;
            this.dgvCampaigns.Name = "dgvCampaigns";
            this.dgvCampaigns.ReadOnly = true;
            this.dgvCampaigns.RowHeadersVisible = false;
            this.dgvCampaigns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCampaigns.Size = new System.Drawing.Size(715, 361);
            this.dgvCampaigns.TabIndex = 0;
            this.dgvCampaigns.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCampaigns_CellDoubleClick);
            // 
            // dgvCampaigns_ID
            // 
            this.dgvCampaigns_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvCampaigns_ID.HeaderText = "ID";
            this.dgvCampaigns_ID.Name = "dgvCampaigns_ID";
            this.dgvCampaigns_ID.ReadOnly = true;
            this.dgvCampaigns_ID.Visible = false;
            // 
            // dgvCampaigns_Name
            // 
            this.dgvCampaigns_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvCampaigns_Name.HeaderText = "Название";
            this.dgvCampaigns_Name.Name = "dgvCampaigns_Name";
            this.dgvCampaigns_Name.ReadOnly = true;
            // 
            // dgvCampaigns_Timing
            // 
            this.dgvCampaigns_Timing.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.dgvCampaigns_Timing.HeaderText = "Сроки проведения";
            this.dgvCampaigns_Timing.Name = "dgvCampaigns_Timing";
            this.dgvCampaigns_Timing.ReadOnly = true;
            this.dgvCampaigns_Timing.Width = 115;
            // 
            // dgvCampaigns_EduLevels
            // 
            this.dgvCampaigns_EduLevels.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvCampaigns_EduLevels.HeaderText = "Уровни образования";
            this.dgvCampaigns_EduLevels.Name = "dgvCampaigns_EduLevels";
            this.dgvCampaigns_EduLevels.ReadOnly = true;
            this.dgvCampaigns_EduLevels.Width = 127;
            // 
            // dgvCampaigns_Status
            // 
            this.dgvCampaigns_Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvCampaigns_Status.HeaderText = "Статус";
            this.dgvCampaigns_Status.Name = "dgvCampaigns_Status";
            this.dgvCampaigns_Status.ReadOnly = true;
            this.dgvCampaigns_Status.Width = 66;
            // 
            // btCreatePriemComp
            // 
            this.btCreatePriemComp.Location = new System.Drawing.Point(138, 42);
            this.btCreatePriemComp.Name = "btCreatePriemComp";
            this.btCreatePriemComp.Size = new System.Drawing.Size(171, 23);
            this.btCreatePriemComp.TabIndex = 1;
            this.btCreatePriemComp.Text = "Создать приемную кампанию";
            this.btCreatePriemComp.UseVisualStyleBackColor = true;
            this.btCreatePriemComp.Click += new System.EventHandler(this.btCreatePriemComp_Click);
            // 
            // btUpdate
            // 
            this.btUpdate.Location = new System.Drawing.Point(439, 42);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(142, 23);
            this.btUpdate.TabIndex = 2;
            this.btUpdate.Text = "Изменить выделенную";
            this.btUpdate.UseVisualStyleBackColor = true;
            this.btUpdate.Click += new System.EventHandler(this.btUpdate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(164, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Текущая приемная кампания:";
            // 
            // cbCurrentCampaign
            // 
            this.cbCurrentCampaign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCurrentCampaign.FormattingEnabled = true;
            this.cbCurrentCampaign.Location = new System.Drawing.Point(331, 12);
            this.cbCurrentCampaign.Name = "cbCurrentCampaign";
            this.cbCurrentCampaign.Size = new System.Drawing.Size(224, 21);
            this.cbCurrentCampaign.TabIndex = 4;
            this.cbCurrentCampaign.SelectionChangeCommitted += new System.EventHandler(this.cbCurrentCampaign_SelectionChangeCommitted);
            // 
            // Campaigns
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 449);
            this.Controls.Add(this.cbCurrentCampaign);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btUpdate);
            this.Controls.Add(this.btCreatePriemComp);
            this.Controls.Add(this.dgvCampaigns);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::PK.Properties.Resources.logo;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Campaigns";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Приемные кампании";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCampaigns)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCampaigns;
        private System.Windows.Forms.Button btCreatePriemComp;
        private System.Windows.Forms.Button btUpdate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCampaigns_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCampaigns_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCampaigns_Timing;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCampaigns_EduLevels;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCampaigns_Status;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbCurrentCampaign;
    }
}