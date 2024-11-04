using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;
using ZC_Informes.Helpers;
using ZC_Informes.Interfaces;
using ZC_Informes.Models;

namespace ZC_Informes.Services
{

    public class PdfGeneratorService : IPdfGeneratorService
    {


        //  =============== Servicios inyectados
        private readonly ISnackbarService _snackbarService;



        //  =============== Constructor
        public PdfGeneratorService()
        {
            _snackbarService = App.ServiceProvider.GetRequiredService<ISnackbarService>();
        }


        public void GeneratePdf(string filePath, 
            ReportConfigurationModel reportConfiguration,
            IEnumerable<ReportSqlDataModelFormatted> table1HeaderSql,
            IEnumerable<ReportSqlDataModelFormatted> table1DataSql,
            IEnumerable<ReportSqlDataModelFormatted> table2HeaderSql,
            IEnumerable<ReportSqlDataModelFormatted> table2DataSql,
            IEnumerable<ReportSqlDataModelFormatted> table3HeaderSql,
            IEnumerable<ReportSqlDataModelFormatted> table3DataSql,
            IEnumerable<ReportSqlDataModelFormatted> table4HeaderSql,
            IEnumerable<ReportSqlDataModelFormatted> table4DataSql,
            IEnumerable<ReportSqlDataModelFormatted> table5HeaderSql,
            IEnumerable<ReportSqlDataModelFormatted> table5DataSql)
        {



            //DataTable dataTable = CreateSampleDataTable();

            //  Crea una instancia del convertidor
            var pageSizeConverter = new PageSizeToQuestPdfConverter();

            //  Llama al método para obtener el tamaño de página adecuado usando el convertidor
            var pdfPageSize = pageSizeConverter.Convert(Tuple.Create(reportConfiguration.GeneralConfiguration.PageSize, reportConfiguration.GeneralConfiguration.IsHorizontal), null, null, CultureInfo.InvariantCulture) as PageSize;


            // Establecer el umbral de excepciones de diseño
            QuestPDF.Settings.DocumentLayoutExceptionThreshold = 10000;

            Document.Create(document =>
            {
                

                //  Generamos el documento
                _ = document.Page(page =>
                {
                    //  Configuracion general
                    page.Size(pdfPageSize);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily(reportConfiguration.GeneralConfiguration.FontFamily).FontSize(8));


                    //  Fecha de impresion
                    page.Header().AlignRight().AlignTop().Text($"Fecha de impresion: {DateTime.Now.ToString()}").FontSize(7).FontColor(Colors.Black);
                        

                    page.Content().Padding(10).Column(column =>
                    {


                        // Encabezado
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                if (reportConfiguration.GeneralConfiguration.HeaderImage1 != "")
                                {
                                    var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\Logo\", $"{reportConfiguration.GeneralConfiguration.HeaderImage1}");
                                    col.Item().Height(80).Width(150).Image(imagePath, ImageScaling.FitArea);
                                }

                            });

                            row.RelativeItem().Column(col =>
                            {
                                if (reportConfiguration.GeneralConfiguration.HeaderImage2 != "")
                                {
                                    var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\Logo\", $"{reportConfiguration.GeneralConfiguration.HeaderImage2}");
                                    col.Item().Height(80).Width(150).Image(imagePath, ImageScaling.FitArea);
                                }
                            });

                            row.RelativeItem().AlignRight().AlignBottom().Column(col =>
                            {
                                col.Item().AlignRight().AlignBottom().Text(reportConfiguration.GeneralConfiguration.HeaderText1).SemiBold().FontSize(8).FontColor(Colors.Black);
                                col.Item().AlignRight().AlignBottom().Text(reportConfiguration.GeneralConfiguration.HeaderText2).FontSize(8).FontColor(Colors.Black);
                                
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
                        
                        //  Generacion de la tabla 1
                        if (reportConfiguration.Table1.Configuration.Enable == true)
                        {
                            column.Item().Element(container => ComposeTableGeneral(container, reportConfiguration.Table1, table1HeaderSql, table1DataSql));
                        }


                        //  Generacion de la tabla 2
                        if (reportConfiguration.Table2.Configuration.Enable == true)
                        {
                            column.Item().Element(container => ComposeTableGeneral(container, reportConfiguration.Table2, table2HeaderSql, table2DataSql));
                        }


                        //  Generacion de la tabla 3
                        if (reportConfiguration.Table3.Configuration.Enable == true)
                        {
                            column.Item().Element(container => ComposeTableGeneral(container, reportConfiguration.Table3, table3HeaderSql, table3DataSql));
                        }


                        //  Generacion de la tabla 4
                        if (reportConfiguration.Table4.Configuration.Enable == true)
                        {
                            column.Item().Element(container => ComposeTableGeneral(container, reportConfiguration.Table4, table4HeaderSql, table4DataSql));
                        }


                        //  Generacion de la tabla 5
                        if (reportConfiguration.Table5.Configuration.Enable == true)
                        {
                            column.Item().Element(container => ComposeTableGeneral(container, reportConfiguration.Table5, table5HeaderSql, table5DataSql));
                        }






                    });



                    // Pie de página
                    page.Footer().AlignCenter().Padding(10).Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                        row.RelativeItem(1).AlignRight().Text("Firma ____________________________");
                    });

                });
            })
            .GeneratePdf(filePath); // Guardar el PDF





            try
            {
                // Abre el archivo PDF generado
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });                
            }
            catch (Exception ex)
            {
                _snackbarService.Show("Generar PDF", $"Error al abrir el PDF: {ex.Message}", ControlAppearance.Danger, null, TimeSpan.FromSeconds(1));
                Log.Information("");
            }

        }


        /*
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
        */






        //  =============== Metodo para generar tablas.
        public void ComposeTableGeneral(IContainer container, TableConfiguration tableConfiguration, IEnumerable<ReportSqlDataModelFormatted> headerData, IEnumerable<ReportSqlDataModelFormatted> tableData)
        {

            //  Generamos el titulo general            
            container.Column(column =>
            {
                column.Spacing(2);


                //  =============== TITULO DE LA SECCION
                if (tableConfiguration.Title.Enable)
                {
                    column.Item().PaddingTop(tableConfiguration.Configuration.TittlePaddingTop).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            var textSpanDescriptor = col.Item()
                            .Background(tableConfiguration.Title.BackgroundColor)
                            .Border(tableConfiguration.Title.Border)
                            .BorderColor(tableConfiguration.Title.BorderColor)
                            .AlignLeft()
                            .Text(tableConfiguration.Title.Description)
                            .FontColor(tableConfiguration.Title.FontColor)
                            .FontSize(tableConfiguration.Title.FontSize);

                            var styleKey = tableConfiguration.Title.FontStyle;
                            // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                            if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                            {
                                styleAction(textSpanDescriptor);
                            }

                        });
                    });
                }



                //  =============== TABLA
                //  Definicion de numero de columnas
                column.Item().PaddingTop(tableConfiguration.Configuration.TablePaddingTop).Table(table =>
                {

                    //  Definimos el numero de columnas
                    table.ColumnsDefinition(columns =>
                    {

                        //  Tabla dinamica
                        if (tableConfiguration.Configuration.TableType == 0)
                        {
                            for (int i = 0; i < tableConfiguration.Configuration.Columns; i++)
                            {
                                columns.RelativeColumn(tableConfiguration.Configuration.ColumnsSizeItems[i]);
                            }
                        }
                        //  Tabla estatica
                        else
                        {
                            for (int i = 0; i < tableConfiguration.Configuration.Columns; i++)
                            {
                                columns.ConstantColumn(tableConfiguration.Configuration.FixedColumnSize);
                            }
                        }

                            
                    });


                    //  Definicion de cabecera principal
                    if (tableConfiguration.Header.Enable)
                    {
                        table.Header(header =>
                        {
                            for (int i = 0; i < tableConfiguration.Header.CombineColumnItems.Count(); i++)
                            {

                                var styleKey = tableConfiguration.Header.FontStyleItems[i];

                                if (tableConfiguration.Header.DataTypeItems[i] == 0)
                                {
                                    // Crea el TextSpanDescriptor                                    
                                    var textSpanDescriptor = header.Cell().ColumnSpan(Convert.ToUInt32(tableConfiguration.Header.CombineColumnItems[i]))
                                    .Background(tableConfiguration.Header.BackgroundColor)
                                    .Border(tableConfiguration.Header.Border)
                                    .BorderColor(tableConfiguration.Header.BorderColor)
                                    .AlignCenter()
                                    .Text(tableConfiguration.Header.DataSourceItems[i])
                                    .FontSize(tableConfiguration.Header.FontSize)
                                    .FontColor(tableConfiguration.Header.FontColor);

                                    // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                    if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                    {
                                        styleAction(textSpanDescriptor);
                                    }
                                    
                                }
                                else
                                {
                                    string fieldName = tableConfiguration.Header.DataSourceItems[i].ToString();
                                    var fieldValue = tableData.First().GetType().GetProperty(fieldName)?.GetValue(headerData.First(), null);
                                    // Verificamos si fieldValue no es null antes de llamar a ToString()
                                    string? fieldValueText = fieldValue != null ? fieldValue.ToString() : "Valor no encontrado";

                                    // Crea el TextSpanDescriptor
                                    var textSpanDescriptor = header.Cell()
                                        .Background(tableConfiguration.Header.BackgroundColor)
                                        .Border(tableConfiguration.Header.Border)
                                        .BorderColor(tableConfiguration.Header.BorderColor)
                                        .AlignCenter()
                                        .Text(fieldValueText)
                                        .FontSize(tableConfiguration.Header.FontSize)
                                        .FontColor(tableConfiguration.Header.FontColor);

                                    // Intenta obtener el estilo del diccionario
                                    if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                    {
                                        styleAction(textSpanDescriptor);
                                    }
                                }
                                
                            }
                        });
                    }


                    //  Definicion de subcabecera 1 principal
                    if (tableConfiguration.SubHeader1.Enable)
                    {
                        table.Header(header =>
                        {
                            for (int i = 0; i < tableConfiguration.SubHeader1.CombineColumnItems.Count(); i++)
                            {

                                var styleKey = tableConfiguration.SubHeader1.FontStyleItems[i];

                                if (tableConfiguration.SubHeader1.DataTypeItems[i] == 0)
                                {
                                    // Crea el TextSpanDescriptor                                    
                                    var textSpanDescriptor = header.Cell().ColumnSpan(Convert.ToUInt32(tableConfiguration.SubHeader1.CombineColumnItems[i]))
                                    .Background(tableConfiguration.SubHeader1.BackgroundColor)
                                    .Border(tableConfiguration.SubHeader1.Border)
                                    .BorderColor(tableConfiguration.SubHeader1.BorderColor)
                                    .AlignCenter()
                                    .Text(tableConfiguration.SubHeader1.DataSourceItems[i])
                                    .FontSize(tableConfiguration.SubHeader1.FontSize)
                                    .FontColor(tableConfiguration.SubHeader1.FontColor);

                                    // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                    if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                    {
                                        styleAction(textSpanDescriptor);
                                    }

                                }
                                else
                                {
                                    string fieldName = tableConfiguration.SubHeader1.DataSourceItems[i].ToString();
                                    var fieldValue = tableData.First().GetType().GetProperty(fieldName)?.GetValue(headerData.First(), null);
                                    // Verificamos si fieldValue no es null antes de llamar a ToString()
                                    string? fieldValueText = fieldValue != null ? fieldValue.ToString() : "Valor no encontrado";

                                    // Crea el TextSpanDescriptor
                                    var textSpanDescriptor = header.Cell()
                                        .Background(tableConfiguration.SubHeader1.BackgroundColor)
                                        .Border(tableConfiguration.SubHeader1.Border)
                                        .BorderColor(tableConfiguration.SubHeader1.BorderColor)
                                        .AlignCenter()
                                        .Text(fieldValueText)
                                        .FontSize(tableConfiguration.SubHeader1.FontSize)
                                        .FontColor(tableConfiguration.SubHeader1.FontColor);

                                    // Intenta obtener el estilo del diccionario
                                    if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                    {
                                        styleAction(textSpanDescriptor);
                                    }
                                }

                            }
                        });
                    }


                    //  Definicion de subcabecera 2 principal
                    if (tableConfiguration.SubHeader2.Enable)
                    {
                        table.Header(header =>
                        {
                            for (int i = 0; i < tableConfiguration.SubHeader2.CombineColumnItems.Count(); i++)
                            {

                                var styleKey = tableConfiguration.SubHeader2.FontStyleItems[i];

                                if (tableConfiguration.SubHeader2.DataTypeItems[i] == 0)
                                {
                                    // Crea el TextSpanDescriptor                                    
                                    var textSpanDescriptor = header.Cell().ColumnSpan(Convert.ToUInt32(tableConfiguration.SubHeader2.CombineColumnItems[i]))
                                    .Background(tableConfiguration.SubHeader2.BackgroundColor)
                                    .Border(tableConfiguration.SubHeader2.Border)
                                    .BorderColor(tableConfiguration.SubHeader2.BorderColor)
                                    .AlignCenter()
                                    .Text(tableConfiguration.SubHeader2.DataSourceItems[i])
                                    .FontSize(tableConfiguration.SubHeader2.FontSize)
                                    .FontColor(tableConfiguration.SubHeader2.FontColor);

                                    // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                    if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                    {
                                        styleAction(textSpanDescriptor);
                                    }

                                }
                                else
                                {
                                    string fieldName = tableConfiguration.SubHeader2.DataSourceItems[i].ToString();
                                    var fieldValue = tableData.First().GetType().GetProperty(fieldName)?.GetValue(headerData.First(), null);
                                    // Verificamos si fieldValue no es null antes de llamar a ToString()
                                    string? fieldValueText = fieldValue != null ? fieldValue.ToString() : "Valor no encontrado";

                                    // Crea el TextSpanDescriptor
                                    var textSpanDescriptor = header.Cell()
                                        .Background(tableConfiguration.SubHeader2.BackgroundColor)
                                        .Border(tableConfiguration.SubHeader2.Border)
                                        .BorderColor(tableConfiguration.SubHeader2.BorderColor)
                                        .AlignCenter()
                                        .Text(fieldValueText)
                                        .FontSize(tableConfiguration.SubHeader2.FontSize)
                                        .FontColor(tableConfiguration.SubHeader2.FontColor);

                                    // Intenta obtener el estilo del diccionario
                                    if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                    {
                                        styleAction(textSpanDescriptor);
                                    }
                                }

                            }
                        });
                    }


                    //  Definicion de subcabecera 3 principal
                    if (tableConfiguration.SubHeader3.Enable)
                    {
                        table.Header(header =>
                        {
                            for (int i = 0; i < tableConfiguration.SubHeader3.CombineColumnItems.Count(); i++)
                            {

                                var styleKey = tableConfiguration.SubHeader3.FontStyleItems[i];

                                if (tableConfiguration.SubHeader3.DataTypeItems[i] == 0)
                                {
                                    // Crea el TextSpanDescriptor                                    
                                    var textSpanDescriptor = header.Cell().ColumnSpan(Convert.ToUInt32(tableConfiguration.SubHeader3.CombineColumnItems[i]))
                                    .Background(tableConfiguration.SubHeader3.BackgroundColor)
                                    .Border(tableConfiguration.SubHeader3.Border)
                                    .BorderColor(tableConfiguration.SubHeader3.BorderColor)
                                    .AlignCenter()
                                    .Text(tableConfiguration.SubHeader3.DataSourceItems[i])
                                    .FontSize(tableConfiguration.SubHeader3.FontSize)
                                    .FontColor(tableConfiguration.SubHeader3.FontColor);

                                    // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                    if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                    {
                                        styleAction(textSpanDescriptor);
                                    }

                                }
                                else
                                {
                                    string fieldName = tableConfiguration.SubHeader3.DataSourceItems[i].ToString();
                                    var fieldValue = tableData.First().GetType().GetProperty(fieldName)?.GetValue(headerData.First(), null);
                                    // Verificamos si fieldValue no es null antes de llamar a ToString()
                                    string? fieldValueText = fieldValue != null ? fieldValue.ToString() : "Valor no encontrado";

                                    // Crea el TextSpanDescriptor
                                    var textSpanDescriptor = header.Cell()
                                        .Background(tableConfiguration.SubHeader3.BackgroundColor)
                                        .Border(tableConfiguration.SubHeader3.Border)
                                        .BorderColor(tableConfiguration.SubHeader3.BorderColor)
                                        .AlignCenter()
                                        .Text(fieldValueText)
                                        .FontSize(tableConfiguration.SubHeader3.FontSize)
                                        .FontColor(tableConfiguration.SubHeader3.FontColor);

                                    // Intenta obtener el estilo del diccionario
                                    if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                    {
                                        styleAction(textSpanDescriptor);
                                    }
                                }

                            }
                        });
                    }


                    //  Tabla dinamica
                    if (tableConfiguration.Configuration.TableType == 0)
                    {                    
                        //  Definicion de datos
                        var numberRow = 1;                   
                        foreach (var rows in tableData)
                        {
                            //  Buscamos que numero de fila es para colorearlas
                            var backgroundColor = (numberRow % 2 == 0) ? tableConfiguration.Data.BackgroundColor : Colors.White;

                            for (int i = 0; i < tableConfiguration.Configuration.Columns; i++)
                            {

                                var styleKey = tableConfiguration.Data.FontStyleItems[i];

                                if (tableConfiguration.Data.DataTypeItems[i] == 0)
                                {
                                    // Crea el TextSpanDescriptor
                                    var textSpanDescriptor = table.Cell()
                                        .Background(backgroundColor)
                                        .Border(tableConfiguration.Data.Border)
                                        .BorderColor(tableConfiguration.Data.BorderColor)
                                        .AlignCenter()
                                        .Text($"{tableConfiguration.Data.DataSourceItems[i]}")
                                        .FontSize(tableConfiguration.Data.FontSize)
                                        .FontColor(tableConfiguration.Data.FontColor);

                                    // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                    if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                    {
                                        styleAction(textSpanDescriptor);
                                    }
                                }
                                else
                                {

                                    var fieldName = tableConfiguration.Data.DataSourceItems[i];
                                    // Cambiar tableData por rows para acceder al item actual
                                    var fieldValue = rows.GetType().GetProperty(fieldName)?.GetValue(rows, null);
                                    // Verificamos si fieldValue no es null antes de llamar a ToString()
                                    string? fieldValueText = fieldValue != null ? fieldValue.ToString() : "Valor no encontrado";

                                    // Crea el TextSpanDescriptor
                                    var textSpanDescriptor = table.Cell()
                                        .Background(backgroundColor)
                                        .Border(tableConfiguration.Data.Border)
                                        .BorderColor(tableConfiguration.Data.BorderColor)
                                        .AlignCenter()
                                        .Text($"{fieldValueText} {tableConfiguration.Data.DataUnitsItems[i]}")
                                        .FontSize(tableConfiguration.Data.FontSize)
                                        .FontColor(tableConfiguration.Data.FontColor);

                                    // Intenta obtener el estilo del diccionario
                                    if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                    {
                                        styleAction(textSpanDescriptor);
                                    }
                                }
                            }

                            numberRow++;
                        
                        }
                    }
                    //  Tabla estatica
                    else
                    {

                        //  Definicion de datos
                        var numberRow = tableConfiguration.Configuration.Columns * tableConfiguration.Configuration.Rows;
                        
                            for (int i = 0; i < numberRow; i++)
                            {

                                var styleKey = tableConfiguration.Data.FontStyleItems[i];

                                if (tableConfiguration.Data.DataTypeItems[i] == 0)
                                {
                                    // Crea el TextSpanDescriptor
                                    var textSpanDescriptor = table.Cell()
                                        .Background(Colors.White)
                                        .AlignLeft()
                                        .Text($"{tableConfiguration.Data.DataSourceItems[i]}")
                                        .FontSize(tableConfiguration.Data.FontSize)
                                        .FontColor(tableConfiguration.Data.FontColor);

                                    // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                    if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                    {
                                        styleAction(textSpanDescriptor);
                                    }
                                }
                                else
                                {

                                    var fieldName = tableConfiguration.Data.DataSourceItems[i];
                                    // Cambiar tableData por rows para acceder al item actual
                                    var fieldValue = tableData.First().GetType().GetProperty(fieldName)?.GetValue(tableData.First(), null);
                                    // Verificamos si fieldValue no es null antes de llamar a ToString()
                                    string? fieldValueText = fieldValue != null ? fieldValue.ToString() : "Valor no encontrado";

                                    // Crea el TextSpanDescriptor
                                    var textSpanDescriptor = table.Cell()
                                            .Background(Colors.White)
                                            .AlignLeft()
                                            .Text($"{fieldValueText} {tableConfiguration.Data.DataUnitsItems[i]}")
                                            .FontSize(tableConfiguration.Data.FontSize)
                                            .FontColor(tableConfiguration.Data.FontColor);

                                    // Intenta obtener el estilo del diccionario
                                    if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                    {
                                        styleAction(textSpanDescriptor);
                                    }
                                }
                            }

                    }


                });                
            });
        } 
    
    
    }


}
