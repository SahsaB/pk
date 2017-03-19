﻿using System;
using System.Windows.Forms;

namespace PK
{
    public partial class MADIOlimpsForm : Form
    {
        DB_Connector _DB_Connection;
        NewApplicForm _Parent;

        public MADIOlimpsForm(NewApplicForm parent)
        {
            InitializeComponent();

            _DB_Connection = new DB_Connector();
            _Parent = parent;
            cbOlympType.SelectedIndex = 0;
            cbDiplomaType.DataSource = Utility.GetDictionaryItems(_DB_Connection, 18);
            cbDiplomaType.SelectedIndex = -1;            
            
            cbOlympProfile.DataSource = Utility.GetDictionaryItems(_DB_Connection, 39);
            cbOlympProfile.SelectedIndex = -1;
            cbClass.SelectedItem = "10";
            cbDiscipline.DataSource = Utility.GetDictionaryItems(_DB_Connection, 1);
            cbDiscipline.SelectedIndex = -1;
            cbContry.DataSource = Utility.GetDictionaryItems(_DB_Connection,7);

            if ((_Parent.OlympicDoc.olympType!=null)&&(_Parent.OlympicDoc.olympType !=""))
            {
                cbOlympType.SelectedItem = _Parent.OlympicDoc.olympType;
                tbOlympName.Text = _Parent.OlympicDoc.olympName;
                tbDocNumber.Text = _Parent.OlympicDoc.olympDocNumber.ToString();
                cbDiplomaType.SelectedItem = _Parent.OlympicDoc.diplomaType;
                cbOlympProfile.SelectedItem = _Parent.OlympicDoc.olympProfile;
                cbClass.SelectedItem = _Parent.OlympicDoc.olympClass.ToString();
                cbDiscipline.SelectedItem = _Parent.OlympicDoc.olympDist;
                cbContry.SelectedItem = _Parent.OlympicDoc.country;
            }
        }

        private void cbOlympType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbOlympType.SelectedItem.ToString() == "Диплом победителя/призера олимпиады школьников")
            {
                tbOlympName.Enabled = false;
                label2.Enabled = false;
                tbDocNumber.Enabled = false;
                label3.Enabled = false;
                cbDiplomaType.Enabled = true;
                label4.Enabled = true;
                cbOlympID.Enabled = true;
                label5.Enabled = true;
                cbClass.Enabled = true;
                label7.Enabled = true;
                cbDiscipline.Enabled = true;
                label8.Enabled = true;
                cbContry.Enabled = false;
                label9.Enabled = false;
            }
            else if (cbOlympType.SelectedItem.ToString() == "Диплом победителя/призера всероссийской олимпиады школьников")
            {
                tbOlympName.Enabled = false;
                label2.Enabled = false;
                tbDocNumber.Enabled = true;
                label3.Enabled = true;
                cbDiplomaType.Enabled = true;
                label4.Enabled = true;
                cbOlympID.Enabled = true;
                label5.Enabled = true;
                cbClass.Enabled = true;
                label7.Enabled = true;
                cbDiscipline.Enabled = true;
                label8.Enabled = true;
                cbContry.Enabled = false;
                label9.Enabled = false;
            }
            else if (cbOlympType.SelectedItem.ToString() == "Диплом 4 этапа всеукраинской олимпиады")
            {
                tbOlympName.Enabled = true;
                label2.Enabled = true;
                tbDocNumber.Enabled = true;
                label3.Enabled = true;
                cbDiplomaType.Enabled = true;
                label4.Enabled = true;
                cbOlympID.Enabled = false;
                label5.Enabled = false;
                cbClass.Enabled = false;
                label7.Enabled = false;
                cbDiscipline.Enabled = false;
                label8.Enabled = false;
                cbContry.Enabled = false;
                label9.Enabled = false;
            }
            else if (cbOlympType.SelectedItem.ToString() == "Диплом международной олимпиады")
            {
                tbOlympName.Enabled = true;
                label2.Enabled = true;
                tbDocNumber.Enabled = true;
                label3.Enabled = true;
                cbDiplomaType.Enabled = false;
                label4.Enabled = false;
                cbOlympID.Enabled = false;
                label5.Enabled = false;
                cbClass.Enabled = false;
                label7.Enabled = false;
                cbDiscipline.Enabled = false;
                label8.Enabled = false;
                cbContry.Enabled = true;
                label9.Enabled = true;
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            _Parent.OlympicDoc.olympType = "";
            _Parent.OlympicDoc.olympName = "";
            _Parent.OlympicDoc.diplomaType = "";
            _Parent.OlympicDoc.olympDocNumber = 0;
            _Parent.OlympicDoc.diplomaType = "";
            //_Parent.OlympicDoc.olympID
            _Parent.OlympicDoc.olympProfile = "";
            _Parent.OlympicDoc.olympClass = 0;
            _Parent.OlympicDoc.olympDist = "";
            _Parent.OlympicDoc.country = "";

            bool saved = false;
            if ((cbOlympType.SelectedIndex == -1))
                MessageBox.Show("Выберите тип олимпиады");
            else
                switch (cbOlympType.SelectedItem.ToString())
                {
                    case "Диплом победителя/призера олимпиады школьников":
                        if ((cbDiplomaType.SelectedIndex == -1) || (cbOlympProfile.SelectedIndex == -1)
                            || (cbClass.SelectedIndex == -1) || (cbDiscipline.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            _Parent.OlympicDoc.olympType = cbOlympType.SelectedItem.ToString();
                            _Parent.OlympicDoc.diplomaType = cbDiplomaType.SelectedItem.ToString();
                            //_Parent.OlympicDoc.olympID = int.Parse(cbOlympID);
                            _Parent.OlympicDoc.olympProfile = cbOlympProfile.SelectedItem.ToString();
                            _Parent.OlympicDoc.olympClass = int.Parse(cbClass.SelectedItem.ToString());
                            _Parent.OlympicDoc.olympDist = cbDiscipline.SelectedItem.ToString();
                            saved = true;
                        }                        
                        break;
                    case "Диплом победителя/призера всероссийской олимпиады школьников":
                        if ((tbDocNumber.Text == "") || (cbDiplomaType.SelectedIndex == -1) || (cbOlympProfile.SelectedIndex == -1)
                            || (cbClass.SelectedIndex == -1) || (cbDiscipline.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            _Parent.OlympicDoc.olympType = cbOlympType.SelectedItem.ToString();
                            _Parent.OlympicDoc.olympDocNumber=int.Parse(tbDocNumber.Text);
                            _Parent.OlympicDoc.diplomaType = cbDiplomaType.SelectedItem.ToString();
                            _Parent.OlympicDoc.olympProfile = cbOlympProfile.SelectedItem.ToString();
                            _Parent.OlympicDoc.olympClass = int.Parse(cbClass.SelectedItem.ToString());
                            _Parent.OlympicDoc.olympDist = cbDiscipline.SelectedItem.ToString();
                            saved = true;
                        }                            
                        break;
                    case "Диплом 4 этапа всеукраинской олимпиады":
                        if ((tbOlympName.Text == "") || (tbDocNumber.Text == "") || (cbDiplomaType.SelectedIndex == -1)
                            || (cbOlympProfile.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            _Parent.OlympicDoc.olympType = cbOlympType.SelectedItem.ToString();
                            _Parent.OlympicDoc.olympName = tbOlympName.Text;
                            _Parent.OlympicDoc.olympDocNumber = int.Parse(tbDocNumber.Text);
                            _Parent.OlympicDoc.diplomaType = cbDiplomaType.SelectedItem.ToString();
                            _Parent.OlympicDoc.olympProfile = cbOlympProfile.SelectedItem.ToString();
                            saved = true;
                        }                            
                        break;
                    case "Диплом международной олимпиады":
                        if ((tbOlympName.Text == "") || (tbDocNumber.Text == "") || (cbOlympProfile.SelectedIndex == -1)
                            || (cbContry.SelectedIndex == -1))
                            MessageBox.Show("Все доступные поля должны быть заполнены");
                        else
                        {
                            _Parent.OlympicDoc.olympType = cbOlympType.SelectedItem.ToString();
                            _Parent.OlympicDoc.olympName = tbOlympName.Text;
                            _Parent.OlympicDoc.olympDocNumber = int.Parse(tbDocNumber.Text);
                            _Parent.OlympicDoc.olympProfile = cbOlympProfile.SelectedItem.ToString();
                            _Parent.OlympicDoc.country = cbContry.SelectedItem.ToString();
                            saved = true;
                        }                            
                        break;
                }
            if (saved)
                DialogResult = DialogResult.OK;
        }
    }
}