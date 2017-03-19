﻿using System;
using System.Windows.Forms;

namespace PK
{
    public partial class SportDocsForm : Form
    {
        DB_Connector _DB_Connection;
        NewApplicForm _Parent;

        public SportDocsForm(NewApplicForm parent)
        {
            InitializeComponent();

            _DB_Connection = new DB_Connector();
            _Parent = parent;
            cbDocType.DataSource = Utility.GetDictionaryItems(_DB_Connection, 43);
            cbDocType.SelectedIndex = 0;

            if ((_Parent.SportDoc.diplomaType!=null)&&(_Parent.SportDoc.diplomaType!=""))
            {
                cbDocType.SelectedItem = _Parent.SportDoc.diplomaType;
                tbDocName.Text = _Parent.SportDoc.docName;
                dtpDocDate.Value = _Parent.SportDoc.docDate;
                tbOrgName.Text = _Parent.SportDoc.orgName;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if ((cbDocType.SelectedIndex == -1) || (tbDocName.Text == "") || (tbOrgName.Text == ""))
                MessageBox.Show("Все поля должны быть заполнены");
            else
            {
                _Parent.SportDoc.diplomaType = cbDocType.SelectedItem.ToString();
                _Parent.SportDoc.docName = tbDocName.Text;
                _Parent.SportDoc.docDate = dtpDocDate.Value;
                _Parent.SportDoc.orgName = tbOrgName.Text;

                DialogResult = DialogResult.OK;
            }
        }
    }
}