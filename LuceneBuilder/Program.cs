using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;

namespace LuceneBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == null)
            {
                const string googleApiKey = "";
                const string sqlConnStr = @"";
                string sqlCmd = "insert into GEO_LCTN_LCNE (PLC_ID, FRMTD_ADDR, NELAT, NELON, SWLAT, SWLON) " +
                                        "values(@PLC_ID, @FRMTD_ADDR, @NELAT, @NELON, @SWLAT, @SWLON)";

                string url = String.Format("https://maps.googleapis.com/maps/api/geocode/json?address=650+J+Street,+Lincoln,+NE&key={0}", googleApiKey);

                SqlConnection sqlConn = new SqlConnection(sqlConnStr);
                sqlConn.Open();

                WebRequest request = WebRequest.Create(url);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string json = JsonConvert.DeserializeObject(reader.ReadToEnd()).ToString();
                        dynamic places = JObject.Parse(json);

                        string formattedAddress = places.results[0].formatted_address;
                        string place_id = places.results[0].place_id;
                        string nthestLat = places.results[0].geometry.viewport.northeast.lat;
                        string nthestLng = places.results[0].geometry.viewport.northeast.lng;
                        string sthwstLat = places.results[0].geometry.viewport.southwest.lat;
                        string sthwstLng = places.results[0].geometry.viewport.southwest.lng;

                        using (var cmd = new SqlCommand(sqlCmd, sqlConn))
                        {
                            cmd.Parameters.AddWithValue("@PLC_ID", places.results[0].place_id.Value);
                            cmd.Parameters.AddWithValue("@FRMTD_ADDR", places.results[0].formatted_address.Value);
                            cmd.Parameters.AddWithValue("@NELAT", places.results[0].geometry.viewport.northeast.lat.Value);
                            cmd.Parameters.AddWithValue("@NELON", places.results[0].geometry.viewport.northeast.lng.Value);
                            cmd.Parameters.AddWithValue("@SWLAT", places.results[0].geometry.viewport.southwest.lat.Value);
                            cmd.Parameters.AddWithValue("@SWLON", places.results[0].geometry.viewport.southwest.lng.Value);
                            cmd.ExecuteNonQuery();
                        }

                        Console.Write(formattedAddress + "\n" +
                            place_id + "\n" +
                            nthestLat + "\n" +
                            nthestLng + "\n" +
                            sthwstLat + "\n" +
                            sthwstLng + "\n");
                    }


                    sqlConn.Close();
                    Console.ReadLine();
                }
            }
            else
            {
                var x = new LuceneIndexBuilder();
                x.StartLuceneIndexCreateProcess();
            }
                    
            }

    }
}
