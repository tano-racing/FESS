﻿using Economia_Social_Y_Solidaria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Economia_Social_Y_Solidaria.Controllers
{

    public class ConfCircuitos
    {
        public int idTanda;
        public int idCircuito;
        public int codigo;
        public string circuito;
        public string fechaAbierto;
        public string leyenda;
        public bool abrir = true;
        public string error;
    }


    public class TandasController : Controller
    {


        public ActionResult Tandas()
        {
            TanoNEEntities ctx = new TanoNEEntities();
            List<Tandas> total = ctx.Tandas.ToList();


            ConfCircuitos conf = new ConfCircuitos();
            if (total.Count == 0)
            {
                conf.leyenda = "No existen tandas creadas";
                conf.abrir = true;
            }
            else
            {
                Tandas ultima = total.LastOrDefault(a => a.fechaCerrado == null);
                if (ultima == null)
                {
                    ultima = total.LastOrDefault();
                    conf.abrir = true;
                    conf.codigo = ultima.Circuitos.codigo;
                    conf.leyenda = "No hay tanda abierta, último abierto: " + ultima.Circuitos.nombre;
                }
                else
                {
                    conf.leyenda = "Circuito Abierto el " + ultima.fechaAbierto.ToShortDateString()  + " por : " + ultima.Vecinos.nombres;
                    conf.idCircuito = ultima.Circuitos.idCircuito;
                    conf.abrir = false;
                }

                conf.idTanda = ultima.idTanda;
                conf.circuito = ultima.Circuitos.nombre;
                conf.fechaAbierto = ultima.fechaAbierto.ToShortDateString();
                
            }

            return View(conf);
        }

        public ActionResult Modificar(int codigo, bool abrir, int idTanda = -1)
        {
            TanoNEEntities ctx = new TanoNEEntities();
            ConfCircuitos conf = new ConfCircuitos();

            Circuitos circuito = ctx.Circuitos.FirstOrDefault(a => a.codigo == codigo);
            Vecinos responsable = ctx.Vecinos.FirstOrDefault(a => a.correo == User.Identity.Name);
            RolesVecinos rol = responsable.RolesVecinos.FirstOrDefault(a => a.Roles.codigoRol == 2);
            if (rol == null)
            {
                conf.error = "Su ususario no tiene permisos";
            }
            else
            {
                if (abrir)
                {
                    Tandas ultima = ctx.Tandas.ToList().LastOrDefault();
                    if (ultima != null && ultima.fechaCerrado == null)
                    {
                        conf.error = "No puede haber 2 tandas abiertas";
                    }
                    else
                    {
                        Tandas tanda = new Tandas();
                        tanda.fechaAbierto = DateTime.Now;
                        tanda.Circuitos = circuito;
                        tanda.Vecinos = responsable;
                        conf.abrir = false;
                        conf.circuito = tanda.Circuitos.nombre;
                        conf.codigo = tanda.Circuitos.codigo;
                        conf.idTanda = tanda.idTanda;
                        conf.idCircuito = tanda.Circuitos.idCircuito;
                        conf.leyenda = "Circuito Abierto: ";

                        ctx.Tandas.Add(tanda);
                        ctx.SaveChanges();
                    }
                }
                else
                {
                    Tandas ultima = ctx.Tandas.ToList().LastOrDefault();
                    if (ultima != null && ultima.fechaCerrado != null)
                    {
                        conf.error = "Ya se encuentra cerrado";
                    }
                    else
                    {
                        if (idTanda == -1)
                            conf.error = "No se encontró la tanda";
                        else
                        {
                            Tandas tanda = ctx.Tandas.FirstOrDefault(a => a.idTanda == idTanda);
                            tanda.fechaCerrado = DateTime.Now;
                            tanda.Vecinos = responsable;

                            conf.leyenda = "No hay tanda abierta, último abierto:";
                            conf.abrir = true;
                            conf.idCircuito = ultima.Circuitos.idCircuito;
                            conf.codigo = ultima.Circuitos.codigo;

                            //CERRAR TODOS LOS PEDIDOS DE ESTA TANDA

                            EstadosCompra confirmado = ctx.EstadosCompra.FirstOrDefault(a => a.codigo == 2);
                            foreach (var compraEnTanda in tanda.Compras)
                            {
                                compraEnTanda.EstadosCompra = confirmado;
                                MandarMailConfirmandoCompra(compraEnTanda.Vecinos.correo);
                            }

                            ctx.SaveChanges();
                        }
                    }

                }
            }



            return RedirectToAction("Tandas", "Tandas");
        }

        public JsonResult Lista()
        {
            TanoNEEntities ctx = new TanoNEEntities();
            var lista = ctx.Tandas.OrderByDescending( a => a.idTanda).ToList().Select(a => new
            {
                idTanda = a.idTanda,
                fechaAbierto = a.fechaAbierto.ToShortDateString(),
                fechaCerrado = a.fechaCerrado.HasValue ? a.fechaCerrado.Value.ToShortDateString() : "-",
                circuito = a.Circuitos.nombre,
                responsable = a.Vecinos.nombres,
            });
            return Json(lista, JsonRequestBehavior.DenyGet);
        }

        private void MandarMailConfirmandoCompra(string p)
        {
            //throw new NotImplementedException();
        }


    }
}