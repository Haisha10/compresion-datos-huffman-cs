using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace compresion_datos_huffman_cs
{
    public partial class frmMain : Form
    {
		public HuffmanTree huff;
		public string inputFileName = "";
		public string outputFileName = "";
		public frmMain()
        {
            InitializeComponent();
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
			Stream input;
			using (OpenFileDialog ofd = new OpenFileDialog())
            {
				try
				{
					ofd.InitialDirectory = ".\\";
					ofd.Filter = "txt & huff files (*.txt, *.huff)|*.txt;*.huff";
					ofd.RestoreDirectory = true;
					if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						if ((input = ofd.OpenFile()) != null)
						{
							tbxPath.Text = ofd.FileName;
							inputFileName = ofd.FileName;
							if (inputFileName.EndsWith(".TXT") || inputFileName.EndsWith(".txt"))
							{
								try
								{
									btnCompress.Enabled = true;
									btnDecompress.Enabled = false;
									tbxInput.Text = File.ReadAllText(ofd.FileName);
									tbxOutput.Clear();
								}
								finally
								{
									input.Close();
								}
							}
							else if (inputFileName.EndsWith(".huff") || inputFileName.EndsWith(".HUFF"))
							{
								try
								{
									input.Close();
									btnCompress.Enabled = false;
									btnDecompress.Enabled = true;
									huff = new HuffmanTree();
									huff.readBinary(inputFileName);
									tbxInput.Text = huff.huffmanCode.ToString();
									tbxOutput.Clear();
								}
								finally
								{
									input.Close();
								}
							}

						}
						else
						{
							MessageBox.Show("Error al cargar el archivo.");
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
			}
			
		}

        private void btnCompress_Click(object sender, EventArgs e)
        {
			huff = new HuffmanTree();
			string fileContent = tbxInput.Text.Trim();
			huff.getListFromFile(tbxInput.Text);
			tbxOutput.Text=huff.huffmanCode.ToString();
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
			using (SaveFileDialog sfd = new SaveFileDialog())
            {
				try
				{
					sfd.InitialDirectory = ".\\";
					if (inputFileName.EndsWith(".txt") || inputFileName.EndsWith(".TXT"))
						sfd.Filter = "huff files (*.huff)|*.huff";
					if (inputFileName.EndsWith(".huff") || inputFileName.EndsWith(".HUFF"))
						sfd.Filter = "txt files (*.txt)|*.txt";
					sfd.FileName = "huffman";
					sfd.Title = "Guardar Archivo";
					if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						outputFileName = sfd.FileName;
						if (outputFileName.EndsWith(".huff") || outputFileName.EndsWith(".HUFF"))
							huff.saveBinary(sfd.FileName);
						if (outputFileName.EndsWith(".txt") || outputFileName.EndsWith(".TXT"))
						{
							using (StreamWriter stream = new StreamWriter(outputFileName))
							{
								stream.Write(tbxOutput.Text.Trim());
							}
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
				}
            }
				
		}

        private void btnDecompress_Click(object sender, EventArgs e)
        {
			huff.Decode();
			tbxOutput.Text = huff.text;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
			Application.Exit();
        }
    }
}
