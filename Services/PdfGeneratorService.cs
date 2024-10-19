using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZC_Informes.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ZC_Informes.Models;
using System.Drawing.Printing;
using System.Globalization;
using System.Net;
using System.Windows.Documents;
using System.Data.Common;

namespace ZC_Informes.Services
{

    public class PdfGeneratorService : IPdfGeneratorService
    {
        private ReportConfigurationModel? _reportConfiguration;

        public void GeneratePdf(string filePath, ReportConfigurationModel reportConfiguration)
        {

            _reportConfiguration = reportConfiguration;
            DataTable dataTable = CreateSampleDataTable();


            // Crea una instancia del convertidor
            var pageSizeConverter = new PageSizeToQuestPdfConverter();

            // Llama al método para obtener el tamaño de página adecuado usando el convertidor
            var pdfPageSize = pageSizeConverter.Convert(Tuple.Create(_reportConfiguration.General.PageSize, _reportConfiguration.General.IsHorizontal), null, null, CultureInfo.InvariantCulture) as PageSize;


            // Establecer el umbral de excepciones de diseño
            QuestPDF.Settings.DocumentLayoutExceptionThreshold = 10000;

            Document.Create(document =>
            {
                //string imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "logoweb.png");

                document.Page(page =>
                {
                    page.Size(pdfPageSize);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily(_reportConfiguration.General.FontFamily).FontSize(8));

                    // ENCABEZADO
                    page.Content().Padding(10).Column(column =>
                    {
                        // Encabezado
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Height(80).Width(150).Image(_reportConfiguration.General.HeaderImage1, ImageScaling.FitArea);
                            });

                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Height(80).Width(150).Image(_reportConfiguration.General.HeaderImage2, ImageScaling.FitArea);
                            });

                            row.RelativeItem().AlignRight().AlignBottom().Column(col =>
                            {
                                col.Item().AlignRight().Text(_reportConfiguration.General.HeaderText1).SemiBold().FontSize(8).FontColor(Colors.Black);
                                col.Item().AlignRight().Text(_reportConfiguration.General.HeaderText2).FontSize(8).FontColor(Colors.Black);
                            });
                        });

                        // Datos generales
                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Background(Colors.Grey.Lighten2).AlignLeft().Text("DATOS GENERALES DE INFORME");
                            });
                        });

                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(100);
                            });

                            table.Cell().AlignLeft().Text("FechaInicio:").Bold();
                            table.Cell().AlignLeft().Text("27/08/2024");
                            table.Cell().AlignLeft().Text("Identificador:").Bold();
                            table.Cell().AlignLeft().Text("2024082719440223000");
                            table.Cell().AlignLeft().Text("Hora Inicio:").Bold();
                            table.Cell().AlignLeft().Text("19:44:03");
                            table.Cell().AlignLeft().Text("Proceso ID:").Bold();
                            table.Cell().AlignLeft().Text("1050");
                            table.Cell().AlignLeft().Text("Fecha Fin:").Bold();
                            table.Cell().AlignLeft().Text("28/08/2024");
                            table.Cell().AlignLeft().Text("Código:").Bold();
                            table.Cell().AlignLeft().Text("2024082719433");
                            table.Cell().AlignLeft().Text("Hora Fin:").Bold();
                            table.Cell().AlignLeft().Text("19:45:30");
                        });

                        // Tipo
                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Background(Colors.Grey.Lighten2).AlignLeft().Text("OSMOSIS INVERSA");
                            });
                        });



                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Background(Colors.Grey.Lighten2).AlignLeft().Text("OSMOSIS INVERSA");
                            });
                        });


                        column.Item().PaddingTop(30).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(25);
                                columns.ConstantColumn(100);
                            });

                            table.Cell().AlignLeft().Text("Tipo:").Bold();
                            table.Cell().AlignLeft().Text("Chequeo osmosis inversa");
                        });

                        // Tabla de datos
                        column.Item().PaddingTop(15).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text("Identificador");
                                header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text("Código");
                                header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text("Fecha");
                                header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text("Hora");
                                header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text("%Bomba");
                                header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text("Temp.");
                                header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text("Caudal");
                                header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text("Caudal Per.");
                                header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text("Conduct.");
                                header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text("Presión");
                                header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text("%R");
                                header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text("Estado");
                            });

                            // Filas de la tabla
                            for (int i = 1; i < dataTable.Rows.Count; i++)
                            {
                                var backgroundColor = (i % 2 == 0) ? Colors.Grey.Lighten3 : Colors.White;
                                DataRow row = dataTable.Rows[i];

                                table.Cell().Background(backgroundColor).AlignCenter().Text(row["Identificador"].ToString());
                                table.Cell().Background(backgroundColor).AlignCenter().Text(row["Código"].ToString());
                                table.Cell().Background(backgroundColor).AlignCenter().Text(row["Fecha"].ToString());
                                table.Cell().Background(backgroundColor).AlignCenter().Text(row["Hora"].ToString());
                                table.Cell().Background(backgroundColor).AlignCenter().Text(row["% Bomba"].ToString());
                                table.Cell().Background(backgroundColor).AlignCenter().Text(row["Temp."].ToString());
                                table.Cell().Background(backgroundColor).AlignCenter().Text(row["Caudal"].ToString());
                                table.Cell().Background(backgroundColor).AlignCenter().Text(row["Caudal Perm."].ToString());
                                table.Cell().Background(backgroundColor).AlignCenter().Text(row["Conduct."].ToString());
                                table.Cell().Background(backgroundColor).AlignCenter().Text(row["Presión"].ToString());
                                table.Cell().Background(backgroundColor).AlignCenter().Text(row["% R"].ToString());

                                string formattedDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                                table.Cell().Background(backgroundColor).AlignCenter().Text(formattedDateTime);
                            }
                        });


                        if (_reportConfiguration.TableGeneral.Configuration.Enable = true)
                        {
                            column.Item().Element(container => ComposeTableGeneral(container, _reportConfiguration.TableGeneral));
                        }
                       

                    });



                    // Pie de página
                    page.Footer().AlignCenter().Padding(10).Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                        row.RelativeItem(1).AlignRight().Text("Firma ____________________________");
                    });
                });
            })
            .GeneratePdf(filePath); // Guardar el PDF
        }

        private DataTable CreateSampleDataTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Identificador");
            dataTable.Columns.Add("Código");
            dataTable.Columns.Add("Fecha");
            dataTable.Columns.Add("Hora");
            dataTable.Columns.Add("% Bomba");
            dataTable.Columns.Add("Temp.");
            dataTable.Columns.Add("Caudal");
            dataTable.Columns.Add("Caudal Perm.");
            dataTable.Columns.Add("Conduct.");
            dataTable.Columns.Add("Presión");
            dataTable.Columns.Add("% R");
            dataTable.Columns.Add("Estado");

            // Agregar filas de ejemplo
            for (int i = 1; i < 15; i++)
            {
                dataTable.Rows.Add("20240801101010", $"{i} - A001", "01/01/2024", "08:00", "50%", "25°C", "100L", "90L", "10µS", "1.5 bar", "90%", "Activo");
            }

            return dataTable;
        }







        //  =============== Metodo para generar tablas.
        public void ComposeTableGeneral(IContainer container, TableConfiguration tableConfiguration)
        {

            


            container.ShowEntire().Column(column =>
            {
                column.Spacing(2);

                //  Generamos el titulo de la seccion.
                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Background(Colors.Grey.Lighten2).AlignLeft().Text(tableConfiguration.Configuration.Description);
                    });
                });

                if (tableConfiguration.Header.Enable = true)
                {

                }





                // Tabla de datos
                column.Item().PaddingTop(15).Table(table =>
                {
                    // Definir las columnas de la tabla de manera dinámica basadas en la configuración
                    table.ColumnsDefinition(columns =>
                    {
                        for (int i = 0; i < tableConfiguration.Configuration.Columns; i++)
                        {
                            columns.RelativeColumn();
                        }
                    });

                    table.Header(header =>
                    {
                        for (int i = 0; i < tableConfiguration.Configuration.Columns; i++)
                        {
                            
                            header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text($"Title {i} {tableConfiguration.Data.DataItems[0].Configuration}").SemiBold();
                        }                       
                    });

                    table.Header(header =>
                    {
                        for (int i = 0; i < tableConfiguration.Configuration.Columns; i++)
                        {

                            header.Cell().Background(Colors.Grey.Lighten2).AlignCenter().Text($"Title {i} {tableConfiguration.Data.DataItems[0].Configuration}").SemiBold();
                        }
                    });

                });
            });


        }

      

    }

}
