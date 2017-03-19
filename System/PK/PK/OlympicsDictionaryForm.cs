﻿using System.Windows.Forms;

namespace PK
{
    partial class OlympicsDictionaryForm : Form
    {
        DB_Connector _DB_Connection;
        FIS_Connector _FIS_Connection;
        DataManager _DataManager;

        public OlympicsDictionaryForm(DB_Connector dbConnection)
        {
            InitializeComponent();

            _DB_Connection = dbConnection;

            try
            {
                _FIS_Connection = new FIS_Connector("XXXX", "****");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка подключения к ФИС", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            _DataManager = new DataManager(_DB_Connection, _FIS_Connection);

            UpdateOlympicsTable();
        }

        void UpdateOlympicsTable()
        {
            dgvOlympics.Rows.Clear();
            foreach (object[] olymp in _DB_Connection.Select(DB_Table.DICTIONARY_19_ITEMS))
                dgvOlympics.Rows.Add(olymp[0], olymp[1], olymp[2]);
        }

        private void dgvOlympics_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            dgvProfiles.Rows.Clear();
            foreach (object[] prof in _DB_Connection.Select(
                DB_Table.DICTIONARY_OLYMPIC_PROFILES,
                new string[] { "profile_dict_id", "profile_id", "level_dict_id", "level_id" },
                new System.Collections.Generic.List<System.Tuple<string, Relation, object>>
                {
                    new System.Tuple<string, Relation, object> ("olympic_id", Relation.EQUAL, dgvOlympics.Rows[e.RowIndex].Cells[0].Value)
                }))
                dgvProfiles.Rows.Add(
                    prof[0],
                    prof[1],
                    _DB_Connection.Select(
                        DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                        new System.Collections.Generic.List<System.Tuple<string, Relation, object>>
                        {
                            new System.Tuple<string, Relation, object>("dictionary_id",Relation.EQUAL,prof[0]),
                            new System.Tuple<string, Relation, object>("item_id",Relation.EQUAL,prof[1])
                        })[0][0],
                    _DB_Connection.Select(
                        DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                        new System.Collections.Generic.List<System.Tuple<string, Relation, object>>
                        {
                            new System.Tuple<string, Relation, object>("dictionary_id",Relation.EQUAL,prof[2]),
                            new System.Tuple<string, Relation, object>("item_id",Relation.EQUAL,prof[3])
                        })[0][0]);
        }

        private void dgvProfiles_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            lbSubjects.Items.Clear();
            if (dgvOlympics.CurrentRow != null)
                foreach (object[] subj in _DB_Connection.Select(
                    DB_Table._DICTIONARY_OLYMPIC_PROFILES_HAS_DICTIONARIES_ITEMS,
                    new string[] { "dictionaries_items_dictionary_id", "dictionaries_items_item_id" },
                    new System.Collections.Generic.List<System.Tuple<string, Relation, object>>
                    {
                        new System.Tuple<string, Relation, object> ("dictionary_olympic_profiles_olympic_id", Relation.EQUAL, dgvOlympics.CurrentRow.Cells[0].Value),
                        new System.Tuple<string, Relation, object> ("dictionary_olympic_profiles_profile_dict_id", Relation.EQUAL, dgvProfiles.Rows[e.RowIndex].Cells[0].Value),
                        new System.Tuple<string, Relation, object> ("dictionary_olympic_profiles_profile_id", Relation.EQUAL, dgvProfiles.Rows[e.RowIndex].Cells[1].Value)
                    }))
                    lbSubjects.Items.Add(
                        _DB_Connection.Select(
                            DB_Table.DICTIONARIES_ITEMS, new string[] { "name" },
                            new System.Collections.Generic.List<System.Tuple<string, Relation, object>>
                            {
                                new System.Tuple<string, Relation, object>("dictionary_id",Relation.EQUAL,subj[0]),
                                new System.Tuple<string, Relation, object>("item_id",Relation.EQUAL,subj[1])
                            })[0][0]);
        }

        private void toolStrip_Update_Click(object sender, System.EventArgs e)
        {
            _DataManager.UpdateOlympicsDictionary();

            UpdateOlympicsTable();
        }
    }
}
