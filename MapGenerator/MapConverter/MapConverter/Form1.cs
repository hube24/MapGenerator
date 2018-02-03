using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;

namespace WindowsFormsApplication1
{
    struct osmNode
    {
        public long id;
        public float lat;
        public float lon;
    }
    
    struct tag
    {
        public string k;
        public string v;
    }

    struct way
    {
        public long id;
        public List<tag> tags;
        public List<long> nodesId;
        public int getIndexOfTag(string k, string v)
        {
            for(int i=0 ; i<tags.Count ; i++)
            {
                if(tags[i].k == k && ( tags[i].v == v || v == "" )) return i;
            }
            return -1;
        }

    }


    public partial class Form1 : Form
    {
        string fileDir;
        string saveFileDir;
        XmlDocument doc = new XmlDocument();
        XmlDocument ndoc = new XmlDocument();
        List<osmNode> Nodes = new List<osmNode>();
        List<way> Ways = new List<way>();
        //bounds
        float minLat, maxLat, minLon, maxLon;

        public Form1()
        {
            InitializeComponent();
        }

        int idIndexBinarySearch(long id)
        {
            int count = Nodes.Count;
            int start_indx = 0, end_index = count - 1;
            int center = (end_index - start_indx) / 2 + start_indx;

            while (Nodes[center].id != id)
            {
                if (Nodes[center].id < id)
                    start_indx = center;
                else
                    end_index = center ;

                center = (end_index - start_indx) / 2 + start_indx;
            }

            return center;
        }

        void parse()
        {
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    if (node.Name == "bounds")
                    {
                        minLat = float.Parse(node.Attributes["minlat"].InnerText, CultureInfo.InvariantCulture);
                        maxLat = float.Parse(node.Attributes["maxlat"].InnerText, CultureInfo.InvariantCulture);
                        minLon = float.Parse(node.Attributes["minlon"].InnerText, CultureInfo.InvariantCulture);
                        maxLon = float.Parse(node.Attributes["maxlon"].InnerText, CultureInfo.InvariantCulture);
                    }

                    osmNode tmpOsmnode = new osmNode();
                    if (node.Name == "node" && node.Attributes != null)
                    {
                        tmpOsmnode = new osmNode();
                        tmpOsmnode.id = Int64.Parse(node.Attributes["id"].InnerText);
                        tmpOsmnode.lat = float.Parse(node.Attributes["lat"].InnerText, CultureInfo.InvariantCulture);
                        tmpOsmnode.lon = float.Parse(node.Attributes["lon"].InnerText, CultureInfo.InvariantCulture);
                        Nodes.Add(tmpOsmnode);
                    }

                    if (node.Name == "way" && node.Attributes != null)
                    {
                        way tmpWay = new way();
                        tag tmpTag = new tag();
                        tmpWay.nodesId = new List<long>();
                        tmpWay.tags = new List<tag>();
                        tmpWay.id = Int64.Parse(node.Attributes["id"].InnerText);
                        if(node.HasChildNodes)
                        {
                            //MessageBox.Show(node.ChildNodes.Count.ToString());
                            foreach( XmlNode t_node in node.ChildNodes)
                            {
                                if(t_node.Name == "nd" && t_node.Attributes["ref"]!=null)
                                {
                                    tmpWay.nodesId.Add(Int64.Parse(t_node.Attributes["ref"].InnerText));
                                }

                                if(t_node.Name == "tag" && t_node.Attributes["k"] != null && t_node.Attributes["v"] != null)
                                {
                                    tmpTag = new tag();
                                    tmpTag.k = t_node.Attributes["k"].InnerText;
                                    tmpTag.v = t_node.Attributes["v"].InnerText;
                                    tmpWay.tags.Add(tmpTag);                                         
                                }
                            }
                        }
                        Ways.Add(tmpWay);
                    }
                }
            }

            Nodes.Sort((x,y) => x.id.CompareTo(y.id));
            
            // debug 
            /*
            foreach(osmNode node in Nodes)
            {
                richTextBox1.Text += "id=" + node.id.ToString() + "  lat=" + node.lat.ToString() +  "  lon=" + node.lon.ToString() + "\n";
            }
            
            foreach (way _w in Ways)
            {
                richTextBox1.Text += "Way id=" + _w.id.ToString() + "\n";
                for (int i = 0; i < _w.nodesId.Count; i++) richTextBox1.Text += "   ref=" + _w.nodesId[i].ToString() + "\n";
                richTextBox1.Text += " tags: \n";
                for (int i = 0; i < _w.tags.Count; i++) richTextBox1.Text += "   k=" + _w.tags[i].k + " v=" + _w.tags[i].v + "\n";
            }
            */
            MessageBox.Show("parsing done.");
        }

        void convert()
        {
                XmlTextWriter writer = new XmlTextWriter(saveFileDir, Encoding.UTF8);
                writer.WriteStartDocument();
                writer.WriteStartElement("map");
                writer.WriteAttributeString("minlat", minLat.ToString().Replace(',', '.'));
                writer.WriteAttributeString("minlon", minLon.ToString().Replace(',', '.'));
                writer.WriteAttributeString("maxlat", maxLat.ToString().Replace(',', '.'));
                writer.WriteAttributeString("maxlon", maxLon.ToString().Replace(',', '.'));
                    foreach (way t_way in Ways)
                    {      
                       
                        if (t_way.getIndexOfTag("building", "yes") != -1)
                        {
                            writer.WriteStartElement("way");
                            writer.WriteAttributeString("id", t_way.id.ToString());
                            writer.WriteAttributeString("building", "yes");
                            foreach (long t_node in t_way.nodesId)
                            {
                                int i = idIndexBinarySearch(t_node);
                                writer.WriteStartElement("loc");
                                writer.WriteAttributeString("lat", Nodes[i].lat.ToString().Replace(',', '.'));
                                writer.WriteAttributeString("lon", Nodes[i].lon.ToString().Replace(',', '.'));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();//way
                        }

                        int _i = t_way.getIndexOfTag("highway", "");
                        if (_i != -1)
                        {
                            writer.WriteStartElement("way");
                            writer.WriteAttributeString("id", t_way.id.ToString());
                            writer.WriteAttributeString("highway", t_way.tags[_i].v);
                            foreach (long t_node in t_way.nodesId)
                            {
                                int i = idIndexBinarySearch(t_node);
                                writer.WriteStartElement("loc");
                                writer.WriteAttributeString("lat", Nodes[i].lat.ToString().Replace(',', '.'));
                                writer.WriteAttributeString("lon", Nodes[i].lon.ToString().Replace(',', '.'));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();//way
                        }

                   
                }
                    writer.WriteEndElement();//map
                    writer.WriteEndDocument();
                    writer.Close();

                    MessageBox.Show("converting done.");
          }
      


        private void button2_Click(object sender, EventArgs e)
        {
            parse();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileDir = openFileDialog1.FileName;
                textBox1.Text = fileDir;
                doc.Load(fileDir);
            }

        }



        private void button3_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveFileDir = saveFileDialog1.FileName;
                convert();
            }
        }
    }
}
