
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Economia_Social_Y_Solidaria.Models
{

using System;
    using System.Collections.Generic;
    
public partial class RolesVecinos
{

    public int idRolVecino { get; set; }

    public int vecinoId { get; set; }

    public int rolId { get; set; }



    public virtual Roles Roles { get; set; }

    public virtual Vecinos Vecinos { get; set; }

}

}
