using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ISProjectSchool.DataBase
{
    public class ComboBoxFiller
    {
        private readonly SqlConnection connection;
        private readonly string query;

        public ComboBoxFiller(SqlConnection connection, string query)
        {
            this.connection = connection;
            this.query = query;
        }
        public void FillComboBox(ComboBox comboBox, string displayMember, string idMember)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string item = reader.GetString(reader.GetOrdinal(displayMember));
                        int id = reader.GetInt32(reader.GetOrdinal(idMember));
                        ComboBoxItem comboBoxItem = new ComboBoxItem()
                        {
                            Content = item,
                            Tag = id,
                        };
                        comboBox.Items.Add(comboBoxItem);
                    }
                }
            }
        }
    }

}