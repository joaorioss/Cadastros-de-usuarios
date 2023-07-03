using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GravarDadosMySQL
{


    public partial class Form1 : Form
    {
        MySqlConnection Conexao;
        private string data_source = "datasource=localhost;username=root;password=;database=db_agenda";

        private int ?id_contato_selecionado = null;
        public Form1()
        {
            InitializeComponent();


            listaContatos.View = View.Details;
            listaContatos.LabelEdit = true;
            listaContatos.AllowColumnReorder = true;
            listaContatos.FullRowSelect = true;
            listaContatos.GridLines = true;

            listaContatos.Columns.Add("ID",30, HorizontalAlignment.Left);
            listaContatos.Columns.Add("nome", 150, HorizontalAlignment.Left);
            listaContatos.Columns.Add("email", 150, HorizontalAlignment.Left);
            listaContatos.Columns.Add("telefone", 150, HorizontalAlignment.Left);

            carregar_contatos();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ValidarEmail Valido = new ValidarEmail();

            if (String.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Preencha o nome do usuário","Dados Cadastrais",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (Valido.IsValidEmail(txtEmail.Text))
            {
                try
                {
                    Conexao = new MySqlConnection(data_source);

                    Conexao.Open();
                    //Criar conexão com MYSQL

                    MySqlCommand cmd = new MySqlCommand();

                    cmd.Connection = Conexao;
                    if (id_contato_selecionado == null)
                    {

                        cmd.CommandText = "INSERT INTO contato (nome, email, telefone) VALUES (@nome, @email, @telefone)";

                        cmd.Parameters.AddWithValue("@nome", txtNome.Text);
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@telefone", txtTelefone.Text);

                        cmd.Prepare();

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Deu tudo certo!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        //Att de contato
                        cmd.CommandText = "UPDATE contato SET nome=@nome, email=@email, telefone=@telefone WHERE id=@id";


                        cmd.Parameters.AddWithValue("@nome", txtNome.Text);
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@telefone", txtTelefone.Text);
                        cmd.Parameters.AddWithValue("@id", id_contato_selecionado);


                        cmd.Prepare();

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Deu tudo certo! Atualizado", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        button4.Visible = false;

                    }


                    id_contato_selecionado = null;
                    txtNome.Text = String.Empty;
                    txtEmail.Text = "";
                    txtTelefone.Text = "";


                    carregar_contatos();


                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);

                }
                catch (Exception ex)
                {

                    MessageBox.Show("Erro by Exception " + ex.Message);

                }
                finally { Conexao.Close(); }
            }
            else
            {
                MessageBox.Show("Email inválido", "Invalido", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                //Criar conexão com MYSQL
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conexao;
                cmd.CommandText = "SELECT * FROM contato WHERE nome LIKE @q OR email LIKE @q";
                cmd.Parameters.AddWithValue("@q", "%" + txtBuscar.Text + "%");


                cmd.Prepare();


                MySqlDataReader reader =  cmd.ExecuteReader();

                listaContatos.Items.Clear();

                while (reader.Read())
                {
                    string[] row =
                    {
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                    };
                    listaContatos.Items.Add(new ListViewItem(row));
                }
            }
            catch(Exception ex)
            {
                 MessageBox.Show(ex.Message);
            }finally {
                Conexao.Close();
            }
        }

        private void carregar_contatos()
        {
            try
            {

                //Criar conexão com MYSQL
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conexao;
                cmd.CommandText = "SELECT * FROM contato ORDER BY id DESC";


                cmd.Prepare();


                MySqlDataReader reader = cmd.ExecuteReader();

                listaContatos.Items.Clear();

                while (reader.Read())
                {
                    string[] row =
                    {
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                    };
                    listaContatos.Items.Add(new ListViewItem(row));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Conexao.Close();
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
                
        }

        private void listaContatos_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ListView.SelectedListViewItemCollection itens_selecionados = listaContatos.SelectedItems;
            foreach(ListViewItem item in itens_selecionados)
            {
                id_contato_selecionado = Convert.ToInt32(item.SubItems[0].Text);

                txtNome.Text = item.SubItems[1].Text;
                txtEmail.Text = item.SubItems[2].Text;
                txtTelefone.Text = item.SubItems[3].Text;

                button4.Visible = true;

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            id_contato_selecionado = null;
            txtNome.Text = String.Empty;
            txtEmail.Text = "";
            txtTelefone.Text = "";

            txtNome.Focus();
            button4.Visible = false;
        }

        private void listaContatos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            excluir_contato();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            excluir_contato();
        }

        private void excluir_contato()
        {
            try
            {
                DialogResult conf = MessageBox.Show("Você deseja excluir o registro?", "Ops, tem certeza?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (conf == DialogResult.Yes)
                {
                    //Excluir no BD
                    Conexao = new MySqlConnection(data_source);
                    Conexao.Open();

                    MySqlCommand cmd = new MySqlCommand();

                    cmd.Connection = Conexao;

                    cmd.CommandText = "DELETE FROM contato WHERE id = @id";

                    cmd.Parameters.AddWithValue("@id", id_contato_selecionado);

                    cmd.Prepare();

                    cmd.ExecuteNonQuery();



                    MessageBox.Show("Contato " + id_contato_selecionado + " excluído com sucesso!", "Sucesso", MessageBoxButtons.OK,
                        MessageBoxIcon.Asterisk);

                    carregar_contatos();

                    id_contato_selecionado = null;
                    txtNome.Text = String.Empty;
                    txtEmail.Text = "";
                    txtTelefone.Text = "";

                    button4.Visible = false;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Conexao.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                Conexao = new MySqlConnection(data_source);

                Conexao.Open();

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = Conexao;

                cmd.CommandText = "SET @count = 0";

                cmd.CommandText = "UPDATE contato SET contato.id = @count:= @count + 1";

                MessageBox.Show("???");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Conexao.Close();
            }
        }

        private void txtTelefone_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTelefone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8)
            {
                e.Handled = true;

            }
        }

        private void txtNome_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*if (!(Char.IsLetter(e.KeyChar) || Char.IsControl(e.KeyChar)))
                e.Handled = true;*/
        }

        private void txtEmail_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
