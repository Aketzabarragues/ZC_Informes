using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
            _snackbarService = App.ServiceProvider!.GetRequiredService<ISnackbarService>();
        }


        public void GeneratePdf(string filePath,
            ReportConfigFullModel reportConfiguration,
            List<ConfigBoolModel>? configBool,
            IEnumerable<ReportSqlCategoryFormattedModel>? reportCategory,
            IEnumerable<ReportSqlDataFormattedModel> table1HeaderSql,
            IEnumerable<ReportSqlDataFormattedModel> table1DataSql,
            IEnumerable<ReportSqlDataFormattedModel> table2HeaderSql,
            IEnumerable<ReportSqlDataFormattedModel> table2DataSql,
            IEnumerable<ReportSqlDataFormattedModel> table3HeaderSql,
            IEnumerable<ReportSqlDataFormattedModel> table3DataSql,
            IEnumerable<ReportSqlDataFormattedModel> table4HeaderSql,
            IEnumerable<ReportSqlDataFormattedModel> table4DataSql,
            IEnumerable<ReportSqlDataFormattedModel> table5HeaderSql,
            IEnumerable<ReportSqlDataFormattedModel> table5DataSql)
        {


            var pageSize = reportConfiguration?.GeneralConfiguration?.PageSize;
            var isHorizontal = reportConfiguration?.GeneralConfiguration?.IsHorizontal;

            // Verifica los valores antes de pasarlos al converter
            if (string.IsNullOrEmpty(pageSize))
            {
                throw new ArgumentException("Page size cannot be null or empty.");
            }
            if (isHorizontal == null)
            {
                throw new ArgumentException("IsHorizontal cannot be null.");
            }

            //  Crea una instancia del convertidor
            var pageSizeConverter = new PageSizeToQuestPdfConverter();
            // Llama al método para obtener el tamaño de página adecuado usando el convertidor
            var pdfPageSize = pageSizeConverter.Convert(Tuple.Create(pageSize, isHorizontal.Value), null, null, CultureInfo.InvariantCulture) as PageSize;


            // Establecer el umbral de excepciones de diseño
            QuestPDF.Settings.DocumentLayoutExceptionThreshold = 10000;
            try
            {
                Document.Create(document =>
                {


                    //  Generamos el documento
                    _ = document.Page(page =>
                    {
                        //  Configuracion general
                        page.Size(pdfPageSize!);
                        page.Margin(1, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontFamily(reportConfiguration!.GeneralConfiguration!.FontFamily!).FontSize(8));


                        //  Fecha de impresion
                        page.Header().AlignRight().AlignTop().Text($"Fecha de impresion: {DateTime.Now.ToString()}").FontSize(7).FontColor(Colors.Black);


                        page.Content().Padding(10).Column(column =>
                        {


                            // Encabezado
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    if (reportConfiguration!.GeneralConfiguration!.HeaderImage1 != "")
                                    {
                                        var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\Logo\", $"{reportConfiguration.GeneralConfiguration.HeaderImage1}");
                                        col.Item().Height(80).Width(150).Image(imagePath, ImageScaling.FitArea);
                                    }

                                });

                                row.RelativeItem().Column(col =>
                                {
                                    if (reportConfiguration!.GeneralConfiguration!.HeaderImage2 != "")
                                    {
                                        var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\Logo\", $"{reportConfiguration.GeneralConfiguration.HeaderImage2}");
                                        col.Item().Height(80).Width(150).Image(imagePath, ImageScaling.FitArea);
                                    }
                                });

                                row.RelativeItem().AlignRight().AlignBottom().Column(col =>
                                {
                                    col.Item().AlignRight().AlignBottom().Text(reportConfiguration!.GeneralConfiguration!.HeaderText1).SemiBold().FontSize(8).FontColor(Colors.Black);
                                    col.Item().AlignRight().AlignBottom().Text(reportConfiguration!.GeneralConfiguration!.HeaderText2).FontSize(8).FontColor(Colors.Black);

                                });
                            });

                            //  Generacion de la tabla 1
                            if (reportConfiguration!.Table1!.Configuration!.Enable == true)
                            {
                                column.Item().Element(container => ComposeTableGeneral(container, reportConfiguration.Table1, configBool, reportCategory, table1HeaderSql, table1DataSql));
                            }


                            //  Generacion de la tabla 2
                            if (reportConfiguration!.Table2!.Configuration!.Enable == true)
                            {
                                column.Item().Element(container => ComposeTableGeneral(container, reportConfiguration.Table2, configBool, reportCategory, table2HeaderSql, table2DataSql));
                            }


                            //  Generacion de la tabla 3
                            if (reportConfiguration!.Table3!.Configuration!.Enable == true)
                            {
                                column.Item().Element(container => ComposeTableGeneral(container, reportConfiguration.Table3, configBool, reportCategory, table3HeaderSql, table3DataSql));
                            }


                            //  Generacion de la tabla 4
                            if (reportConfiguration!.Table4!.Configuration!.Enable == true)
                            {
                                column.Item().Element(container => ComposeTableGeneral(container, reportConfiguration.Table4, configBool, reportCategory, table4HeaderSql, table4DataSql));
                            }


                            //  Generacion de la tabla 5
                            if (reportConfiguration!.Table5!.Configuration!.Enable == true)
                            {
                                column.Item().Element(container => ComposeTableGeneral(container, reportConfiguration.Table5, configBool, reportCategory, table5HeaderSql, table5DataSql));
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
                            row.RelativeItem(1).AlignRight().Text("Firma ____________________________").FontSize(7);
                        });

                    });
                })
                .GeneratePdf(filePath); // Guardar el PDF
            }
            catch (Exception ex)
            {
                _snackbarService.Show("Generar PDF", $"Error al generar el PDF: {ex.Message}", ControlAppearance.Danger, null, TimeSpan.FromSeconds(1));
                Log.Error($"Error al generar el PDF: {ex.Message}");
            }



            try
            {
                // Abre el archivo PDF generado
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });                
            }
            catch (Exception ex)
            {
                _snackbarService.Show("Generar PDF", $"Error al abrir el PDF: {ex.Message}", ControlAppearance.Danger, null, TimeSpan.FromSeconds(1));
                Log.Error($"Error al abrir el PDF: {ex.Message}");
            }

        }



        //  =============== Metodo para generar tablas.
        public void ComposeTableGeneral(IContainer container, 
            ReportConfigTableModel tableConfiguration, 
            List<ConfigBoolModel> configBool,
            IEnumerable<ReportSqlCategoryFormattedModel>? reportCategory,
            IEnumerable<ReportSqlDataFormattedModel> 
            headerData, 
            IEnumerable<ReportSqlDataFormattedModel> tableData)
        {

            try
            {
                //  Generamos el titulo general            
                container.Column(column =>
                {
                    column.Spacing(2);


                    //  =============== TITULO DE LA SECCION
                    if (tableConfiguration!.Title!.Enable)
                    {
                        column.Item().PaddingTop(tableConfiguration!.Configuration!.TittlePaddingTop!).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                var textSpanDescriptor = col.Item()
                                .Background(tableConfiguration!.Title!.BackgroundColor!)
                                .Border(tableConfiguration!.Title!.Border)
                                .BorderColor(tableConfiguration!.Title!.BorderColor!)
                                .AlignLeft()
                                .Text(tableConfiguration.Title.Description)
                                .FontColor(tableConfiguration.Title.FontColor!)
                                .FontSize(tableConfiguration.Title.FontSize);

                                var styleKey = tableConfiguration.Title.FontStyle;
                                // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                if (TextStyleHelper.StyleMap.TryGetValue(styleKey!, out var styleAction))
                                {
                                    styleAction(textSpanDescriptor);
                                }

                            });
                        });
                    }



                    //  =============== TABLA
                    //  Definicion de numero de columnas
                    column.Item().PaddingTop(tableConfiguration!.Configuration!.TablePaddingTop).Table(table =>
                    {

                        //  Definimos el numero de columnas
                        table.ColumnsDefinition(columns =>
                        {

                            //  Tabla dinamica
                            if (tableConfiguration.Configuration.TableType == 0)
                            {
                                for (int i = 0; i < tableConfiguration.Configuration.Columns; i++)
                                {
                                    columns.RelativeColumn(tableConfiguration!.Configuration!.ColumnsSizeItems![i]);
                                }
                            }
                            //  Tabla estatica
                            else
                            {
                                for (int i = 0; i < tableConfiguration.Configuration.Columns; i++)
                                {
                                    if (tableConfiguration!.Configuration!.FixedColumnSize! == 0)
                                    {
                                        columns.RelativeColumn(tableConfiguration!.Configuration!.ColumnsSizeItems![i]);
                                    }
                                    else
                                    {
                                        columns.ConstantColumn(tableConfiguration.Configuration.FixedColumnSize);
                                    }

                                }
                            }


                        });


                        //  Definicion de cabecera principal
                        if (tableConfiguration!.Header!.Enable!)
                        {
                            table.Header(header =>
                            {
                                for (int i = 0; i < tableConfiguration!.Header!.CombineColumnItems!.Count; i++)
                                {

                                    var styleKey = tableConfiguration!.Header!.FontStyleItems![i];

                                    if (tableConfiguration!.Header!.DataTypeItems![i] == 0)
                                    {
                                        // Crea el TextSpanDescriptor                                    
                                        var textSpanDescriptor = header.Cell().ColumnSpan(Convert.ToUInt32(tableConfiguration!.Header!.CombineColumnItems![i]))
                                        .Background(tableConfiguration!.Header!.BackgroundColor!)
                                        .Border(tableConfiguration.Header.Border)
                                        .BorderColor(tableConfiguration!.Header!.BorderColor!)
                                        .AlignCenter()
                                        .Text(tableConfiguration!.Header!.DataSourceItems![i])
                                        .FontSize(tableConfiguration!.Header!.FontSize!)
                                        .FontColor(tableConfiguration!.Header!.FontColor!);

                                        // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                        if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                        {
                                            styleAction(textSpanDescriptor);
                                        }

                                    }
                                    else
                                    {
                                        string fieldName = tableConfiguration!.Header!.DataSourceItems![i].ToString();
                                        var fieldValue = tableData.First().GetType().GetProperty(fieldName)?.GetValue(headerData.First(), null);
                                        // Verificamos si fieldValue no es null antes de llamar a ToString()
                                        string? fieldValueText = fieldValue != null ? fieldValue.ToString() : "Valor no encontrado";

                                        // Crea el TextSpanDescriptor
                                        var textSpanDescriptor = header.Cell()
                                            .Background(tableConfiguration!.Header!.BackgroundColor!)
                                            .Border(tableConfiguration.Header.Border)
                                            .BorderColor(tableConfiguration!.Header!.BorderColor!)
                                            .AlignCenter()
                                            .Text(fieldValueText)
                                            .FontSize(tableConfiguration!.Header!.FontSize!)
                                            .FontColor(tableConfiguration!.Header!.FontColor!);

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
                        if (tableConfiguration!.SubHeader1!.Enable!)
                        {
                            table.Header(header =>
                            {
                                for (int i = 0; i < tableConfiguration!.SubHeader1!.CombineColumnItems!.Count(); i++)
                                {

                                    var styleKey = tableConfiguration!.SubHeader1!.FontStyleItems![i];

                                    if (tableConfiguration!.SubHeader1!.DataTypeItems![i] == 0)
                                    {
                                        // Crea el TextSpanDescriptor                                    
                                        var textSpanDescriptor = header.Cell().ColumnSpan(Convert.ToUInt32(tableConfiguration!.SubHeader1!.CombineColumnItems![i]))
                                        .Background(tableConfiguration!.SubHeader1!.BackgroundColor!)
                                        .Border(tableConfiguration!.SubHeader1!.Border!)
                                        .BorderColor(tableConfiguration!.SubHeader1!.BorderColor!)
                                        .AlignCenter()
                                        .Text(tableConfiguration!.SubHeader1!.DataSourceItems![i])
                                        .FontSize(tableConfiguration!.SubHeader1!.FontSize!)
                                        .FontColor(tableConfiguration!.SubHeader1!.FontColor!);

                                        // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                        if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                        {
                                            styleAction(textSpanDescriptor);
                                        }

                                    }
                                    else
                                    {
                                        string fieldName = tableConfiguration!.SubHeader1!.DataSourceItems![i].ToString();
                                        var fieldValue = tableData.First().GetType().GetProperty(fieldName)?.GetValue(headerData.First(), null);
                                        // Verificamos si fieldValue no es null antes de llamar a ToString()
                                        string? fieldValueText = fieldValue != null ? fieldValue.ToString() : "Valor no encontrado";

                                        // Crea el TextSpanDescriptor
                                        var textSpanDescriptor = header.Cell()
                                            .Background(tableConfiguration!.SubHeader1!.BackgroundColor!)
                                            .Border(tableConfiguration!.SubHeader1!.Border!)
                                            .BorderColor(tableConfiguration!.SubHeader1!.BorderColor!)
                                            .AlignCenter()
                                            .Text(fieldValueText)
                                            .FontSize(tableConfiguration!.SubHeader1!.FontSize!)
                                            .FontColor(tableConfiguration!.SubHeader1!.FontColor!);

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
                        if (tableConfiguration!.SubHeader2!.Enable!)
                        {
                            table.Header(header =>
                            {
                                for (int i = 0; i < tableConfiguration!.SubHeader2!.CombineColumnItems!.Count(); i++)
                                {

                                    var styleKey = tableConfiguration!.SubHeader2!.FontStyleItems![i];

                                    if (tableConfiguration!.SubHeader2!.DataTypeItems![i] == 0)
                                    {
                                        // Crea el TextSpanDescriptor                                    
                                        var textSpanDescriptor = header.Cell().ColumnSpan(Convert.ToUInt32(tableConfiguration!.SubHeader2!.CombineColumnItems![i]))
                                        .Background(tableConfiguration!.SubHeader2!.BackgroundColor!)
                                        .Border(tableConfiguration!.SubHeader2!.Border!)
                                        .BorderColor(tableConfiguration!.SubHeader2!.BorderColor!)
                                        .AlignCenter()
                                        .Text(tableConfiguration!.SubHeader2!.DataSourceItems![i])
                                        .FontSize(tableConfiguration!.SubHeader2!.FontSize!)
                                        .FontColor(tableConfiguration!.SubHeader2!.FontColor!);

                                        // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                        if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                        {
                                            styleAction(textSpanDescriptor);
                                        }

                                    }
                                    else
                                    {
                                        string fieldName = tableConfiguration!.SubHeader2!.DataSourceItems![i].ToString();
                                        var fieldValue = tableData.First().GetType().GetProperty(fieldName)?.GetValue(headerData.First(), null);
                                        // Verificamos si fieldValue no es null antes de llamar a ToString()
                                        string? fieldValueText = fieldValue != null ? fieldValue.ToString() : "Valor no encontrado";

                                        // Crea el TextSpanDescriptor
                                        var textSpanDescriptor = header.Cell()
                                            .Background(tableConfiguration!.SubHeader2!.BackgroundColor!)
                                            .Border(tableConfiguration!.SubHeader2!.Border!)
                                            .BorderColor(tableConfiguration!.SubHeader2!.BorderColor!)
                                            .AlignCenter()
                                            .Text(fieldValueText)
                                            .FontSize(tableConfiguration!.SubHeader2!.FontSize!)
                                            .FontColor(tableConfiguration!.SubHeader2!.FontColor!);

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
                        if (tableConfiguration!.SubHeader3!.Enable!)
                        {
                            table.Header(header =>
                            {
                                for (int i = 0; i < tableConfiguration!.SubHeader3!.CombineColumnItems!.Count; i++)
                                {

                                    var styleKey = tableConfiguration!.SubHeader3!.FontStyleItems![i];

                                    if (tableConfiguration!.SubHeader3!.DataTypeItems![i] == 0)
                                    {
                                        // Crea el TextSpanDescriptor                                    
                                        var textSpanDescriptor = header.Cell().ColumnSpan(Convert.ToUInt32(tableConfiguration!.SubHeader3!.CombineColumnItems![i]))
                                        .Background(tableConfiguration!.SubHeader3!.BackgroundColor!)
                                        .Border(tableConfiguration!.SubHeader3!.Border!)
                                        .BorderColor(tableConfiguration!.SubHeader3!.BorderColor!)
                                        .AlignCenter()
                                        .Text(tableConfiguration!.SubHeader3!.DataSourceItems![i])
                                        .FontSize(tableConfiguration!.SubHeader3!.FontSize!)
                                        .FontColor(tableConfiguration!.SubHeader3!.FontColor!);

                                        // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                        if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                        {
                                            styleAction(textSpanDescriptor);
                                        }

                                    }
                                    else
                                    {
                                        string fieldName = tableConfiguration!.SubHeader3!.DataSourceItems![i].ToString();
                                        var fieldValue = tableData.First().GetType().GetProperty(fieldName)?.GetValue(headerData.First(), null);
                                        // Verificamos si fieldValue no es null antes de llamar a ToString()
                                        string? fieldValueText = fieldValue != null ? fieldValue.ToString() : "Valor no encontrado";

                                        // Crea el TextSpanDescriptor
                                        var textSpanDescriptor = header.Cell()
                                            .Background(tableConfiguration!.SubHeader3!.BackgroundColor!)
                                            .Border(tableConfiguration!.SubHeader3!.Border!)
                                            .BorderColor(tableConfiguration!.SubHeader3!.BorderColor!)
                                            .AlignCenter()
                                            .Text(fieldValueText)
                                            .FontSize(tableConfiguration!.SubHeader3!.FontSize!)
                                            .FontColor(tableConfiguration!.SubHeader3!.FontColor!);

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
                        if (tableConfiguration!.Data!.Enable!)
                        {
                            if (tableConfiguration.Configuration.TableType == 0)
                            {
                                //  Definicion de datos
                                var numberRow = 1;
                                foreach (var rows in tableData)
                                {
                                    //  Buscamos que numero de fila es para colorearlas
                                    var backgroundColor = (numberRow % 2 == 0) ? tableConfiguration!.Data!.BackgroundColor! : Colors.White;

                                    for (int i = 0; i < tableConfiguration!.Data!.CombineColumnItems!.Count; i++)
                                    {

                                        var styleKey = tableConfiguration!.Data!.FontStyleItems![i];

                                        if (tableConfiguration!.Data!.DataTypeItems![i] == 0)
                                        {
                                            // Crea el TextSpanDescriptor
                                            var textSpanDescriptor = table.Cell()
                                                .Background(backgroundColor)
                                                .Border(tableConfiguration!.Data!.Border!)
                                                .BorderColor(tableConfiguration!.Data!.BorderColor!)
                                                .AlignCenter()
                                                .Text($"{tableConfiguration!.Data!.DataSourceItems![i]}")
                                                .FontSize(tableConfiguration!.Data!.FontSize!)
                                                .FontColor(tableConfiguration!.Data!.FontColor!);

                                            // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                            if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                            {
                                                styleAction(textSpanDescriptor);
                                            }
                                        }
                                        else
                                        {

                                            var fieldName = tableConfiguration!.Data!.DataSourceItems![i];
                                            var fieldValue = rows.GetType().GetProperty(fieldName)?.GetValue(rows, null);
                                            string? fieldValueText = fieldValue != null ? fieldValue.ToString() : "Valor no encontrado";

                                            // Variables temporales para background y texto
                                            string cellBackgroundColor;
                                            string cellText;

                                            // Comprueba si el DataSource es un valor booleano
                                            if (tableConfiguration!.Data!.DataSourceItems![i].StartsWith("Bool_"))
                                            {
                                                // Extrae el número del booleano de "Bool_n"
                                                var boolNumber = int.Parse(tableConfiguration!.Data!.DataSourceItems![i].Substring(5));

                                                // Busca el item en reportCategory con ID = 1
                                                var reportCategoryItem = reportCategory?.FirstOrDefault(rc => rc.Id == rows.Tipo);

                                                if (reportCategoryItem != null && boolNumber > 0 && boolNumber <= reportCategoryItem.ConfigBoolItems.Count)
                                                {
                                                    // Accede al booleano específico dentro de ConfigBoolItems
                                                    var selectedBoolConfig = reportCategoryItem.ConfigBoolItems[boolNumber - 1]; // boolNumber - 1 para ajustar al índice de lista
                                                    bool boolValue = Convert.ToBoolean(fieldValueText);
                                                    if (selectedBoolConfig > configBool.Count())
                                                    {
                                                        cellText = $"{fieldValueText}";
                                                        cellBackgroundColor = backgroundColor;
                                                    }
                                                    else
                                                    {
                                                        cellText = boolValue ? configBool[selectedBoolConfig].TextoTrue : configBool[selectedBoolConfig].TextoFalse;
                                                        cellBackgroundColor = boolValue ? configBool[selectedBoolConfig].ColorTrue : configBool[selectedBoolConfig].ColorFalse;
                                                    }
                                                    
                                                }
                                                else
                                                {
                                                    // Ajusta el texto y color para otros valores
                                                    cellText = $"{fieldValueText}";
                                                    cellBackgroundColor = backgroundColor;
                                                }    
                                                    
                                            }
                                            else
                                            {
                                                // Ajusta el texto y color para otros valores
                                                cellText = $"{fieldValueText}";
                                                cellBackgroundColor = backgroundColor;
                                            }

                                            // Crea el TextSpanDescriptor
                                            var textSpanDescriptor = table.Cell()
                                                .Background(cellBackgroundColor)
                                                .Border(tableConfiguration!.Data!.Border!)
                                                .BorderColor(tableConfiguration!.Data!.BorderColor!)
                                                .AlignCenter()
                                                .Text(cellText)
                                                .FontSize(tableConfiguration!.Data!.FontSize!)
                                                .FontColor(tableConfiguration!.Data!.FontColor!);

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

                                    var styleKey = tableConfiguration!.Data!.FontStyleItems![i];

                                    if (tableConfiguration!.Data!.DataTypeItems![i] == 0)
                                    {
                                        // Crea el TextSpanDescriptor
                                        var textSpanDescriptor = table.Cell()
                                            .Background(Colors.White)
                                            .Border(tableConfiguration!.Data!.Border!)
                                            .BorderColor(tableConfiguration!.Data!.BorderColor!)
                                            .AlignLeft()
                                            .Text($"  {tableConfiguration!.Data!.DataSourceItems![i]}")
                                            .FontSize(tableConfiguration!.Data!.FontSize!)
                                            .FontColor(tableConfiguration!.Data!.FontColor!);

                                        // Intenta obtener el estilo del diccionario y aplica el estilo si se encontró
                                        if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                        {
                                            styleAction(textSpanDescriptor);
                                        }
                                    }
                                    else
                                    {

                                        if (!tableData.IsNullOrEmpty())
                                        {
                                            var fieldName = tableConfiguration!.Data!.DataSourceItems![i];
                                            // Cambiar tableData por rows para acceder al item actual
                                            var fieldValue = headerData.First().GetType().GetProperty(fieldName)?.GetValue(headerData.First(), null);
                                            // Verificamos si fieldValue no es null antes de llamar a ToString()
                                            string? fieldValueText = fieldValue != null ? fieldValue.ToString() : "Valor no encontrado";

                                            // Crea el TextSpanDescriptor
                                            var textSpanDescriptor = table.Cell()
                                                    .Background(Colors.White)
                                                    .Border(tableConfiguration!.Data!.Border!)
                                                    .BorderColor(tableConfiguration!.Data!.BorderColor!)
                                                    .AlignLeft()
                                                    .Text($"  {fieldValueText} {tableConfiguration!.Data!.DataUnitsItems![i]}")
                                                    .FontSize(tableConfiguration!.Data!.FontSize!)
                                                    .FontColor(tableConfiguration!.Data!.FontColor!);

                                            // Intenta obtener el estilo del diccionario
                                            if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                            {
                                                styleAction(textSpanDescriptor);
                                            }
                                        }
                                        else
                                        {
                                            // Crea el TextSpanDescriptor
                                            var textSpanDescriptor = table.Cell()
                                                    .Background(Colors.White)
                                                    .Border(tableConfiguration!.Data!.Border!)
                                                    .BorderColor(tableConfiguration!.Data!.BorderColor!)
                                                    .AlignLeft()
                                                    .Text($"  null")
                                                    .FontSize(tableConfiguration!.Data!.FontSize!)
                                                    .FontColor(tableConfiguration!.Data!.FontColor!);
                                            // Intenta obtener el estilo del diccionario
                                            if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                                            {
                                                styleAction(textSpanDescriptor);
                                            }
                                        }

                                    }
                                }

                            }
                        }

                    });
                });
            }
            catch (Exception ex)
            {
                _snackbarService.Show("Generar PDF", $"Error al generar las tablas del PDF: {ex.Message}", ControlAppearance.Danger, null, TimeSpan.FromSeconds(1));
                Log.Error($"Error al generar las tablas del PDF: {ex.Message}");
            }



            
        } 
    
    
    }


}
