﻿using Economia_Social_Y_Solidaria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Economia_Social_Y_Solidaria.Controllers
{
    [Authorize]
    public class LocalesController : Controller
    {
        public ActionResult Locales()
        {
            return View();
        }

        public JsonResult Lista()
        {
            TanoNEEntities ctx = new TanoNEEntities();
            var listado = ctx.Locales.Select(a => new
            {
                idLocal = a.idLocal,
                nombre	= a.nombre,
                direccion = a.direccion,
                barrio	= a.barrio,
                horario	= a.horario,
                comuna		= a.comuna,
                circuitoId = a.Circuitos.idCircuito,
                circuito = a.Circuitos.nombre,
                activo = a.activo
            });

            return Json(listado, JsonRequestBehavior.DenyGet); 
        }

        public JsonResult Crear(Locales crear)
        {
            TanoNEEntities ctx = new TanoNEEntities();
            Locales local = ctx.Locales.FirstOrDefault(a => a.direccion == crear.direccion);
            if (local == null)
            {
                local = new Locales();
                local.nombre = string.IsNullOrWhiteSpace(crear.nombre) ? null : crear.nombre;
                local.direccion = crear.direccion;
                local.barrio = crear.barrio;
                local.horario = crear.horario;
                local.comuna = crear.comuna;
                local.activo = true;

                Circuitos cir = ctx.Circuitos.FirstOrDefault(a => a.idCircuito == crear.circuitoId);
                local.Circuitos = cir;
                ctx.Locales.Add(local);
                ctx.SaveChanges();
            }
            else
            {
                return Json(new { error = true, mensaje = "Ya existe el local" }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { error = false, mensaje = "Local grabado satisfactoriamente" }, JsonRequestBehavior.DenyGet);
        }

        public JsonResult Editar(Locales editar)
        {
            TanoNEEntities ctx = new TanoNEEntities();

            Locales item = ctx.Locales.FirstOrDefault(a => a.idLocal == editar.idLocal);
            if (item != null)
            {
                Circuitos cir = ctx.Circuitos.FirstOrDefault(a => a.idCircuito == editar.circuitoId);
                item.nombre = editar.nombre;
                item.direccion = editar.direccion;
                item.barrio = editar.barrio;
                item.horario = editar.horario;
                item.comuna = editar.comuna;

                ctx.SaveChanges();
            }
            else
            {
                return Json(new { error = true, mensaje = "No existe el producto" }, JsonRequestBehavior.DenyGet);
            }

            return Json(new { error = false, mensaje = "Producto editado satisfactoriamente" }, JsonRequestBehavior.DenyGet);
        }

        public JsonResult Desactivar(int idLocal, bool activar)
        {
            TanoNEEntities ctx = new TanoNEEntities();

            Locales item = ctx.Locales.FirstOrDefault(a => a.idLocal == idLocal);
            item.activo = activar;

            ctx.SaveChanges();

            return Json(activar, JsonRequestBehavior.DenyGet);
        }

    }
}
