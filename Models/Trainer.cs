//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fo6League.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Trainer
    {
        public int ID { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int TeamID { get; set; }
        public byte[] Photo { get; set; }
        public string PhotoMIME { get; set; }
    
        public virtual Team Team { get; set; }
    }
}
