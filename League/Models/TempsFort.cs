//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace League.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TempsFort
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public int MatchID { get; set; }
        public byte[] Video { get; set; }
        public string VideoMIME { get; set; }
    
        public virtual Match Match { get; set; }
    }
}
