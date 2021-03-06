﻿using MongoDBOperator;
using System.Collections.Generic;
using System.Data.Entity;
using ExcelDataHandler;
using PDFWriter;
using WayneCarCorps.Data;
using WayneCarCorps.Data.Migrations;
using WayneCarCorps.JsonHandler;
using WayneCarCorps.MongoDBModels;
using WayneCarCorps.XmlHandler;
using Ninject;
using System.Reflection;
using WayneCarCorps.Importer;
using WayneCarCorps.MySQL;
using WayneCarCorps.MySQL.FluentModels;
using WayneCarCorps.MySQL.Models;

namespace WayneCarCorps.ConsoleClient
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<WayneCarCorpsContext, Configuration>());
            var db = new WayneCarCorpsContext();
            db.Database.CreateIfNotExists();
            var cars = new MongoDBExtractor<MongoCar>().GetEntitiesCollection("Cars");
            var kernel = BootstrapNinject();

            var carsImporter = kernel.Get<CarsImporter>();
            carsImporter.import(cars);

            var xmlImporter = kernel.Get<XmlImporter>();
            xmlImporter.Import();

            var zipExtractor = kernel.Get<ExcelReader>();
            zipExtractor.ExtractZipFiles();

            var pdfExporter = kernel.Get<PdfExporter>();
            pdfExporter.CreatePdfTable();

            var jsonExporter = kernel.Get<JsonWriter>();
            jsonExporter.WriteToJson();

            var xmlExporter = kernel.Get<XmlReportExporter>();
            xmlExporter.GetSalesForEachDealership();


            // CHANGE PASSWORD IN CONNECTIONS STRING
            FluentModelContext context = new FluentModelContext();
            OpenAccessRepository<SalesReport> report = new OpenAccessRepository<SalesReport>(context);
            //ExcelWriter.WriteToExcel(report);

            //UpdateMongoDB();
        }

        private static void UpdateMongoDB()
        {
            var mongoCarsCollection = CreateCarsCollection();
            var mongoDBLoader = new MongoDBLoader<MongoCar>();
            mongoDBLoader.LoadEntities(mongoCarsCollection, "Cars");
        }

        public static IEnumerable<MongoCar> CreateCarsCollection()
        {
            var carsCollection = new List<MongoCar>();

            carsCollection.Add(new MongoCar("VW", "Golf", "Estate", 2016, "Red", 150, 5, 17500, "Porsche Sofia East"));
            carsCollection.Add(new MongoCar("Lancia", "Delta", "Sallon", 2016, "White", 140, 5, 17800, "Auto Italia"));
            carsCollection.Add(new MongoCar("Skoda", "Octavia", "Estate", 2016, "Silver", 150, 5, 16500, "Euratek Auto"));
            carsCollection.Add(new MongoCar("BMW", "X5", "Off-Road", 2016, "Black", 320, 5, 75300, "Daru Car"));
            carsCollection.Add(new MongoCar("BMW", "M3", "Sport", 2016, "Blue", 530, 2, 56000, "Daru Car"));

            return carsCollection;
        }

        private static IKernel BootstrapNinject()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            return kernel;
        }
    }
}
