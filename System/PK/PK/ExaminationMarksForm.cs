﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PK
{
    partial class ExaminationMarksForm : Form
    {
        readonly DB_Connector _DB_Connection;
        readonly uint _ExaminationID;

        public ExaminationMarksForm(DB_Connector connection, uint examinationID)
        {
            #region Components
            InitializeComponent();

            dataGridView_UID.ValueType = typeof(uint);
            dataGridView_Mark.ValueType = typeof(sbyte);
            #endregion

            _DB_Connection = connection;
            _ExaminationID = examinationID;

            object[] exam = _DB_Connection.Select(
                DB_Table.EXAMINATIONS,
                new string[] { "subject_dict_id", "subject_id", "date" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("id", Relation.EQUAL,examinationID)
                })[0];
            Text = _DB_Connection.Select(
                DB_Table.DICTIONARIES_ITEMS,
                new string[] { "name" },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("dictionary_id",Relation.EQUAL,exam[0]),
                    new Tuple<string, Relation, object>("item_id",Relation.EQUAL,exam[1]),
                }
                )[0][0] + " " + ((DateTime)exam[2]).ToShortDateString();

            foreach (object[] row in _DB_Connection.Select(
                DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                new string[] { dataGridView_UID.DataPropertyName, dataGridView_Mark.DataPropertyName },
                new List<Tuple<string, Relation, object>>
                {
                    new Tuple<string, Relation, object>("examination_id", Relation.EQUAL,examinationID)
                }
                ))
            {
                object[] entrant = _DB_Connection.Select(
                    DB_Table.ENTRANTS,
                    new string[] { "last_name", "first_name", "middle_name" },
                    new List<Tuple<string, Relation, object>> { new Tuple<string, Relation, object>("uid", Relation.EQUAL, row[0]) }
                    )[0];
                dataGridView.Rows.Add(row[0], entrant[0].ToString() + " " + entrant[1].ToString() + " " + entrant[2].ToString(), row[1]);
            }
        }

        private void toolStrip_Print_Click(object sender, EventArgs e)
        {
            DocumentCreator.Create(_DB_Connection, "D:\\Dmitry\\Documents\\GitHub\\pk\\System\\DocumentTemplates\\AlphaMarks.xml", "AlphaMarks", _ExaminationID);
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
                _DB_Connection.Update(DB_Table.ENTRANTS_EXAMINATIONS_MARKS,
                    new Dictionary<string, object>
                    {
                        {dataGridView_Mark.DataPropertyName,dataGridView[e.ColumnIndex,e.RowIndex].Value }
                    },
                    new Dictionary<string, object>
                    {
                        {dataGridView_UID.DataPropertyName,dataGridView[0,e.RowIndex].Value },
                        { "examination_id", _ExaminationID}
                    }
                    );
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Некорректные данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}