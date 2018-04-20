using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nx_test
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Файлы PRT (*.prt)|*.prt|Все файлы (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Say("Импорт данных из файла: " + ofd.FileName);

                NXOpen.Session ses = null;
                try
                {
                    ses = NXOpen.Session.GetSession();
                }
                catch (Exception ex)
                {
                    Say("Ошибка: " + ex.ToString());
                }

                if (ses != null)
                {
                    NXOpen.PartLoadStatus status = null;
                    ses.Parts.Open(ofd.FileName, out status);
                    foreach (NXOpen.Part part in ses.Parts)
                    {
                        string name = part.Name;

                        int pointNumber = 1;

                        foreach (NXOpen.Body bd in part.Bodies)
                        {
                            Say("bd.Name=" + bd.Name);
                            NXOpen.Face[] faces = bd.GetFaces();
                            for (int f = 0; f < faces.Length; f++)
                            {
                                Say("faces.Name=" + faces[f].Name);
                                NXOpen.Edge[] edges = faces[f].GetEdges();
                                for (int g = 0; g < edges.Length; g++)
                                {
                                    Say("edges.Name=" + edges[g].Name);

                                    NXOpen.Point3d vert1 = new NXOpen.Point3d();
                                    NXOpen.Point3d vert2 = new NXOpen.Point3d();

                                    edges[g].GetVertices(out vert1, out vert2);

                                    Say(string.Format("faces[{0}].edges[{1}].vert1: X={2:0.0000}; Y={3:0.0000}; Z={4:0.0000}", f, g, vert1.X, vert1.Y, vert2.Z));
                                    Say(string.Format("faces[{0}].edges[{1}].vert2: X={2:0.0000}; Y={3:0.0000}; Z={4:0.0000}", f, g, vert2.X, vert2.Y, vert2.Z));
                                }
                            }
                        }

                        foreach (NXOpen.Sketch sk in part.Sketches)
                        {
                            NXOpen.NXObject[] objs = sk.GetAllGeometry();

                            Say(string.Format("objs.Length = {0}", objs.Length));
                            Say(string.Format("sk.Name = {0}", sk.Name));

                            for (int i = 0; i < objs.Length; i++)
                            {
                                int any_length = objs[i].GetAttributeTitlesByType(NXOpen.NXObject.AttributeType.Any).Length;

                                if (objs[i] is NXOpen.Line)
                                {
                                    NXOpen.Line line = (NXOpen.Line)objs[i];
                                    Say(string.Format("[StartPoint] X={0}, Y={1}, Z={2}", line.StartPoint.X, line.StartPoint.Y, line.StartPoint.Z));
                                    Say(string.Format("[EndPoint] X={0}, Y={1}, Z={2}", line.EndPoint.X, line.EndPoint.Y, line.EndPoint.Z));
                                }
                            }
                        }
                    }
                }
                else
                {
                    Say("Ошибка получения сессии NX!");
                }
            }
            

            

        }

        private void Say(string text)
        {
            bool first = true;
            if (this.textBox1.Text != string.Empty)
            {
                first = false;
            }
            if (!first)
            {
                this.textBox1.Text += System.Environment.NewLine;
            }

            this.textBox1.Text = this.textBox1.Text + text;
        }
    }
}
