namespace encryptTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String data = textBox1.Text;
            textBox2.Text = EncryptService.Encrypt(data);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String data = textBox1.Text;
            textBox2.Text = EncryptService.Decrypt(data);
        }
    }
}