using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;
using Lucene.Net.Index;
using System.IO;
using LuceneBuilder.Models;
using System.Data.SqlClient;

namespace LuceneBuilder
{
    class LuceneIndexBuilder
    {
        public void StartLuceneIndexCreateProcess()
        {
            string sqlConnStr = @ConfigurationManager.AppSettings["SqlConnectionString"];
            string sqlCmd = "select ID, PLC_ID, FRMTD_ADDR, NELAT, NELON, SWLAT, SWLON FROM GEO_LCTN_LCNE ";

            SqlConnection sqlConn = new SqlConnection(sqlConnStr);

            string luceneIndexStoragePath = @ConfigurationManager.AppSettings["LuceneIndexStoragePath"];
            bool folderExists = System.IO.Directory.Exists(luceneIndexStoragePath);
            if (!folderExists)
                System.IO.Directory.CreateDirectory(luceneIndexStoragePath);

            StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            Lucene.Net.Store.Directory directory = FSDirectory.Open(new DirectoryInfo(luceneIndexStoragePath));
            IndexWriter writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.LIMITED);

            try
            {
                // We will populate below list to create Lucene index.
                var locationList = new List<LocationModel>();

                sqlConn.Open();

                using (var cmd = new SqlCommand(sqlCmd, sqlConn))
                {
                    var sqlReader = cmd.ExecuteReader();

                    while (sqlReader.Read())   
                    {
                        locationList.Add(new LocationModel
                        {
                            ID = Convert.ToInt16(sqlReader["ID"]),
                            PLC_ID = sqlReader["PLC_ID"].ToString(),
                            FRMTD_ADDR = sqlReader["FRMTD_ADDR"].ToString(),
                            NELAT = sqlReader["NELAT"].ToString(),
                            NELON = sqlReader["NELON"].ToString(),
                            SWLAT = sqlReader["SWLAT"].ToString(),
                            SWLON = sqlReader["SWLON"].ToString()
                        });
                    }
                }

                foreach (var item in locationList)
                {
                    writer.AddDocument(CreateDocument(item));
                }
            }
            catch
            {
                IndexWriter.Unlock(directory);
                throw;
            }
            finally
            {
                writer.Optimize();
                analyzer.Close();
                writer.Dispose();
                analyzer.Dispose();
            }
        }
        private Lucene.Net.Documents.Document CreateDocument(LocationModel location)
        {
            try
            {
                Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();
                doc.Add(new Lucene.Net.Documents.Field("ID", location.ID.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED));
                doc.Add(new Lucene.Net.Documents.Field("PLC_ID", location.PLC_ID, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED));
                doc.Add(new Lucene.Net.Documents.Field("FRMTD_ADDR", location.FRMTD_ADDR, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED));
                doc.Add(new Lucene.Net.Documents.Field("NELAT", location.NELAT, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED));
                doc.Add(new Lucene.Net.Documents.Field("NELON", location.NELON, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED));
                doc.Add(new Lucene.Net.Documents.Field("SWLAT", location.SWLAT, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED));
                doc.Add(new Lucene.Net.Documents.Field("SWLON", location.SWLON, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.ANALYZED));
                return doc;
            }
            catch
            {
                throw;
            }
        }
    }
}
