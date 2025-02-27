﻿using System.Collections.ObjectModel;
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

        }   //  Fin metodo GeneratePdf



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
                                .Text($" {tableConfiguration.Title.Description}")
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
                                    var cell = header.Cell().ColumnSpan(Convert.ToUInt32(tableConfiguration!.Header!.CombineColumnItems![i]));
                                    var textSpanDescriptor2 = GetCellContent(
                                        cell: cell,
                                        data: headerData.First(),
                                        tableConfiguration: tableConfiguration.Header!,
                                        columnIndex: i,
                                        backgroundColor: tableConfiguration!.Header!.BackgroundColor!,
                                        reportCategory: reportCategory!,
                                        configBool: configBool,
                                        "Center"
                                        );  
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
                                    var cell = header.Cell().ColumnSpan(Convert.ToUInt32(tableConfiguration!.SubHeader1!.CombineColumnItems![i]));
                                    var textSpanDescriptor2 = GetCellContent(
                                        cell: cell,
                                        data: headerData.First(),
                                        tableConfiguration: tableConfiguration.SubHeader1!,
                                        columnIndex: i,
                                        backgroundColor: tableConfiguration!.SubHeader1!.BackgroundColor!,
                                        reportCategory: reportCategory!,
                                        configBool: configBool,
                                        "Center"
                                        );
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
                                    var cell = header.Cell().ColumnSpan(Convert.ToUInt32(tableConfiguration!.SubHeader2!.CombineColumnItems![i]));
                                    var textSpanDescriptor2 = GetCellContent(
                                        cell: cell,
                                        data: headerData.First(),
                                        tableConfiguration: tableConfiguration.SubHeader2!,
                                        columnIndex: i,
                                        backgroundColor: tableConfiguration!.SubHeader2!.BackgroundColor!,
                                        reportCategory: reportCategory!,
                                        configBool: configBool,
                                        "Center"
                                        );
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
                                    var cell = header.Cell().ColumnSpan(Convert.ToUInt32(tableConfiguration!.SubHeader3!.CombineColumnItems![i]));
                                    var textSpanDescriptor2 = GetCellContent(
                                        cell: cell,
                                        data: headerData.First(),
                                        tableConfiguration: tableConfiguration.SubHeader3!,
                                        columnIndex: i,
                                        backgroundColor: tableConfiguration!.SubHeader3!.BackgroundColor!,
                                        reportCategory: reportCategory!,
                                        configBool: configBool,
                                        "Center"
                                        );
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
                                        var textSpanDescriptor2 = GetCellContent(
                                            cell: table.Cell(),
                                            data: rows,
                                            tableConfiguration: tableConfiguration.Data!,
                                            columnIndex: i,
                                            backgroundColor: backgroundColor,
                                            reportCategory: reportCategory!,
                                            configBool: configBool,
                                            "Center"
                                            );                                        
                                    }

                                    numberRow++;

                                }
                            }
                            //  Tabla estatica
                            else
                            {

                                //  Definicion de datos
                                var numberRow = tableConfiguration.Configuration.Columns * tableConfiguration.Configuration.Rows;
                                //  Buscamos que numero de fila es para colorearlas
                                var backgroundColor = tableConfiguration!.Data!.BackgroundColor!;

                                for (int i = 0; i < numberRow; i++)
                                {


                                    var textSpanDescriptor2 = GetCellContent(
                                        cell: table.Cell(),
                                        data: headerData.First(),
                                        tableConfiguration: tableConfiguration.Data!,
                                        columnIndex: i,
                                        backgroundColor: backgroundColor,
                                        reportCategory: reportCategory!,
                                        configBool: configBool,
                                        "left"
                                        );


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




        }   //  Fin metodo ComposeTableGeneral



        //  =============== Metodo para generar el contenido de la celda
        private TextSpanDescriptor GetCellContent(
            IContainer cell,            
            ReportSqlDataFormattedModel data,
            ReportConfigTableDataModel tableConfiguration,
            int columnIndex,
            string backgroundColor,
            IEnumerable<ReportSqlCategoryFormattedModel> reportCategory,
            List<ConfigBoolModel> configBool,
            string alignment)
        {
            string cellText;
            string cellBackgroundColor = backgroundColor;
            string styleKey = tableConfiguration!.FontStyleItems![columnIndex];

            // Obtiene el tipo de dato de la columna
            int dataType = tableConfiguration!.DataTypeItems![columnIndex];

            // Comprobación para tipos de datos básicos (0) o SQL (1)
            if (dataType == 0)
            {
                cellText = tableConfiguration!.DataSourceItems![columnIndex];

                // Configura el textContainer con alineación y estilos básicos
                var textContainer = cell
                    .Background(cellBackgroundColor)
                    .Border(tableConfiguration!.Border!)
                    .BorderColor(tableConfiguration!.BorderColor!);

                var cellTextSpacing = "";
                switch (alignment.ToLower())
                {
                    case "center":
                        textContainer = textContainer.AlignCenter();
                        cellTextSpacing = "";
                        break;
                    case "left":
                        textContainer = textContainer.AlignLeft();
                        cellTextSpacing = "  ";
                        break;
                    case "right":
                        textContainer = textContainer.AlignRight();
                        cellTextSpacing = "";
                        break;
                    default:
                        textContainer = textContainer.AlignCenter();
                        cellTextSpacing = "";
                        break;
                }

                var textSpanDescriptor = textContainer
                    .Text($"{cellTextSpacing}{cellText}")
                    .FontSize(tableConfiguration!.FontSize!)
                    .FontColor(tableConfiguration!.FontColor!);

                // Aplica estilo adicional si existe en el diccionario
                if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                {
                    styleAction(textSpanDescriptor);
                }

                return textSpanDescriptor;
            }
            else
            {
                //  Comprobamos si TIPO <> 0  en la data actual, esto quiere decir que el recorset es nulo
                if (data.Tipo != 0)
                {
                    // Caso en el que el campo es de tipo booleano o requiere configuración de `ConfigBool`
                    var fieldName = tableConfiguration!.DataSourceItems![columnIndex];
                    var fieldValue = data.GetType().GetProperty(fieldName)?.GetValue(data, null);
                    string? fieldValueText = fieldValue?.ToString() ?? "Valor no encontrado";

                    // Comprobación de valor booleano
                    if (fieldName.StartsWith("Bool_"))
                    {
                        var boolNumber = int.Parse(fieldName.Substring(5));
                        var reportCategoryItem = reportCategory?.FirstOrDefault(rc => rc.Id == data.Tipo);

                        if (reportCategoryItem != null && boolNumber > 0 && boolNumber <= reportCategoryItem.ConfigBoolItems.Count)
                        {
                            var selectedBoolConfig = reportCategoryItem.ConfigBoolItems[boolNumber - 1];
                            bool boolValue = Convert.ToBoolean(fieldValueText);

                            if (selectedBoolConfig < configBool.Count())
                            {
                                cellText = boolValue ? configBool[selectedBoolConfig].TextoTrue : configBool[selectedBoolConfig].TextoFalse;
                                cellBackgroundColor = boolValue ? configBool[selectedBoolConfig].ColorTrue : configBool[selectedBoolConfig].ColorFalse;
                            }
                            else
                            {
                                cellText = fieldValueText;
                            }
                        }
                        else
                        {
                            cellText = fieldValueText;
                        }
                    }
                    else
                    {
                        // Texto y color para otros tipos de datos
                        cellText = fieldValueText;
                    }

                    if (tableConfiguration!.DataUnitsItems![columnIndex] != "")
                    {
                        var cellTextWithUnits = $"{cellText} {tableConfiguration!.DataUnitsItems![columnIndex]}";
                        cellText = cellTextWithUnits;


                    }
                }
                else
                {
                    cellText = "";
                }

                // Configura el textContainer con alineación y estilos básicos
                var textContainer = cell
                    .Background(cellBackgroundColor)
                    .Border(tableConfiguration!.Border!)
                    .BorderColor(tableConfiguration!.BorderColor!);

                var cellTextSpacing = "";
                switch (alignment.ToLower())
                {
                    case "center":
                        textContainer = textContainer.AlignCenter();
                        cellTextSpacing = "";
                        break;
                    case "left":
                        textContainer = textContainer.AlignLeft();
                        cellTextSpacing = "  ";
                        break;
                    case "right":
                        textContainer = textContainer.AlignRight();
                        cellTextSpacing = "";
                        break;
                    default:
                        textContainer = textContainer.AlignCenter();
                        cellTextSpacing = "";
                        break;
                }

                var textSpanDescriptor = textContainer
                    .Text($"{cellTextSpacing}{cellText}")
                    .FontSize(tableConfiguration!.FontSize!)
                    .FontColor(tableConfiguration!.FontColor!);

                if (TextStyleHelper.StyleMap.TryGetValue(styleKey, out var styleAction))
                {
                    styleAction(textSpanDescriptor);
                }

                return textSpanDescriptor;
            }
        }   //  Fin metodo GetCellContent



    } // Fin principal


}
