﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace PK.Forms
{
    partial class Orders : Form
    {
        private string SelectedOrderNumber
        {
            get { return dataGridView.SelectedRows[0].Cells[dataGridView_Number.Index].Value.ToString(); }
        }

        private readonly Dictionary<string, string> _OrderTypes = new Dictionary<string, string>
        {
            { "admission" ,"Зачисление" },
            { "exception" ,"Отчисление" },
            { "hostel" ,"Выделение мест в общежитии" }
        };

        private readonly Classes.DB_Connector _DB_Connection;
        private readonly Classes.DB_Helper _DB_Helper;

        public Orders(Classes.DB_Connector connection)
        {
            InitializeComponent();

            _DB_Connection = connection;
            _DB_Helper = new Classes.DB_Helper(_DB_Connection);

            UpdateTable();
        }

        private void toolStrip_New_Click(object sender, EventArgs e)
        {
            OrderEdit form = new OrderEdit(_DB_Connection, null);
            form.ShowDialog();

            UpdateTable();
        }

        private void toolStrip_Edit_Click(object sender, EventArgs e)
        {
            OrderEdit form = new OrderEdit(_DB_Connection, SelectedOrderNumber);
            form.ShowDialog();

            UpdateTable();
        }

        private void toolStrip_Delete_Click(object sender, EventArgs e)
        {
            if (Classes.Utility.ShowUnrevertableActionMessageBox())
            {
                _DB_Connection.Delete(DB_Table.ORDERS, new Dictionary<string, object> { { "number", SelectedOrderNumber } });
                UpdateTable();
            }
        }

        private void toolStrip_Register_Click(object sender, EventArgs e)
        {
            OrderRegistration form = new OrderRegistration(_DB_Connection, SelectedOrderNumber);
            if (form.ShowDialog() == DialogResult.OK)
            {
                dataGridView.SelectedRows[0].Cells[dataGridView_ProtNumber.Index].Value = _DB_Connection.Select(
                    DB_Table.ORDERS,
                    new string[] { "protocol_number" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("number", Relation.EQUAL, SelectedOrderNumber) }
                    )[0][0];

                toolStrip_Edit.Enabled = false;
                toolStrip_Delete.Enabled = false;
                toolStrip_Register.Enabled = false;
                toolStrip_Print.Enabled = true;
            }
        }

        private void toolStrip_Print_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Classes.OutDocuments.Order(_DB_Connection, SelectedOrderNumber);
            Cursor.Current = Cursors.Default;
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Cells[dataGridView_ProtNumber.Index].Value != null)
            {
                MessageBox.Show("Невозможно удалить зарегестрированный приказ.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            else if (Classes.Utility.ShowUnrevertableActionMessageBox())
                _DB_Connection.Delete(DB_Table.ORDERS, new Dictionary<string, object> { { "number", e.Row.Cells[dataGridView_Number.Index].Value } });
            else
                e.Cancel = true;
        }

        private void dataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            bool registered = dataGridView[dataGridView_ProtNumber.Index, e.RowIndex].Value != null;
            toolStrip_Edit.Enabled = !registered;
            toolStrip_Delete.Enabled = !registered;
            toolStrip_Register.Enabled = !registered;
            toolStrip_Print.Enabled = registered;
        }

        private void UpdateTable()
        {
            dataGridView.Rows.Clear();

            dataGridView.Rows.Clear();
            foreach (object[] row in _DB_Connection.Select(
                DB_Table.ORDERS,
                new string[] { "number", "type", "date", "protocol_number" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("campaign_id",Relation.EQUAL,_DB_Helper.CurrentCampaignID)
                }))
                dataGridView.Rows.Add(
                    row[0],
                    _OrderTypes[row[1].ToString()],
                    ((DateTime)row[2]).ToShortDateString(),
                    row[3] as ushort?
                    );
        }
    }
}
