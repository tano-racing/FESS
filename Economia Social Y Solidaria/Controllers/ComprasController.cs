﻿using Economia_Social_Y_Solidaria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Economia_Social_Y_Solidaria.Controllers
{

    public class Changuito
    {
        public int idProducto { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public decimal precio = 0;
    }

    public class ChanguitoCompleta
    {
        public int totalCompraTandaUsuario = 0;
        public List<Locales> locales = new List<Locales>();
        public List<Changuito> changuito = new List<Changuito>();
    }


    public class ProductosComprados
    {
        public int idProducto;
        public int cantidad;
        public string nombre;
        public bool comentado;
        public decimal precioUnidad;
    }

    public class Comprados
    {
        public int idCompra;
        public int comuna;
        public string local;
        public string barrio;
        public string fecha;
        public bool editar = false;
        public bool comentar = false;
        public List<ProductosComprados> productos;
        public string estado;
    }

    public class MisCompras
    {
        public List<Comprados> Compras = new List<Comprados>();
    }

    public class ComprasController : Controller
    {
        public ActionResult Carrito()
        {
            TanoNEEntities ctx = new TanoNEEntities();
            ChanguitoCompleta completo = new ChanguitoCompleta();

            Tandas ultima = ctx.Tandas.ToList().LastOrDefault();
            if (ultima != null && ultima.fechaCerrado == null)
                completo.locales = ultima.Circuitos.Locales.ToList();

            if (User.Identity.IsAuthenticated && ultima != null)
            {
                EstadosCompra EstadoEntregado = ctx.EstadosCompra.FirstOrDefault(a => a.codigo == 1);

                Vecinos actual = ctx.Vecinos.FirstOrDefault(a => a.correo == User.Identity.Name);
                completo.totalCompraTandaUsuario = ctx.Compras.Where(a => a.vecinoId == actual.idVecino && a.tandaId == ultima.idTanda && a.estadoId == EstadoEntregado.idEstadoCompra).Count();
            }

            completo.changuito = ctx.Productos.ToList().Where(a => a.activo).Select(a => new Changuito()
            {
                idProducto = a.idProducto,
                nombre = a.nombre,
                descripcion = a.descripcion.Replace("\n", "<br/>"),
                precio = a.Precios.LastOrDefault().precio
            }).ToList();

            return View(completo);
        }

        public ActionResult Detalle(int id)
        {
            TanoNEEntities ctx = new TanoNEEntities();

            Productos actual = ctx.Productos.FirstOrDefault(a => a.idProducto == id);

            return View(new Changuito()
            {
                idProducto = actual.idProducto,
                nombre = actual.nombre,
                descripcion = actual.descripcion.Replace("\n", "<br/>"),
                precio = actual.Precios.LastOrDefault().precio
            });
        }

        public ActionResult Historial()
        {
            TanoNEEntities ctx = new TanoNEEntities();
            Vecinos vecino = ctx.Vecinos.FirstOrDefault(a => a.correo == User.Identity.Name);
            if (vecino == null)
                return RedirectToAction("Carrito", "Compras");

            EstadosCompra EstadoEntregado = ctx.EstadosCompra.FirstOrDefault(a => a.codigo == 1);
            EstadosCompra confirmado = ctx.EstadosCompra.FirstOrDefault(a => a.codigo == 3);

            MisCompras compras = new MisCompras();
            compras.Compras = ctx.Compras.Where(a => a.vecinoId == vecino.idVecino).ToList().Select(a => new Comprados
                {
                    idCompra = a.idCompra,
                    estado = a.EstadosCompra.nombre,
                    fecha = a.fecha.ToString("hh:mm dd/MM/yyyy"),
                    local = a.Locales.direccion,
                    barrio = a.Locales.barrio,
                    editar = a.estadoId == EstadoEntregado.idEstadoCompra,
                    comentar = a.estadoId == confirmado.idEstadoCompra,
                    comuna = a.Locales.comuna,
                    productos = a.CompraProducto.ToList().Select(b => new ProductosComprados
                    {
                        idProducto = b.Productos.idProducto,
                        nombre = b.Productos.nombre,
                        cantidad = b.cantidad,
                        comentado = a.Comentarios.Count == 1 ? a.Comentarios.FirstOrDefault().ComentariosProducto.FirstOrDefault(cp => cp.productoId == b.productoId).Productos != null : false,
                        precioUnidad = b.Productos.Precios.FirstOrDefault(precio => a.fecha > precio.fecha).precio
                    }).ToList()
                }).ToList();

            return View(compras);
        }

        public ActionResult Entregar()
        {
            return View();
        }

        public ActionResult CancelarPedido(int idCompra)
        {
            TanoNEEntities ctx = new TanoNEEntities();
            Vecinos vecino = ctx.Vecinos.FirstOrDefault(a => a.correo == User.Identity.Name);

            EstadosCompra EstadoEntregado = ctx.EstadosCompra.FirstOrDefault(a => a.codigo == 1);
            Compras cancelar = ctx.Compras.FirstOrDefault(a => a.idCompra == idCompra && vecino.idVecino == a.vecinoId);
            if (cancelar != null)
            {
                ctx.Compras.Remove(cancelar);
                ctx.SaveChanges();
            }

            MisCompras compras = new MisCompras();
            compras.Compras = ctx.Compras.Where(a => a.vecinoId == vecino.idVecino).ToList().Select(a => new Comprados
            {
                idCompra = a.idCompra,
                estado = a.EstadosCompra.nombre,
                fecha = a.fecha.ToString("hh:mm dd/MM/yyyy"),
                local = a.Locales.direccion,
                barrio = a.Locales.barrio,
                editar = a.estadoId == EstadoEntregado.idEstadoCompra,
                comuna = a.Locales.comuna,
                productos = a.CompraProducto.ToList().Select(b => new ProductosComprados
                {
                    idProducto = b.Productos.idProducto,
                    nombre = b.Productos.nombre,
                    cantidad = b.cantidad,
                    precioUnidad = b.Productos.Precios.FirstOrDefault(precio => a.fecha > precio.fecha).precio
                }).ToList()
            }).ToList();

            return Json(compras);
        }

        public ActionResult ConfirmarPedido(int local, int[] idProducto, int[] cantidad)
        {
            string error = null;

            TanoNEEntities ctx = new TanoNEEntities();

            Locales localCompro = ctx.Locales.FirstOrDefault(a => a.idLocal == local);
            if (localCompro == null)
                error = "No se indico en que local va a retirar lo pedido";

            Vecinos vecino = ctx.Vecinos.FirstOrDefault(a => a.correo == User.Identity.Name);
            if (vecino == null)
                error = "Hay que iniciar sesion para realizar un pedido";

            Tandas ultimaTanda = ctx.Tandas.ToList().LastOrDefault(a => a.fechaCerrado == null);
            if (ultimaTanda == null)
                error = "No hay circuitos abiertos en este momento";

            //Encargado
            EstadosCompra estado = ctx.EstadosCompra.FirstOrDefault(a => a.codigo == 1);

            Compras compra = new Compras();
            compra.fecha = DateTime.Now;
            compra.Locales = localCompro;
            compra.Vecinos = vecino;
            compra.Tandas = ultimaTanda;
            compra.EstadosCompra = estado;
            ctx.Compras.Add(compra);


            for (int x = 0; x < idProducto.Length; x++)
            {
                int prodActual = idProducto[x];
                int cantActual = cantidad[x];

                Productos prod = ctx.Productos.FirstOrDefault(a => a.idProducto == prodActual);

                CompraProducto productos = new CompraProducto();
                productos.Productos = prod;
                productos.Compras = compra;
                productos.cantidad = cantidad[x];

                ctx.CompraProducto.Add(productos);
            }



            if (string.IsNullOrEmpty(error))
                ctx.SaveChanges();

            return RedirectToAction("Historial", "Compras");
        }

        public ActionResult Calificar(int idCompra, int[] idProducto, string[] tComentarios, int[] rating)
        {
            TanoNEEntities ctx = new TanoNEEntities();
            Vecinos vecino = ctx.Vecinos.FirstOrDefault(a => a.correo == User.Identity.Name);

            EstadosCompra comentado = ctx.EstadosCompra.FirstOrDefault(a => a.codigo == 4);

            Compras compra = ctx.Compras.FirstOrDefault(a => a.idCompra == idCompra);
            

            Comentarios comentario = new Comentarios();
            if ( ctx.Comentarios.FirstOrDefault(a => a.vecinoId == vecino.idVecino && a.compraId == compra.idCompra) == null)
            {
                compra.EstadosCompra = comentado;

                comentario.Vecinos = vecino;
                comentario.fecha = DateTime.Now;
                comentario.Compras = compra;

                for (int x = 0; x < idProducto.Length; x++)
                {
                    int usar = idProducto[x];
                    Productos prod = ctx.Productos.FirstOrDefault(a => a.idProducto == usar);

                    ComentariosProducto cp = new ComentariosProducto();
                    cp.comentario = tComentarios[x];
                    cp.estrellas = rating[x];
                    cp.Productos = prod;
                    comentario.ComentariosProducto.Add(cp);
                }

                ctx.Comentarios.Add(comentario);
                ctx.SaveChanges();
                return Json(new { bien = true, idCompra = idCompra, comentario = comentado.nombre });
            }

            return Json(new { bien = false, mensaje = "No se puede comentar la misma compra 2 veces" });
        }

        public ActionResult ListaTanda()
        {
            TanoNEEntities ctx = new TanoNEEntities();
            Vecinos vecino = ctx.Vecinos.FirstOrDefault(a => a.correo == User.Identity.Name);

            Tandas ultima = ctx.Tandas.ToList().LastOrDefault(a => a.fechaCerrado != null);
            EstadosCompra confirmado = ctx.EstadosCompra.FirstOrDefault(a => a.codigo == 2);

            var lista = ctx.Compras.Where(a => a.tandaId == ultima.idTanda && a.Locales.comuna == vecino.comuna).ToList().Select(a => new
            {
                idCompra = a.idCompra,
                nombre = a.Vecinos.nombres,
                productos = string.Join("<br/>", a.CompraProducto.ToList().Select(b => "(" + b.cantidad + ") " + b.Productos.nombre)),
                precio = a.CompraProducto.ToList().Sum(b => b.cantidad * b.Productos.Precios.FirstOrDefault(precio => a.fecha > precio.fecha).precio),
                retiro = a.EstadosCompra != confirmado
            });
            return Json(lista, JsonRequestBehavior.DenyGet);
        }

        public ActionResult EnregarPedido(int idCompra)
        {
            TanoNEEntities ctx = new TanoNEEntities();
            Vecinos vecino = ctx.Vecinos.FirstOrDefault(a => a.correo == User.Identity.Name);

            EstadosCompra entregado = ctx.EstadosCompra.FirstOrDefault(a => a.codigo == 3);
            EstadosCompra confirmado = ctx.EstadosCompra.FirstOrDefault(a => a.codigo == 2);
            Compras compra = ctx.Compras.FirstOrDefault(a => a.idCompra == idCompra && a.estadoId == confirmado.idEstadoCompra);
            compra.EstadosCompra = entregado;
            ctx.SaveChanges();

            return Json(new { error = false }, JsonRequestBehavior.DenyGet);
        }
    }
}