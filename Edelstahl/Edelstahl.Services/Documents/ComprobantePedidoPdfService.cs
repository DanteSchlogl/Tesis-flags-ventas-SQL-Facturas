using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using Edelstahl.Domain.Comercial;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;



namespace Edelstahl.Services.Documents
{
    /// <summary>
    /// Genera el comprobante PDF de un pedido confirmado.
    /// </summary>
    public class ComprobantePedidoPdfService
    {
        public string Generar(
            Pedido pedido,
            Cliente cliente)
        {
            if (pedido == null)
            {
                throw new ArgumentNullException(
                    nameof(pedido));
            }

            if (cliente == null)
            {
                throw new ArgumentNullException(
                    nameof(cliente));
            }

            if (pedido.Estado !=
                EstadoPedido.Confirmado)
            {
                throw new InvalidOperationException(
                    "Solo se puede generar el comprobante " +
                    "de un pedido confirmado.");
            }

            QuestPDF.Settings.License =
                LicenseType.Community;

            string carpetaComprobantes =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.MyDocuments),
                    "Edelstahl ERP",
                    "Comprobantes");

            Directory.CreateDirectory(
                carpetaComprobantes);

            string nombreArchivo =
                "Comprobante-" +
                LimpiarNombreArchivo(
                    pedido.Numero) +
                ".pdf";

            string rutaArchivo =
                Path.Combine(
                    carpetaComprobantes,
                    nombreArchivo);

            Document
                .Create(documento =>
                {
                    documento.Page(pagina =>
                    {
                        pagina.Size(
                            PageSizes.A4);

                        pagina.Margin(
                            1.5f,
                            Unit.Centimetre);

                        pagina.PageColor(
                            Colors.Grey.Lighten5);

                        pagina.DefaultTextStyle(
                            estilo =>
                                estilo
                                    .FontFamily("Arial")
                                    .FontSize(9)
                                    .FontColor(
                                        Colors.Grey.Darken3));

                        pagina.Header()
                            .Element(
                                contenedor =>
                                    CrearEncabezado(
                                        contenedor,
                                        pedido));

                        pagina.Content()
                            .PaddingVertical(15)
                            .Column(columna =>
                            {
                                columna.Spacing(14);

                                columna.Item()
                                    .Element(
                                        contenedor =>
                                            CrearDatosCliente(
                                                contenedor,
                                                cliente,
                                                pedido));

                                columna.Item()
                                    .Element(
                                        contenedor =>
                                            CrearTablaDetalles(
                                                contenedor,
                                                pedido));

                                columna.Item()
                                    .Element(
                                        contenedor =>
                                            CrearResumen(
                                                contenedor,
                                                pedido));

                                columna.Item()
                                    .Element(
                                        contenedor =>
                                            CrearDatosComerciales(
                                                contenedor,
                                                pedido));
                            });

                        pagina.Footer()
                            .AlignCenter()
                            .Text(texto =>
                            {
                                texto.Span(
                                        "Edelstahl ERP - " +
                                        "Comprobante generado electrónicamente - Página ")
                                    .FontSize(8)
                                    .FontColor(
                                        Colors.Grey.Medium);

                                texto.CurrentPageNumber()
                                    .FontSize(8);

                                texto.Span(" de ")
                                    .FontSize(8);

                                texto.TotalPages()
                                    .FontSize(8);
                            });
                    });
                })
                .GeneratePdf(
                    rutaArchivo);

            return rutaArchivo;
        }

        private static void CrearEncabezado(
            IContainer contenedor,
            Pedido pedido)
        {
            contenedor
                .Background(
                    Colors.BlueGrey.Lighten4)
                .Padding(18)
                .Row(fila =>
                {
                    fila.RelativeItem()
                        .Column(columna =>
                        {
                            columna.Item()
                                .Text(
                                    "EDELSTAHL ERP")
                                .FontSize(22)
                                .Bold()
                                .FontColor(
                                    Colors.BlueGrey.Darken3);

                            columna.Item()
                                .PaddingTop(4)
                                .Text(
                                    "Gestión Comercial y Ventas")
                                .FontSize(10)
                                .FontColor(
                                    Colors.BlueGrey.Darken2);

                            columna.Item()
                                .PaddingTop(8)
                                .Text(
                                    "Soluciones en acero inoxidable")
                                .FontSize(9)
                                .FontColor(
                                    Colors.Grey.Darken1);
                        });

                    fila.ConstantItem(220)
                        .AlignRight()
                        .Column(columna =>
                        {
                            columna.Item()
                                .AlignRight()
                                .Text(
                                    "COMPROBANTE DE PEDIDO")
                                .FontSize(15)
                                .Bold()
                                .FontColor(
                                    Colors.BlueGrey.Darken3);

                            columna.Item()
                                .PaddingTop(8)
                                .AlignRight()
                                .Text(
                                    pedido.Numero)
                                .FontSize(12)
                                .Bold();

                            columna.Item()
                                .PaddingTop(4)
                                .AlignRight()
                                .Text(
                                    "Fecha: " +
                                    pedido.FechaPedido.ToString(
                                        "dd/MM/yyyy HH:mm"));

                            columna.Item()
                                .PaddingTop(3)
                                .AlignRight()
                                .Text(
                                    "Estado: " +
                                    pedido.Estado);
                        });
                });
        }

        private static void CrearDatosCliente(
            IContainer contenedor,
            Cliente cliente,
            Pedido pedido)
        {
            contenedor
                .Background(
                    Colors.White)
                .Border(1)
                .BorderColor(
                    Colors.BlueGrey.Lighten3)
                .Padding(15)
                .Column(columna =>
                {
                    columna.Item()
                        .Text(
                            "DATOS DEL CLIENTE")
                        .FontSize(12)
                        .Bold()
                        .FontColor(
                            Colors.BlueGrey.Darken3);

                    columna.Item()
                        .PaddingTop(10)
                        .Row(fila =>
                        {
                            fila.RelativeItem()
                                .Column(datos =>
                                {
                                    datos.Item()
                                        .Text(texto =>
                                        {
                                            texto.Span(
                                                    "Razón social: ")
                                                .Bold();

                                            texto.Span(
                                                ValorSeguro(
                                                    cliente.RazonSocial));
                                        });

                                    datos.Item()
                                        .PaddingTop(5)
                                        .Text(texto =>
                                        {
                                            texto.Span(
                                                    "CUIT: ")
                                                .Bold();

                                            texto.Span(
                                                ValorSeguro(
                                                    cliente.CUIT));
                                        });

                                    datos.Item()
                                        .PaddingTop(5)
                                        .Text(texto =>
                                        {
                                            texto.Span(
                                                    "Correo: ")
                                                .Bold();

                                            texto.Span(
                                                ValorSeguro(
                                                    cliente.Email));
                                        });
                                });

                            fila.RelativeItem()
                                .Column(datos =>
                                {
                                    datos.Item()
                                        .Text(texto =>
                                        {
                                            texto.Span(
                                                    "Teléfono: ")
                                                .Bold();

                                            texto.Span(
                                                ValorSeguro(
                                                    cliente.Telefono));
                                        });

                                    datos.Item()
                                        .PaddingTop(5)
                                        .Text(texto =>
                                        {
                                            texto.Span(
                                                    "Localidad: ")
                                                .Bold();

                                            texto.Span(
                                                ValorSeguro(
                                                    cliente.Localidad));
                                        });

                                    datos.Item()
                                        .PaddingTop(5)
                                        .Text(texto =>
                                        {
                                            texto.Span(
                                                    "Pedido: ")
                                                .Bold();

                                            texto.Span(
                                                ValorSeguro(
                                                    pedido.Numero));
                                        });
                                });
                        });
                });
        }

        private static void CrearTablaDetalles(
            IContainer contenedor,
            Pedido pedido)
        {
            contenedor
                .Background(
                    Colors.White)
                .Border(1)
                .BorderColor(
                    Colors.BlueGrey.Lighten3)
                .Padding(15)
                .Column(columna =>
                {
                    columna.Item()
                        .Text(
                            "DETALLE DEL PEDIDO")
                        .FontSize(12)
                        .Bold()
                        .FontColor(
                            Colors.BlueGrey.Darken3);

                    columna.Item()
                        .PaddingTop(10)
                        .Table(tabla =>
                        {
                            tabla.ColumnsDefinition(
                                columnas =>
                                {
                                    columnas.ConstantColumn(75);
                                    columnas.RelativeColumn(3);
                                    columnas.ConstantColumn(65);
                                    columnas.ConstantColumn(90);
                                    columnas.ConstantColumn(90);
                                });

                            tabla.Header(encabezado =>
                            {
                                encabezado.Cell()
                                    .Element(
                                        CeldaEncabezado)
                                    .Text("Código");

                                encabezado.Cell()
                                    .Element(
                                        CeldaEncabezado)
                                    .Text("Descripción");

                                encabezado.Cell()
                                    .Element(
                                        CeldaEncabezado)
                                    .AlignRight()
                                    .Text("Cantidad");

                                encabezado.Cell()
                                    .Element(
                                        CeldaEncabezado)
                                    .AlignRight()
                                    .Text("Precio");

                                encabezado.Cell()
                                    .Element(
                                        CeldaEncabezado)
                                    .AlignRight()
                                    .Text("Subtotal");
                            });

                            if (pedido.Detalles != null)
                            {
                                foreach (DetallePedido detalle
                                    in pedido.Detalles)
                                {
                                    tabla.Cell()
                                        .Element(
                                            CeldaDetalle)
                                        .Text(
                                            ValorSeguro(
                                                detalle.Codigo));

                                    tabla.Cell()
                                        .Element(
                                            CeldaDetalle)
                                        .Text(
                                            ValorSeguro(
                                                detalle.Descripcion));

                                    tabla.Cell()
                                        .Element(
                                            CeldaDetalle)
                                        .AlignRight()
                                        .Text(
                                            detalle.Cantidad.ToString(
                                                "N2",
                                                ObtenerCultura()));

                                    tabla.Cell()
                                        .Element(
                                            CeldaDetalle)
                                        .AlignRight()
                                        .Text(
                                            FormatearImporte(
                                                detalle.PrecioUnitario,
                                                pedido.Moneda));

                                    tabla.Cell()
                                        .Element(
                                            CeldaDetalle)
                                        .AlignRight()
                                        .Text(
                                            FormatearImporte(
                                                detalle.CalcularSubtotal(),
                                                pedido.Moneda));
                                }
                            }
                        });
                });
        }

        private static void CrearResumen(
            IContainer contenedor,
            Pedido pedido)
        {
            contenedor
                .Row(fila =>
                {
                    fila.RelativeItem();

                    fila.ConstantItem(310)
                        .Background(
                            Colors.Green.Lighten5)
                        .Border(1)
                        .BorderColor(
                            Colors.Green.Lighten2)
                        .Padding(15)
                        .Column(columna =>
                        {
                            columna.Item()
                                .Text("RESUMEN")
                                .FontSize(12)
                                .Bold()
                                .FontColor(
                                    Colors.Green.Darken3);

                            CrearLineaResumen(
                                columna,
                                "Subtotal",
                                FormatearImporte(
                                    pedido.CalcularSubtotal(),
                                    pedido.Moneda));

                            CrearLineaResumen(
                                columna,
                                "Descuento",
                                FormatearImporte(
                                    pedido.CalcularDescuentoGeneral(),
                                    pedido.Moneda));

                            CrearLineaResumen(
                                columna,
                                "Recargo",
                                FormatearImporte(
                                    pedido.CalcularRecargo(),
                                    pedido.Moneda));

                            CrearLineaResumen(
                                columna,
                                "Neto gravado",
                                FormatearImporte(
                                    pedido.CalcularNetoGravado(),
                                    pedido.Moneda));

                            CrearLineaResumen(
                                columna,
                                "IVA",
                                FormatearImporte(
                                    pedido.CalcularIVA(),
                                    pedido.Moneda));

                            CrearLineaResumen(
                                columna,
                                "Anticipo registrado",
                                FormatearImporte(
                                    pedido.ImporteAnticipo,
                                    pedido.Moneda));

                            columna.Item()
                                .PaddingTop(10)
                                .BorderTop(1)
                                .BorderColor(
                                    Colors.Green.Lighten2)
                                .PaddingTop(10)
                                .Row(resumen =>
                                {
                                    resumen.RelativeItem()
                                        .Text("TOTAL")
                                        .FontSize(12)
                                        .Bold();

                                    resumen.RelativeItem()
                                        .AlignRight()
                                        .Text(
                                            FormatearImporte(
                                                pedido.CalcularTotal(),
                                                pedido.Moneda))
                                        .FontSize(13)
                                        .Bold()
                                        .FontColor(
                                            Colors.Green.Darken3);
                                });

                            columna.Item()
                                .PaddingTop(8)
                                .Row(resumen =>
                                {
                                    resumen.RelativeItem()
                                        .Text(
                                            "Saldo a facturar")
                                        .Bold();

                                    resumen.RelativeItem()
                                        .AlignRight()
                                        .Text(
                                            FormatearImporte(
                                                pedido.CalcularSaldoPendiente(),
                                                pedido.Moneda))
                                        .Bold();
                                });
                        });
                });
        }

        private static void CrearDatosComerciales(
            IContainer contenedor,
            Pedido pedido)
        {
            contenedor
                .Background(
                    Colors.BlueGrey.Lighten5)
                .Border(1)
                .BorderColor(
                    Colors.BlueGrey.Lighten3)
                .Padding(15)
                .Column(columna =>
                {
                    columna.Item()
                        .Text(
                            "INFORMACIÓN COMERCIAL")
                        .FontSize(11)
                        .Bold()
                        .FontColor(
                            Colors.BlueGrey.Darken3);

                    columna.Item()
                        .PaddingTop(8)
                        .Text(texto =>
                        {
                            texto.Span(
                                    "Condición de pago: ")
                                .Bold();

                            texto.Span(
                                ValorSeguro(
                                    pedido.CondicionPago));
                        });

                    columna.Item()
                        .PaddingTop(5)
                        .Text(texto =>
                        {
                            texto.Span(
                                    "Plazo de entrega: ")
                                .Bold();

                            texto.Span(
                                ValorSeguro(
                                    pedido.PlazoEntrega));
                        });

                    columna.Item()
                        .PaddingTop(5)
                        .Text(texto =>
                        {
                            texto.Span(
                                    "Entrega incluida: ")
                                .Bold();

                            texto.Span(
                                pedido.EntregaIncluida
                                    ? "Sí"
                                    : "No");
                        });

                    if (pedido.ImporteAnticipo > 0m)
                    {
                        columna.Item()
                            .PaddingTop(5)
                            .Text(texto =>
                            {
                                texto.Span(
                                        "Medio de pago del anticipo: ")
                                    .Bold();

                                texto.Span(
                                    ValorSeguro(
                                        pedido.MedioPagoAnticipo));
                            });

                        columna.Item()
                            .PaddingTop(5)
                            .Text(texto =>
                            {
                                texto.Span(
                                        "Comprobante del anticipo: ")
                                    .Bold();

                                texto.Span(
                                    ValorSeguro(
                                        pedido.ComprobanteAnticipo));
                            });

                        columna.Item()
                            .PaddingTop(5)
                            .Text(texto =>
                            {
                                texto.Span(
                                        "Fecha del anticipo: ")
                                    .Bold();

                                texto.Span(
                                    pedido.FechaAnticipo.HasValue
                                        ? pedido.FechaAnticipo.Value
                                            .ToString("dd/MM/yyyy")
                                        : "No informada");
                            });
                    }

                    if (pedido.Moneda ==
                        Moneda.DolaresEstadounidenses)
                    {
                        columna.Item()
                            .PaddingTop(10)
                            .Background(
                                Colors.Orange.Lighten4)
                            .Padding(10)
                            .Text(
                                "El total expresado en pesos argentinos, " +
                                "considerando un tipo de cambio de $" +
                                pedido.TipoCambio.ToString(
                                    "N2",
                                    ObtenerCultura()) +
                                ", asciende a $" +
                                pedido.CalcularTotalEnPesos().ToString(
                                    "N2",
                                    ObtenerCultura()) +
                                ".")
                            .FontSize(9);
                    }

                    if (!string.IsNullOrWhiteSpace(
                        pedido.Observaciones))
                    {
                        columna.Item()
                            .PaddingTop(10)
                            .Text(texto =>
                            {
                                texto.Span(
                                        "Observaciones: ")
                                    .Bold();

                                texto.Span(
                                    pedido.Observaciones);
                            });
                    }
                });
        }

        private static void CrearLineaResumen(
            ColumnDescriptor columna,
            string titulo,
            string importe)
        {
            columna.Item()
                .PaddingTop(7)
                .Row(fila =>
                {
                    fila.RelativeItem()
                        .Text(titulo);

                    fila.RelativeItem()
                        .AlignRight()
                        .Text(importe);
                });
        }

        private static IContainer CeldaEncabezado(
            IContainer contenedor)
        {
            return contenedor
                .Background(
                    Colors.BlueGrey.Lighten3)
                .BorderBottom(1)
                .BorderColor(
                    Colors.BlueGrey.Medium)
                .PaddingVertical(8)
                .PaddingHorizontal(6);
        }

        private static IContainer CeldaDetalle(
            IContainer contenedor)
        {
            return contenedor
                .BorderBottom(1)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .PaddingVertical(7)
                .PaddingHorizontal(6);
        }

        private static string FormatearImporte(
            decimal importe,
            Moneda moneda)
        {
            string prefijo =
                moneda ==
                Moneda.DolaresEstadounidenses
                    ? "USD "
                    : "$";

            return prefijo +
                importe.ToString(
                    "N2",
                    ObtenerCultura());
        }

        private static string ValorSeguro(
            string valor)
        {
            return string.IsNullOrWhiteSpace(
                valor)
                    ? "No informado"
                    : valor.Trim();
        }

        private static string LimpiarNombreArchivo(
            string nombre)
        {
            string nombreLimpio =
                string.IsNullOrWhiteSpace(nombre)
                    ? Guid.NewGuid().ToString("N")
                    : nombre.Trim();

            foreach (char caracter
                in Path.GetInvalidFileNameChars())
            {
                nombreLimpio =
                    nombreLimpio.Replace(
                        caracter,
                        '_');
            }

            return nombreLimpio;
        }

        private static CultureInfo ObtenerCultura()
        {
            return CultureInfo.GetCultureInfo(
                "es-AR");
        }
    }
}