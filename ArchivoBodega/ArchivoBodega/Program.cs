using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchivoBodega
{
    class Program
    {
        public struct VectorInventario
        {
            public string Codigo;
            public string Detalle;
            public int CantidadStock;
        }
        public struct VectorPedidos
        {
            public int CodigoPedido;
            public string IdProducto;
            public string Detalle;
            public int CantidadPedido;
        }
        public struct VectorVentas
        {
            public int CodigoVentas;
            public string CodigoProductoVenta;
            public string Detalle;
            public int CantidadVentas;
        }
        public struct VectorResumido
        {
            public int CodigoResumido;
            public string DetalleProducto;
            public int CantidadExistencia;
            public int CantidadVentas;
            public int CantidadComprometida;
        }

        static void Main(string[] args)
        {
            VectorPedidos[] Pedidos;
            VectorInventario[] Inventario;
            VectorVentas[] Ventas;
            VectorResumido[] Resumido;
            string RutaVentas = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Ventas.txt";
            string RutaInventario = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Inventario.txt";
            string RutasPedidos = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Pedidos.txt";
            string RutasResumido = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Resumido.txt";
            char op = ' ';
            do
            {
                Console.Clear();
                int CodigoPedido = ObtenerCodigoVentaPedido(RutasPedidos), CodigoVentas = ObtenerCodigoVentaPedido(RutaVentas), CodigoResumido = ObtenerCodigoResumido(RutasResumido);
                Console.WriteLine("1.- Registrar Producto");
                Console.WriteLine("2.- Realizar Pedido");
                Console.WriteLine("3.- Realizar Ventas");
                Console.WriteLine("4.- Marcar Pedidos");
                Console.WriteLine("5.- Mostrar Stock");
                Console.WriteLine("6.- Mostrar Pedidos");
                Console.WriteLine("7.- Mostrar Ventas");
                Console.WriteLine("8.- Mostrar Resumido");
                Console.WriteLine("9.- Salir");
                op = Console.ReadKey().KeyChar;
                switch (op)
                {
                    case '1':
                        ConsoleKeyInfo tecla;
                        do
                        {
                            VectorInventario NuevoProducto = new VectorInventario();
                            int axx = ObtenerCodigoResumido(RutasResumido) + 1;
                            do
                            {
                                Console.Clear();
                                Console.SetCursorPosition(1, 2); Console.Write("Presione ESP para salir");
                                NuevoProducto.Codigo = Validacion_Cadenas2(1, 4, "Codigo:");
                                NuevoProducto.Detalle = Validacion_Cadenas(1, 6, "Nombre Producto:");
                                if (ValidaCodigo(NuevoProducto.Codigo, NuevoProducto.Detalle) != true)
                                    MensajeError("Producto Existente");
                            } while (ValidaCodigo(NuevoProducto.Codigo, NuevoProducto.Detalle) != true);
                            NuevoProducto.CantidadStock = VALIDA_NUMERO(1, 8, "Cantidad:");
                            IngresoDatosProducto(NuevoProducto.Codigo, NuevoProducto.Detalle, NuevoProducto.CantidadStock);
                            IngresarDatosResumido(RutasResumido, axx, NuevoProducto.Detalle, NuevoProducto.CantidadStock, 0, 0);
                            tecla = Console.ReadKey(true);
                        } while (tecla.Key != ConsoleKey.Escape);
                        break;
                    case '2':
                        Console.Clear();
                        VectorPedidos NuevoPedido = new VectorPedidos();
                        Inventario = new VectorInventario[ObtenerLimite(RutaInventario)];
                        Resumido = new VectorResumido[ObtenerLimiteResumido(RutasResumido)];
                        LlenarVectorInventario(Inventario);
                        LlenarVectorResumido(Resumido);
                        int aux = CodigoPedido + 1;
                        bool ban;
                        Console.SetCursorPosition(1, 2); Console.Write("Pedido #: {0}", aux);
                        do
                        {
                            NuevoPedido.Detalle = Validacion_Cadenas(1, 4, "Producto: ");
                            ban = BusquedaProducto(NuevoPedido.Detalle);
                            if (ban == true)
                                MensajeError("Producto no existente");
                        } while (ban == true);
                        NuevoPedido.CantidadPedido = VALIDA_NUMERO(1, 6, "Cantidad: ");
                        int indice = BusquedaInventario(Inventario, NuevoPedido.Detalle);
                        int inddd = BusquedaResumido(Resumido, NuevoPedido.Detalle);
                        Resumido[inddd].CantidadComprometida = Resumido[inddd].CantidadComprometida + NuevoPedido.CantidadPedido;
                        IngresarDatosPedido(RutasPedidos, aux, Inventario[indice].Codigo, Inventario[indice].Detalle, NuevoPedido.CantidadPedido);
                        ActualizarResumido(RutasResumido, Resumido);
                        break;
                    case '3':
                        Console.Clear();
                        VectorVentas NuevaVenta = new VectorVentas();
                        Inventario = new VectorInventario[ObtenerLimite(RutaInventario)];
                        Resumido = new VectorResumido[ObtenerLimiteResumido(RutasResumido)];
                        LlenarVectorInventario(Inventario);
                        LlenarVectorResumido(Resumido);
                        int auxi = CodigoVentas + 1;
                        bool ba;
                        Console.SetCursorPosition(1, 2); Console.Write("Venta #: {0}", auxi);
                        do
                        {
                            NuevaVenta.Detalle = Validacion_Cadenas(1, 4, "Producto: ");
                            ba = BusquedaProducto(NuevaVenta.Detalle);
                            if (ba == true && Inventario[BusquedaInventario(Inventario, NuevaVenta.Detalle)].CantidadStock == 0)
                                MensajeError("Producto no existente");
                        } while (ba == true && Inventario[BusquedaInventario(Inventario, NuevaVenta.Detalle)].CantidadStock == 0);
                        int index = BusquedaInventario(Inventario, NuevaVenta.Detalle);
                        int indiceresu = BusquedaResumido(Resumido, NuevaVenta.Detalle);
                        do
                        {
                            NuevaVenta.CantidadVentas = VALIDA_NUMERO(1, 6, "Cantidad: ");
                            if ((Inventario[index].CantidadStock - NuevaVenta.CantidadVentas) < 0)
                                MensajeError("Cantidad erronea");
                        } while ((Inventario[index].CantidadStock - NuevaVenta.CantidadVentas) < 0);
                        Resumido[indiceresu].CantidadVentas = NuevaVenta.CantidadVentas + Resumido[indiceresu].CantidadVentas;
                        Resumido[indiceresu].CantidadExistencia = Resumido[indiceresu].CantidadExistencia - NuevaVenta.CantidadVentas;
                        IngresarDatosPedido(RutaVentas, auxi, Inventario[index].Codigo, NuevaVenta.Detalle, NuevaVenta.CantidadVentas);
                        ActualizarStock(RutaInventario, Math.Abs(Inventario[index].CantidadStock - NuevaVenta.CantidadVentas), Inventario, index);
                        ActualizarResumido(RutasResumido, Resumido);
                        break;
                    case '4':
                        Console.Clear();
                        Pedidos = new VectorPedidos[ObtenerLimitePedidos(RutasPedidos)];
                        Resumido = new VectorResumido[ObtenerLimiteResumido(RutasResumido)];
                        Inventario = new VectorInventario[ObtenerLimite(RutaInventario)];
                        LlenarVectorPedidos(Pedidos);
                        LlenarVectorResumido(Resumido);
                        LlenarVectorInventario(Inventario);
                        int C = 0;
                        bool bndr;
                        MostrarCompras();
                        do
                        {
                            C = VALIDA_NUMERO(30, 2, "Código Pedido:");
                            bndr = BusquedaPedidosExit(Pedidos, C);
                            if (bndr == true)
                                MensajeError("Error codigo no existente");
                        } while (bndr == true);
                        int indicepedido = BusquedaPedidos(Pedidos, C);
                        int indiceresumido = BusquedaResumido(Resumido, Pedidos[indicepedido].Detalle);
                        int indiceinventario = BusquedaInventario(Inventario, Pedidos[indicepedido].Detalle);
                        Resumido[indiceresumido].CantidadExistencia = Pedidos[indicepedido].CantidadPedido + Resumido[indiceresumido].CantidadExistencia;
                        Resumido[indiceresumido].CantidadComprometida = Resumido[indiceresumido].CantidadComprometida - Pedidos[indicepedido].CantidadPedido;
                        Inventario[indiceinventario].CantidadStock = Inventario[indiceinventario].CantidadStock + Pedidos[indicepedido].CantidadPedido;
                        ActualizarResumido(RutasResumido, Resumido);
                        ActualizarPedido(RutasPedidos, Pedidos, indicepedido);
                        ActualizarInventario(RutaInventario, Inventario);
                        break;
                    case '5':
                        Console.Clear();
                        MostrarInventario();
                        break;
                    case '6':
                        Console.Clear();
                        MostrarCompras();
                        break;
                    case '7':
                        Console.Clear();
                        MostrarVentas();
                        break;
                    case '8':
                        Console.Clear();
                        MostrarResumido();
                        break;
                    default:
                        MensajeError("Opción no existente");
                        break;
                }
            } while (op != '9');
        }

        static void MensajeError(string Mess)
        {
            Console.SetCursorPosition(2, 23); Console.Write(Mess);
            Console.SetCursorPosition(2, 24); Console.Write("Continuar...");
            Console.ReadKey();
            Console.SetCursorPosition(2, 23); Console.Write("                                         ");
            Console.SetCursorPosition(2, 24); Console.Write("                                         ");
        }

        static void MostrarInventario()
        {
            string RutaInventario = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Inventario.txt";
            FileStream archivoBinary = new FileStream(RutaInventario, FileMode.Open, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            while (leerBinary.PeekChar() != -1)
            {
                Console.WriteLine("Código: {0}", leerBinary.ReadString());
                Console.WriteLine("Detalle: {0}", leerBinary.ReadString());
                Console.WriteLine("Cantidad: {0}", leerBinary.ReadInt32());
                Console.WriteLine();
                Console.ReadKey();
            }
            leerBinary.Close();
            archivoBinary.Close();
        }
        static void MostrarVentas()
        {
            string RutaVentas = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Ventas.txt";
            FileStream archivoBinary = new FileStream(RutaVentas, FileMode.Open, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            while (leerBinary.PeekChar() != -1)
            {
                Console.WriteLine("Código Venta: {0}", leerBinary.ReadInt32());
                Console.WriteLine("Código Producto: {0}", leerBinary.ReadString());
                Console.WriteLine("Detalle: {0}", leerBinary.ReadString());
                Console.WriteLine("Cantidad: {0}", leerBinary.ReadInt32());
                Console.WriteLine();
                Console.ReadKey();
            }
            leerBinary.Close();
            archivoBinary.Close();
        }
        static void MostrarCompras()
        {
            string RutaCompras = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Pedidos.txt";
            FileStream archivoBinary = new FileStream(RutaCompras, FileMode.Open, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            while (leerBinary.PeekChar() != -1)
            {
                Console.WriteLine("Código Compra: {0}", leerBinary.ReadInt32());
                Console.WriteLine("Código Producto: {0}", leerBinary.ReadString());
                Console.WriteLine("Detalle: {0}", leerBinary.ReadString());
                Console.WriteLine("Cantidad: {0}", leerBinary.ReadInt32());
                Console.WriteLine();
                Console.ReadKey();
            }
            leerBinary.Close();
            archivoBinary.Close();
        }
        static void MostrarResumido()
        {
            string RutaResumen = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Resumido.txt";
            FileStream archivoBinary = new FileStream(RutaResumen, FileMode.Open, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            while (leerBinary.PeekChar() != -1)
            {
                Console.WriteLine("Código Resumido: {0}", leerBinary.ReadInt32());
                Console.WriteLine("Producto: {0}", leerBinary.ReadString());
                Console.WriteLine("Cantidad Existente: {0}", leerBinary.ReadInt32());
                Console.WriteLine("Cantidad Vendidas: {0}", leerBinary.ReadInt32());
                Console.WriteLine("Cantidad por Recibir: {0}", leerBinary.ReadInt32());
                Console.WriteLine();
                Console.ReadKey();
            }
            leerBinary.Close();
            archivoBinary.Close();
        }

        static void IngresoDatosProducto(string CodigoProducto, string Detalle, int Cantidad)
        {
            string RutaInventario = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Inventario.txt";
            FileStream archivo = new FileStream(RutaInventario, FileMode.Append, FileAccess.Write);
            BinaryWriter escribe = new BinaryWriter(archivo, Encoding.UTF8);
            escribe.Write(CodigoProducto);
            escribe.Write(Detalle);
            escribe.Write(Cantidad);
            escribe.Close();
            archivo.Close();
        }
        static void IngresarDatosPedido(string Ruta, int CodigoPedido, string CodigoProducto, string Detalle, int cantidad)
        {
            FileStream archivo = new FileStream(Ruta, FileMode.Append, FileAccess.Write);
            BinaryWriter escribe = new BinaryWriter(archivo, Encoding.UTF8);
            escribe.Write(CodigoPedido);
            escribe.Write(CodigoProducto);
            escribe.Write(Detalle);
            escribe.Write(cantidad);
            escribe.Close();
            archivo.Close();
        }
        static void IngresarDatosResumido(string Ruta, int Cod, string Producto, int CantidadExistente, int CantidadVendida, int CantidadComprometida)
        {
            FileStream archivo = new FileStream(Ruta, FileMode.Append, FileAccess.Write);
            BinaryWriter escribe = new BinaryWriter(archivo, Encoding.UTF8);
            escribe.Write(Cod);
            escribe.Write(Producto);
            escribe.Write(CantidadExistente);
            escribe.Write(CantidadVendida);
            escribe.Write(CantidadComprometida);
            escribe.Close();
            archivo.Close();
        }

        static int ObtenerLimitePedidos(string Ruta)
        {
            int Contador = 0;
            FileStream archivoBinary = new FileStream(Ruta, FileMode.Open, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            while (leerBinary.PeekChar() != -1)
            {
                leerBinary.ReadInt32();
                leerBinary.ReadString();
                leerBinary.ReadString();
                leerBinary.ReadInt32();
                Contador++;
            }
            leerBinary.Close();
            archivoBinary.Close();
            return Contador;
        }
        static int ObtenerLimite(string Ruta)
        {
            int Contador = 0;
            FileStream archivoBinary = new FileStream(Ruta, FileMode.Open, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            while (leerBinary.PeekChar() != -1)
            {
                leerBinary.ReadString();
                leerBinary.ReadString();
                leerBinary.ReadInt32();
                Contador++;
            }
            leerBinary.Close();
            archivoBinary.Close();
            return Contador;
        }
        static int ObtenerLimiteResumido(string Ruta)
        {
            int Contador = 0;
            FileStream archivoBinary = new FileStream(Ruta, FileMode.Open, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            while (leerBinary.PeekChar() != -1)
            {
                leerBinary.ReadInt32();
                leerBinary.ReadString();
                leerBinary.ReadInt32();
                leerBinary.ReadInt32();
                leerBinary.ReadInt32();
                Contador++;
            }
            leerBinary.Close();
            archivoBinary.Close();
            return Contador;
        }
        static int ObtenerCodigoVentaPedido(string Ruta)
        {
            int Codigo = 0;
            FileStream archivoBinary = new FileStream(Ruta, FileMode.OpenOrCreate, FileAccess.Read);
            BinaryReader Leyendo = new BinaryReader(archivoBinary, Encoding.UTF8);
            while (Leyendo.PeekChar() != -1)
            {
                Codigo = Leyendo.ReadInt32();
                Leyendo.ReadString();
                Leyendo.ReadString();
                Leyendo.ReadInt32();
            }
            Leyendo.Close();
            archivoBinary.Close();
            return Codigo;
        }
        static int ObtenerCodigoResumido(string Ruta)
        {
            int Code = 0;
            FileStream ArchivoBinary = new FileStream(Ruta, FileMode.OpenOrCreate, FileAccess.Read);
            BinaryReader Lee = new BinaryReader(ArchivoBinary, Encoding.UTF8);
            while (Lee.PeekChar() != -1)
            {
                Code = Lee.ReadInt32();
                Lee.ReadString();
                Lee.ReadInt32();
                Lee.ReadInt32();
                Lee.ReadInt32();
            }
            Lee.Close();
            ArchivoBinary.Close();
            return Code;
        }

        static void ActualizarStock(string Ruta, int CantidadUpdate, VectorInventario[] Invec, int indice)
        {
            FileStream Archivo = new FileStream(Ruta, FileMode.Truncate);
            Archivo.Close();
            FileStream archivo = new FileStream(Ruta, FileMode.Append, FileAccess.Write);
            BinaryWriter escribe = new BinaryWriter(archivo, Encoding.UTF8);
            for (int i = 0; i <= Invec.GetUpperBound(0); i++)
            {
                escribe.Write(Invec[i].Codigo);
                escribe.Write(Invec[i].Detalle);
                if (i == indice)
                    escribe.Write(CantidadUpdate);
                else
                    escribe.Write(Invec[i].CantidadStock);
            }
            escribe.Close();
            archivo.Close();
        }
        static void ActualizarResumido(string Ruta, VectorResumido[] Invec)
        {
            FileStream Archivo = new FileStream(Ruta, FileMode.Truncate);
            Archivo.Close();
            FileStream archivo = new FileStream(Ruta, FileMode.Append, FileAccess.Write);
            BinaryWriter escribe = new BinaryWriter(archivo, Encoding.UTF8);
            for (int i = 0; i <= Invec.GetUpperBound(0); i++)
            {
                escribe.Write(Invec[i].CodigoResumido);
                escribe.Write(Invec[i].DetalleProducto);
                escribe.Write(Invec[i].CantidadExistencia);
                escribe.Write(Invec[i].CantidadVentas);
                escribe.Write(Invec[i].CantidadComprometida);
            }
            escribe.Close();
            archivo.Close();
        }
        static void ActualizarInventario(string Ruta, VectorInventario[] Invec)
        {
            FileStream Archivo = new FileStream(Ruta, FileMode.Truncate);
            Archivo.Close();
            FileStream archivo = new FileStream(Ruta, FileMode.Append, FileAccess.Write);
            BinaryWriter escribe = new BinaryWriter(archivo, Encoding.UTF8);
            for (int i = 0; i <= Invec.GetUpperBound(0); i++)
            {
                escribe.Write(Invec[i].Codigo);
                escribe.Write(Invec[i].Detalle);
                escribe.Write(Invec[i].CantidadStock);
            }
            escribe.Close();
            archivo.Close();
        }
        static void ActualizarPedido(string Ruta, VectorPedidos[] Invec, int inss)
        {
            FileStream Archivo = new FileStream(Ruta, FileMode.Truncate);
            Archivo.Close();
            FileStream archivo = new FileStream(Ruta, FileMode.Append, FileAccess.Write);
            BinaryWriter escribe = new BinaryWriter(archivo, Encoding.UTF8);
            for (int i = 0; i <= Invec.GetUpperBound(0); i++)
            {
                if (i != inss)
                {
                    escribe.Write(Invec[i].CodigoPedido);
                    escribe.Write(Invec[i].IdProducto);
                    escribe.Write(Invec[i].Detalle);
                    escribe.Write(Invec[i].CantidadPedido);
                }
            }
            escribe.Close();
            archivo.Close();
        }

        static string Validacion_Cadenas(int x, int y, string mensaje)
        {
            string cadena = "";
            int n = 0;
            char letra = ' ';
            bool ban = true;
            do
            {
                ban = true;
                Console.SetCursorPosition(x + mensaje.Length + 1, y); Console.Write("                                                            ");
                Console.SetCursorPosition(x, y); Console.Write(mensaje);
                Console.SetCursorPosition(x + mensaje.Length + 1, y); cadena = Console.ReadLine();
                for (int i = 0; i < cadena.Length && ban == true; i++)
                {
                    letra = cadena[i];
                    n = (int)letra;
                    if (i == 0)
                    {
                        if (n == 32)
                        {
                            MensajeError("Error: Cadena inválida");
                            ban = false;
                        }
                    }
                    if ((n >= 65 && n <= 90) || (n >= 97 && n <= 122) || (n == 241) || (n == 209) || (n == 225) || (n == 233) || (n == 237) || (n == 243) || (n == 250) || (n == 32) || (n == 193) || (n == 201) || (n == 205) || (n == 211) || (n == 218))
                    {
                        break;
                    }
                    else
                    {
                        ban = false;
                        MensajeError("Error: Cadena inválida");
                    }
                }
            } while (ban == false);
            return cadena;
        }
        static string Validacion_Cadenas2(int x, int y, string mensaje)
        {
            string cadena = "";
            int n = 0;
            char letra = ' ';
            bool ban = true;
            do
            {
                ban = true;
                Console.SetCursorPosition(x + mensaje.Length + 1, y); Console.Write("                                                            ");
                Console.SetCursorPosition(x, y); Console.Write(mensaje);
                Console.SetCursorPosition(x + mensaje.Length + 1, y); cadena = Console.ReadLine();
                if (cadena == string.Empty)
                {
                    MensajeError("Error: Cadena inválida");
                    ban = false;
                }
                else
                {
                    for (int i = 0; i < cadena.Length && ban == true; i++)
                    {
                        letra = cadena[i];
                        n = (int)letra;
                        if (n == 32)
                        {
                            MensajeError("Error: Cadena inválida");
                            ban = false;
                        }
                    }
                }
            } while (ban == false);
            return cadena;
        }
        static bool ValidaCodigo(string Codigo, string Detalle)
        {
            bool ban = true;
            string Cod, detalle;
            int cantidad;
            string RutaInventario = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Inventario.txt";
            FileStream archivoBinary = new FileStream(RutaInventario, FileMode.OpenOrCreate, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            while (leerBinary.PeekChar() != -1 && ban == true)
            {
                Cod = leerBinary.ReadString();
                detalle = leerBinary.ReadString();
                cantidad = leerBinary.ReadInt32();
                if (Cod == Codigo || detalle == Detalle)
                    ban = false;
            }
            leerBinary.Close();
            archivoBinary.Close();
            return ban;
        }
        static int VALIDA_NUMERO(int x, int y, string mensaje)
        {
            int calificacion = 0;
            bool ban;
            do
            {
                do
                {
                    Console.SetCursorPosition(x + mensaje.Length + 1, y); Console.Write("                            ");
                    Console.SetCursorPosition(x, y); Console.Write(mensaje);
                    Console.SetCursorPosition(x + mensaje.Length + 1, y);
                    ban = int.TryParse(Console.ReadLine(), out calificacion);
                    if (!ban)
                        MensajeError("Error: Ingrese números");
                } while (!ban);
                if (calificacion < 0)
                    MensajeError("Error: Ingrese números positivos");
            } while (calificacion < 0);
            return calificacion;
        }

        static bool BusquedaProducto(string Clave)
        {
            bool ban = true;
            string Cod, detalle;
            int cantidad;
            string RutaInventario = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Inventario.txt";
            FileStream archivoBinary = new FileStream(RutaInventario, FileMode.Open, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            while (leerBinary.PeekChar() != -1 && ban == true)
            {
                Cod = leerBinary.ReadString();
                detalle = leerBinary.ReadString();
                cantidad = leerBinary.ReadInt32();
                if (detalle == Clave)
                    ban = false;
            }
            leerBinary.Close();
            archivoBinary.Close();
            return ban;
        }
        static int BusquedaInventario(VectorInventario[] VectorInventario, string Clave)
        {
            int indice = 0;
            for (int i = 0; i < VectorInventario.Length; i++)
            {
                if (VectorInventario[i].Detalle == Clave)
                    indice = i;
            }
            return indice;
        }
        static int BusquedaResumido(VectorResumido[] VectorResumidos, string Clave)
        {
            int indice = 0;
            for (int i = 0; i < VectorResumidos.Length; i++)
            {
                if (VectorResumidos[i].DetalleProducto == Clave)
                    indice = i;
            }
            return indice;
        }
        static int BusquedaPedidos(VectorPedidos[] VectorPedidos, int Clave)
        {
            int indice = 0;
            for (int i = 0; i < VectorPedidos.Length; i++)
            {
                if (VectorPedidos[i].CodigoPedido == Clave)
                    indice = i;
            }
            return indice;
        }
        static bool BusquedaPedidosExit(VectorPedidos[] VectorPedidos, int Clave)
        {
            bool indice = true;
            for (int i = 0; i < VectorPedidos.Length; i++)
            {
                if (VectorPedidos[i].CodigoPedido == Clave)
                    indice = false;
            }
            return indice;
        }

        static void LlenarVectorVentas(VectorVentas[] VentasAlmacen)
        {
            string RutaCompras = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Ventas.txt";
            FileStream archivoBinary = new FileStream(RutaCompras, FileMode.OpenOrCreate, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            for (int i = 0; i < VentasAlmacen.Length; i++)
            {
                while (leerBinary.PeekChar() != -1)
                {
                    VentasAlmacen[i].CodigoVentas = leerBinary.ReadInt32();
                    VentasAlmacen[i].CodigoProductoVenta = leerBinary.ReadString();
                    VentasAlmacen[i].Detalle = leerBinary.ReadString();
                    VentasAlmacen[i].CantidadVentas = leerBinary.ReadInt32();
                    i++;
                }
            }
            leerBinary.Close();
            archivoBinary.Close();
        }
        static void LlenarVectorPedidos(VectorPedidos[] PedidosAlmacen)
        {
            string RutaCompras = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Pedidos.txt";
            FileStream archivoBinary = new FileStream(RutaCompras, FileMode.Open, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            for (int i = 0; i < PedidosAlmacen.Length; i++)
            {
                while (leerBinary.PeekChar() != -1)
                {
                    PedidosAlmacen[i].CodigoPedido = leerBinary.ReadInt32();
                    PedidosAlmacen[i].IdProducto = leerBinary.ReadString();
                    PedidosAlmacen[i].Detalle = leerBinary.ReadString();
                    PedidosAlmacen[i].CantidadPedido = leerBinary.ReadInt32();
                    i++;
                }
            }
            leerBinary.Close();
            archivoBinary.Close();
        }
        static void LlenarVectorInventario(VectorInventario[] InventarioAlmacen)
        {
            string RutaCompras = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Inventario.txt";
            FileStream archivoBinary = new FileStream(RutaCompras, FileMode.Open, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            for (int i = 0; i < InventarioAlmacen.Length; i++)
            {
                while (leerBinary.PeekChar() != -1)
                {
                    InventarioAlmacen[i].Codigo = leerBinary.ReadString();
                    InventarioAlmacen[i].Detalle = leerBinary.ReadString();
                    InventarioAlmacen[i].CantidadStock = leerBinary.ReadInt32();
                    i++;
                }
            }
            leerBinary.Close();
            archivoBinary.Close();
        }
        static void LlenarVectorResumido(VectorResumido[] VectorResumidos)
        {
            string RutaResumido = @"F:\UNIVERSIDAD\SegundoSemestre\POO_Repeticion\STrabajoClacesBodega\Respaldos\Resumido.txt";
            FileStream archivoBinary = new FileStream(RutaResumido, FileMode.Open, FileAccess.Read);
            BinaryReader leerBinary = new BinaryReader(archivoBinary, Encoding.UTF8);
            for (int i = 0; i < VectorResumidos.Length; i++)
            {
                while (leerBinary.PeekChar() != -1)
                {
                    VectorResumidos[i].CodigoResumido = leerBinary.ReadInt32();
                    VectorResumidos[i].DetalleProducto = leerBinary.ReadString();
                    VectorResumidos[i].CantidadExistencia = leerBinary.ReadInt32();
                    VectorResumidos[i].CantidadVentas = leerBinary.ReadInt32();
                    VectorResumidos[i].CantidadComprometida = leerBinary.ReadInt32();
                    i++;
                }
            }
            leerBinary.Close();
            archivoBinary.Close();
        }

    }
}
