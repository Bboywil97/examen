using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO.Ports;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        delegate void SetTextDelegate(string value);
        public SerialPort ArduinoPort
        {
            get;
        }
        string conexionSQL = "Server=localhost;port=3306;database=exapractico2;Uid=root;pwd=Balboa97*;";
        DateTime fecha = DateTime.Now;
        public Form1()
        {
            InitializeComponent();
            ArduinoPort = new System.IO.Ports.SerialPort();
            ArduinoPort.PortName = "COM4";
            ArduinoPort.BaudRate = 9600;
            ArduinoPort.DataBits = 8;
            ArduinoPort.ReadTimeout = 500;
            ArduinoPort.WriteTimeout = 500;
            ArduinoPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }
        
        private void InsertarRegistro(string nombre, string fecha)
        {
            using (MySqlConnection conection = new MySqlConnection(conexionSQL))
            {
                conection.Open();
                string insertQuery = "INSERT INTO usuario (nombre,fecha)" +
                    "Values (@nombre,@fecha)";

                using (MySqlCommand command = new MySqlCommand(insertQuery, conection))
                {
                    command.Parameters.AddWithValue("@nombre", nombre);
                    command.Parameters.AddWithValue("@fecha", fecha.ToString());

                    command.ExecuteNonQuery();
                }
                conection.Close();
            }
        }
        private void InsertarRegistro2(string TempFahrenheit)
        {
            using (MySqlConnection conection = new MySqlConnection(conexionSQL))
            {
                conection.Open();
                string insertQuery2 = "INSERT INTO temperatura (TempFahrenheit) " +
                    "VALUES (@TempFahrenheit)";

                using (MySqlCommand command = new MySqlCommand(insertQuery2, conection))
                {
                    command.Parameters.AddWithValue("@TempFahrenheit", TempFahrenheit);

                    command.ExecuteNonQuery();
                }
                conection.Close();
            }
        }
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string dato = sp.ReadLine();

            if (dato.StartsWith("TEMP:"))
            {
                dato = dato.Replace("TEMP:", ""); // Asegurarse de tomar solo el valor numérico
                EscribirTxt(dato);
            }
        }


        private void EscribirTxt(string dato)
        {
            if (InvokeRequired)
                try
                {
                    Invoke(new SetTextDelegate(EscribirTxt), dato);
                }
                catch { }
            else
                lblTemp.Text = dato;
        }


        private void button1_Click(object sender, EventArgs e)
        {

            string tempFahrenheit = lblTemp.Text;
            InsertarRegistro2(tempFahrenheit); 

            string nombre = textNombre.Text;
            string fecha = textFecha.Text;
            string fechaString = fecha.ToString();
            InsertarRegistro(nombre, fechaString);
        }
    }
}
