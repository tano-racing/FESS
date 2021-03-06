
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Economia_Social_Y_Solidaria.Models
{

using System;
    using System.Collections.Generic;
    
public partial class Vecinos
{

    public Vecinos()
    {

        this.AlertasVecinxs = new HashSet<AlertasVecinxs>();

        this.Comentarios = new HashSet<Comentarios>();

        this.Compras = new HashSet<Compras>();

        this.RolesVecinos = new HashSet<RolesVecinos>();

        this.Tandas = new HashSet<Tandas>();

    }


    public int idVecino { get; set; }

    public string correo { get; set; }

    public string nombres { get; set; }

    public string telefono { get; set; }

    public Nullable<int> comuna { get; set; }

    public string contrasena { get; set; }

    public string hash { get; set; }

    public bool verificado { get; set; }

    public Nullable<int> localId { get; set; }

    public Nullable<System.DateTime> fechaCreado { get; set; }

    public string token { get; set; }



    public virtual ICollection<AlertasVecinxs> AlertasVecinxs { get; set; }

    public virtual ICollection<Comentarios> Comentarios { get; set; }

    public virtual ICollection<Compras> Compras { get; set; }

    public virtual Locales Locales { get; set; }

    public virtual ICollection<RolesVecinos> RolesVecinos { get; set; }

    public virtual ICollection<Tandas> Tandas { get; set; }

}

}
