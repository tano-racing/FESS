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

    public partial class RolesVecinos
    {
        public int idRolVecino { get; set; }
        public int vecinoId { get; set; }
        public int rolId { get; set; }
    
        public virtual Roles Roles { get; set; }
        public virtual Vecinos Vecinos { get; set; }
    }
}
