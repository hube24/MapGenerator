using UnityEngine;
using System.Xml;
using System.Collections.Generic;

public class RoadGeneration: MonoBehaviour {

    public XmlDocument SourceFile = new XmlDocument();
    //public List<LineRenderer> roadlines = new List<LineRenderer>();
    public Terrain CurrentTerrain;
    public GameObject roadPrefab;
    // Use this for initialization
    
    void Start () {
        /*
        GameObject tmpRoad = GameObject.Instantiate(roadPrefab);

        Vector3[] vec = new Vector3[3];

        vec[0] = new Vector3(0, 0, 0);
        vec[1] = new Vector3(100, 0, 0);
        vec[2] = new Vector3(100, 0, -100);

        tmpRoad.GetComponent<LineRenderer>().SetPositions(vec);
        */
        float minLat, maxLat, minLon, maxLon;
        float terrainWidth, terrainLength;
        const float latConvertFactor = 110.574f;
        SourceFile.Load(Application.dataPath + "/Resources/maptest.xml");

        if (SourceFile != null && SourceFile.DocumentElement.HasChildNodes)
        {
            XmlNode tmpNode;
            tmpNode = SourceFile.DocumentElement;
            minLat = float.Parse(tmpNode.Attributes["minlat"].InnerText);
            maxLat = float.Parse(tmpNode.Attributes["maxlat"].InnerText);
            minLon = float.Parse(tmpNode.Attributes["minlon"].InnerText);
            maxLon = float.Parse(tmpNode.Attributes["maxlon"].InnerText);

            CurrentTerrain.terrainData.size.Set((maxLat - minLat) * latConvertFactor * 1000, 600 , (maxLat - minLat) * latConvertFactor * 1000);
            terrainWidth = CurrentTerrain.terrainData.size.x;
            terrainLength = CurrentTerrain.terrainData.size.z;
            
            int count = 0;
            
            foreach (XmlNode node in SourceFile.DocumentElement.ChildNodes)
            {
                if(node.Name == "way" && node.Attributes["highway"] != null)
                {
                    //making actual road
                    if (node.HasChildNodes)
                    {
                        count++;
                        int countnode = 0;
                        GameObject tmpRoad = GameObject.Instantiate(roadPrefab);
                        tmpRoad.name = "road" + count.ToString();
                        //Vector3 lastLoc = new Vector3();
                        List <Vector3> currLoc = new List<Vector3>();
                        float lat, lon;
                        foreach (XmlNode t_node in node.ChildNodes)
                        {
                            if (t_node.Name == "loc")
                            {
                                lat = float.Parse(t_node.Attributes["lat"].InnerText);
                                lon = float.Parse(t_node.Attributes["lon"].InnerText);

                                Debug.Log("lat =" + lat.ToString() + " lon=" + lon.ToString());

                                //x - latitude, z- longitude 
                                float x = (lat - minLat) * (terrainWidth / (maxLat - minLat));
                                float z = (-(lon - minLon) * (terrainLength / (maxLon - minLon)) + terrainLength);
                                float y = CurrentTerrain.terrainData.GetHeight((int)x, (int)z);
                                currLoc.Add(new Vector3( 
                                                x,
                                                y,
                                                z
                                            ));
                                
                                
                              
                            }
                        }
                        tmpRoad.GetComponent<LineRenderer>().SetVertexCount(currLoc.Count);
                        tmpRoad.GetComponent<LineRenderer>().SetPositions(currLoc.ToArray());
                    }

                }
            }

        
        }

    }
	
}
